using System;
using System.Collections.Generic;
using NKC.Publisher;
using NKC.UI.Component.Office;
using NKM;
using NKM.Shop;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCPopupShopBuyConfirm : NKCUIBase, IScrollHandler, IEventSystemHandler
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_shop";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_SHOP_BUY_CONFIRM";

	private static NKCPopupShopBuyConfirm m_Instance;

	private const string COUNT_COLOR = "ffcf3b";

	public Text m_lbItemName;

	public GameObject m_objItemInventoryCountParent;

	public Text m_lbItemInventoryCount;

	public Text m_lbItemDesc;

	public GameObject m_objRemainCount;

	public Text m_lbItemRemainCount;

	public GameObject m_objItemSlot;

	public NKCUISlot m_NKCUIItemSlot;

	public GameObject m_objEquipSlot;

	public NKCUIInvenEquipSlot m_NKCUIInvenEquipSlot;

	public GameObject m_objBG;

	public GameObject m_objBuyCount;

	public NKCUIComStateButton m_btnBuyCountMinus;

	public NKCUIComStateButton m_btnBuyCountPlus;

	public Text m_lbBuyCount;

	[Header("가격")]
	public NKCUIPriceTag m_priceTag;

	public GameObject m_objSalePriceRoot;

	public Text m_lbOldPrice;

	public GameObject m_objDiscountDay;

	public Text m_txtDiscountDay;

	public GameObject m_objDiscountRate;

	public Text m_txtDiscountRate;

	[Header("하단 버튼")]
	public NKCUIComStateButton m_btnCancel;

	public NKCUIComStateButton m_btnBuy;

	public NKCUIComStateButton m_btnBuyLocked;

	public NKCUIComStateButton m_csbtnBillingRestore;

	[Header("청약철회")]
	public GameObject m_objPolicyParent;

	public NKCUIComStateButton m_btnPolicy;

	[Header("연계 상품")]
	public NKCUIComStateButton m_csbtnLinkedItem;

	[Header("가구 관련")]
	public NKCUIComOfficeInteriorDetail m_comInteriorDetail;

	public NKCUIComOfficeInteriorInteractionBubble m_comInteriorInteractionBubble;

	[Header("일본 법무 대응")]
	public GameObject m_JPN_BTN;

	public GameObject m_JPN_POLICY;

	public NKCUIComStateButton m_csbtnJPNPaymentLaw;

	public NKCUIComStateButton m_csbtnJPNCommercialLaw;

	[Header("기간제 아이템")]
	public GameObject m_objTimeInterval;

	public Text m_lbTimeLeft;

	private const int ITEM_MULTIPLE_USE_MAX_COUNT = 10;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	private NKCUIShop.OnProductBuyDelegate dOnOKButton;

	private DateTime m_tEndDateDiscountTime = DateTime.MinValue;

	private ShopItemTemplet m_cNKMShopItemTemplet;

	private int m_iBuyCount;

	private bool m_bUseMinusToMax;

	private float m_tDeltaTime;

	private float ONE_SECOND = 1f;

	public const float PRESS_GAP_MAX = 0.35f;

	public const float PRESS_GAP_MIN = 0.01f;

	public const float DAMPING = 0.8f;

	private float m_fDelay = 0.5f;

	private float m_fHoldTime;

	private int m_iChangeValue;

	private bool m_bPress;

	private bool m_bWasHold;

	public static NKCPopupShopBuyConfirm Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupShopBuyConfirm>("ab_ui_nkm_ui_shop", "NKM_UI_POPUP_SHOP_BUY_CONFIRM", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupShopBuyConfirm>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_POPUP_ITEM_BOX;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.ResourceOnly;

	private int m_iMaxBuyCount { get; set; }

	public override List<int> UpsideMenuShowResourceList => NKCShopManager.GetUpsideMenuItemList(m_cNKMShopItemTemplet);

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public void InitUI()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		m_NKCUIItemSlot.Init();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			Close();
		});
		EventTrigger eventTrigger = m_objBG.GetComponent<EventTrigger>();
		if (eventTrigger == null)
		{
			eventTrigger = m_objBG.AddComponent<EventTrigger>();
		}
		eventTrigger.triggers.Add(entry);
		m_btnCancel.PointerClick.RemoveAllListeners();
		m_btnCancel.PointerClick.AddListener(base.Close);
		m_btnBuy.m_bGetCallbackWhileLocked = true;
		m_btnBuy.PointerClick.RemoveAllListeners();
		m_btnBuy.PointerClick.AddListener(OnOK);
		NKCUtil.SetHotkey(m_btnBuy, HotkeyEventType.Confirm);
		m_btnBuyLocked.m_bGetCallbackWhileLocked = true;
		m_btnBuyLocked.PointerClick.RemoveAllListeners();
		m_btnBuyLocked.PointerClick.AddListener(OnOK);
		m_btnPolicy.PointerClick.RemoveAllListeners();
		m_btnPolicy.PointerClick.AddListener(OnPolicy);
		m_btnBuyCountMinus?.PointerDown.RemoveAllListeners();
		m_btnBuyCountMinus?.PointerDown.AddListener(OnMinusDown);
		m_btnBuyCountMinus?.PointerUp.RemoveAllListeners();
		m_btnBuyCountMinus?.PointerUp.AddListener(OnButtonUp);
		NKCUtil.SetHotkey(m_btnBuyCountMinus, HotkeyEventType.Minus, this, bUpDownEvent: true);
		m_btnBuyCountPlus?.PointerDown.RemoveAllListeners();
		m_btnBuyCountPlus?.PointerDown.AddListener(OnPlusDown);
		m_btnBuyCountPlus?.PointerUp.RemoveAllListeners();
		m_btnBuyCountPlus?.PointerUp.AddListener(OnButtonUp);
		NKCUtil.SetHotkey(m_btnBuyCountPlus, HotkeyEventType.Plus, this, bUpDownEvent: true);
		NKCUtil.SetBindFunction(m_btnBuyCountMinus, delegate
		{
			OnChangeCount(bPlus: false);
		});
		NKCUtil.SetBindFunction(m_btnBuyCountPlus, delegate
		{
			OnChangeCount();
		});
		NKCUtil.SetBindFunction(m_csbtnLinkedItem, OnBtnLinkedItem);
		NKCUtil.SetGameobjectActive(m_JPN_BTN, bValue: false);
		NKCUtil.SetGameobjectActive(m_JPN_POLICY, bValue: false);
		if (NKCPublisherModule.InAppPurchase.ShowJPNPaymentPolicy())
		{
			NKCUtil.SetBindFunction(m_csbtnJPNPaymentLaw, OnBtnJPNPaymentLaw);
			NKCUtil.SetBindFunction(m_csbtnJPNCommercialLaw, OnBtnJPNCommercialLaw);
		}
		if (NKCPublisherModule.InAppPurchase.IsBillingRestoreActive())
		{
			NKCUtil.SetBindFunction(m_csbtnBillingRestore, OnClickBillingRestore);
		}
		base.gameObject.SetActive(value: false);
	}

	private void UpdateByPriceItemId()
	{
		if (m_cNKMShopItemTemplet.m_PriceItemID == 0)
		{
			Instance.m_priceTag.SetData(m_cNKMShopItemTemplet, showMinus: false, changeColor: false);
			Instance.m_priceTag.SetLabelTextColor(Color.white);
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				int realPrice = nKMUserData.m_ShopData.GetRealPrice(m_cNKMShopItemTemplet);
				NKCUtil.SetGameobjectActive(m_objSalePriceRoot, m_cNKMShopItemTemplet.m_Price > realPrice);
				if (m_cNKMShopItemTemplet.m_Price > realPrice)
				{
					NKCResourceUtility.GetOrLoadMiscItemSmallIcon(m_cNKMShopItemTemplet.m_PriceItemID);
					NKCUtil.SetLabelText(m_lbOldPrice, m_cNKMShopItemTemplet.m_Price.ToString());
				}
				NKCUtil.SetGameobjectActive(m_objPolicyParent, NKCShopManager.IsShowPurchasePolicy());
				NKCUtil.SetGameobjectActive(m_csbtnBillingRestore, NKCPublisherModule.InAppPurchase.IsBillingRestoreActive());
			}
		}
		else
		{
			UpdatePriceInfo();
			NKCUtil.SetGameobjectActive(m_objPolicyParent, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnBillingRestore, bValue: false);
		}
	}

	public void Open(ShopItemTemplet productTemplet, NKCUIShop.OnProductBuyDelegate onOkButton)
	{
		bool bShowCount = productTemplet != null && IsCountVisible(productTemplet);
		bool bFirstBuy = productTemplet != null && NKCShopManager.IsFirstBuy(productTemplet.m_ProductID, NKCScenManager.CurrentUserData());
		m_iBuyCount = 1;
		m_cNKMShopItemTemplet = productTemplet;
		NKCUtil.SetGameobjectActive(m_objRemainCount, m_cNKMShopItemTemplet.resetType != SHOP_RESET_TYPE.Unlimited);
		NKCUtil.SetLabelText(m_lbItemRemainCount, NKCShopManager.GetBuyCountString(m_cNKMShopItemTemplet.resetType, NKCShopManager.GetBuyCountLeft(m_cNKMShopItemTemplet.m_ProductID), m_cNKMShopItemTemplet.m_QuantityLimit, bRemoveBracket: true));
		NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeShopItemData(m_cNKMShopItemTemplet, bFirstBuy);
		SetData(slotData, onOkButton, bShowCount);
		NKCShopManager.ShowShopItemCashCount(m_NKCUIItemSlot, slotData, m_cNKMShopItemTemplet.m_FreeValue, m_cNKMShopItemTemplet.m_PaidValue);
		Instance.m_lbItemDesc.text = NKCUtilString.GetShopDescriptionText(m_cNKMShopItemTemplet.GetItemDescPopup(), bFirstBuy);
		CheckDiscount(m_cNKMShopItemTemplet);
		UpdateByPriceItemId();
		int buyCountLeft = NKCShopManager.GetBuyCountLeft(m_cNKMShopItemTemplet.m_ProductID);
		if (m_cNKMShopItemTemplet.m_PriceItemID != 0)
		{
			if (m_cNKMShopItemTemplet.m_Price > 0)
			{
				int realPrice = NKCScenManager.CurrentUserData().m_ShopData.GetRealPrice(m_cNKMShopItemTemplet);
				m_iMaxBuyCount = (int)(NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_cNKMShopItemTemplet.m_PriceItemID) / realPrice);
				if (m_cNKMShopItemTemplet.m_QuantityLimit > 0)
				{
					m_iMaxBuyCount = Math.Min(buyCountLeft, m_iMaxBuyCount);
				}
			}
			else if (m_cNKMShopItemTemplet.m_QuantityLimit > 0)
			{
				m_iMaxBuyCount = Math.Min(buyCountLeft, m_iMaxBuyCount);
			}
			else
			{
				m_iMaxBuyCount = 1;
			}
		}
		if (buyCountLeft >= 0)
		{
			m_iMaxBuyCount = Math.Min(m_iMaxBuyCount, buyCountLeft);
		}
		if (m_iMaxBuyCount < 1)
		{
			m_iMaxBuyCount = 1;
		}
		int priceItemID = m_cNKMShopItemTemplet.m_PriceItemID;
		if (priceItemID == 0 || (uint)(priceItemID - 101) <= 1u)
		{
			m_bUseMinusToMax = buyCountLeft > 0;
		}
		else
		{
			m_bUseMinusToMax = true;
		}
		List<ShopItemTemplet> linkedItem = NKCShopManager.GetLinkedItem(m_cNKMShopItemTemplet.m_ProductID);
		NKCUtil.SetGameobjectActive(m_csbtnLinkedItem, linkedItem != null && linkedItem.Count > 0);
		bool num = NKCPublisherModule.InAppPurchase.IsJPNPaymentPolicy();
		bool flag = m_cNKMShopItemTemplet.m_PriceItemID == 0;
		bool flag2 = NKCUtil.IsJPNPolicyRelatedItem(productTemplet.m_PriceItemID);
		if (num)
		{
			NKCUtil.SetGameobjectActive(m_JPN_BTN, flag);
			NKCUtil.SetGameobjectActive(m_JPN_POLICY, !flag && flag2);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_JPN_BTN, bValue: false);
			NKCUtil.SetGameobjectActive(m_JPN_POLICY, bValue: false);
		}
	}

	private void SetData(NKCUISlot.SlotData data, NKCUIShop.OnProductBuyDelegate onOkButton = null, bool bShowCount = false)
	{
		Instance.CommonOpenProcess(data, bShowCount);
		SetButton(onOkButton);
	}

	private void CommonOpenProcess(NKCUISlot.SlotData data, bool bShowNumber)
	{
		base.gameObject.SetActive(value: true);
		bool flag = data.eType == NKCUISlot.eSlotMode.Equip || data.eType == NKCUISlot.eSlotMode.EquipCount;
		if (flag)
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(data.ID);
			if (equipTemplet != null && (equipTemplet.m_EquipUnitStyleType == NKM_UNIT_STYLE_TYPE.NUST_ENCHANT || equipTemplet.m_ItemEquipPosition == ITEM_EQUIP_POSITION.IEP_ENCHANT))
			{
				flag = false;
			}
		}
		NKCUtil.SetGameobjectActive(m_objEquipSlot, flag);
		NKCUtil.SetGameobjectActive(m_objItemSlot, !flag);
		if (flag)
		{
			m_NKCUIInvenEquipSlot.SetData(NKCEquipSortSystem.MakeTempEquipData(data.ID, data.GroupID));
		}
		else if (m_NKCUIItemSlot != null)
		{
			m_NKCUIItemSlot.SetData(data, bShowName: false, bShowNumber, bEnableLayoutElement: false, null);
			m_NKCUIItemSlot.SetOnClickAction(NKCUISlot.SlotClickType.RatioList, NKCUISlot.SlotClickType.ChoiceList, NKCUISlot.SlotClickType.BoxList);
		}
		if (data.eType == NKCUISlot.eSlotMode.ItemMisc)
		{
			if (m_comInteriorDetail != null)
			{
				m_comInteriorDetail.SetData(data.ID);
			}
			if (m_comInteriorInteractionBubble != null)
			{
				m_comInteriorInteractionBubble.SetData(data.ID);
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_comInteriorDetail, bValue: false);
			NKCUtil.SetGameobjectActive(m_comInteriorInteractionBubble, bValue: false);
		}
		SetTextFromSlotdata(data);
		m_NKCUIOpenAnimator.PlayOpenAni();
		NKCUtil.SetGameobjectActive(m_objBuyCount, CanChangeBuyCount(data, flag));
		SetIntervalItem(data);
		UIOpened();
	}

	private void CheckDiscount(ShopItemTemplet productTemplet)
	{
		bool flag = false;
		if (productTemplet.m_DiscountRate > 0f && NKCSynchronizedTime.IsEventTime(productTemplet.discountIntervalId, productTemplet.DiscountStartDateUtc, productTemplet.DiscountEndDateUtc) && productTemplet.HasDiscountDateLimit)
		{
			flag = true;
			m_tEndDateDiscountTime = productTemplet.DiscountEndDateUtc;
			UpdateDiscountTime(m_tEndDateDiscountTime);
		}
		else
		{
			m_tEndDateDiscountTime = DateTime.MinValue;
		}
		NKCUtil.SetGameobjectActive(m_objDiscountDay, flag);
		if (!productTemplet.HasDiscountDateLimit)
		{
			NKCUtil.SetGameobjectActive(m_objDiscountRate, productTemplet.m_DiscountRate > 0f);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objDiscountRate, productTemplet.m_DiscountRate > 0f && flag);
		}
		NKCUtil.SetLabelText(m_txtDiscountRate, $"-{(int)productTemplet.m_DiscountRate}%");
	}

	public void UpdateDiscountTime(DateTime endTime)
	{
		NKCUtil.SetLabelText(msg: (!NKCSynchronizedTime.IsFinished(endTime)) ? NKCUtilString.GetRemainTimeStringOneParam(endTime) : NKCUtilString.GET_STRING_QUIT, label: m_txtDiscountDay);
	}

	private bool HaveDetail(NKCUISlot.SlotData data)
	{
		if (data.eType == NKCUISlot.eSlotMode.ItemMisc)
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(data.ID);
			if (itemMiscTempletByID != null && itemMiscTempletByID.IsUsable() && (itemMiscTempletByID.IsPackageItem || itemMiscTempletByID.IsRatioOpened()))
			{
				return true;
			}
		}
		return false;
	}

	private bool CanChangeBuyCount(NKCUISlot.SlotData data, bool bShowEquipSlot)
	{
		bool flag = true;
		if (m_cNKMShopItemTemplet.m_QuantityLimit > 0)
		{
			flag &= m_cNKMShopItemTemplet.m_QuantityLimit > 1;
		}
		flag &= m_cNKMShopItemTemplet.m_PriceItemID != 0;
		flag &= !m_cNKMShopItemTemplet.IsSubscribeItem();
		switch (m_cNKMShopItemTemplet.m_ItemType)
		{
		default:
			return false;
		case NKM_REWARD_TYPE.RT_EQUIP:
			if (bShowEquipSlot)
			{
				return false;
			}
			break;
		case NKM_REWARD_TYPE.RT_UNIT:
		case NKM_REWARD_TYPE.RT_SHIP:
		case NKM_REWARD_TYPE.RT_MISC:
		case NKM_REWARD_TYPE.RT_USER_EXP:
		case NKM_REWARD_TYPE.RT_MOLD:
		case NKM_REWARD_TYPE.RT_MISSION_POINT:
		case NKM_REWARD_TYPE.RT_OPERATOR:
			break;
		}
		switch (data.eType)
		{
		default:
			return false;
		case NKCUISlot.eSlotMode.Equip:
		case NKCUISlot.eSlotMode.EquipCount:
			if (bShowEquipSlot)
			{
				return false;
			}
			break;
		case NKCUISlot.eSlotMode.Unit:
		case NKCUISlot.eSlotMode.ItemMisc:
		case NKCUISlot.eSlotMode.Mold:
		case NKCUISlot.eSlotMode.DiveArtifact:
		case NKCUISlot.eSlotMode.Buff:
		case NKCUISlot.eSlotMode.UnitCount:
		case NKCUISlot.eSlotMode.Emoticon:
		case NKCUISlot.eSlotMode.Etc:
			break;
		}
		return flag;
	}

	private void SetButton(NKCUIShop.OnProductBuyDelegate onOkButton1 = null)
	{
		NKCUtil.SetGameobjectActive(m_btnCancel, bValue: true);
		NKCUtil.SetGameobjectActive(m_btnBuy, bValue: true);
		NKCUtil.SetGameobjectActive(m_btnBuyLocked, bValue: false);
		NKCUtil.SetGameobjectActive(m_btnPolicy, NKCShopManager.IsShowPurchasePolicyBtn());
		NKCUtil.SetGameobjectActive(m_csbtnBillingRestore, bValue: false);
		dOnOKButton = onOkButton1;
	}

	private bool IsCountVisible(ShopItemTemplet productTemplet)
	{
		switch (productTemplet.m_ItemType)
		{
		case NKM_REWARD_TYPE.RT_MISC:
		case NKM_REWARD_TYPE.RT_USER_EXP:
		case NKM_REWARD_TYPE.RT_MOLD:
		case NKM_REWARD_TYPE.RT_SKIN:
		case NKM_REWARD_TYPE.RT_MISSION_POINT:
			return true;
		default:
			return productTemplet.TotalValue > 1;
		case NKM_REWARD_TYPE.RT_NONE:
		case NKM_REWARD_TYPE.RT_UNIT:
		case NKM_REWARD_TYPE.RT_SHIP:
		case NKM_REWARD_TYPE.RT_OPERATOR:
			return false;
		}
	}

	private void SetTextFromSlotdata(NKCUISlot.SlotData data)
	{
		switch (data.eType)
		{
		case NKCUISlot.eSlotMode.ItemMisc:
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(data.ID);
			if (itemMiscTempletByID != null)
			{
				m_lbItemName.text = itemMiscTempletByID.GetItemName();
				m_lbItemDesc.text = itemMiscTempletByID.GetItemDesc();
				switch (itemMiscTempletByID.m_ItemMiscType)
				{
				case NKM_ITEM_MISC_TYPE.IMT_MISC:
				case NKM_ITEM_MISC_TYPE.IMT_RESOURCE:
				case NKM_ITEM_MISC_TYPE.IMT_CHOICE_MISC:
				case NKM_ITEM_MISC_TYPE.IMT_PIECE:
				{
					NKCUtil.SetGameobjectActive(m_objItemInventoryCountParent, bValue: true);
					string arg = string.Format("<color=#ffcf3b>{0}</color>", NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(itemMiscTempletByID).ToString("N0"));
					m_lbItemInventoryCount.text = string.Format(NKCUtilString.GET_STRING_ITEM_COUNT_ONE_PARAM, arg);
					break;
				}
				default:
					NKCUtil.SetGameobjectActive(m_objItemInventoryCountParent, bValue: false);
					break;
				}
			}
			else
			{
				Debug.LogError("ItemTemplet Not Found. ItemID : " + data.ID);
				m_lbItemName.text = "";
				m_lbItemDesc.text = "";
				NKCUtil.SetGameobjectActive(m_objItemInventoryCountParent, bValue: false);
			}
			break;
		}
		case NKCUISlot.eSlotMode.Mold:
		{
			NKMItemMoldTemplet itemMoldTempletByID = NKMItemManager.GetItemMoldTempletByID(data.ID);
			if (itemMoldTempletByID != null)
			{
				m_lbItemName.text = itemMoldTempletByID.GetItemName();
				m_lbItemDesc.text = itemMoldTempletByID.GetItemDesc();
				NKCUtil.SetGameobjectActive(m_objItemInventoryCountParent, bValue: false);
			}
			else
			{
				Debug.LogError("NKMItemMoldTemplet Not Found. moldItemID : " + data.ID);
				m_lbItemName.text = "";
				m_lbItemDesc.text = "";
				NKCUtil.SetGameobjectActive(m_objItemInventoryCountParent, bValue: false);
			}
			break;
		}
		case NKCUISlot.eSlotMode.Unit:
		case NKCUISlot.eSlotMode.UnitCount:
		{
			NKCUtil.SetGameobjectActive(m_objItemInventoryCountParent, bValue: true);
			string arg2 = string.Format("<color=#ffcf3b>{0}</color>", NKCShopManager.OwnedItemCount(data).ToString("N0"));
			m_lbItemInventoryCount.text = string.Format(NKCUtilString.GET_STRING_ITEM_COUNT_ONE_PARAM, arg2);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(data.ID);
			if (unitTempletBase != null)
			{
				m_lbItemName.text = unitTempletBase.GetUnitName();
				m_lbItemDesc.text = "";
			}
			else
			{
				Debug.LogError("UnitTemplet Not Found. UnitID : " + data.ID);
				m_lbItemName.text = "";
				m_lbItemDesc.text = "";
			}
			break;
		}
		case NKCUISlot.eSlotMode.Equip:
		case NKCUISlot.eSlotMode.EquipCount:
		{
			NKCUtil.SetGameobjectActive(m_objItemInventoryCountParent, bValue: false);
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(data.ID);
			if (equipTemplet != null)
			{
				if (data.Count > 1 && data.eType == NKCUISlot.eSlotMode.Equip)
				{
					m_lbItemName.text = $"{NKCUtilString.GetItemEquipNameWithTier(equipTemplet)} +{data.Count}";
				}
				else
				{
					m_lbItemName.text = NKCUtilString.GetItemEquipNameWithTier(equipTemplet);
				}
				m_lbItemDesc.text = equipTemplet.GetItemDesc();
			}
			else
			{
				Debug.LogError("EquipTemplet Not Found. EquipID : " + data.ID);
				m_lbItemName.text = "";
				m_lbItemDesc.text = "";
			}
			break;
		}
		case NKCUISlot.eSlotMode.Skin:
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(data.ID);
			if (skinTemplet != null)
			{
				NKMUnitManager.GetUnitTempletBase(skinTemplet.m_SkinEquipUnitID);
				m_lbItemName.text = skinTemplet.GetTitle();
				m_lbItemDesc.text = skinTemplet.GetSkinDesc();
				NKCUtil.SetGameobjectActive(m_objItemInventoryCountParent, bValue: false);
			}
			break;
		}
		case NKCUISlot.eSlotMode.Buff:
		{
			NKMCompanyBuffTemplet companyBuffTemplet = NKMCompanyBuffManager.GetCompanyBuffTemplet(data.ID);
			m_lbItemName.text = companyBuffTemplet.GetBuffName();
			m_lbItemDesc.text = companyBuffTemplet.GetBuffDescForItemPopup();
			NKCUtil.SetGameobjectActive(m_objItemInventoryCountParent, bValue: false);
			break;
		}
		case NKCUISlot.eSlotMode.Emoticon:
		{
			NKMEmoticonTemplet nKMEmoticonTemplet = NKMEmoticonTemplet.Find(data.ID);
			m_lbItemName.text = nKMEmoticonTemplet.GetEmoticonName();
			m_lbItemDesc.text = nKMEmoticonTemplet.GetEmoticonDesc();
			NKCUtil.SetGameobjectActive(m_objItemInventoryCountParent, bValue: false);
			break;
		}
		default:
			Debug.LogError("Undefined type");
			m_lbItemName.text = "";
			m_lbItemDesc.text = "";
			NKCUtil.SetGameobjectActive(m_objItemInventoryCountParent, bValue: false);
			break;
		}
	}

	private void UpdatePriceInfo()
	{
		if (NKCUtil.IsNullObject(m_cNKMShopItemTemplet))
		{
			return;
		}
		int realPrice = NKCScenManager.CurrentUserData().m_ShopData.GetRealPrice(m_cNKMShopItemTemplet, m_iBuyCount);
		bool flag = Instance.m_priceTag.SetData(m_cNKMShopItemTemplet.m_PriceItemID, realPrice, showMinus: false, changeColor: true, bHidePriceIcon: true);
		NKCUtil.SetLabelText(m_lbBuyCount, m_iBuyCount.ToString());
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			int realPrice2 = nKMUserData.m_ShopData.GetRealPrice(m_cNKMShopItemTemplet);
			NKCUtil.SetGameobjectActive(m_objSalePriceRoot, m_cNKMShopItemTemplet.m_Price > realPrice2);
			if (m_cNKMShopItemTemplet.m_Price > realPrice2)
			{
				NKCResourceUtility.GetOrLoadMiscItemSmallIcon(m_cNKMShopItemTemplet.m_PriceItemID);
				NKCUtil.SetLabelText(m_lbOldPrice, (m_cNKMShopItemTemplet.m_Price * m_iBuyCount).ToString());
			}
			if (!flag)
			{
				Instance.m_priceTag.SetLabelTextColor(Color.red);
				NKCUtil.SetLabelTextColor(m_lbBuyCount, Color.red);
			}
			else if (m_iBuyCount > 1)
			{
				Instance.m_priceTag.SetLabelTextColor(new Color(1f, 69f / 85f, 0.23137255f));
				NKCUtil.SetLabelTextColor(m_lbBuyCount, new Color(1f, 69f / 85f, 0.23137255f));
			}
			else
			{
				Instance.m_priceTag.SetLabelTextColor(Color.white);
				NKCUtil.SetLabelTextColor(m_lbBuyCount, Color.white);
			}
			if (m_iBuyCount > 0 && m_cNKMShopItemTemplet != null && m_NKCUIItemSlot != null)
			{
				bool bShowNumber = IsCountVisible(m_cNKMShopItemTemplet);
				bool bFirstBuy = NKCShopManager.IsFirstBuy(m_cNKMShopItemTemplet.m_ProductID, NKCScenManager.CurrentUserData());
				NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeShopItemData(m_cNKMShopItemTemplet, bFirstBuy);
				slotData.Count *= m_iBuyCount;
				m_NKCUIItemSlot.SetData(slotData, bShowName: false, bShowNumber, bEnableLayoutElement: false, null);
				m_NKCUIItemSlot.SetOnClickAction(NKCUISlot.SlotClickType.RatioList, NKCUISlot.SlotClickType.ChoiceList, NKCUISlot.SlotClickType.BoxList);
			}
		}
	}

	private void SetIntervalItem(NKCUISlot.SlotData slotData)
	{
		bool flag = false;
		if (slotData != null && slotData.eType == NKCUISlot.eSlotMode.ItemMisc)
		{
			NKMItemMiscTemplet nKMItemMiscTemplet = NKMItemMiscTemplet.Find(slotData.ID);
			flag = nKMItemMiscTemplet?.IsTimeIntervalItem ?? false;
			if (flag)
			{
				string timeSpanStringDHM = NKCUtilString.GetTimeSpanStringDHM(nKMItemMiscTemplet.GetIntervalTimeSpanLeft());
				NKCUtil.SetLabelText(m_lbTimeLeft, timeSpanStringDHM);
			}
		}
		NKCUtil.SetGameobjectActive(m_objTimeInterval, flag);
	}

	public void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
		}
		if (m_tEndDateDiscountTime != DateTime.MinValue)
		{
			m_tDeltaTime += Time.unscaledDeltaTime;
			if (m_tDeltaTime > ONE_SECOND)
			{
				UpdateDiscountTime(m_tEndDateDiscountTime);
				m_tDeltaTime = 0f;
			}
		}
		OnUpdateButtonHold();
	}

	public void OnOK()
	{
		List<NKCShopManager.ShopRewardSubstituteData> list = NKCShopManager.MakeShopBuySubstituteItemList(m_cNKMShopItemTemplet, m_iBuyCount, null);
		if (list != null && list.Count > 0)
		{
			NKCPopupShopCustomPackageSubstitude.Instance.Open(list, ConfirmBuy);
		}
		else
		{
			ConfirmBuy();
		}
	}

	private void ConfirmBuy()
	{
		NKCPopupShopCustomPackageSubstitude.CheckInstanceAndClose();
		Close();
		if (dOnOKButton != null)
		{
			dOnOKButton(m_cNKMShopItemTemplet.m_ProductID, m_iBuyCount);
		}
	}

	public void OnPolicy()
	{
		NKCPublisherModule.InAppPurchase.OpenPolicy(null);
	}

	public void OnChangeCount(bool bPlus = true)
	{
		if (m_bWasHold)
		{
			m_bWasHold = false;
			return;
		}
		if (!bPlus && m_iBuyCount == 1)
		{
			if (m_iMaxBuyCount > 0 && m_bUseMinusToMax)
			{
				m_iBuyCount = m_iMaxBuyCount;
				UpdatePriceInfo();
			}
			OnButtonUp();
			return;
		}
		m_iBuyCount += (bPlus ? 1 : (-1));
		if (!bPlus && m_iBuyCount <= 1)
		{
			m_iBuyCount = 1;
		}
		if (bPlus && m_iBuyCount >= m_iMaxBuyCount)
		{
			m_iBuyCount = m_iMaxBuyCount;
		}
		UpdatePriceInfo();
	}

	private void OnMinusDown(PointerEventData eventData)
	{
		m_iChangeValue = -1;
		m_bPress = true;
		m_fDelay = 0.35f;
		m_fHoldTime = 0f;
		m_bWasHold = false;
	}

	private void OnPlusDown(PointerEventData eventData)
	{
		m_iChangeValue = 1;
		m_bPress = true;
		m_fDelay = 0.35f;
		m_fHoldTime = 0f;
		m_bWasHold = false;
	}

	private void OnButtonUp()
	{
		m_iChangeValue = 0;
		m_fDelay = 0.35f;
		m_bPress = false;
	}

	private void OnUpdateButtonHold()
	{
		if (!m_bPress)
		{
			return;
		}
		m_fHoldTime += Time.deltaTime;
		if (m_fHoldTime >= m_fDelay)
		{
			m_fHoldTime = 0f;
			m_fDelay *= 0.8f;
			int num = ((!(m_fDelay < 0.01f)) ? 1 : 5);
			m_fDelay = Mathf.Clamp(m_fDelay, 0.01f, 0.35f);
			m_iBuyCount += m_iChangeValue * num;
			m_bWasHold = true;
			if (m_iChangeValue < 0 && m_iBuyCount < 1)
			{
				m_iBuyCount = 1;
				m_bPress = false;
			}
			if (m_iChangeValue > 0 && m_iBuyCount > m_iMaxBuyCount)
			{
				m_iBuyCount = m_iMaxBuyCount;
				m_bPress = false;
			}
			UpdatePriceInfo();
		}
	}

	public void OnScroll(PointerEventData eventData)
	{
		if (!m_objBuyCount.activeSelf)
		{
			return;
		}
		if (eventData.scrollDelta.y < 0f)
		{
			if (m_iBuyCount > 0)
			{
				m_iBuyCount--;
			}
		}
		else if (eventData.scrollDelta.y > 0f && m_iBuyCount < m_iMaxBuyCount)
		{
			m_iBuyCount++;
		}
		if (m_iMaxBuyCount == 0)
		{
			m_iMaxBuyCount = 1;
		}
		int iBuyCount = Mathf.Clamp(m_iBuyCount, 1, m_iMaxBuyCount);
		m_iBuyCount = iBuyCount;
		UpdateByPriceItemId();
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}

	private void OnBtnLinkedItem()
	{
		if (m_cNKMShopItemTemplet != null)
		{
			NKCPopupShopLinkPreview.Instance.Open(m_cNKMShopItemTemplet.m_ProductID);
		}
	}

	private void OnBtnJPNPaymentLaw()
	{
		NKCPublisherModule.InAppPurchase.OpenPaymentLaw(null);
	}

	private void OnBtnJPNCommercialLaw()
	{
		NKCPublisherModule.InAppPurchase.OpenCommercialLaw(null);
	}

	private void OnClickBillingRestore()
	{
		NKCPublisherModule.InAppPurchase.BillingRestore(NKCShopManager.OnBillingRestoreManual);
	}
}
