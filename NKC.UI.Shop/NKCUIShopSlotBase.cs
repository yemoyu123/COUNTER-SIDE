using System;
using ClientPacket.Shop;
using Cs.Core.Util;
using Cs.Logging;
using NKM;
using NKM.Shop;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public abstract class NKCUIShopSlotBase : MonoBehaviour
{
	private enum eSaleRibbonType
	{
		None,
		Sale10,
		Sale25,
		Sale50,
		Sale70
	}

	public delegate void OnBuy(int shopItemID);

	public delegate void OnRefreshRequired();

	[Header("공통")]
	public Image m_imgRibbon;

	public Text m_lbRibbon;

	public Text m_lbName;

	[Header("구매 관련")]
	public Text m_lbBuyCount;

	public GameObject m_objSoldOut;

	public NKCUIComButton m_cbtnBuy;

	public GameObject m_objPriceRoot;

	[Header("다중 구매 관련")]
	public NKCUIComToggle m_tglSelection;

	[Header("레드닷")]
	public GameObject m_objRedDot;

	public GameObject m_objReddot_RED;

	public GameObject m_objReddot_YELLOW;

	[Header("할인 관련")]
	public GameObject m_objDiscountDay;

	public Text m_txtDiscountDay;

	public GameObject m_objDiscountRate;

	public Text m_txtDiscountRate;

	public GameObject m_objProfitRate;

	public Text m_lbProfitRate;

	[Header("판매 기간")]
	public GameObject m_objEventTimeRoot;

	public Text m_txtEventTime;

	[Header("잠김")]
	public GameObject m_objLocked;

	public Text m_lbLockedReason;

	public GameObject m_objLockedTime;

	public Text m_lbLockedTime;

	[Header("어드민 오브젝트")]
	public GameObject m_objAdmin;

	public NKCUIComStateButton m_csbtnAdminReset;

	private bool m_bTimerUpdate;

	private const float TIMER_UPDATE_INTERVAL = 1f;

	private float m_updateTimer;

	private ShopItemTemplet m_ProductTemplet;

	private DateTime m_tEndDateDiscountTime;

	private DateTime m_tEndDateEventTime;

	private DateTime m_tEndDateSubscriptionTime;

	private DateTime m_tEndDateLockedTime;

	private const int TEN_YEAR_DAYS = 3650;

	protected string m_OverrideImageAsset;

	protected bool m_bUseCommonTimeText = true;

	protected NKCUIShop m_uiShop;

	protected const string SHOP_ICON_BUNDLE_NAME = "AB_UI_NKM_UI_SHOP_IMG";

	private OnBuy dOnBuy;

	private OnRefreshRequired dOnRefreshRequired;

	public int ProductID { get; private set; }

	public void SetOverrideImageAsset(string value)
	{
		m_OverrideImageAsset = value;
	}

	public virtual void Init(OnBuy onBuy, OnRefreshRequired onRefreshRequired)
	{
		dOnBuy = onBuy;
		dOnRefreshRequired = onRefreshRequired;
		NKCUtil.SetButtonClickDelegate(m_cbtnBuy, OnBtnBuy);
		NKCUtil.SetToggleValueChangedDelegate(m_tglSelection, OnTglSelection);
		NKCUtil.SetGameobjectActive(m_objAdmin, bValue: false);
		ActivateSelection(value: false);
	}

	protected void OnBtnBuy()
	{
		if (dOnBuy == null)
		{
			return;
		}
		if (m_objLocked != null && m_objLocked.activeSelf && m_ProductTemplet != null)
		{
			if (m_ProductTemplet.IsSubscribeItem() && NKCScenManager.CurrentUserData().m_ShopData.subscriptions.ContainsKey(m_ProductTemplet.m_ProductID))
			{
				NKMShopSubscriptionData nKMShopSubscriptionData = NKCScenManager.CurrentUserData().m_ShopData.subscriptions[m_ProductTemplet.m_ProductID];
				if (NKCSynchronizedTime.GetServerUTCTime().AddDays(NKMCommonConst.SubscriptionBuyCriteriaDate) < nKMShopSubscriptionData.endDate)
				{
					NKCUIManager.NKCPopupMessage.Open(new PopupMessage(string.Format(NKCUtilString.GET_STRING_SHOP_SUBSCRIBE_DAY_ENOUGH_DESC, NKMCommonConst.SubscriptionBuyCriteriaDate), NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
					return;
				}
			}
			bool flag = true;
			if (m_ProductTemplet.m_paidAmountRequired > 0.0)
			{
				flag = false;
			}
			else if (!NKMContentUnlockManager.IsContentUnlocked(NKCScenManager.CurrentUserData(), in m_ProductTemplet.m_UnlockInfo))
			{
				flag = false;
			}
			if (!flag)
			{
				PopupMessage msg = new PopupMessage(NKCUtilString.GET_STRING_SHOP_NOT_ENOUGH_REQUIREMENT, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false);
				NKCUIManager.NKCPopupMessage.Open(msg);
				return;
			}
		}
		dOnBuy(ProductID);
		if (m_objRedDot != null && m_ProductTemplet != null && NKCShopManager.GetReddotType(m_ProductTemplet) != ShopReddotType.REDDOT_PURCHASED)
		{
			NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
		}
	}

	protected void OnTglSelection(bool value)
	{
		OnBtnBuy();
	}

	protected abstract void SetPrice(int priceItemID, int Price, bool bSale = false, int oldPrice = 0);

	protected abstract void SetInappPurchasePrice(ShopItemTemplet cShopItemTemplet, int price, bool bSale = false, int oldPrice = 0);

	protected abstract void SetGoodsImage(ShopItemTemplet shopTemplet, bool bFirstBuy);

	protected virtual void SetGoodsImage(NKMShopRandomListData shopRandomTemplet)
	{
	}

	protected virtual void PostSetData(ShopItemTemplet shopTemplet)
	{
	}

	protected virtual void UpdateTimeLeft(DateTime eventEndTime)
	{
	}

	protected virtual void SetShowTimeLeft(bool bValue)
	{
	}

	protected virtual bool IsProductAvailable(ShopItemTemplet shopTemplet, out bool bAdmin, bool bIncludeLockedItemWithReason)
	{
		return NKCShopManager.IsProductAvailable(shopTemplet, out bAdmin, bIncludeLockedItemWithReason);
	}

	public bool SetData(NKCUIShop uiShop, ShopItemTemplet shopTemplet, int buyCountLeft = -1, bool bFirstBuy = false)
	{
		NKCUtil.SetGameobjectActive(m_imgRibbon, bValue: false);
		NKCUtil.SetGameobjectActive(m_csbtnAdminReset, bValue: false);
		m_uiShop = uiShop;
		m_ProductTemplet = shopTemplet;
		ProductID = shopTemplet.m_ProductID;
		SetRedDot();
		bool bAdmin;
		bool flag = IsProductAvailable(shopTemplet, out bAdmin, bIncludeLockedItemWithReason: true);
		NKCUtil.SetGameobjectActive(m_objAdmin, bAdmin);
		if (!flag && m_ProductTemplet.TabTemplet.m_ShopDisplay != ShopDisplayType.Custom)
		{
			NKCUtil.SetGameobjectActive(m_objLocked, bValue: true);
			NKCUtil.SetGameobjectActive(m_lbLockedReason, bValue: false);
			PostSetData(shopTemplet);
			return false;
		}
		bool flag2 = false;
		bool flag3 = false;
		m_tEndDateLockedTime = default(DateTime);
		NKCUtil.SetGameobjectActive(m_objLocked, bValue: false);
		NKCUtil.SetGameobjectActive(m_lbLockedReason, bValue: false);
		if (buyCountLeft != -1)
		{
			if (buyCountLeft == 0)
			{
				NKCUtil.SetGameobjectActive(m_objPriceRoot, bValue: false);
				NKCUtil.SetGameobjectActive(m_objSoldOut, bValue: true);
				NKMShopData shopData = NKCScenManager.CurrentUserData().m_ShopData;
				if (shopData.histories.ContainsKey(ProductID) && !NKCSynchronizedTime.IsFinished(shopData.histories[ProductID].nextResetDate))
				{
					NKCUtil.SetLabelText(m_lbLockedTime, NKCSynchronizedTime.GetTimeLeftString(shopData.histories[ProductID].nextResetDate));
					m_tEndDateLockedTime = new DateTime(shopData.histories[ProductID].nextResetDate);
					flag2 = true;
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objPriceRoot, bValue: true);
				NKCUtil.SetGameobjectActive(m_objSoldOut, bValue: false);
			}
			NKCUtil.SetGameobjectActive(m_lbBuyCount, bValue: true);
			if (shopTemplet.TabTemplet == null)
			{
				Log.Error($"[Error] ShopTemplet[{shopTemplet.m_ItemID}] ShopTabTemplet is null!!  [{shopTemplet.m_TabID}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Shop/NKCUIShopSlotBase.cs", 284);
			}
			if (shopTemplet.TabTemplet.IsCountResetType)
			{
				NKCUtil.SetLabelText(m_lbBuyCount, string.Format(NKCUtilString.GET_STRING_SHOP_PURCHASE_COUNT_TWO_PARAM, buyCountLeft, shopTemplet.m_QuantityLimit));
			}
			else
			{
				NKCUtil.SetLabelText(m_lbBuyCount, NKCShopManager.GetBuyCountString(shopTemplet.resetType, buyCountLeft, shopTemplet.m_QuantityLimit));
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objPriceRoot, bValue: true);
			NKCUtil.SetGameobjectActive(m_lbBuyCount, bValue: false);
			NKCUtil.SetGameobjectActive(m_objSoldOut, bValue: false);
		}
		switch (shopTemplet.m_TagImage)
		{
		case ShopItemRibbon.ONE_PLUS_ONE:
			if (shopTemplet.m_PurchaseEventType == PURCHASE_EVENT_REWARD_TYPE.FIRST_PURCHASE_CHANGE_REWARD_VALUE && bFirstBuy)
			{
				SetRibbon(ShopItemRibbon.ONE_PLUS_ONE);
			}
			break;
		default:
			SetRibbon(shopTemplet.m_TagImage);
			break;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			PostSetData(shopTemplet);
			return false;
		}
		int realPrice = nKMUserData.m_ShopData.GetRealPrice(shopTemplet);
		bool flag4 = realPrice < shopTemplet.m_Price;
		if (shopTemplet.m_PriceItemID == 0)
		{
			if (flag4)
			{
				SetInappPurchasePrice(shopTemplet, realPrice, bSale: true, shopTemplet.m_Price);
			}
			else
			{
				SetInappPurchasePrice(shopTemplet, shopTemplet.m_Price);
			}
		}
		else if (flag4)
		{
			SetPrice(shopTemplet.m_PriceItemID, realPrice, bSale: true, shopTemplet.m_Price);
		}
		else
		{
			SetPrice(shopTemplet.m_PriceItemID, realPrice);
		}
		NKCUtil.SetLabelText(m_lbName, shopTemplet.GetItemName());
		SetGoodsImage(shopTemplet, bFirstBuy);
		bool flag5 = false;
		if (shopTemplet.m_DiscountRate > 0f && NKCSynchronizedTime.IsEventTime(shopTemplet.discountIntervalId, shopTemplet.DiscountStartDateUtc, shopTemplet.DiscountEndDateUtc) && shopTemplet.DiscountEndDateUtc != DateTime.MinValue && shopTemplet.DiscountEndDateUtc != DateTime.MaxValue)
		{
			flag5 = true;
			m_tEndDateDiscountTime = shopTemplet.DiscountEndDateUtc;
			UpdateDiscountTime(m_tEndDateDiscountTime);
		}
		else
		{
			m_tEndDateDiscountTime = default(DateTime);
		}
		if (!shopTemplet.HasDiscountDateLimit)
		{
			NKCUtil.SetGameobjectActive(m_objDiscountRate, shopTemplet.m_DiscountRate > 0f);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objDiscountRate, shopTemplet.m_DiscountRate > 0f && flag5);
		}
		float num = (100f - shopTemplet.m_DiscountRate) / 10f;
		NKCUtil.SetLabelText(m_txtDiscountRate, NKCStringTable.GetString("SI_DP_SHOP_DISCOUNT_RATE", (int)shopTemplet.m_DiscountRate, num));
		if (shopTemplet.m_ProfitRate > 100)
		{
			NKCUtil.SetGameobjectActive(m_objProfitRate, bValue: true);
			NKCUtil.SetLabelText(m_lbProfitRate, $"X {(float)shopTemplet.m_ProfitRate / 100f:#.##}");
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objProfitRate, bValue: false);
		}
		bool flag6 = false;
		DayOfWeek result;
		if (shopTemplet.IsReturningProduct)
		{
			if (!nKMUserData.IsReturnUser())
			{
				PostSetData(shopTemplet);
				return false;
			}
			m_tEndDateEventTime = nKMUserData.GetReturnEndDate(shopTemplet.m_ReturningUserType);
			if ((m_tEndDateEventTime - ServiceTime.UtcNow).TotalDays < 3650.0)
			{
				flag6 = true;
				UpdateEventTime(m_tEndDateEventTime);
			}
		}
		else if (shopTemplet.IsNewbieProduct)
		{
			DateTime newbieEndDate = NKCScenManager.CurrentUserData().GetNewbieEndDate(shopTemplet.m_NewbieDate);
			if (shopTemplet.HasDateLimit && shopTemplet.EventDateEndUtc < newbieEndDate)
			{
				m_tEndDateEventTime = shopTemplet.EventDateEndUtc;
			}
			else
			{
				m_tEndDateEventTime = newbieEndDate;
			}
			if ((m_tEndDateEventTime - ServiceTime.UtcNow).TotalDays < 3650.0)
			{
				flag6 = true;
				UpdateEventTime(m_tEndDateEventTime);
			}
		}
		else if (shopTemplet.HasDateLimit)
		{
			if (NKCSynchronizedTime.IsEventTime(shopTemplet.eventIntervalId, shopTemplet.EventDateStartUtc, shopTemplet.EventDateEndUtc))
			{
				m_tEndDateEventTime = shopTemplet.EventDateEndUtc;
				if ((m_tEndDateEventTime - ServiceTime.UtcNow).TotalDays < 3650.0)
				{
					flag6 = true;
					UpdateEventTime(m_tEndDateEventTime);
				}
			}
			else if (!NKCSynchronizedTime.IsFinished(shopTemplet.EventDateStartUtc))
			{
				m_tEndDateLockedTime = shopTemplet.EventDateStartUtc;
				if ((m_tEndDateLockedTime - ServiceTime.UtcNow).TotalDays < 3650.0)
				{
					flag2 = true;
					UpdateLockedTime(m_tEndDateLockedTime);
					NKCUtil.SetGameobjectActive(m_objLocked, bValue: true);
				}
			}
		}
		else if (shopTemplet.IsInstantProduct)
		{
			InstantProduct instantProduct = NKCShopManager.GetInstantProduct(shopTemplet.m_ProductID);
			if (instantProduct != null)
			{
				m_tEndDateEventTime = NKMTime.LocalToUTC(instantProduct.endDate);
				if ((m_tEndDateEventTime - ServiceTime.UtcNow).TotalDays < 3650.0)
				{
					flag6 = true;
					UpdateEventTime(m_tEndDateEventTime);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objLocked, bValue: true);
			}
		}
		else if (shopTemplet.resetType.ToDayOfWeek(out result))
		{
			m_tEndDateEventTime = NKCSynchronizedTime.GetStartOfServiceTime(1);
			flag6 = true;
			UpdateEventTime(m_tEndDateEventTime);
		}
		else
		{
			m_tEndDateEventTime = default(DateTime);
		}
		bool flag7 = false;
		if (NKCScenManager.CurrentUserData().m_ShopData.subscriptions.ContainsKey(shopTemplet.m_ProductID))
		{
			NKMShopSubscriptionData nKMShopSubscriptionData = NKCScenManager.CurrentUserData().m_ShopData.subscriptions[shopTemplet.m_ProductID];
			if (NKCSynchronizedTime.IsEventTime(nKMShopSubscriptionData.startDate, nKMShopSubscriptionData.endDate))
			{
				flag7 = true;
				m_tEndDateSubscriptionTime = NKCScenManager.CurrentUserData().m_ShopData.subscriptions[shopTemplet.m_ProductID].endDate;
				UpdateTimeLeft(m_tEndDateSubscriptionTime);
				if (NKCSynchronizedTime.GetServerUTCTime().AddDays(NKMCommonConst.SubscriptionBuyCriteriaDate) < nKMShopSubscriptionData.endDate)
				{
					flag3 = true;
					NKCUtil.SetGameobjectActive(m_objLocked, bValue: true);
				}
			}
			else
			{
				m_tEndDateSubscriptionTime = default(DateTime);
			}
		}
		else
		{
			m_tEndDateSubscriptionTime = default(DateTime);
		}
		if (NKCScenManager.CurrentUserData().m_ShopData.GetTotalPayment() < shopTemplet.m_paidAmountRequired)
		{
			NKCUtil.SetGameobjectActive(m_objLocked, bValue: true);
		}
		else if (buyCountLeft != 0 && !NKMContentUnlockManager.IsContentUnlocked(nKMUserData, in m_ProductTemplet.m_UnlockInfo))
		{
			NKCUtil.SetGameobjectActive(m_objLocked, bValue: true);
			if (!string.IsNullOrEmpty(m_ProductTemplet.m_UnlockReqStrID))
			{
				NKCUtil.SetGameobjectActive(m_lbLockedReason, bValue: true);
				if (m_ProductTemplet.m_UnlockReqStrID == "AUTO")
				{
					NKCUtil.SetLabelText(m_lbLockedReason, NKCContentManager.MakeUnlockConditionString(in m_ProductTemplet.m_UnlockInfo, bSimple: false));
				}
				else
				{
					NKCUtil.SetLabelText(m_lbLockedReason, NKCStringTable.GetString(m_ProductTemplet.m_UnlockReqStrID));
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_lbLockedReason, bValue: false);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objLocked, flag2 || flag3);
		}
		if (!flag)
		{
			NKCUtil.SetGameobjectActive(m_objLocked, bValue: true);
			NKCUtil.SetGameobjectActive(m_lbLockedReason, bValue: true);
			NKCUtil.SetGameobjectActive(m_objPriceRoot, bValue: false);
			NKCUtil.SetLabelText(m_lbLockedReason, NKCStringTable.GetString("SI_DP_SHOP_PRODUCT_PROHIBITED"));
		}
		m_bTimerUpdate = flag5 || flag6 || flag7 || flag2;
		SetShowBadgeTime(flag5);
		SetShowEventTime(flag6);
		SetShowLockedTime(flag2);
		SetShowTimeLeft(flag7);
		PostSetData(shopTemplet);
		return true;
	}

	public bool SetData(NKCUIShop uiShop, NKMShopRandomListData shopRandomTemplet, int index)
	{
		m_uiShop = uiShop;
		m_ProductTemplet = null;
		ProductID = index;
		NKCUtil.SetGameobjectActive(m_imgRibbon, bValue: false);
		NKCUtil.SetGameobjectActive(m_objLocked, bValue: false);
		SetRedDot();
		switch (shopRandomTemplet.itemType)
		{
		case NKM_REWARD_TYPE.RT_USER_EXP:
		{
			NKMItemMiscTemplet itemMiscTempletByRewardType = NKMItemManager.GetItemMiscTempletByRewardType(shopRandomTemplet.itemType);
			if (itemMiscTempletByRewardType == null)
			{
				Debug.LogError("itemTemplet null! ID : " + shopRandomTemplet.itemId);
				return false;
			}
			NKCUtil.SetLabelText(m_lbName, itemMiscTempletByRewardType.GetItemName());
			break;
		}
		case NKM_REWARD_TYPE.RT_MISC:
		case NKM_REWARD_TYPE.RT_MISSION_POINT:
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(shopRandomTemplet.itemId);
			if (itemMiscTempletByID == null)
			{
				Debug.LogError("itemTemplet null! ID : " + shopRandomTemplet.itemId);
				return false;
			}
			NKCUtil.SetLabelText(m_lbName, itemMiscTempletByID.GetItemName());
			break;
		}
		case NKM_REWARD_TYPE.RT_EQUIP:
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(shopRandomTemplet.itemId);
			if (equipTemplet == null)
			{
				Debug.LogError("equipTemplet null! ID : " + shopRandomTemplet.itemId);
				return false;
			}
			NKCUtil.SetLabelText(m_lbName, NKCUtilString.GetItemEquipNameWithTier(equipTemplet));
			break;
		}
		case NKM_REWARD_TYPE.RT_UNIT:
		case NKM_REWARD_TYPE.RT_SHIP:
		case NKM_REWARD_TYPE.RT_OPERATOR:
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(shopRandomTemplet.itemId);
			if (unitTempletBase == null)
			{
				Debug.LogError("UnitTemplet null! ID : " + shopRandomTemplet.itemId);
				return false;
			}
			NKCUtil.SetLabelText(m_lbName, unitTempletBase.GetUnitName());
			break;
		}
		case NKM_REWARD_TYPE.RT_SKIN:
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(shopRandomTemplet.itemId);
			if (skinTemplet == null)
			{
				Debug.LogError("SkinTemplet null! ID : " + shopRandomTemplet.itemId);
				return false;
			}
			NKCUtil.SetLabelText(m_lbName, skinTemplet.GetTitle());
			break;
		}
		case NKM_REWARD_TYPE.RT_MOLD:
		{
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(shopRandomTemplet.itemId);
			if (itemMoldTempletByID == null)
			{
				Debug.LogError("MoldTemplet null! ID : " + shopRandomTemplet.itemId);
				return false;
			}
			NKCUtil.SetLabelText(m_lbName, itemMoldTempletByID.GetItemName());
			break;
		}
		case NKM_REWARD_TYPE.RT_NONE:
			Debug.LogError("RandomShopTemplet Type None! Index : " + index);
			return false;
		}
		NKCUtil.SetGameobjectActive(m_lbBuyCount, bValue: false);
		NKCUtil.SetGameobjectActive(m_objPriceRoot, bValue: true);
		NKCUtil.SetGameobjectActive(m_objSoldOut, shopRandomTemplet.isBuy);
		SetPrice(shopRandomTemplet.priceItemId, shopRandomTemplet.GetPrice(), shopRandomTemplet.discountRatio != 0, shopRandomTemplet.price);
		NKCUtil.SetGameobjectActive(m_objDiscountRate, shopRandomTemplet.discountRatio > 0);
		float num = (float)(100 - shopRandomTemplet.discountRatio) / 10f;
		NKCUtil.SetLabelText(m_txtDiscountRate, NKCStringTable.GetString("SI_DP_SHOP_DISCOUNT_RATE", shopRandomTemplet.discountRatio, num));
		NKCUtil.SetGameobjectActive(m_lbRibbon, bValue: false);
		NKCUtil.SetGameobjectActive(m_imgRibbon, bValue: false);
		SetGoodsImage(shopRandomTemplet);
		m_bTimerUpdate = false;
		SetShowTimeLeft(bValue: false);
		SetShowEventTime(bValue: false);
		SetShowBadgeTime(bValue: false);
		SetShowLockedTime(bValue: false);
		return true;
	}

	private void Update()
	{
		if (!m_bTimerUpdate || m_ProductTemplet == null)
		{
			return;
		}
		m_updateTimer += Time.deltaTime;
		if (1f < m_updateTimer)
		{
			m_updateTimer = 0f;
			if (m_tEndDateDiscountTime != DateTime.MinValue)
			{
				UpdateDiscountTime(m_tEndDateDiscountTime);
			}
			if (m_tEndDateEventTime != DateTime.MinValue)
			{
				UpdateEventTime(m_tEndDateEventTime);
			}
			if (m_tEndDateSubscriptionTime != DateTime.MinValue)
			{
				UpdateTimeLeft(m_tEndDateSubscriptionTime);
			}
			if (m_tEndDateLockedTime != DateTime.MinValue)
			{
				UpdateLockedTime(m_tEndDateLockedTime);
			}
		}
	}

	public void UpdateEventTime(DateTime eventEndTime)
	{
		string msg;
		if (NKCSynchronizedTime.IsFinished(eventEndTime))
		{
			msg = NKCUtilString.GET_STRING_QUIT;
			if (m_ProductTemplet != null)
			{
				dOnRefreshRequired?.Invoke();
				return;
			}
		}
		else
		{
			msg = NKCUtilString.GetRemainTimeStringOneParam(eventEndTime);
		}
		NKCUtil.SetLabelText(m_txtEventTime, msg);
	}

	public void SetShowEventTime(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objEventTimeRoot, bValue);
	}

	public void UpdateDiscountTime(DateTime endTime)
	{
		NKCUtil.SetLabelText(msg: (!NKCSynchronizedTime.IsFinished(endTime)) ? NKCUtilString.GetRemainTimeStringOneParam(endTime) : NKCUtilString.GET_STRING_QUIT, label: m_txtDiscountDay);
	}

	public void SetShowBadgeTime(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objDiscountDay, bValue);
	}

	public void UpdateLockedTime(DateTime endTimeUTC)
	{
		string msg;
		if (NKCSynchronizedTime.IsFinished(endTimeUTC))
		{
			msg = NKCUtilString.GET_STRING_QUIT;
			if (m_uiShop != null)
			{
				m_uiShop.RefreshCurrentTab();
				m_uiShop.RefreshShopRedDot();
			}
		}
		else
		{
			msg = NKCSynchronizedTime.GetTimeLeftString(endTimeUTC);
		}
		NKCUtil.SetLabelText(m_lbLockedTime, msg);
	}

	public void SetShowLockedTime(bool bValue)
	{
		NKCUtil.SetGameobjectActive(m_objLockedTime, bValue);
		NKCUtil.SetGameobjectActive(m_lbLockedTime, bValue);
	}

	public void SetRedDot()
	{
		if (m_ProductTemplet != null)
		{
			if (m_ProductTemplet.m_QuantityLimit > NKCScenManager.CurrentUserData().m_ShopData.GetPurchasedCount(m_ProductTemplet))
			{
				if (NKCShopManager.CanBuyFixShop(NKCScenManager.CurrentUserData(), m_ProductTemplet, out var _, out var _) == NKM_ERROR_CODE.NEC_OK)
				{
					NKCUtil.SetShopReddotImage(NKCShopManager.GetReddotType(m_ProductTemplet), m_objRedDot, m_objReddot_RED, m_objReddot_YELLOW);
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
				}
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objRedDot, bValue: false);
		}
	}

	public void SetNameText(string name)
	{
		NKCUtil.SetLabelText(m_lbName, name);
	}

	protected virtual void SetRibbon(ShopItemRibbon ribbonType)
	{
		NKCUtil.SetImageColor(m_imgRibbon, NKCShopManager.GetRibbonColor(ribbonType));
		NKCUtil.SetLabelText(m_lbRibbon, NKCShopManager.GetRibbonString(ribbonType));
		NKCUtil.SetGameobjectActive(m_lbRibbon, ribbonType != ShopItemRibbon.None);
		NKCUtil.SetGameobjectActive(m_imgRibbon, ribbonType != ShopItemRibbon.None);
	}

	protected Sprite GetPriceImage(int priceItemID)
	{
		return NKCResourceUtility.GetOrLoadMiscItemSmallIcon(priceItemID);
	}

	protected Sprite GetSpoilerSprite()
	{
		return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_inven_icon_bg", "AB_INVEN_ICON_SPOILER");
	}

	public void ActivateSelection(bool value)
	{
		NKCUtil.SetGameobjectActive(m_tglSelection, value);
	}

	public void SetSelection(bool value)
	{
		if (m_tglSelection != null)
		{
			m_tglSelection.Select(value, bForce: true);
		}
	}
}
