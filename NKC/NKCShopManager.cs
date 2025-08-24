using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClientPacket.Shop;
using Cs.Logging;
using NKC.Publisher;
using NKC.Templet;
using NKC.UI;
using NKC.UI.Result;
using NKC.UI.Shop;
using NKM;
using NKM.Item;
using NKM.Shop;
using NKM.Templet;
using NKM.Templet.Base;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public static class NKCShopManager
{
	public delegate void OnWaitComplete(bool bSuccess);

	public struct ShopRewardSubstituteData
	{
		public NKMRewardInfo Before;

		public NKMRewardInfo After;
	}

	public enum ShopTabCategory
	{
		NONE = -1,
		PACKAGE,
		SEASON,
		EXCHANGE,
		SKIN,
		SIDESTORY,
		RESERVED_001,
		RESERVED_002,
		RESERVED_003,
		COUNT
	}

	public const string UNLOCK_AUTO_STRING = "AUTO";

	internal static readonly TimeSpan ShopResetHour = TimeSpan.FromHours(0.0);

	private static List<ShopItemTemplet> m_lstLevelPackageTemplet = new List<ShopItemTemplet>();

	private static bool m_bReserveForceRefreshShop = false;

	private static int m_totalBundleCount = 0;

	private static HashSet<int> m_lstBundleItemIds = new HashSet<int>();

	private static List<NKMRewardData> m_lstBundleItemReward = new List<NKMRewardData>();

	private static bool bFeaturedTempletLoaded = false;

	private static bool bLevelupPackageTempletLoaded = false;

	private static Dictionary<int, List<ShopItemTemplet>> s_dicLinkedItemCache = new Dictionary<int, List<ShopItemTemplet>>();

	private static readonly List<int> PACKAGE_RESOURCE_LIST = new List<int> { 1, 2, 101, 102 };

	private static List<ShopItemTemplet> m_lstSpecialItemTemplet = new List<ShopItemTemplet>();

	public static List<int> ShopItemList { get; private set; }

	public static Dictionary<int, InstantProduct> InstantProducts { get; private set; }

	public static long ShopItemUpdatedTimestamp { get; private set; } = 0L;

	public static bool IsShopItemListReady
	{
		get
		{
			if (ShopItemList != null)
			{
				return NKCPublisherModule.InAppPurchase.CheckReceivedBillingProductList;
			}
			return false;
		}
	}

	public static InstantProduct GetInstantProduct(int id)
	{
		if (InstantProducts != null && InstantProducts.TryGetValue(id, out var value))
		{
			return value;
		}
		return null;
	}

	public static void SetShopItemList(List<int> lstShopItem, List<InstantProduct> lstInstantProduct)
	{
		ShopItemList = lstShopItem;
		InstantProducts = new Dictionary<int, InstantProduct>();
		foreach (InstantProduct item in lstInstantProduct)
		{
			InstantProducts.Add(item.productId, item);
		}
		ShopItemUpdatedTimestamp = NKCSynchronizedTime.GetServerUTCTime().Ticks;
	}

	public static void InvalidateShopItemList()
	{
		ShopItemList = null;
	}

	public static void RequestShopItemList(NKC_OPEN_WAIT_BOX_TYPE waitBoxType, bool bForceRefreshServerItemList = false)
	{
		if (bForceRefreshServerItemList || ShopItemList == null)
		{
			ShopItemList = null;
			NKCPacketSender.Send_NKMPacket_SHOP_FIXED_LIST_REQ(waitBoxType);
		}
		if (!NKCPublisherModule.InAppPurchase.CheckReceivedBillingProductList)
		{
			NKCPublisherModule.InAppPurchase.RequestBillingProductList(null);
		}
	}

	public static void SetReserveRefreshShop()
	{
		m_bReserveForceRefreshShop = true;
	}

	public static void FetchShopItemList(NKC_OPEN_WAIT_BOX_TYPE waitBoxType, OnWaitComplete onWaitComplete, bool bForceRefreshServerItemList = false)
	{
		if (m_bReserveForceRefreshShop)
		{
			bForceRefreshServerItemList = true;
		}
		if (!bForceRefreshServerItemList && IsShopItemListReady && NKCEmoticonManager.m_bReceivedEmoticonData)
		{
			Debug.Log("Skip fecth shop item list...");
			onWaitComplete?.Invoke(bSuccess: true);
		}
		else
		{
			NKCScenManager.GetScenManager().StartCoroutine(WaitForShopItemList(waitBoxType, onWaitComplete, bForceRefreshServerItemList));
			m_bReserveForceRefreshShop = false;
		}
	}

	private static IEnumerator WaitForShopItemList(NKC_OPEN_WAIT_BOX_TYPE waitBoxType, OnWaitComplete onWaitComplete, bool bForceRefreshServerItemList = false)
	{
		Debug.Log("Fecthing shop item list...");
		float waitTime = 0f;
		if (!IsShopItemListReady || bForceRefreshServerItemList)
		{
			RequestShopItemList(waitBoxType, bForceRefreshServerItemList);
		}
		if (!NKCEmoticonManager.m_bReceivedEmoticonData || bForceRefreshServerItemList)
		{
			NKCPacketSender.Send_NKMPacket_EMOTICON_DATA_REQ(waitBoxType);
		}
		while (!IsShopItemListReady || !NKCEmoticonManager.m_bReceivedEmoticonData)
		{
			if (!NKMPopUpBox.IsOpenedWaitBox())
			{
				NKMPopUpBox.OpenWaitBox(waitBoxType);
			}
			waitTime += Time.unscaledDeltaTime;
			if (waitTime > 5f)
			{
				NKMPopUpBox.CloseWaitBox();
				onWaitComplete?.Invoke(bSuccess: false);
				yield break;
			}
			yield return null;
		}
		m_lstSpecialItemTemplet = GetLimitedCountSpecialItems();
		NKMPopUpBox.CloseWaitBox();
		onWaitComplete?.Invoke(bSuccess: true);
	}

	public static NKM_ERROR_CODE CanBuyFixShop(NKMUserData user_data, ShopItemTemplet shop_templet, out bool is_init, out long next_reset_date, bool bCheckChainIndex = true)
	{
		is_init = false;
		next_reset_date = 0L;
		if (user_data == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_USER_DATA_NULL;
		}
		if (shop_templet.IsNewbieProduct && !user_data.IsNewbieUser(shop_templet.m_NewbieDate))
		{
			return NKM_ERROR_CODE.NKE_FAIL_SHOP_NOT_EVENT_TIME;
		}
		if (!NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in shop_templet.m_UnlockInfo, shop_templet.m_bUnlockBanner))
		{
			return NKM_ERROR_CODE.NEC_FAIL_INVALID_LIMITED_ITEM_UNLOCK;
		}
		if (!shop_templet.IsReturningProduct)
		{
			if (!NKCSynchronizedTime.IsEventTime(shop_templet.eventIntervalId, shop_templet.EventDateStartUtc, shop_templet.EventDateEndUtc))
			{
				return NKM_ERROR_CODE.NKE_FAIL_SHOP_NOT_EVENT_TIME;
			}
		}
		else
		{
			if (!user_data.IsReturnUser())
			{
				return NKM_ERROR_CODE.NKE_FAIL_SHOP_NOT_EVENT_TIME;
			}
			if (!NKCSynchronizedTime.IsEventTime(user_data.GetReturnStartDate(shop_templet.m_ReturningUserType), shop_templet.eventIntervalId, shop_templet.EventDateStartUtc, shop_templet.EventDateEndUtc))
			{
				return NKM_ERROR_CODE.NKE_FAIL_SHOP_NOT_EVENT_TIME;
			}
		}
		ShopTabTemplet shopTabTemplet = ShopTabTemplet.Find(shop_templet.m_TabID, shop_templet.m_TabSubIndex);
		if (shopTabTemplet != null)
		{
			if (!NKCSynchronizedTime.IsEventTime(shopTabTemplet.intervalId, shopTabTemplet.EventDateStartUtc, shopTabTemplet.EventDateEndUtc))
			{
				return NKM_ERROR_CODE.NKE_FAIL_SHOP_NOT_EVENT_TIME;
			}
			if (bCheckChainIndex && shop_templet.m_ChainIndex > 0 && GetCurrentTargetChainIndex(shopTabTemplet) != shop_templet.m_ChainIndex)
			{
				return NKM_ERROR_CODE.NKE_FAIL_SHOP_INVALID_CHAIN_TAB;
			}
		}
		if (WillOverflowOnGain(shop_templet.m_ItemType, shop_templet.m_ItemID, shop_templet.TotalValue))
		{
			switch (shop_templet.m_ItemType)
			{
			case NKM_REWARD_TYPE.RT_SKIN:
				return NKM_ERROR_CODE.NKE_FAIL_SHOP_SKIN_ALREADY_OWNED;
			case NKM_REWARD_TYPE.RT_MISC:
			{
				NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(shop_templet.m_ItemID);
				if (itemMiscTempletByID == null)
				{
					return NKM_ERROR_CODE.NEC_FAIL_INVALID_ITEM_ID;
				}
				switch (itemMiscTempletByID.m_ItemMiscType)
				{
				case NKM_ITEM_MISC_TYPE.IMT_BACKGROUND:
					return NKM_ERROR_CODE.NEC_FAIL_SHOP_BACKGROUND_ALREADY_OWNED;
				case NKM_ITEM_MISC_TYPE.IMT_SELFIE_FRAME:
					return NKM_ERROR_CODE.NEC_FAIL_SHOP_FRAME_ALREADY_OWNED;
				case NKM_ITEM_MISC_TYPE.IMT_TITLE:
					return NKM_ERROR_CODE.NEC_FAIL_SHOP_ALREADY_OWNED;
				case NKM_ITEM_MISC_TYPE.IMT_INTERIOR:
				{
					NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMOfficeInteriorTemplet.Find(shop_templet.m_ItemID);
					if (nKMOfficeInteriorTemplet != null)
					{
						switch (nKMOfficeInteriorTemplet.InteriorCategory)
						{
						case InteriorCategory.FURNITURE:
							return NKM_ERROR_CODE.NEC_FAIL_SHOP_FURNITURE_OWNED_MAX;
						case InteriorCategory.DECO:
							return NKM_ERROR_CODE.NEC_FAIL_SHOP_DECORATION_ALREADY_OWNED;
						}
					}
					break;
				}
				}
				break;
			}
			}
			return NKM_ERROR_CODE.NEC_FAIL_SHOP_ALREADY_OWNED;
		}
		if (shop_templet.NeedHistory && shop_templet.resetType != SHOP_RESET_TYPE.Unlimited)
		{
			if (!user_data.m_ShopData.histories.TryGetValue(shop_templet.m_ProductID, out var value))
			{
				is_init = true;
				next_reset_date = GetNextResetDate(shop_templet.resetType);
			}
			else
			{
				next_reset_date = value.nextResetDate;
				if (shop_templet.IsCountResetType() && NKCSynchronizedTime.IsFinished(value.nextResetDate))
				{
					is_init = true;
					next_reset_date = GetNextResetDate(shop_templet.resetType);
				}
				int num = value.purchaseCount;
				if (is_init)
				{
					num = 0;
				}
				if (shop_templet.m_QuantityLimit <= num)
				{
					return NKM_ERROR_CODE.NEC_FAIL_LIMITED_SHOP_COUNT_FAIL;
				}
			}
			if (shop_templet.m_paidAmountRequired > 0.0 && NKCScenManager.CurrentUserData().m_ShopData.GetTotalPayment() < shop_templet.m_paidAmountRequired)
			{
				return NKM_ERROR_CODE.NEC_FAIL_SHOP_NOT_ENOUGH_PAID_AMOUNT;
			}
		}
		if (!shop_templet.m_bEnabled)
		{
			return NKM_ERROR_CODE.NKE_FAIL_SHOP_NOT_EVENT_TIME;
		}
		if (shop_templet.m_PriceItemID == 0)
		{
			if (string.IsNullOrEmpty(shop_templet.m_MarketID))
			{
				return NKM_ERROR_CODE.NEC_FAIL_INVALID_SHOP_ID;
			}
			if (!NKCPublisherModule.InAppPurchase.IsRegisteredProduct(shop_templet.m_MarketID, shop_templet.m_ProductID))
			{
				return NKM_ERROR_CODE.NEC_FAIL_INVALID_SHOP_ID;
			}
		}
		if (shop_templet.m_PurchaseEventType == PURCHASE_EVENT_REWARD_TYPE.CONSUMER_PACKAGE && NKCScenManager.CurrentUserData().GetConsumerPackageData(shop_templet.m_ProductID, out var _))
		{
			return NKM_ERROR_CODE.NEC_FAIL_CONSUMER_PACKAGE_ALREADY_PURCHASED;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static ShopItemTemplet GetShopTempletBySkinID(int skinID)
	{
		foreach (ShopItemTemplet value in NKMTempletContainer<ShopItemTemplet>.Values)
		{
			if (value.m_bEnabled && value.m_ItemType == NKM_REWARD_TYPE.RT_SKIN && value.m_ItemID == skinID)
			{
				return value;
			}
		}
		return null;
	}

	public static bool IsPackageItem(int shopID)
	{
		ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(shopID);
		if (shopItemTemplet == null)
		{
			Log.Error("ShopTemplet not found. ID : " + shopID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCShopManager.cs", 369);
			return false;
		}
		if (shopItemTemplet.m_ItemType != NKM_REWARD_TYPE.RT_MISC)
		{
			return false;
		}
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(shopItemTemplet.m_ItemID);
		if (itemMiscTempletByID == null)
		{
			Log.Error($"ItemTemplet {shopItemTemplet.m_ItemID} from ShopTemplet {shopID} not found.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCShopManager.cs", 381);
			return false;
		}
		return itemMiscTempletByID.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_PACKAGE;
	}

	public static bool IsCustomPackageItem(int shopID)
	{
		ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(shopID);
		if (shopItemTemplet == null)
		{
			Log.Error("ShopTemplet not found. ID : " + shopID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCShopManager.cs", 394);
			return false;
		}
		if (shopItemTemplet.m_ItemType != NKM_REWARD_TYPE.RT_MISC)
		{
			return false;
		}
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(shopItemTemplet.m_ItemID);
		if (itemMiscTempletByID == null)
		{
			Log.Error($"ItemTemplet {shopItemTemplet.m_ItemID} from ShopTemplet {shopID} not found.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCShopManager.cs", 406);
			return false;
		}
		return itemMiscTempletByID.IsCustomPackageItem;
	}

	public static ShopItemTemplet GetShopTempletByMarketID(string marketID)
	{
		if (string.IsNullOrEmpty(marketID))
		{
			return null;
		}
		foreach (ShopItemTemplet value in NKMTempletContainer<ShopItemTemplet>.Values)
		{
			if (string.Equals(value.m_MarketID, marketID))
			{
				return value;
			}
		}
		return null;
	}

	public static ShopItemTemplet GetShopTempletByProductID(int productID)
	{
		foreach (ShopItemTemplet value in NKMTempletContainer<ShopItemTemplet>.Values)
		{
			if (value.m_ProductID == productID)
			{
				return value;
			}
		}
		return null;
	}

	public static long GetNextResetDate(SHOP_RESET_TYPE limit_cond)
	{
		DateTime serverUTCTime = NKCSynchronizedTime.GetServerUTCTime();
		switch (limit_cond)
		{
		case SHOP_RESET_TYPE.DAY:
			return NKMTime.GetNextResetTime(serverUTCTime, NKMTime.TimePeriod.Day).Ticks;
		case SHOP_RESET_TYPE.WEEK:
		case SHOP_RESET_TYPE.WEEK_SUN:
		case SHOP_RESET_TYPE.WEEK_MON:
		case SHOP_RESET_TYPE.WEEK_TUE:
		case SHOP_RESET_TYPE.WEEK_WED:
		case SHOP_RESET_TYPE.WEEK_THU:
		case SHOP_RESET_TYPE.WEEK_FRI:
		case SHOP_RESET_TYPE.WEEK_SAT:
			return NKMTime.GetNextResetTime(serverUTCTime, NKMTime.TimePeriod.Week).Ticks;
		case SHOP_RESET_TYPE.MONTH:
			return NKMTime.GetNextResetTime(serverUTCTime, NKMTime.TimePeriod.Month).Ticks;
		case SHOP_RESET_TYPE.FIXED:
			return serverUTCTime.AddYears(100).Ticks;
		default:
			return 0L;
		}
	}

	public static int GetBundleItemPrice(ShopTabTemplet tabTemplet)
	{
		int num = 0;
		if (tabTemplet != null)
		{
			NKMUserData user_data = NKCScenManager.CurrentUserData();
			if (tabTemplet.IsBundleTab)
			{
				for (int i = 0; i < tabTemplet.Goods.Count; i++)
				{
					if (CanBuyFixShop(user_data, tabTemplet.Goods[i], out var _, out var next_reset_date) == NKM_ERROR_CODE.NEC_OK || NKCSynchronizedTime.IsFinished(next_reset_date))
					{
						num += tabTemplet.Goods[i].m_Price * GetBuyCountLeft(tabTemplet.Goods[i].m_ProductID);
					}
				}
			}
			else
			{
				Log.Error($"Bundle 타입이 아님 - {tabTemplet.m_ShopDisplay}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCShopManager.cs", 487);
			}
		}
		return num;
	}

	public static int GetBundleItemPriceItemID(ShopTabTemplet tabTemplet)
	{
		int num = -1;
		if (tabTemplet.IsBundleTab)
		{
			for (int i = 0; i < tabTemplet.Goods.Count; i++)
			{
				if (num < 0)
				{
					num = tabTemplet.Goods[i].m_PriceItemID;
				}
				if (num >= 0 && num != tabTemplet.Goods[i].m_PriceItemID)
				{
					Log.Error("Bundle 탭의 소모재화 종류가 다름", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCShopManager.cs", 506);
					return num;
				}
			}
		}
		else
		{
			Log.Error($"Bundle 타입이 아님 - {tabTemplet.m_ShopDisplay}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCShopManager.cs", 513);
		}
		return num;
	}

	public static int GetCurrentTargetChainIndex(ShopTabTemplet tabTemplet)
	{
		NKMUserData user_data = NKCScenManager.CurrentUserData();
		NKMShopData shopData = NKCScenManager.CurrentUserData().m_ShopData;
		int num = 0;
		if (tabTemplet != null)
		{
			for (int i = 1; i <= 3 && tabTemplet.GetChainGoods(i) != null; i++)
			{
				num = i;
				foreach (ShopItemTemplet chainGood in tabTemplet.GetChainGoods(i))
				{
					ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(chainGood.m_ProductID);
					if (shopItemTemplet != null && CanBuyFixShop(user_data, shopItemTemplet, out var _, out var _, bCheckChainIndex: false) == NKM_ERROR_CODE.NEC_OK)
					{
						if (!shopData.histories.ContainsKey(chainGood.m_ProductID))
						{
							return i;
						}
						if (shopData.histories[chainGood.m_ProductID].purchaseCount < shopItemTemplet.m_QuantityLimit)
						{
							return i;
						}
						if (shopData.histories[chainGood.m_ProductID].nextResetDate < NKCSynchronizedTime.GetServerUTCTime().Ticks)
						{
							return i;
						}
					}
				}
			}
			if (num == 0)
			{
				Log.Debug($"Invalid ChianGoods - tabID : {tabTemplet.TabType}, subIndex : {tabTemplet.SubIndex}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCShopManager.cs", 555);
			}
		}
		return num;
	}

	public static List<NKCShopBannerTemplet> GetHomeBannerTemplet()
	{
		List<NKCShopBannerTemplet> list = new List<NKCShopBannerTemplet>();
		foreach (NKCShopBannerTemplet value in NKMTempletContainer<NKCShopBannerTemplet>.Values)
		{
			if (!NKCSynchronizedTime.IsEventTime(NKCSynchronizedTime.GetServerUTCTime(), value.m_DateStrID) || !value.m_Enable || !value.EnableByTag || !CheckRecommendCond(value.m_DisplayCond, value.m_DisplayCondValue))
			{
				continue;
			}
			if (value.m_ProductID > 0)
			{
				ShopItemTemplet shop_templet = ShopItemTemplet.Find(value.m_ProductID);
				if (CanBuyFixShop(NKCScenManager.CurrentUserData(), shop_templet, out var _, out var _) != NKM_ERROR_CODE.NEC_OK)
				{
					continue;
				}
			}
			list.Add(value);
		}
		return list;
	}

	public static List<ShopItemTemplet> GetItemTempletListByTab(ShopTabTemplet tabTemplet, bool bIncludeLockedItemWithReason = false)
	{
		List<ShopItemTemplet> list = new List<ShopItemTemplet>();
		if (tabTemplet == null)
		{
			return list;
		}
		for (int i = 0; i < tabTemplet.Goods.Count; i++)
		{
			if (CanExhibitItem(tabTemplet.Goods[i], bIncludeLockedItemWithReason))
			{
				list.Add(tabTemplet.Goods[i]);
			}
		}
		return list;
	}

	public static bool IsTabSoldOut(ShopTabTemplet tabTemplet)
	{
		new List<ShopItemTemplet>();
		if (tabTemplet == null)
		{
			return true;
		}
		for (int i = 0; i < tabTemplet.Goods.Count; i++)
		{
			if (CanExhibitItem(tabTemplet.Goods[i], bIncludeLockedItemWithReason: true) && GetBuyCountLeft(tabTemplet.Goods[i].m_ProductID) != 0)
			{
				return false;
			}
		}
		return true;
	}

	public static bool CanExhibitItem(ShopItemTemplet shopItemTemplet, bool bIncludeLockedItemWithReason = false, bool bIgnoreContentUnlock = false)
	{
		if (shopItemTemplet == null)
		{
			return false;
		}
		if (!shopItemTemplet.m_bVisible)
		{
			return false;
		}
		if (!shopItemTemplet.EnableByTag)
		{
			return false;
		}
		if (!shopItemTemplet.ItemEnableByTag)
		{
			return false;
		}
		if (!IsProductAvailable(shopItemTemplet, out var _, bIncludeLockedItemWithReason, bIgnoreContentUnlock))
		{
			return false;
		}
		if (shopItemTemplet.m_HideWhenSoldOut && GetBuyCountLeft(shopItemTemplet.m_ProductID) == 0)
		{
			return false;
		}
		return true;
	}

	private static int CompareByLimitShowIndex(ShopItemTemplet left, ShopItemTemplet right)
	{
		return right.m_LimitShowIndex.CompareTo(left.m_LimitShowIndex);
	}

	public static bool IsFirstBuy(int ProductID, NKMUserData userData)
	{
		if (userData != null && userData.m_ShopData.histories.TryGetValue(ProductID, out var value))
		{
			return value.purchaseCount == 0;
		}
		return true;
	}

	public static bool IsProductAvailable(ShopItemTemplet shopTemplet, out bool bAdmin, bool bIncludeLockedItemWithReason = false, bool bIgnoreContentUnlock = false)
	{
		bAdmin = false;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return false;
		}
		if (shopTemplet == null)
		{
			Log.Error("ShopTemplet null!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCShopManager.cs", 713);
			return false;
		}
		if (!shopTemplet.IsReturningProduct)
		{
			if (shopTemplet.HasDateLimit && !NKCSynchronizedTime.IsEventTime(shopTemplet.eventIntervalId))
			{
				return false;
			}
		}
		else if (nKMUserData.IsReturnUser())
		{
			if (!NKCSynchronizedTime.IsEventTime(nKMUserData.GetReturnStartDate(shopTemplet.m_ReturningUserType), shopTemplet.eventIntervalId, shopTemplet.EventDateStartUtc, shopTemplet.EventDateEndUtc))
			{
				if (!IsAdmin(nKMUserData))
				{
					return false;
				}
				bAdmin = true;
			}
		}
		else
		{
			if (!IsAdmin(nKMUserData))
			{
				return false;
			}
			bAdmin = true;
		}
		if (shopTemplet.IsNewbieProduct && !nKMUserData.IsNewbieUser(shopTemplet.m_NewbieDate))
		{
			return false;
		}
		if (shopTemplet.resetType.ToDayOfWeek(out var result) && NKCSynchronizedTime.ServiceTime.DayOfWeek != result)
		{
			return false;
		}
		if (!bIgnoreContentUnlock)
		{
			bool bAdmin2;
			bool flag = NKMContentUnlockManager.IsContentUnlocked(nKMUserData, in shopTemplet.m_UnlockInfo, out bAdmin2);
			bAdmin |= bAdmin2;
			if (!flag && (!bIncludeLockedItemWithReason || string.IsNullOrEmpty(shopTemplet.m_UnlockReqStrID)))
			{
				return false;
			}
			if (shopTemplet.IsInstantProduct)
			{
				InstantProduct instantProduct = GetInstantProduct(shopTemplet.m_ProductID);
				if (instantProduct != null)
				{
					if (NKCSynchronizedTime.IsFinished(NKMTime.LocalToUTC(instantProduct.endDate)))
					{
						return false;
					}
				}
				else if (flag)
				{
					return false;
				}
			}
		}
		if (!shopTemplet.m_bEnabled)
		{
			return false;
		}
		if (shopTemplet.m_PriceItemID == 0)
		{
			if (string.IsNullOrEmpty(shopTemplet.m_MarketID))
			{
				return false;
			}
			if (!NKCPublisherModule.InAppPurchase.IsRegisteredProduct(shopTemplet.m_MarketID, shopTemplet.m_ProductID))
			{
				return false;
			}
		}
		return true;
		static bool IsAdmin(NKMUserData userData)
		{
			if (userData.IsSuperUser())
			{
				return true;
			}
			return false;
		}
	}

	public static int GetDailyMissionTicketShopID(int episodeID)
	{
		return episodeID switch
		{
			101 => 40121, 
			103 => 40122, 
			102 => 40123, 
			_ => 0, 
		};
	}

	public static int GetBundleCount()
	{
		return m_totalBundleCount;
	}

	public static void SetBundleItemIds(HashSet<int> lstBundleItemIds)
	{
		m_totalBundleCount = lstBundleItemIds.Count;
		m_lstBundleItemIds = lstBundleItemIds;
		m_lstBundleItemReward = new List<NKMRewardData>();
	}

	public static void RemoveBundleItemId(int bundleItemId, NKMRewardData rewardData)
	{
		if (m_lstBundleItemIds.Contains(bundleItemId))
		{
			m_lstBundleItemIds.Remove(bundleItemId);
			m_lstBundleItemReward.Add(rewardData);
		}
		if (m_lstBundleItemIds.Count == 0)
		{
			NKCUIResult.Instance.OpenBoxGain(NKCScenManager.CurrentUserData().m_ArmyData, m_lstBundleItemReward, NKCUtilString.GET_STRING_SHOP_BUY_ALL_TITLE);
			m_totalBundleCount = 0;
		}
	}

	public static List<ShopItemTemplet> GetLockedProductList(bool bIgnoreSuperUser = false)
	{
		List<ShopItemTemplet> list = new List<ShopItemTemplet>();
		foreach (ShopItemTemplet value in NKMTempletContainer<ShopItemTemplet>.Values)
		{
			if (value.m_bEnabled && value.m_UnlockInfo.eReqType != STAGE_UNLOCK_REQ_TYPE.SURT_ALWAYS_UNLOCKED && !NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in value.m_UnlockInfo, bIgnoreSuperUser))
			{
				list.Add(value);
			}
		}
		return list;
	}

	public static List<ShopItemTemplet> GetLimitedCountSpecialItems()
	{
		List<ShopItemTemplet> list = new List<ShopItemTemplet>();
		NKMShopData shopData = NKCScenManager.CurrentUserData().m_ShopData;
		foreach (ShopItemTemplet value in NKMTempletContainer<ShopItemTemplet>.Values)
		{
			if (value.HasDateLimit && NKCSynchronizedTime.IsFinished(value.EventDateEndUtc))
			{
				continue;
			}
			if (value.m_TagImage == ShopItemRibbon.SPECIAL && value.resetType != SHOP_RESET_TYPE.Unlimited)
			{
				list.Add(value);
			}
			else if (shopData.GetRealPrice(value) == 0)
			{
				list.Add(value);
			}
			else if (GetReddotType(value) != ShopReddotType.NONE)
			{
				if (value.m_ItemType == NKM_REWARD_TYPE.RT_EMOTICON && GetBuyCountLeft(value.m_ProductID) == 0)
				{
					SetLastCheckedUTCTime(value);
				}
				else
				{
					list.Add(value);
				}
			}
		}
		return list;
	}

	public static Dictionary<int, string> GetMarketProductList()
	{
		Dictionary<int, string> dictionary = new Dictionary<int, string>();
		foreach (ShopItemTemplet value in NKMTempletContainer<ShopItemTemplet>.Values)
		{
			if (!string.IsNullOrEmpty(value.m_MarketID) && value.m_bEnabled && !dictionary.ContainsKey(value.m_ProductID))
			{
				dictionary.Add(value.m_ProductID, value.m_MarketID);
			}
		}
		return dictionary;
	}

	public static Dictionary<int, string> GetMarketAllProductList()
	{
		Dictionary<int, string> dictionary = new Dictionary<int, string>();
		foreach (ShopItemTemplet value in NKMTempletContainer<ShopItemTemplet>.Values)
		{
			if (!string.IsNullOrEmpty(value.m_MarketID) && !dictionary.ContainsKey(value.m_ProductID))
			{
				dictionary.Add(value.m_ProductID, value.m_MarketID);
			}
		}
		return dictionary;
	}

	public static bool IsMoveToShopDefined(int itemID)
	{
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(itemID);
		if (itemMiscTempletByID == null)
		{
			return false;
		}
		if (itemMiscTempletByID.m_lstRecommandProductItemIfNotEnough != null && itemMiscTempletByID.m_lstRecommandProductItemIfNotEnough.Count > 0)
		{
			return true;
		}
		NKCShopCategoryTemplet categoryFromTab = GetCategoryFromTab(itemMiscTempletByID.m_ShortCutShopTabID);
		ShopTabTemplet shopTabTemplet = ShopTabTemplet.Find(itemMiscTempletByID.m_ShortCutShopTabID, itemMiscTempletByID.m_ShortCutShopIndex);
		if (categoryFromTab != null)
		{
			return shopTabTemplet != null;
		}
		return false;
	}

	public static bool CanUsePopupShopBuy(int itemID)
	{
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(itemID);
		if (itemMiscTempletByID == null)
		{
			return false;
		}
		if (itemMiscTempletByID.m_lstRecommandProductItemIfNotEnough == null || itemMiscTempletByID.m_lstRecommandProductItemIfNotEnough.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < itemMiscTempletByID.m_lstRecommandProductItemIfNotEnough.Count; i++)
		{
			if (CanExhibitItem(ShopItemTemplet.Find(itemMiscTempletByID.m_lstRecommandProductItemIfNotEnough[i])))
			{
				return true;
			}
		}
		return false;
	}

	public static TabId GetShopMoveTab(int itemID)
	{
		TabId result = default(TabId);
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(itemID);
		if (itemMiscTempletByID != null)
		{
			result = new TabId(itemMiscTempletByID.m_ShortCutShopTabID, itemMiscTempletByID.m_ShortCutShopIndex);
		}
		return result;
	}

	public static bool CheckRecommendCond(SHOP_RECOMMEND_COND cond, string value)
	{
		NKMUserData user_data = NKCScenManager.CurrentUserData();
		if (cond != SHOP_RECOMMEND_COND.NONE && (uint)(cond - 1) <= 1u)
		{
			string[] array = value.Split(',', ' ');
			for (int i = 0; i < array.Length; i++)
			{
				if (int.TryParse(array[i], out var result) && result > 0)
				{
					ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(result);
					if (IsProductAvailable(shopItemTemplet, out var _) && CanBuyFixShop(user_data, shopItemTemplet, out var _, out var _) == NKM_ERROR_CODE.NEC_OK)
					{
						return true;
					}
				}
			}
			return false;
		}
		return true;
	}

	public static void OnInappPurchase(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		Debug.Log($"[InappPurchase] OnInappPurchase ResultCode[{resultCode}] Additional[{additionalError}]");
		switch (resultCode)
		{
		case NKC_PUBLISHER_RESULT_CODE.NPRC_INAPP_FAIL_NOT_SUPPORTED:
			NKCPopupMessageManager.AddPopupMessage(resultCode, additionalError);
			break;
		default:
			NKCPopupOKCancel.OpenOKBox(resultCode, additionalError);
			break;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_INAPP_FAIL_USER_CANCEL:
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCPublisherModule.GetErrorMessage(resultCode));
			break;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_INAPP_FAIL:
		case NKC_PUBLISHER_RESULT_CODE.NPRC_INAPP_FAIL_TRANSACTION_ERROR:
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_TOY_BILLING_PAYMENT_FAIL, NKCPublisherModule.LastError);
			break;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_INAPP_RESTORE_NEEDED_ITEM_VENDOR_NOT_CONSUMED:
			NKCPopupOKCancel.OpenOKBox(NKC_PUBLISHER_RESULT_CODE.NPRC_INAPP_RESTORE_NEEDED_ITEM_VENDOR_NOT_CONSUMED, additionalError, delegate
			{
				NKCPublisherModule.InAppPurchase.BillingRestore(OnBillingRestore);
			});
			break;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_OK:
			break;
		}
	}

	public static void OnBillingRestore(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		if (resultCode == NKC_PUBLISHER_RESULT_CODE.NPRC_INAPP_NOT_EXIST_RESTORE_ITEM || !NKCPublisherModule.InAppPurchase.IsBillingRestoreActive())
		{
			NKMPopUpBox.CloseWaitBox();
		}
		else if (NKCPublisherModule.CheckError(resultCode, additionalError, bCloseWaitBox: true, null, popupMessage: true))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_OPTION_BILLING_RESTORE_TITLE, NKCUtilString.GET_STRING_OPTION_BILLING_RESTORE_SUCCESS_DESC);
		}
	}

	public static void OnBillingRestoreManual(NKC_PUBLISHER_RESULT_CODE resultCode, string additionalError)
	{
		switch (resultCode)
		{
		case NKC_PUBLISHER_RESULT_CODE.NPRC_OK:
			NKMPopUpBox.CloseWaitBox();
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_OPTION_BILLING_RESTORE_TITLE, NKCUtilString.GET_STRING_OPTION_BILLING_RESTORE_SUCCESS_DESC);
			break;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_INAPP_NOT_EXIST_RESTORE_ITEM:
			NKMPopUpBox.CloseWaitBox();
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_OPTION_BILLING_RESTORE_TITLE, NKCUtilString.GET_STRING_OPTION_BILLING_RESTORE_EMPTY_LIST_DESC);
			break;
		case NKC_PUBLISHER_RESULT_CODE.NPRC_INAPP_FAIL_RESTORE:
			NKMPopUpBox.CloseWaitBox();
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_OPTION_BILLING_RESTORE_TITLE, NKCUtilString.GET_STRING_OPTION_BILLING_RESTORE_FAIL_DESC);
			break;
		default:
			NKCPublisherModule.CheckError(resultCode, additionalError, bCloseWaitBox: true, null, popupMessage: true);
			break;
		}
	}

	public static bool IsAllCustomSlotSelected(NKMItemMiscTemplet customItemTemplet, List<int> lstSelection)
	{
		if (customItemTemplet == null || !customItemTemplet.IsCustomPackageItem)
		{
			return false;
		}
		if (lstSelection == null)
		{
			return false;
		}
		if (lstSelection.Count != customItemTemplet.CustomPackageTemplets.Count)
		{
			return false;
		}
		for (int i = 0; i < customItemTemplet.CustomPackageTemplets.Count; i++)
		{
			int index = lstSelection[i];
			if (customItemTemplet.CustomPackageTemplets[i].Get(index) == null)
			{
				return false;
			}
		}
		return true;
	}

	public static bool IsCustomPackageSelectionHasDuplicate(NKMItemMiscTemplet customItemTemplet, int targetIndex, List<int> lstSelection, bool bIgnoreIfFirstItem)
	{
		if (customItemTemplet == null || !customItemTemplet.IsCustomPackageItem)
		{
			return false;
		}
		if (lstSelection == null)
		{
			return false;
		}
		if (targetIndex >= customItemTemplet.CustomPackageTemplets.Count)
		{
			return false;
		}
		if (targetIndex < 0)
		{
			return false;
		}
		NKMCustomPackageElement nKMCustomPackageElement = customItemTemplet.CustomPackageTemplets[targetIndex].Get(lstSelection[targetIndex]);
		if (!NKMItemManager.IsRedudantItemProhibited(nKMCustomPackageElement.RewardType, nKMCustomPackageElement.RewardId))
		{
			return false;
		}
		for (int i = 0; i < lstSelection.Count; i++)
		{
			if (i == targetIndex)
			{
				if (bIgnoreIfFirstItem)
				{
					return false;
				}
				continue;
			}
			int index = lstSelection[i];
			NKMCustomPackageElement nKMCustomPackageElement2 = customItemTemplet.CustomPackageTemplets[i].Get(index);
			if (nKMCustomPackageElement2 != null && nKMCustomPackageElement2.RewardType == nKMCustomPackageElement.RewardType && nKMCustomPackageElement2.RewardId == nKMCustomPackageElement.RewardId)
			{
				return true;
			}
		}
		return false;
	}

	public static NKMRewardInfo GetSubstituteItem(NKM_REWARD_TYPE rewardType, int ID, int overCount)
	{
		switch (rewardType)
		{
		case NKM_REWARD_TYPE.RT_MISC:
		{
			NKMItemMiscTemplet nKMItemMiscTemplet = NKMItemMiscTemplet.Find(ID);
			if (nKMItemMiscTemplet != null && nKMItemMiscTemplet.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_INTERIOR)
			{
				NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMOfficeInteriorTemplet.Find(ID);
				if (nKMOfficeInteriorTemplet != null)
				{
					return new NKMRewardInfo
					{
						ID = nKMOfficeInteriorTemplet.RefundItem.m_ItemMiscID,
						Count = (int)nKMOfficeInteriorTemplet.RefundItemPrice * overCount,
						rewardType = NKM_REWARD_TYPE.RT_MISC,
						paymentType = NKM_ITEM_PAYMENT_TYPE.NIPT_FREE
					};
				}
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_SKIN:
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(ID);
			if (skinTemplet != null)
			{
				return new NKMRewardInfo
				{
					ID = skinTemplet.m_ReturnItemId,
					Count = skinTemplet.m_ReturnItemCount * overCount,
					rewardType = NKM_REWARD_TYPE.RT_MISC,
					paymentType = NKM_ITEM_PAYMENT_TYPE.NIPT_FREE
				};
			}
			break;
		}
		}
		return null;
	}

	public static int GetItemOverCount(NKM_REWARD_TYPE rewardType, int itemID, int gainCount)
	{
		switch (rewardType)
		{
		case NKM_REWARD_TYPE.RT_MISC:
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(itemID);
			if (itemMiscTempletByID == null)
			{
				return 0;
			}
			if (itemMiscTempletByID.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_INTERIOR)
			{
				NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMOfficeInteriorTemplet.Find(itemID);
				int num = (int)NKCScenManager.CurrentUserData().OfficeData.GetInteriorCount(itemID);
				int maxStack = nKMOfficeInteriorTemplet.MaxStack;
				return gainCount + num - maxStack;
			}
			if (NKMItemManager.IsRedudantItemProhibited(itemMiscTempletByID.m_ItemMiscType, itemMiscTempletByID.m_ItemMiscSubType))
			{
				return (int)(NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(itemID) + gainCount - 1);
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_SKIN:
			if (!NKCScenManager.CurrentUserData().m_InventoryData.HasItemSkin(itemID))
			{
				return gainCount - 1;
			}
			return gainCount;
		case NKM_REWARD_TYPE.RT_EMOTICON:
			if (!NKCEmoticonManager.HasEmoticon(itemID))
			{
				return gainCount - 1;
			}
			return gainCount;
		}
		return 0;
	}

	public static bool WillOverflowOnGain(NKM_REWARD_TYPE rewardType, int itemID, int gainCount)
	{
		return GetItemOverCount(rewardType, itemID, gainCount) > 0;
	}

	public static bool IsHaveUnit(NKM_REWARD_TYPE rewardType, int itemID)
	{
		return rewardType switch
		{
			NKM_REWARD_TYPE.RT_UNIT => NKCScenManager.CurrentUserData().m_ArmyData.HaveUnit(itemID, bIncludeRearm: false), 
			NKM_REWARD_TYPE.RT_OPERATOR => NKCScenManager.CurrentUserData().m_ArmyData.GetOperatorCountByID(itemID) > 0, 
			NKM_REWARD_TYPE.RT_SHIP => NKCScenManager.CurrentUserData().m_ArmyData.GetSameKindShipCountFromID(itemID) > 0, 
			_ => false, 
		};
	}

	public static List<ShopRewardSubstituteData> MakeShopBuySubstituteItemList(ShopItemTemplet shopItemTemplet, int buyCount, List<int> lstSelection)
	{
		if (shopItemTemplet == null)
		{
			return null;
		}
		List<ShopRewardSubstituteData> list = new List<ShopRewardSubstituteData>();
		NKMItemMiscTemplet nKMItemMiscTemplet = null;
		if (shopItemTemplet.m_ItemType == NKM_REWARD_TYPE.RT_MISC)
		{
			nKMItemMiscTemplet = NKMItemManager.GetItemMiscTempletByID(shopItemTemplet.m_ItemID);
		}
		if (nKMItemMiscTemplet == null || (!nKMItemMiscTemplet.IsCustomPackageItem && !nKMItemMiscTemplet.IsPackageItem))
		{
			int itemOverCount = GetItemOverCount(shopItemTemplet.m_ItemType, shopItemTemplet.m_ItemID, buyCount);
			if (itemOverCount <= 0)
			{
				return null;
			}
			if (itemOverCount == buyCount)
			{
				return null;
			}
			NKMRewardInfo substituteItem = GetSubstituteItem(shopItemTemplet.m_ItemType, shopItemTemplet.m_ItemID, itemOverCount);
			if (substituteItem == null)
			{
				return null;
			}
			NKMRewardInfo before = new NKMRewardInfo
			{
				rewardType = shopItemTemplet.m_ItemType,
				ID = shopItemTemplet.m_ItemID,
				Count = itemOverCount
			};
			list.Add(new ShopRewardSubstituteData
			{
				Before = before,
				After = substituteItem
			});
			return list;
		}
		List<NKMRandomBoxItemTemplet> randomBoxItemTempletList = NKCRandomBoxManager.GetRandomBoxItemTempletList(nKMItemMiscTemplet.m_RewardGroupID);
		if (nKMItemMiscTemplet.m_RewardGroupID != 0 && randomBoxItemTempletList == null)
		{
			Log.Error("rewardgroup null! ID : " + nKMItemMiscTemplet.m_RewardGroupID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCShopManager.cs", 1412);
		}
		if (randomBoxItemTempletList != null)
		{
			for (int i = 0; i < randomBoxItemTempletList.Count; i++)
			{
				if (MakeSubstituteItem(randomBoxItemTempletList[i], buyCount, out var data))
				{
					list.Add(data);
				}
			}
		}
		if (nKMItemMiscTemplet.CustomPackageTemplets != null && lstSelection != null)
		{
			for (int j = 0; j < nKMItemMiscTemplet.CustomPackageTemplets.Count; j++)
			{
				int index = lstSelection[j];
				NKMCustomPackageElement nKMCustomPackageElement = nKMItemMiscTemplet.CustomPackageTemplets[j].Get(index);
				if (nKMCustomPackageElement == null)
				{
					continue;
				}
				int itemOverCount2 = GetItemOverCount(nKMCustomPackageElement.RewardType, nKMCustomPackageElement.RewardId, nKMCustomPackageElement.TotalRewardCount);
				if (itemOverCount2 > 0 || IsCustomPackageSelectionHasDuplicate(nKMItemMiscTemplet, j, lstSelection, bIgnoreIfFirstItem: true))
				{
					NKMRewardInfo substituteItem2 = GetSubstituteItem(nKMCustomPackageElement.RewardType, nKMCustomPackageElement.RewardId, itemOverCount2);
					if (substituteItem2 != null)
					{
						NKMRewardInfo before2 = new NKMRewardInfo
						{
							rewardType = nKMCustomPackageElement.RewardType,
							ID = nKMCustomPackageElement.RewardId,
							Count = itemOverCount2
						};
						list.Add(new ShopRewardSubstituteData
						{
							Before = before2,
							After = substituteItem2
						});
					}
				}
			}
		}
		return list;
	}

	public static bool MakeSubstituteItem(NKMRandomBoxItemTemplet boxItemTemplet, int count, out ShopRewardSubstituteData data)
	{
		int itemOverCount = GetItemOverCount(boxItemTemplet.m_reward_type, boxItemTemplet.m_RewardID, boxItemTemplet.TotalQuantity_Max * count);
		if (itemOverCount <= 0)
		{
			data = default(ShopRewardSubstituteData);
			return false;
		}
		NKMRewardInfo substituteItem = GetSubstituteItem(boxItemTemplet.m_reward_type, boxItemTemplet.m_RewardID, itemOverCount);
		if (substituteItem == null)
		{
			data = default(ShopRewardSubstituteData);
			return false;
		}
		NKMRewardInfo before = new NKMRewardInfo
		{
			rewardType = boxItemTemplet.m_reward_type,
			ID = boxItemTemplet.m_RewardID,
			Count = itemOverCount
		};
		data = new ShopRewardSubstituteData
		{
			Before = before,
			After = substituteItem
		};
		return true;
	}

	public static string EncodeCustomPackageSelectList(List<int> lstSelection)
	{
		if (lstSelection == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (int item in lstSelection)
		{
			stringBuilder.Append(item);
			stringBuilder.Append(',');
		}
		return stringBuilder.ToString().TrimEnd(',');
	}

	public static List<int> DecodeCustomPackageSelectList(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return null;
		}
		string[] array = value.Split(',', ' ');
		List<int> list = new List<int>();
		for (int i = 0; i < array.Length; i++)
		{
			if (int.TryParse(array[i], out var result))
			{
				list.Add(result);
				continue;
			}
			Log.Error("DecodeCustomPackageSelectList : Bad input!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCShopManager.cs", 1542);
			return null;
		}
		return list;
	}

	public static NKM_ERROR_CODE OnBtnProductBuy(int ProductID, bool bSupply)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (bSupply)
		{
			NKMShopRandomData randomShop = myUserData.m_ShopData.randomShop;
			if (!randomShop.datas.ContainsKey(ProductID))
			{
				Log.Error("invalid index " + ProductID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCShopManager.cs", 1563);
				return NKM_ERROR_CODE.NEC_FAIL_INVALID_SHOP_ID;
			}
			NKMShopRandomListData nKMShopRandomListData = randomShop.datas[ProductID];
			if (nKMShopRandomListData.isBuy)
			{
				return NKM_ERROR_CODE.NEC_FAIL_LIMITED_SHOP_COUNT_FAIL;
			}
			NKCPopupItemBox.Instance.Open(nKMShopRandomListData, showDropInfo: false, delegate
			{
				TrySupplyProductBuy(ProductID);
			});
		}
		else
		{
			ShopItemTemplet productTemplet = ShopItemTemplet.Find(ProductID);
			if (!myUserData.IsSuperUser())
			{
				bool is_init;
				long next_reset_date;
				NKM_ERROR_CODE nKM_ERROR_CODE = CanBuyFixShop(myUserData, productTemplet, out is_init, out next_reset_date);
				if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
				{
					return nKM_ERROR_CODE;
				}
			}
			if (productTemplet.IsSubscribeItem() || productTemplet.m_PurchaseEventType == PURCHASE_EVENT_REWARD_TYPE.LEVELUP_PACKAGE)
			{
				NKCPopupShopBuyConfirm.Instance.Open(productTemplet, TryProductBuy);
				SetLastCheckedUTCTime(productTemplet);
				return NKM_ERROR_CODE.NEC_OK;
			}
			switch (productTemplet.m_ItemType)
			{
			case NKM_REWARD_TYPE.RT_EMOTICON:
				NKCPopupItemBox.Instance.Open(productTemplet, delegate
				{
					NKCPopupShopBuyConfirm.Instance.Open(productTemplet, TryProductBuy);
				});
				break;
			case NKM_REWARD_TYPE.RT_SKIN:
				NKCUIShopSkinPopup.Instance.OpenForShop(productTemplet);
				break;
			case NKM_REWARD_TYPE.RT_MISC:
			{
				NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(productTemplet.m_ItemID);
				if (itemMiscTempletByID != null)
				{
					if (itemMiscTempletByID.IsPackageItem)
					{
						NKCPopupShopPackageConfirm.Instance.Open(productTemplet, TryProductBuy);
					}
					else if (itemMiscTempletByID.IsCustomPackageItem)
					{
						NKCPopupShopCustomPackage.Instance.Open(productTemplet, TryProductBuy);
					}
					else
					{
						NKCPopupShopBuyConfirm.Instance.Open(productTemplet, TryProductBuy);
					}
				}
				else
				{
					Log.Error($"NKMItemMiscTemplet is null - {productTemplet.m_ItemID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCShopManager.cs", 1631);
				}
				break;
			}
			default:
				NKCPopupShopBuyConfirm.Instance.Open(productTemplet, TryProductBuy);
				break;
			}
			SetLastCheckedUTCTime(productTemplet);
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE OnMultipleProductBuy(HashSet<int> hsProductID, bool bSupply)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Prepare buy : ");
		foreach (int item in hsProductID)
		{
			stringBuilder.Append(item.ToString());
			stringBuilder.Append(", ");
		}
		Log.Warn(stringBuilder.ToString(), "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCShopManager.cs", 1664);
		if (bSupply)
		{
			NKMShopRandomData randomShop = myUserData.m_ShopData.randomShop;
			foreach (int item2 in hsProductID)
			{
				if (!randomShop.datas.ContainsKey(item2))
				{
					Log.Error("invalid index " + item2, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCShopManager.cs", 1676);
					return NKM_ERROR_CODE.NEC_FAIL_INVALID_SHOP_ID;
				}
				if (randomShop.datas[item2].isBuy)
				{
					return NKM_ERROR_CODE.NEC_FAIL_LIMITED_SHOP_COUNT_FAIL;
				}
			}
			NKCPopupShopMultiBuy.Instance.OpenForSupply(hsProductID, delegate
			{
				TrySupplyProductMultiBuy(hsProductID);
			});
		}
		else
		{
			foreach (int item3 in hsProductID)
			{
				ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(item3);
				if (!myUserData.IsSuperUser())
				{
					bool is_init;
					long next_reset_date;
					NKM_ERROR_CODE nKM_ERROR_CODE = CanBuyFixShop(myUserData, shopItemTemplet, out is_init, out next_reset_date);
					if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
					{
						return nKM_ERROR_CODE;
					}
				}
				if (shopItemTemplet.IsInAppProduct)
				{
					return NKM_ERROR_CODE.NEC_FAIL_INVALID_SHOP_ID;
				}
				if (shopItemTemplet.IsSubscribeItem() || shopItemTemplet.m_PurchaseEventType == PURCHASE_EVENT_REWARD_TYPE.LEVELUP_PACKAGE)
				{
					return NKM_ERROR_CODE.NEC_FAIL_INVALID_SHOP_ID;
				}
			}
			NKCPopupShopMultiBuy.Instance.Open(hsProductID, delegate
			{
				TryProductMultiBuy(hsProductID);
			});
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static void TryProductBuy(int ProductID, int ProductCount = 1, List<int> lstSelection = null)
	{
		ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(ProductID);
		if (GetBuyCountLeft(ProductID) == 0)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_PF_SHOP_SOLD_OUT"));
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (!myUserData.HaveEnoughResourceToBuy(shopItemTemplet, ProductCount))
		{
			OpenItemLackPopup(shopItemTemplet.m_PriceItemID, myUserData.m_ShopData.GetRealPrice(shopItemTemplet) * ProductCount);
			return;
		}
		if (shopItemTemplet.m_PurchaseEventType == PURCHASE_EVENT_REWARD_TYPE.CONSUMER_PACKAGE)
		{
			if (NKCScenManager.CurrentUserData().GetConsumerPackageData(ProductID, out var _))
			{
				NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_CONSUMER_PACKAGE_ALREADY_PURCHASED);
				return;
			}
			if (NKCSynchronizedTime.IsFinished(shopItemTemplet.EventDateEndUtc.AddDays(-5.0)))
			{
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_DP_SHOP_PURCHASE_WARNING", NKCUtilString.GetRemainTimeString(shopItemTemplet.EventDateEndUtc, 1)), delegate
				{
					BuyProductInternal(ProductID, ProductCount, lstSelection);
				});
				return;
			}
		}
		if (shopItemTemplet.m_ItemType == NKM_REWARD_TYPE.RT_SKIN)
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(shopItemTemplet.m_ItemID);
			if (skinTemplet == null)
			{
				NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_INVALID_SKIN_ITEM_ID);
				return;
			}
			if (!NKCScenManager.CurrentUserData().m_ArmyData.HaveUnit(skinTemplet.m_SkinEquipUnitID, bIncludeRearm: true))
			{
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_PF_SHOP_SKIN_NO_UNIT_NOTICE"), delegate
				{
					BuyProductInternal(ProductID, ProductCount, lstSelection);
				});
				return;
			}
		}
		BuyProductInternal(ProductID, ProductCount, lstSelection);
	}

	private static void BuyProductInternal(int ProductID, int ProductCount = 1, List<int> lstSelection = null)
	{
		ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(ProductID);
		if (shopItemTemplet.m_PriceItemID == 0)
		{
			NKCPacketSender.Send_NKMPacket_SHOP_FIX_SHOP_CASH_BUY_POSSIBLE_REQ(shopItemTemplet.m_MarketID, lstSelection);
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_SHOP_FIX_SHOP_BUY_REQ(ProductID, ProductCount, lstSelection);
		}
	}

	private static void TrySupplyProductBuy(int index)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMShopRandomListData nKMShopRandomListData = myUserData.m_ShopData.randomShop.datas[index];
		int price = nKMShopRandomListData.GetPrice();
		if (myUserData.CheckPrice(price, nKMShopRandomListData.priceItemId))
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_SHOP().Send_NKMPacket_SHOP_RANDOM_SHOP_BUY_REQ(index);
		}
		else
		{
			OpenItemLackPopup(nKMShopRandomListData.priceItemId, nKMShopRandomListData.GetPrice());
		}
	}

	private static void TryProductMultiBuy(HashSet<int> hsProducrID)
	{
		Debug.LogError("Not Implemented!");
	}

	private static void TrySupplyProductMultiBuy(HashSet<int> hsIndex)
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMShopRandomData randomShop = myUserData.m_ShopData.randomShop;
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		foreach (int item in hsIndex)
		{
			NKMShopRandomListData nKMShopRandomListData = randomShop.datas[item];
			if (!dictionary.TryGetValue(nKMShopRandomListData.priceItemId, out var value))
			{
				value = 0;
			}
			int price = nKMShopRandomListData.GetPrice();
			dictionary[nKMShopRandomListData.priceItemId] = value + price;
		}
		foreach (KeyValuePair<int, int> item2 in dictionary)
		{
			int key = item2.Key;
			int value2 = item2.Value;
			if (!myUserData.CheckPrice(value2, key))
			{
				OpenItemLackPopup(key, value2);
				return;
			}
		}
		NKCPacketSender.Send_NKMPacket_SHOP_RANDOM_SHOP_BUY_LIST_REQ(hsIndex.ToList());
	}

	private static void LoadFeaturedTemplet()
	{
		if (!bFeaturedTempletLoaded)
		{
			bFeaturedTempletLoaded = true;
			NKCShopFeaturedTemplet.Load();
		}
	}

	public static List<NKCShopFeaturedTemplet> GetFeaturedList(NKMUserData userData, string packageGroupID, bool bUseExhibitCount)
	{
		if (!bFeaturedTempletLoaded)
		{
			LoadFeaturedTemplet();
		}
		List<NKCShopFeaturedTemplet> list = new List<NKCShopFeaturedTemplet>();
		foreach (NKCShopFeaturedTemplet value in NKMTempletContainer<NKCShopFeaturedTemplet>.Values)
		{
			if (packageGroupID.Equals(value.m_PackageGroupID, StringComparison.InvariantCultureIgnoreCase) && value.CheckCondition(userData))
			{
				ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(value.m_PackageID);
				if (shopItemTemplet == null)
				{
					Log.Error($"피쳐드 템플릿에 지정된 상품 {value.m_PackageID}이 존재하지 않음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCShopManager.cs", 1886);
				}
				else if ((!value.m_ReddotRequired || (shopItemTemplet.m_Reddot != ShopReddotType.NONE && shopItemTemplet.m_hsReddotAllow.Contains(NKMContentsVersionManager.GetCountryTag()))) && CanExhibitItem(shopItemTemplet) && GetBuyCountLeft(shopItemTemplet.m_ProductID) != 0)
				{
					list.Add(value);
				}
			}
		}
		list.Sort(CompareByReddotDescending);
		if (userData.m_ShopData.GetTotalPayment() >= NKMCommonConst.FeaturedListTotalPaymentThreshold)
		{
			list.Sort(NKCShopFeaturedTemplet.CompareHighPriceFirst);
		}
		else
		{
			list.Sort(NKCShopFeaturedTemplet.CompareLowPriceFirst);
		}
		if (bUseExhibitCount && list.Count > NKMCommonConst.FeaturedListExhibitCount)
		{
			list.RemoveRange(NKMCommonConst.FeaturedListExhibitCount, list.Count - NKMCommonConst.FeaturedListExhibitCount);
		}
		return list;
	}

	private static int CompareByReddotDescending(NKCShopFeaturedTemplet lhs, NKCShopFeaturedTemplet rhs)
	{
		ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(lhs.m_PackageID);
		return GetReddotType(ShopItemTemplet.Find(rhs.m_PackageID)).CompareTo(GetReddotType(shopItemTemplet));
	}

	public static void Drop()
	{
		bFeaturedTempletLoaded = false;
		bLevelupPackageTempletLoaded = false;
	}

	public static void LoadLevelupPackageTemplet()
	{
		if (!bLevelupPackageTempletLoaded)
		{
			NKMTempletContainer<ShopLevelUpPackageGroupTemplet>.Load(from e in NKMTempletLoader<ShopLevelUpPackageGroupData>.LoadGroup("AB_SCRIPT", "LUA_LEVELUP_PACKAGE_TEMPLET", "LEVELUP_PACKAGE_TEMPLET", ShopLevelUpPackageGroupData.LoadFromLUA)
				select new ShopLevelUpPackageGroupTemplet(e.Key, e.Value), null);
			bLevelupPackageTempletLoaded = true;
		}
	}

	public static ShopLevelUpPackageGroupTemplet GetLevelUpPackageGroupTemplet(int key)
	{
		if (!bLevelupPackageTempletLoaded)
		{
			LoadLevelupPackageTemplet();
		}
		return NKMTempletContainer<ShopLevelUpPackageGroupTemplet>.Find(key);
	}

	public static int GetBuyCountLeft(int ProductID)
	{
		ShopItemTemplet shopItemTemplet = ShopItemTemplet.Find(ProductID);
		if (shopItemTemplet == null)
		{
			return 0;
		}
		switch (shopItemTemplet.m_ItemType)
		{
		case NKM_REWARD_TYPE.RT_SKIN:
			if (NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.HasItemSkin(shopItemTemplet.m_ItemID))
			{
				return 0;
			}
			break;
		case NKM_REWARD_TYPE.RT_EMOTICON:
			if (NKCEmoticonManager.HasEmoticon(shopItemTemplet.m_ItemID))
			{
				return 0;
			}
			break;
		case NKM_REWARD_TYPE.RT_MISC:
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(shopItemTemplet.m_ItemID);
			if (itemMiscTempletByID == null)
			{
				return 0;
			}
			if (NKMItemManager.IsRedudantItemProhibited(itemMiscTempletByID.m_ItemMiscType, itemMiscTempletByID.m_ItemMiscSubType) && NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(shopItemTemplet.m_ItemID) > 0)
			{
				return 0;
			}
			break;
		}
		}
		if (shopItemTemplet.TabTemplet != null)
		{
			if (shopItemTemplet.TabTemplet.IsChainTab)
			{
				if (shopItemTemplet.resetType == SHOP_RESET_TYPE.Unlimited && shopItemTemplet.m_QuantityLimit <= 0)
				{
					return -1;
				}
			}
			else if (shopItemTemplet.resetType == SHOP_RESET_TYPE.Unlimited)
			{
				return -1;
			}
		}
		if (NKCScenManager.GetScenManager().GetMyUserData().m_ShopData.histories.TryGetValue(ProductID, out var value))
		{
			if (shopItemTemplet.IsCountResetType() && NKCSynchronizedTime.IsFinished(value.nextResetDate))
			{
				return shopItemTemplet.m_QuantityLimit;
			}
			return shopItemTemplet.m_QuantityLimit - value.purchaseCount;
		}
		int a = int.MaxValue;
		if (shopItemTemplet.m_ItemType == NKM_REWARD_TYPE.RT_MISC)
		{
			NKMItemMiscTemplet itemMiscTempletByID2 = NKMItemManager.GetItemMiscTempletByID(shopItemTemplet.m_ItemID);
			if (itemMiscTempletByID2.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_INTERIOR)
			{
				NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMOfficeInteriorTemplet.Find(itemMiscTempletByID2.Key);
				if (nKMOfficeInteriorTemplet != null)
				{
					int num = (int)NKCScenManager.CurrentUserData().OfficeData.GetInteriorCount(nKMOfficeInteriorTemplet);
					a = nKMOfficeInteriorTemplet.MaxStack - num;
				}
			}
		}
		return Mathf.Min(a, shopItemTemplet.m_QuantityLimit);
	}

	public static string GetBuyCountString(SHOP_RESET_TYPE type, int currentCount, int maxCount, bool bRemoveBracket = false)
	{
		string empty = string.Empty;
		switch (type)
		{
		case SHOP_RESET_TYPE.FIXED:
			empty = string.Format(NKCUtilString.GET_STRING_SHOP_ACCOUNT_PURCHASE_COUNT_TWO_PARAM, currentCount, maxCount);
			break;
		case SHOP_RESET_TYPE.DAY:
			empty = string.Format(NKCUtilString.GET_STRING_SHOP_DAY_PURCHASE_COUNT_TWO_PARAM, currentCount, maxCount);
			break;
		case SHOP_RESET_TYPE.WEEK:
		case SHOP_RESET_TYPE.WEEK_SUN:
		case SHOP_RESET_TYPE.WEEK_MON:
		case SHOP_RESET_TYPE.WEEK_TUE:
		case SHOP_RESET_TYPE.WEEK_WED:
		case SHOP_RESET_TYPE.WEEK_THU:
		case SHOP_RESET_TYPE.WEEK_FRI:
		case SHOP_RESET_TYPE.WEEK_SAT:
			empty = string.Format(NKCUtilString.GET_STRING_SHOP_WEEK_PURCHASE_COUNT_TWO_PARAM, currentCount, maxCount);
			break;
		case SHOP_RESET_TYPE.MONTH:
			empty = string.Format(NKCUtilString.GET_STRING_SHOP_MONTH_PURCHASE_COUNT_TWO_PARAM, currentCount, maxCount);
			break;
		default:
			empty = string.Format(NKCUtilString.GET_STRING_SHOP_PURCHASE_COUNT_TWO_PARAM, currentCount, maxCount);
			break;
		case SHOP_RESET_TYPE.Unlimited:
			return "";
		}
		if (bRemoveBracket)
		{
			empty = empty.Substring(1, empty.Length - 2);
		}
		return empty;
	}

	public static void OpenItemLackPopup(int itemID, int itemCnt)
	{
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(itemID);
		bool flag = false;
		if (itemMiscTempletByID != null && itemMiscTempletByID.m_lstRecommandProductItemIfNotEnough != null && itemMiscTempletByID.m_lstRecommandProductItemIfNotEnough.Count > 0)
		{
			foreach (int item in itemMiscTempletByID.m_lstRecommandProductItemIfNotEnough)
			{
				if (CanExhibitItem(ShopItemTemplet.Find(item)) && GetBuyCountLeft(item) != 0)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			if (itemMiscTempletByID.m_lstRecommandProductItemIfNotEnough.Count == 1)
			{
				OnBtnProductBuy(itemMiscTempletByID.m_lstRecommandProductItemIfNotEnough[0], bSupply: false);
			}
			else
			{
				NKCPopupShopBuyShortcut.Open(itemMiscTempletByID);
			}
		}
		else if (itemID == 24)
		{
			NKCPopupItemLack.Instance.OpenItemMiscLackPopup(itemID, itemCnt, (NKCGuildManager.MyGuildData != null) ? NKCGuildManager.MyGuildData.unionPoint : 0);
		}
		else
		{
			NKCPopupItemLack.Instance.OpenItemMiscLackPopup(itemID, itemCnt);
		}
	}

	public static List<ShopItemTemplet> GetLinkedItem(int productID)
	{
		if (s_dicLinkedItemCache.TryGetValue(productID, out var value))
		{
			return value;
		}
		List<ShopItemTemplet> list = new List<ShopItemTemplet>();
		foreach (ShopItemTemplet value2 in NKMTempletContainer<ShopItemTemplet>.Values)
		{
			if (value2.m_UnlockInfo.eReqType == STAGE_UNLOCK_REQ_TYPE.SURT_SHOP_BUY_ITEM_ALL && value2.m_UnlockInfo.reqValue == productID && CanExhibitItem(value2, bIncludeLockedItemWithReason: false, bIgnoreContentUnlock: true))
			{
				list.Add(value2);
			}
		}
		if (list.Count == 0)
		{
			s_dicLinkedItemCache[productID] = null;
			return null;
		}
		s_dicLinkedItemCache[productID] = list;
		return list;
	}

	public static bool HasLinkedItem(int productID)
	{
		List<ShopItemTemplet> linkedItem = GetLinkedItem(productID);
		if (linkedItem != null)
		{
			return linkedItem.Count > 0;
		}
		return false;
	}

	public static void ClearLinkedItemCache()
	{
		s_dicLinkedItemCache.Clear();
	}

	public static List<int> GetUpsideMenuItemList(ShopItemTemplet shopTemplet)
	{
		if (shopTemplet == null)
		{
			return PACKAGE_RESOURCE_LIST;
		}
		ShopTabTemplet shopTabTemplet = ShopTabTemplet.Find(shopTemplet.m_TabID, shopTemplet.m_TabSubIndex);
		if (shopTabTemplet != null)
		{
			List<int> list = MakeShopTabResourceList(shopTabTemplet);
			if (list.Count > 0)
			{
				if (shopTemplet.m_PriceItemID != 0 && !list.Contains(shopTemplet.m_PriceItemID))
				{
					list.Add(shopTemplet.m_PriceItemID);
				}
				return list;
			}
		}
		if (shopTemplet.m_PriceItemID == 0)
		{
			return PACKAGE_RESOURCE_LIST;
		}
		if (PACKAGE_RESOURCE_LIST.Contains(shopTemplet.m_PriceItemID))
		{
			return PACKAGE_RESOURCE_LIST;
		}
		return new List<int> { 101, 102, shopTemplet.m_PriceItemID };
	}

	public static List<int> MakeShopTabResourceList(ShopTabTemplet tabTemplet)
	{
		if (tabTemplet == null)
		{
			return PACKAGE_RESOURCE_LIST;
		}
		List<int> list = new List<int>();
		if (tabTemplet.m_ResourceTypeID_1 > 0)
		{
			list.Add(tabTemplet.m_ResourceTypeID_1);
		}
		if (tabTemplet.m_ResourceTypeID_2 > 0)
		{
			list.Add(tabTemplet.m_ResourceTypeID_2);
		}
		if (tabTemplet.m_ResourceTypeID_3 > 0)
		{
			list.Add(tabTemplet.m_ResourceTypeID_3);
		}
		if (tabTemplet.m_ResourceTypeID_4 > 0)
		{
			list.Add(tabTemplet.m_ResourceTypeID_4);
		}
		if (tabTemplet.m_ResourceTypeID_5 > 0)
		{
			list.Add(tabTemplet.m_ResourceTypeID_5);
		}
		if (list.Count == 0)
		{
			list = PACKAGE_RESOURCE_LIST;
		}
		return list;
	}

	public static bool CanDisplayTab(string tabType, bool UseTabVisible)
	{
		foreach (ShopTabTemplet value in ShopTabTemplet.Values)
		{
			if (value.TabType == tabType && value.m_ShopDisplay != ShopDisplayType.None && (!UseTabVisible || value.m_Visible) && value.EnableByTag && NKCSynchronizedTime.IsEventTime(value.intervalId, value.EventDateStartUtc, value.EventDateEndUtc))
			{
				return true;
			}
		}
		return false;
	}

	public static Color GetRibbonColor(ShopItemRibbon ribbonType)
	{
		return ribbonType switch
		{
			ShopItemRibbon.NEW => NKCUtil.GetColor("#FFC528"), 
			ShopItemRibbon.EVENT => NKCUtil.GetColor("#E996F3"), 
			ShopItemRibbon.HOT => NKCUtil.GetColor("#FF4B40"), 
			ShopItemRibbon.BEST => NKCUtil.GetColor("#FFC528"), 
			_ => Color.white, 
		};
	}

	public static string GetRibbonString(ShopItemRibbon ribbonType)
	{
		return ribbonType switch
		{
			ShopItemRibbon.ONE_PLUS_ONE => "1 + 1", 
			ShopItemRibbon.FIRST_PURCHASE => NKCUtilString.GET_STRING_SHOP_FIRST_PURCHASE, 
			ShopItemRibbon.TIME_LIMITED => NKCUtilString.GET_STRING_SHOP_TIME_LIMIT, 
			ShopItemRibbon.NEW => NKCUtilString.GET_STRING_SHOP_NEW, 
			ShopItemRibbon.LIMITED => NKCUtilString.GET_STRING_SHOP_LIMIT, 
			ShopItemRibbon.POPULAR => NKCUtilString.GET_STRING_SHOP_POPULAR, 
			ShopItemRibbon.SPECIAL => NKCUtilString.GET_STRING_SHOP_SPECIAL, 
			ShopItemRibbon.EVENT => NKCStringTable.GetString("SI_DP_SHOP_EVENT"), 
			ShopItemRibbon.HOT => NKCStringTable.GetString("SI_DP_SHOP_HOT"), 
			ShopItemRibbon.BEST => NKCUtilString.GET_STRING_SHOP_BEST, 
			_ => "", 
		};
	}

	public static bool IsShowPurchasePolicy()
	{
		return NKCPublisherModule.InAppPurchase.ShowPurchasePolicy();
	}

	public static bool IsShowPurchasePolicyBtn()
	{
		return NKCPublisherModule.InAppPurchase.ShowPurchasePolicyBtn();
	}

	public static bool ShowShopItemCashCount(NKCUISlot slot, NKCUISlot.SlotData slotData, int freeCount, int paidCount)
	{
		if (slot == null)
		{
			return false;
		}
		if (UseSuperuserItemCount())
		{
			string itemCountString = GetItemCountString(freeCount, paidCount);
			slot.SetSlotItemCountString(NKCUISlot.WillShowCount(slotData), itemCountString);
			return true;
		}
		return false;
	}

	public static bool ShowShopItemCashCount(Text label, int freeCount, int paidCount)
	{
		if (label == null)
		{
			return false;
		}
		if (UseSuperuserItemCount())
		{
			string itemCountString = GetItemCountString(freeCount, paidCount);
			NKCUtil.SetLabelText(label, itemCountString);
			return true;
		}
		return false;
	}

	public static string GetItemCountString(long freeCount, long cashCount)
	{
		return (freeCount + cashCount).ToString("N0");
	}

	public static bool UseSuperuserItemCount()
	{
		if (!NKCDefineManager.DEFINE_SERVICE() && NKCScenManager.CurrentUserData().IsSuperUser())
		{
			return true;
		}
		return false;
	}

	public static NKM_ITEM_GRADE GetGrade(ShopItemTemplet templet)
	{
		switch (templet.m_ItemType)
		{
		case NKM_REWARD_TYPE.RT_NONE:
			return NKM_ITEM_GRADE.NIG_N;
		case NKM_REWARD_TYPE.RT_UNIT:
		case NKM_REWARD_TYPE.RT_SHIP:
		case NKM_REWARD_TYPE.RT_OPERATOR:
			return NKMUnitTempletBase.Find(templet.m_ItemID).m_NKM_UNIT_GRADE switch
			{
				NKM_UNIT_GRADE.NUG_R => NKM_ITEM_GRADE.NIG_R, 
				NKM_UNIT_GRADE.NUG_SR => NKM_ITEM_GRADE.NIG_SR, 
				NKM_UNIT_GRADE.NUG_SSR => NKM_ITEM_GRADE.NIG_SSR, 
				_ => NKM_ITEM_GRADE.NIG_N, 
			};
		case NKM_REWARD_TYPE.RT_SKIN:
			switch (NKMSkinManager.GetSkinTemplet(templet.m_ItemID).m_SkinGrade)
			{
			case NKMSkinTemplet.SKIN_GRADE.SG_VARIATION:
				return NKM_ITEM_GRADE.NIG_N;
			default:
				return NKM_ITEM_GRADE.NIG_R;
			case NKMSkinTemplet.SKIN_GRADE.SG_RARE:
				return NKM_ITEM_GRADE.NIG_SR;
			case NKMSkinTemplet.SKIN_GRADE.SG_PREMIUM:
			case NKMSkinTemplet.SKIN_GRADE.SG_SPECIAL:
				return NKM_ITEM_GRADE.NIG_SSR;
			}
		case NKM_REWARD_TYPE.RT_MISC:
			return NKMItemMiscTemplet.Find(templet.m_ItemID).m_NKM_ITEM_GRADE;
		case NKM_REWARD_TYPE.RT_EMOTICON:
			return NKMEmoticonTemplet.Find(templet.m_ItemID).m_EmoticonGrade switch
			{
				NKM_EMOTICON_GRADE.NEG_R => NKM_ITEM_GRADE.NIG_R, 
				NKM_EMOTICON_GRADE.NEG_SR => NKM_ITEM_GRADE.NIG_SR, 
				NKM_EMOTICON_GRADE.NEG_SSR => NKM_ITEM_GRADE.NIG_SSR, 
				_ => NKM_ITEM_GRADE.NIG_N, 
			};
		case NKM_REWARD_TYPE.RT_EQUIP:
			return NKMItemManager.GetEquipTemplet(templet.m_ItemID).m_NKM_ITEM_GRADE;
		case NKM_REWARD_TYPE.RT_MOLD:
			return NKMItemMoldTemplet.Find(templet.m_ItemID).m_Grade;
		default:
			return NKM_ITEM_GRADE.NIG_N;
		}
	}

	public static List<ShopTabTemplet> GetUseTabList(ShopTabCategory category)
	{
		List<ShopTabTemplet> list = new List<ShopTabTemplet>();
		NKCShopCategoryTemplet nKCShopCategoryTemplet = NKCShopCategoryTemplet.Find(category);
		if (nKCShopCategoryTemplet == null)
		{
			Debug.LogError($"Category Templet for {category} not found!");
			return list;
		}
		HashSet<string> hashSet = new HashSet<string>(nKCShopCategoryTemplet.m_UseTabID);
		foreach (ShopTabTemplet value in ShopTabTemplet.Values)
		{
			if (value.m_Visible && hashSet.Contains(value.TabType))
			{
				list.Add(value);
			}
		}
		return list;
	}

	public static NKCShopCategoryTemplet GetCategoryFromTab(string type)
	{
		foreach (NKCShopCategoryTemplet value in NKMTempletContainer<NKCShopCategoryTemplet>.Values)
		{
			if (value.HasTab(type))
			{
				return value;
			}
		}
		return null;
	}

	private static string GetReddotKey(ShopItemTemplet shopItemTemplet)
	{
		return $"REDDOT_CHECK_UTC_TIME_{NKCScenManager.CurrentUserData().m_UserUID}_{shopItemTemplet.m_ProductID}";
	}

	public static void SetLastCheckedUTCTime(string tabType, int tabSubIndex = -1)
	{
		if (tabType == "TAB_NONE")
		{
			return;
		}
		NKCScenManager.CurrentUserData();
		foreach (ShopItemTemplet value in NKMTempletContainer<ShopItemTemplet>.Values)
		{
			if (!(value.m_TabID != tabType) && (tabSubIndex < 0 || value.m_TabSubIndex == tabSubIndex) && value.m_Reddot == ShopReddotType.REDDOT_CHECKED && IsProductAvailable(value, out var bAdmin) && !bAdmin && (value.m_hsReddotAllow.Count <= 0 || value.m_hsReddotAllow.Contains(NKMContentsVersionManager.GetCountryTag())))
			{
				SetLastCheckedUTCTime(value);
			}
		}
	}

	public static void SetLastCheckedUTCTime(ShopItemTemplet shopItemTemplet)
	{
		if (shopItemTemplet.m_Reddot == ShopReddotType.REDDOT_CHECKED && (shopItemTemplet.m_hsReddotAllow.Count <= 0 || shopItemTemplet.m_hsReddotAllow.Contains(NKMContentsVersionManager.GetCountryTag())))
		{
			PlayerPrefs.SetString(GetReddotKey(shopItemTemplet), NKCSynchronizedTime.GetServerUTCTime().Ticks.ToString());
			if (NKCUIShop.IsInstanceOpen && NKCUIShop.Instance.gameObject.activeSelf)
			{
				NKCUIShop.Instance.RefreshShopRedDot();
			}
		}
	}

	public static ShopReddotType GetReddotType(ShopItemTemplet shopItemTemplet)
	{
		if (shopItemTemplet.m_hsReddotAllow.Count > 0 && !shopItemTemplet.m_hsReddotAllow.Contains(NKMContentsVersionManager.GetCountryTag()))
		{
			return ShopReddotType.NONE;
		}
		if (shopItemTemplet.m_Reddot == ShopReddotType.REDDOT_CHECKED)
		{
			string reddotKey = GetReddotKey(shopItemTemplet);
			if (PlayerPrefs.HasKey(reddotKey))
			{
				long num = long.Parse(PlayerPrefs.GetString(reddotKey));
				NKMShopData shopData = NKCScenManager.CurrentUserData().m_ShopData;
				if (!shopData.histories.ContainsKey(shopItemTemplet.m_ProductID))
				{
					return ShopReddotType.NONE;
				}
				if (!NKCSynchronizedTime.IsFinished(shopData.histories[shopItemTemplet.m_ProductID].nextResetDate))
				{
					return ShopReddotType.NONE;
				}
				if (shopData.histories[shopItemTemplet.m_ProductID].nextResetDate < num)
				{
					return ShopReddotType.NONE;
				}
				return ShopReddotType.REDDOT_CHECKED;
			}
			return ShopReddotType.REDDOT_CHECKED;
		}
		return shopItemTemplet.m_Reddot;
	}

	public static int CheckTabReddotCount(out ShopReddotType reddotType, string tabType = "TAB_NONE", int tabSubIndex = 0)
	{
		int num = 0;
		reddotType = ShopReddotType.NONE;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return num;
		}
		if (m_lstSpecialItemTemplet == null || m_lstSpecialItemTemplet.Count == 0)
		{
			m_lstSpecialItemTemplet = GetLimitedCountSpecialItems();
		}
		foreach (ShopItemTemplet item in m_lstSpecialItemTemplet)
		{
			if (CanBuyFixShop(nKMUserData, item, out var _, out var _) == NKM_ERROR_CODE.NEC_OK && item.m_bEnabled && item.m_bVisible && item.TabTemplet.m_Visible && (!(tabType != "TAB_NONE") || !(tabType != item.m_TabID)) && (tabSubIndex <= 0 || tabSubIndex == item.m_TabSubIndex) && NKCSynchronizedTime.IsEventTime(item.eventIntervalId, item.EventDateStartUtc, item.EventDateEndUtc) && GetReddotType(item) != ShopReddotType.NONE && CanExhibitItem(item, bIncludeLockedItemWithReason: true) && item.m_QuantityLimit > nKMUserData.m_ShopData.GetPurchasedCount(item))
			{
				if (reddotType < GetReddotType(item))
				{
					reddotType = GetReddotType(item);
				}
				num++;
			}
		}
		return num;
	}

	public static long OwnedItemCount(NKCUISlot.SlotData data)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return 0L;
		}
		switch (data.eType)
		{
		case NKCUISlot.eSlotMode.ItemMisc:
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(data.ID);
			if (itemMiscTempletByID != null)
			{
				return nKMUserData.m_InventoryData.GetCountMiscItem(itemMiscTempletByID);
			}
			break;
		}
		case NKCUISlot.eSlotMode.Unit:
		case NKCUISlot.eSlotMode.UnitCount:
			return GetOwnedUnitCount(data.ID);
		case NKCUISlot.eSlotMode.Equip:
		case NKCUISlot.eSlotMode.EquipCount:
			return nKMUserData.m_InventoryData.GetSameKindEquipCount(data.ID);
		case NKCUISlot.eSlotMode.Skin:
			return nKMUserData.m_InventoryData.HasItemSkin(data.ID) ? 1 : 0;
		case NKCUISlot.eSlotMode.Mold:
			return nKMUserData.m_CraftData.GetMoldCount(data.ID);
		case NKCUISlot.eSlotMode.Emoticon:
			return NKCEmoticonManager.HasEmoticon(data.ID) ? 1 : 0;
		case NKCUISlot.eSlotMode.Buff:
			return (nKMUserData.m_companyBuffDataList.Find((NKMCompanyBuffData e) => e.Id == data.ID) != null) ? 1 : 0;
		default:
			Log.Error($"{data.eType}: 소유할 수 있는 아이템 타입인지 확인", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCShopManager.cs", 2759);
			break;
		}
		return 0L;
	}

	public static int GetOwnedUnitCount(int unitId)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return 0;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitId);
		switch (unitTempletBase.m_NKM_UNIT_TYPE)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
			return nKMUserData.m_ArmyData.GetUnitCountByID(unitId);
		case NKM_UNIT_TYPE.NUT_SHIP:
			return nKMUserData.m_ArmyData.GetSameKindShipCountFromID(unitId);
		case NKM_UNIT_TYPE.NUT_OPERATOR:
			return nKMUserData.m_ArmyData.GetOperatorCountByID(unitId);
		default:
			Log.Error($"{unitTempletBase.m_NKM_UNIT_TYPE}: 수량 표시할 유닛 타입인지 확인", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCShopManager.cs", 2783);
			return 0;
		}
	}

	public static void DoAfterLogout()
	{
		if (m_lstSpecialItemTemplet != null)
		{
			m_lstSpecialItemTemplet.Clear();
		}
	}
}
