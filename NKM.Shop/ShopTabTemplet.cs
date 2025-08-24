using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Core.Util;
using Cs.Logging;
using NKC;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Shop;

public sealed class ShopTabTemplet
{
	public const string MainTabName = "TAB_MAIN";

	public const string CashTabName = "TAB_CASH";

	public const string NoneTabName = "TAB_NONE";

	public const string HomeLevelTabName = "TAB_HOME_LEVEL";

	public const string HomeLimitTabName = "TAB_HOME_LIMIT";

	public const string SupplyTabName = "TAB_SUPPLY";

	public const string EpisodeTabName = "TAB_EPISODE";

	private NKMItemMiscTemplet bundleTabPriceTemplet;

	public const int MaxChainCount = 3;

	private readonly List<ShopItemTemplet> goods = new List<ShopItemTemplet>();

	private readonly List<ShopItemTemplet>[] chainGoods = new List<ShopItemTemplet>[3];

	private readonly List<int> resetDays = new List<int>();

	private TabId tabId;

	public string m_TabName;

	public ShopDisplayType m_ShopDisplay;

	public string m_TabImageSelect = "";

	public string m_TopBannerText = "";

	public string m_ImgBGSelected = "";

	public string m_ImgBGUnSelected = "";

	public string m_SpecialColorCode = "";

	public string m_PackageGroupID = "";

	public bool m_HideWhenSoldOut;

	public bool m_Visible = true;

	public bool m_MultiBuy;

	private bool m_bTabChain;

	private bool m_bBundlePurchase;

	public string intervalId;

	public string m_OpenTag;

	public int m_ResourceTypeID_1;

	public int m_ResourceTypeID_2;

	public int m_ResourceTypeID_3;

	public int m_ResourceTypeID_4;

	public int m_ResourceTypeID_5;

	public ShopItemRibbon m_TagImage;

	private DateTime m_dtIntervalStartTime = DateTime.MinValue;

	private DateTime m_dtIntervalEndTime = DateTime.MaxValue;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public static IEnumerable<ShopTabTemplet> Values => ShopTabTempletContainer.Values;

	public TabId TabId => tabId;

	public string TabType => tabId.Type;

	public int SubIndex => tabId.SubIndex;

	public IReadOnlyList<ShopItemTemplet> Goods => goods;

	public bool HasDateLimit => m_dtIntervalEndTime < DateTime.MaxValue;

	public bool IsChainTab => m_bTabChain;

	public bool IsBundleTab => m_bBundlePurchase;

	public bool IsCustomPackageTab => m_ShopDisplay == ShopDisplayType.CustomPackage;

	public NKMItemMiscTemplet BundleTabPriceTemplet => bundleTabPriceTemplet;

	public bool IsCountResetType => resetDays.Count > 0;

	public DateTime EventDateStartUtc => NKCSynchronizedTime.ToUtcTime(m_dtIntervalStartTime);

	public DateTime EventDateEndUtc => NKCSynchronizedTime.ToUtcTime(m_dtIntervalEndTime);

	public ShopTabTemplet()
	{
		for (int i = 0; i < chainGoods.Length; i++)
		{
			chainGoods[i] = new List<ShopItemTemplet>();
		}
	}

	public static ShopTabTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopTabTemplet.cs", 85))
		{
			return null;
		}
		ShopTabTemplet shopTabTemplet = new ShopTabTemplet();
		bool flag = true;
		flag &= lua.GetData("m_TabID", out var rValue, "TAB_CASH");
		flag &= lua.GetData("m_TabSubIndex", out var rValue2, 0);
		flag &= lua.GetData("m_TabName", ref shopTabTemplet.m_TabName);
		flag &= lua.GetData("m_ShopDisplay", ref shopTabTemplet.m_ShopDisplay);
		lua.GetData("m_TabImageSelect", ref shopTabTemplet.m_TabImageSelect);
		lua.GetData("m_TopBannerText", ref shopTabTemplet.m_TopBannerText);
		lua.GetData("m_ImgBGSelected", ref shopTabTemplet.m_ImgBGSelected);
		lua.GetData("m_ImgBGUnSelected", ref shopTabTemplet.m_ImgBGUnSelected);
		lua.GetData("m_SpecialColorCode", ref shopTabTemplet.m_SpecialColorCode);
		lua.GetData("m_PackageGroupID", ref shopTabTemplet.m_PackageGroupID);
		lua.GetData("m_HideWhenSoldOut", ref shopTabTemplet.m_HideWhenSoldOut);
		lua.GetData("m_Visible", ref shopTabTemplet.m_Visible);
		lua.GetData("m_MultiBuy", ref shopTabTemplet.m_MultiBuy);
		lua.GetData("m_DateStrID", ref shopTabTemplet.intervalId);
		lua.GetData("m_ResourceTypeID_1", ref shopTabTemplet.m_ResourceTypeID_1);
		lua.GetData("m_ResourceTypeID_2", ref shopTabTemplet.m_ResourceTypeID_2);
		lua.GetData("m_ResourceTypeID_3", ref shopTabTemplet.m_ResourceTypeID_3);
		lua.GetData("m_ResourceTypeID_4", ref shopTabTemplet.m_ResourceTypeID_4);
		lua.GetData("m_ResourceTypeID_5", ref shopTabTemplet.m_ResourceTypeID_5);
		lua.GetData("m_OpenTag", ref shopTabTemplet.m_OpenTag);
		lua.GetData("m_TagImage", ref shopTabTemplet.m_TagImage);
		lua.GetData("m_bTabChain", ref shopTabTemplet.m_bTabChain);
		lua.GetData("m_bBundlePurchase", ref shopTabTemplet.m_bBundlePurchase);
		lua.GetData("m_ResetDays", out var rValue3, string.Empty);
		if (!string.IsNullOrEmpty(rValue3))
		{
			string[] array = rValue3.Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				if (!int.TryParse(array[i], out var result))
				{
					Log.ErrorAndExit("[ShopTab] invalid ResetDays data:" + rValue3, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopTabTemplet.cs", 124);
				}
				shopTabTemplet.resetDays.Add(result);
			}
		}
		shopTabTemplet.resetDays.Sort();
		shopTabTemplet.tabId = new TabId(rValue, rValue2);
		if (!flag)
		{
			return null;
		}
		return shopTabTemplet;
	}

	public static ShopTabTemplet Find(string tab, int subIndex)
	{
		return ShopTabTempletContainer.Find(tab, subIndex);
	}

	public IReadOnlyList<ShopItemTemplet> GetChainGoods(int chainIndex)
	{
		int num = chainIndex - 1;
		return chainGoods[num];
	}

	public bool CalcNextResetTime(DateTime current, TimeSpan resetHourSpan, out DateTime result)
	{
		if (!IsCountResetType)
		{
			result = default(DateTime);
			return false;
		}
		int day = current.Day;
		for (int i = 0; i < resetDays.Count; i++)
		{
			int num = resetDays[i];
			if (num >= day && (num != day || !(resetHourSpan < current.TimeOfDay)))
			{
				result = new DateTime(current.Year, current.Month, num) + resetHourSpan;
				return true;
			}
		}
		result = new DateTime(current.Year, current.Month, resetDays[0]).AddMonths(1) + resetHourSpan;
		return true;
	}

	internal void Add(ShopItemTemplet itemTemplet)
	{
		goods.Add(itemTemplet);
		if (IsChainTab)
		{
			int num = itemTemplet.m_ChainIndex - 1;
			if (num < 0 || num >= chainGoods.Length)
			{
				NKMTempletError.Add($"[ShopTab] invalid chainIndex:{itemTemplet.m_ChainIndex} productId:{itemTemplet.m_ProductID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopTabTemplet.cs", 186);
			}
			chainGoods[num].Add(itemTemplet);
		}
	}

	internal void Join()
	{
	}

	internal void UnionItemIntervals(DateTime currentServiceTime)
	{
		if (Goods.Count == 0)
		{
			m_dtIntervalStartTime = DateTime.MinValue;
			m_dtIntervalEndTime = DateTime.MaxValue;
			return;
		}
		m_dtIntervalStartTime = DateTime.MaxValue;
		m_dtIntervalEndTime = DateTime.MinValue;
		bool flag = false;
		foreach (ShopItemTemplet good in Goods)
		{
			if (GetGoodInterval(good, currentServiceTime, out var start, out var end))
			{
				flag = true;
				if (m_dtIntervalStartTime > start)
				{
					m_dtIntervalStartTime = start;
				}
				if (m_dtIntervalEndTime < end)
				{
					m_dtIntervalEndTime = end;
				}
			}
		}
		List<ShopItemTemplet>[] array = chainGoods;
		for (int i = 0; i < array.Length; i++)
		{
			foreach (ShopItemTemplet item in array[i])
			{
				if (GetGoodInterval(item, currentServiceTime, out var start2, out var end2))
				{
					flag = true;
					if (m_dtIntervalStartTime > start2)
					{
						m_dtIntervalStartTime = start2;
					}
					if (m_dtIntervalEndTime < end2)
					{
						m_dtIntervalEndTime = end2;
					}
				}
			}
		}
		if (!flag)
		{
			m_dtIntervalStartTime = DateTime.MinValue;
			m_dtIntervalEndTime = DateTime.MinValue.AddDays(1.0);
		}
	}

	private bool GetGoodInterval(ShopItemTemplet item, DateTime current, out DateTime start, out DateTime end)
	{
		start = DateTime.MinValue;
		end = DateTime.MaxValue;
		if (item == null)
		{
			return false;
		}
		if (!item.m_bEnabled)
		{
			return false;
		}
		if (!item.EnableByTag)
		{
			return false;
		}
		if (!item.ItemEnableByTag)
		{
			return false;
		}
		if (!item.m_bVisible)
		{
			return false;
		}
		if (item.IsReturningProduct)
		{
			return true;
		}
		if (!item.EventIntervalTemplet.IsValid)
		{
			return true;
		}
		if (item.EventIntervalTemplet.IsRepeatDate)
		{
			return true;
		}
		if (HasDateLimit && !item.EventIntervalTemplet.IsValidTime(current))
		{
			return false;
		}
		if (item.IsNewbieProduct)
		{
			if (HasDateLimit)
			{
				start = item.EventIntervalTemplet.StartDate;
				end = item.EventIntervalTemplet.EndDate;
			}
			return true;
		}
		start = item.EventIntervalTemplet.StartDate;
		end = item.EventIntervalTemplet.EndDate;
		return true;
	}

	internal void UnionTabIntervals(List<ShopTabTemplet> lstTab, DateTime current)
	{
		if (TabType == "TAB_MAIN")
		{
			m_dtIntervalStartTime = DateTime.MinValue;
			m_dtIntervalEndTime = DateTime.MaxValue;
			return;
		}
		m_dtIntervalStartTime = DateTime.MaxValue;
		m_dtIntervalEndTime = DateTime.MinValue;
		bool flag = false;
		for (int i = 1; i < lstTab.Count; i++)
		{
			ShopTabTemplet shopTabTemplet = lstTab[i];
			if (shopTabTemplet != null && current.IsBetween(shopTabTemplet.m_dtIntervalStartTime, shopTabTemplet.m_dtIntervalEndTime))
			{
				flag = true;
				if (m_dtIntervalStartTime > shopTabTemplet.m_dtIntervalStartTime)
				{
					m_dtIntervalStartTime = shopTabTemplet.m_dtIntervalStartTime;
				}
				if (m_dtIntervalEndTime < shopTabTemplet.m_dtIntervalEndTime)
				{
					m_dtIntervalEndTime = shopTabTemplet.m_dtIntervalEndTime;
				}
			}
		}
		if (!flag)
		{
			m_dtIntervalStartTime = DateTime.MinValue;
			m_dtIntervalEndTime = DateTime.MinValue.AddDays(1.0);
		}
	}

	internal void Validate()
	{
		if (IsChainTab)
		{
			foreach (ShopItemTemplet good in goods)
			{
				if (good.m_ChainIndex == 0)
				{
					NKMTempletError.Add($"[ShopTab] 단계형 탭 전시품에 chainIndex 미설정. tabId:{tabId} productId:{good.m_ProductID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopTabTemplet.cs", 343);
				}
				if (good.m_ChainIndex < 3 && good.resetType != SHOP_RESET_TYPE.Unlimited)
				{
					NKMTempletError.Add($"[ShopTab] 단계형 탭에서 마지막이 아닌 탭에 진열된 전시품은 구매제한 설정 불가. tabId:{tabId} productId:{good.m_ProductID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopTabTemplet.cs", 348);
				}
				if (good.IsReturningProduct)
				{
					NKMTempletError.Add($"[ShopTab] 단계형 탭에 복귀 상품은 전시 불가. tabId:{tabId} productId:{good.m_ProductID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopTabTemplet.cs", 353);
				}
			}
			foreach (int resetDay in resetDays)
			{
				if (resetDay <= 0 || resetDay > 28)
				{
					NKMTempletError.Add($"[ShopTab] 단계형 탭 초기화 일자 오류:{resetDay} tabId:{tabId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopTabTemplet.cs", 361);
				}
			}
		}
		else
		{
			ShopItemTemplet shopItemTemplet = goods.FirstOrDefault((ShopItemTemplet e) => e.m_ChainIndex != 0);
			if (shopItemTemplet != null)
			{
				NKMTempletError.Add($"[ShopTab] 비 단계형 탭에 전시된 상품에 chainIndex 설정. tabId:{tabId} productId:{shopItemTemplet.m_ProductID} shopDisplay:{m_ShopDisplay}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopTabTemplet.cs", 370);
			}
			if (resetDays.Any())
			{
				NKMTempletError.Add($"[ShopTab] 비 단계형 탭에 전시된 상품에 초기화 일자 설정. tabId:{tabId} shopDisplay:{m_ShopDisplay}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopTabTemplet.cs", 375);
			}
		}
		if (IsBundleTab)
		{
			int num = -1;
			foreach (ShopItemTemplet good2 in goods)
			{
				if (good2.IsInAppProduct)
				{
					NKMTempletError.Add($"[ShopTab] 번들탭에 인앱결제 상품 전시 불가. tabId:{tabId} productId:{good2.m_ProductID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopTabTemplet.cs", 386);
				}
				if (!good2.IsCountResetType())
				{
					NKMTempletError.Add($"[ShopTab] 번들탭은 횟수 초기화 상품만 전시 가능. tabId:{tabId} productId:{good2.m_ProductID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopTabTemplet.cs", 391);
				}
				if (good2.HasDateLimit)
				{
					NKMTempletError.Add($"[ShopTab] 번들탭에 판매기간 정해진 상품은 전시 불가. tabId:{tabId} productId:{good2.m_ProductID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopTabTemplet.cs", 396);
				}
				if (good2.IsReturningProduct)
				{
					NKMTempletError.Add($"[ShopTab] 번들탭에 복귀 상품은 전시 불가. tabId:{tabId} productId:{good2.m_ProductID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopTabTemplet.cs", 401);
				}
				if (good2.m_Price != 0)
				{
					if (num < 0)
					{
						num = good2.m_PriceItemID;
					}
					else if (num != good2.m_PriceItemID)
					{
						NKMTempletError.Add($"[ShopTab] 번들탭에 서로 다른 재화종류 사용 불가. tabId:{tabId} priceId:{num}, {good2.m_PriceItemID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopTabTemplet.cs", 413);
					}
				}
			}
			bundleTabPriceTemplet = NKMItemManager.GetItemMiscTempletByID(num);
			if (bundleTabPriceTemplet == null)
			{
				NKMTempletError.Add($"[ShopTab] 잘못된 번들탭 가격 타입. tabId:{tabId} priceId:{num}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopTabTemplet.cs", 421);
			}
		}
		if (m_HideWhenSoldOut)
		{
			foreach (ShopItemTemplet good3 in goods)
			{
				if (good3.IsCountResetType())
				{
					NKMTempletError.Add($"[ShopTab] 완판시 가려지는 탭에는 구매횟수 초기화 상품을 진열할 수 없음. tabId:{tabId} productId:{good3.Key}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopTabTemplet.cs", 431);
				}
			}
		}
		if (IsCustomPackageTab)
		{
			foreach (ShopItemTemplet good4 in goods)
			{
				NKMItemMiscTemplet miscProductTemplet = good4.MiscProductTemplet;
				if (miscProductTemplet == null || miscProductTemplet.m_ItemMiscType != NKM_ITEM_MISC_TYPE.IMT_CUSTOM_PACKAGE)
				{
					NKMTempletError.Add($"[ShopTab] 커스텀 패키지 탭에는 다른 타입 상품 상품 전시 불가. tabId:{tabId} productId:{good4.m_ProductID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopTabTemplet.cs", 443);
				}
			}
		}
		else
		{
			foreach (ShopItemTemplet good5 in goods)
			{
				NKMItemMiscTemplet miscProductTemplet2 = good5.MiscProductTemplet;
				if (miscProductTemplet2 != null && miscProductTemplet2.m_ItemMiscType == NKM_ITEM_MISC_TYPE.IMT_CUSTOM_PACKAGE)
				{
					NKMTempletError.Add($"[ShopTab] 커스텀 패키지 상품은 전용 탭 외에는 전시 불가. tabId:{tabId} productId:{good5.m_ProductID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopTabTemplet.cs", 454);
				}
			}
		}
		if (string.IsNullOrEmpty(intervalId) && NKMIntervalTemplet.Find(intervalId) == null)
		{
			Log.ErrorAndExit($"[ShopTabTemplet:{tabId}]잘못된 interval id:{intervalId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Shop/ShopTabTemplet.cs", 464);
		}
	}

	public string GetTabName()
	{
		return NKCStringTable.GetString(m_TabName);
	}
}
