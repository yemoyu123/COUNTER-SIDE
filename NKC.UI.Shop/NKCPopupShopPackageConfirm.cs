using System;
using System.Collections.Generic;
using NKC.Publisher;
using NKC.UI.Contract;
using NKM;
using NKM.Contract2;
using NKM.Item;
using NKM.Shop;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Shop;

public class NKCPopupShopPackageConfirm : NKCUIBase, IScrollHandler, IEventSystemHandler
{
	private enum Mode
	{
		Buy,
		Preview
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_shop";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_SHOP_BUY_PACKAGE_CONFIRM";

	private static NKCPopupShopPackageConfirm m_Instance;

	[Header("UI")]
	public Text m_lbTitle;

	public Text m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_INFO_TITLE_TEXT;

	public Text m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_INFO_DESC_TEXT;

	[Space]
	public NKCUISlot m_PackageIcon;

	public RectTransform m_rt_Package_Contents;

	[Header("구매횟수 제한")]
	public GameObject m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_COUNT;

	public Text m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_COUNT_TEXT;

	[Header("갯수 변경")]
	public GameObject m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_BUY_COUNT;

	public NKCUIComStateButton m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_BUY_COUNT_MINUS;

	public NKCUIComStateButton m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_BUY_COUNT_PLUS;

	public Text m_BUY_COUNT_TEXT;

	[Header("사용 재화")]
	public Image m_NKM_UI_POPUP_ITEM_BOX_PRICE_ICON;

	public Text m_NKM_UI_POPUP_ITEM_BOX_PRICE_TEXT;

	public GameObject m_NKM_UI_POPUP_ITEM_BOX_Discountline;

	public Text m_NKM_UI_POPUP_ITEM_BOX_BEFORE;

	[Header("버튼")]
	public NKCUIComStateButton m_NKM_UI_POPUP_CANCLE_BUTTON;

	public NKCUIComStateButton m_NKM_UI_POPUP_BUY_BUTTON;

	public NKCUIComStateButton m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_BG;

	public NKCUIComStateButton m_csbtnOK;

	[Header("할인율")]
	public GameObject m_NKM_UI_SHOP_BUY_CONFIRM_BADGE_DISCOUNT_RATE;

	public Text m_DISCOUNTRATE_TEXT;

	[Header("기간 한정")]
	public GameObject m_NKM_UI_SHOP_BUY_CONFIRM_BADGE_DISCOUNT_TIME;

	public Text m_DISCOUNT_TIME_TEXT1;

	public Text m_DISCOUNT_TIME_TEXT2;

	[Header("청약 철회")]
	public GameObject m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_POLICY;

	public NKCUIComStateButton m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_POLICY_BUTTON;

	[Header("연계 상품")]
	public NKCUIComStateButton m_csbtnLinkedItem;

	[Header("일본법무대응")]
	public GameObject m_JPN_BTN;

	public GameObject m_JPN_POLICY;

	public NKCUIComStateButton m_csbtnJPNPaymentLaw;

	public NKCUIComStateButton m_csbtnJPNCommercialLaw;

	[Header("유닛 풀 보기")]
	public NKCUIComStateButton m_csbtnLinkedUnitPool;

	[Space]
	public NKCUISlot m_pfbSlot;

	private NKCUIShop.OnProductBuyDelegate dOnOKButton;

	private ShopItemTemplet m_cNKMShopItemTemplet;

	private List<int> m_lstSelection;

	private List<NKCUISlot> m_lstItemSlot = new List<NKCUISlot>();

	private List<CustomPickupContractTemplet> m_lstCustomPickupContractTemplet = new List<CustomPickupContractTemplet>();

	private float m_deltaTime;

	public const float PRESS_GAP_MAX = 0.35f;

	public const float PRESS_GAP_MIN = 0.01f;

	public const float DAMPING = 0.8f;

	private float m_fDelay = 0.5f;

	private float m_fHoldTime;

	private int m_iChangeValue;

	private bool m_bPress;

	private bool m_bWasHold;

	private int m_iBuyCount;

	private bool m_bUseMinusToMax;

	public static NKCPopupShopPackageConfirm Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupShopPackageConfirm>("ab_ui_nkm_ui_shop", "NKM_UI_POPUP_SHOP_BUY_PACKAGE_CONFIRM", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupShopPackageConfirm>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_SHOP_PACKAGE_INFO;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.ResourceOnly;

	public override List<int> UpsideMenuShowResourceList => NKCShopManager.GetUpsideMenuItemList(m_cNKMShopItemTemplet);

	private int m_iMaxBuyCount { get; set; }

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static NKCPopupShopPackageConfirm OpenNewInstance()
	{
		NKCPopupShopPackageConfirm instance = NKCUIManager.OpenNewInstance<NKCPopupShopPackageConfirm>("ab_ui_nkm_ui_shop", "NKM_UI_POPUP_SHOP_BUY_PACKAGE_CONFIRM", NKCUIManager.eUIBaseRect.UIFrontPopup, null).GetInstance<NKCPopupShopPackageConfirm>();
		if (instance != null)
		{
			instance.InitUI();
		}
		return instance;
	}

	private void InitUI()
	{
		NKCUtil.SetBindFunction(m_NKM_UI_POPUP_BUY_BUTTON, OnBtnBuy);
		NKCUtil.SetHotkey(m_NKM_UI_POPUP_BUY_BUTTON, HotkeyEventType.Confirm);
		NKCUtil.SetBindFunction(m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_BG, base.Close);
		NKCUtil.SetBindFunction(m_NKM_UI_POPUP_CANCLE_BUTTON, base.Close);
		NKCUtil.SetBindFunction(m_csbtnOK, base.Close);
		NKCUtil.SetHotkey(m_csbtnOK, HotkeyEventType.Confirm);
		NKCUtil.SetBindFunction(m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_BUY_COUNT_PLUS, delegate
		{
			ChangeValue(bPlus: true);
		});
		NKCUtil.SetHotkey(m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_BUY_COUNT_PLUS, HotkeyEventType.Plus);
		NKCUtil.SetBindFunction(m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_BUY_COUNT_MINUS, delegate
		{
			ChangeValue(bPlus: false);
		});
		NKCUtil.SetHotkey(m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_BUY_COUNT_MINUS, HotkeyEventType.Minus);
		NKCUtil.SetBindFunction(m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_POLICY_BUTTON, OnPolicy);
		NKCUtil.SetBindFunction(m_csbtnLinkedItem, OnBtnLinkedItem);
		NKCUtil.SetBindFunction(m_csbtnLinkedUnitPool, OnBtnLinkedUnitPool);
		m_PackageIcon.Init();
		InitHoldButtonEvent();
		NKCUtil.SetGameobjectActive(m_JPN_BTN, bValue: false);
		NKCUtil.SetGameobjectActive(m_JPN_POLICY, bValue: false);
		if (NKCPublisherModule.InAppPurchase.ShowJPNPaymentPolicy())
		{
			NKCUtil.SetBindFunction(m_csbtnJPNPaymentLaw, OnBtnJPNPaymentLaw);
			NKCUtil.SetBindFunction(m_csbtnJPNCommercialLaw, OnBtnJPNCommercialLaw);
		}
	}

	private void InitHoldButtonEvent()
	{
		UpdateHoldButtonEvent(m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_BUY_COUNT_PLUS.PointerDown, OnPlusDown);
		UpdateHoldButtonEvent(m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_BUY_COUNT_PLUS.PointerUp, OnButtonUp);
		UpdateHoldButtonEvent(m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_BUY_COUNT_MINUS.PointerDown, OnMinusDown);
		UpdateHoldButtonEvent(m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_BUY_COUNT_MINUS.PointerUp, OnButtonUp);
	}

	private void UpdateHoldButtonEvent(NKCUnityEvent newEvent, UnityAction<PointerEventData> eventData)
	{
		if (newEvent != null)
		{
			newEvent.RemoveAllListeners();
			newEvent.AddListener(eventData);
		}
	}

	private void UpdateHoldButtonEvent(UnityEvent newEvent, UnityAction eventData)
	{
		if (newEvent != null)
		{
			newEvent.RemoveAllListeners();
			newEvent.AddListener(eventData);
		}
	}

	public void Open(ShopItemTemplet shopItemTemplet, NKCUIShop.OnProductBuyDelegate onClose, List<int> lstSelection = null)
	{
		if (shopItemTemplet != null)
		{
			dOnOKButton = onClose;
			m_cNKMShopItemTemplet = shopItemTemplet;
			m_lstSelection = lstSelection;
			m_iBuyCount = 1;
			m_iMaxBuyCount = 1;
			SetData(Mode.Buy);
			UIOpened();
		}
	}

	public void OpenPreview(ShopItemTemplet shopItemTemplet)
	{
		if (shopItemTemplet != null)
		{
			dOnOKButton = null;
			m_cNKMShopItemTemplet = shopItemTemplet;
			m_lstSelection = null;
			m_iBuyCount = 1;
			m_iMaxBuyCount = 1;
			SetData(Mode.Preview);
			UIOpened();
		}
	}

	private void SetData(Mode mode)
	{
		if (m_cNKMShopItemTemplet == null)
		{
			return;
		}
		switch (mode)
		{
		case Mode.Buy:
		{
			List<ShopItemTemplet> linkedItem = NKCShopManager.GetLinkedItem(m_cNKMShopItemTemplet.m_ProductID);
			NKCUtil.SetGameobjectActive(m_csbtnLinkedItem, linkedItem != null && linkedItem.Count > 0);
			NKCUtil.SetLabelText(m_lbTitle, NKCStringTable.GetString("SI_PF_SHOP_ITEM_PURCHASE_CONFIRM"));
			NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_CANCLE_BUTTON, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_BUY_BUTTON, bValue: true);
			NKCUtil.SetGameobjectActive(m_csbtnOK, bValue: false);
			break;
		}
		case Mode.Preview:
			NKCUtil.SetGameobjectActive(m_csbtnLinkedItem, bValue: false);
			NKCUtil.SetLabelText(m_lbTitle, NKCStringTable.GetString("SI_PF_SHOP_ITEM_PACKAGE_PREVIEW"));
			NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_CANCLE_BUTTON, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_BUY_BUTTON, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnOK, bValue: true);
			break;
		}
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_INFO_TITLE_TEXT, m_cNKMShopItemTemplet.GetItemName());
		string text = m_cNKMShopItemTemplet.GetItemDesc();
		if (text.Contains("\n"))
		{
			text = text.Replace("\n", " ");
		}
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_INFO_DESC_TEXT, text);
		UpdateSlot();
		UpdatePriceInfo();
		int buyCountLeft = NKCShopManager.GetBuyCountLeft(m_cNKMShopItemTemplet.m_ProductID);
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_COUNT_TEXT, NKCShopManager.GetBuyCountString(m_cNKMShopItemTemplet.resetType, buyCountLeft, m_cNKMShopItemTemplet.m_QuantityLimit));
		NKCUtil.SetGameobjectActive(m_DISCOUNT_TIME_TEXT2, m_cNKMShopItemTemplet.HasDateLimit);
		if (m_cNKMShopItemTemplet.HasDateLimit)
		{
			if (NKCSynchronizedTime.IsFinished(m_cNKMShopItemTemplet.EventDateEndUtc))
			{
				NKCUtil.SetLabelText(m_DISCOUNT_TIME_TEXT2, NKCUtilString.GET_STRING_QUIT);
			}
			else
			{
				NKCUtil.SetLabelText(m_DISCOUNT_TIME_TEXT2, NKCUtilString.GetRemainTimeStringOneParam(m_cNKMShopItemTemplet.EventDateEndUtc));
			}
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_POLICY, m_cNKMShopItemTemplet.m_PriceItemID == 0 && NKCShopManager.IsShowPurchasePolicy());
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_POLICY_BUTTON, NKCShopManager.IsShowPurchasePolicyBtn());
		if (mode == Mode.Buy)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_BUY_COUNT, m_cNKMShopItemTemplet.m_PriceItemID != 0);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_BUY_COUNT, bValue: false);
		}
		if (m_cNKMShopItemTemplet.m_PriceItemID != 0)
		{
			if (m_cNKMShopItemTemplet.m_PriceItemID != 0 && m_cNKMShopItemTemplet.m_Price > 0)
			{
				m_iMaxBuyCount = (int)(NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(m_cNKMShopItemTemplet.m_PriceItemID) / m_cNKMShopItemTemplet.m_Price);
			}
			if (m_cNKMShopItemTemplet.m_QuantityLimit > 0 && buyCountLeft > 0)
			{
				m_iMaxBuyCount = Math.Min(buyCountLeft, m_iMaxBuyCount);
			}
			if (m_iMaxBuyCount < 1)
			{
				m_iMaxBuyCount = 1;
			}
			bool flag = !NKCShopManager.IsCustomPackageItem(m_cNKMShopItemTemplet.m_ProductID);
			NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_BUY_COUNT_MINUS.gameObject, flag && m_iMaxBuyCount > 1);
			NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_SHOP_BUY_CONFIRM_BUY_COUNT_PLUS.gameObject, flag && m_iMaxBuyCount > 1);
			NKCUtil.SetGameobjectActive(m_BUY_COUNT_TEXT.gameObject, flag && m_iMaxBuyCount > 1);
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
		bool num = NKCPublisherModule.InAppPurchase.ShowJPNPaymentPolicy();
		bool flag2 = m_cNKMShopItemTemplet.m_PriceItemID == 0;
		bool flag3 = NKCUtil.IsJPNPolicyRelatedItem(m_cNKMShopItemTemplet.m_PriceItemID);
		if (num)
		{
			NKCUtil.SetGameobjectActive(m_JPN_BTN, flag2);
			NKCUtil.SetGameobjectActive(m_JPN_POLICY, !flag2 && flag3);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_JPN_BTN, bValue: false);
			NKCUtil.SetGameobjectActive(m_JPN_POLICY, bValue: false);
		}
		m_lstCustomPickupContractTemplet.Clear();
		foreach (CustomPickupContractTemplet value in NKMTempletContainer<CustomPickupContractTemplet>.Values)
		{
			if (value.TriggetShopProductID == m_cNKMShopItemTemplet.m_ItemID)
			{
				m_lstCustomPickupContractTemplet.Add(value);
			}
		}
		NKCUtil.SetGameobjectActive(m_csbtnLinkedUnitPool, m_lstCustomPickupContractTemplet.Count > 0);
	}

	private bool IsCountVisible(ShopItemTemplet productTemplet)
	{
		if (productTemplet == null)
		{
			return false;
		}
		switch (productTemplet.m_ItemType)
		{
		case NKM_REWARD_TYPE.RT_MISC:
		case NKM_REWARD_TYPE.RT_USER_EXP:
		case NKM_REWARD_TYPE.RT_MOLD:
		case NKM_REWARD_TYPE.RT_SKIN:
		case NKM_REWARD_TYPE.RT_MISSION_POINT:
			return true;
		default:
			return false;
		}
	}

	private bool IsCountVisible(NKCUISlot.SlotData slotData)
	{
		switch (slotData.eType)
		{
		case NKCUISlot.eSlotMode.Skin:
		case NKCUISlot.eSlotMode.Emoticon:
			return false;
		case NKCUISlot.eSlotMode.ItemMisc:
		case NKCUISlot.eSlotMode.Mold:
		case NKCUISlot.eSlotMode.UnitCount:
			return true;
		default:
			return slotData.Count > 1;
		}
	}

	private void SetSlotCount(int count)
	{
		while (m_lstItemSlot.Count < count)
		{
			NKCUISlot nKCUISlot = UnityEngine.Object.Instantiate(m_pfbSlot);
			nKCUISlot.Init();
			nKCUISlot.transform.SetParent(m_rt_Package_Contents, worldPositionStays: false);
			nKCUISlot.transform.localScale = Vector3.one;
			m_lstItemSlot.Add(nKCUISlot);
		}
	}

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnBtnBuy()
	{
		List<NKCShopManager.ShopRewardSubstituteData> list = NKCShopManager.MakeShopBuySubstituteItemList(m_cNKMShopItemTemplet, m_iBuyCount, m_lstSelection);
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
		if (m_cNKMShopItemTemplet != null && dOnOKButton != null)
		{
			dOnOKButton(m_cNKMShopItemTemplet.m_ProductID, m_iBuyCount, m_lstSelection);
		}
	}

	private void Update()
	{
		if (m_cNKMShopItemTemplet != null && m_cNKMShopItemTemplet.HasDateLimit)
		{
			m_deltaTime += Time.deltaTime;
			if (m_deltaTime > 1f)
			{
				m_deltaTime -= 1f;
				if (NKCSynchronizedTime.IsFinished(m_cNKMShopItemTemplet.EventDateEndUtc))
				{
					NKCUtil.SetLabelText(m_DISCOUNT_TIME_TEXT2, NKCUtilString.GET_STRING_QUIT);
				}
				else
				{
					NKCUtil.SetLabelText(m_DISCOUNT_TIME_TEXT2, NKCUtilString.GetRemainTimeStringOneParam(m_cNKMShopItemTemplet.EventDateEndUtc));
				}
			}
		}
		OnUpdateButtonHold();
	}

	private void UpdatePriceInfo()
	{
		if (m_cNKMShopItemTemplet == null)
		{
			return;
		}
		int realPrice = NKCScenManager.CurrentUserData().m_ShopData.GetRealPrice(m_cNKMShopItemTemplet);
		bool flag = realPrice < m_cNKMShopItemTemplet.m_Price;
		if (m_cNKMShopItemTemplet.m_PriceItemID == 0)
		{
			if (flag)
			{
				SetInappPurchasePrice(m_cNKMShopItemTemplet, realPrice * m_iBuyCount, bSale: true, m_cNKMShopItemTemplet.m_Price);
			}
			else
			{
				SetInappPurchasePrice(m_cNKMShopItemTemplet, m_cNKMShopItemTemplet.m_Price);
			}
		}
		else if (flag)
		{
			SetPrice(m_cNKMShopItemTemplet.m_PriceItemID, realPrice * m_iBuyCount, bSale: true, m_cNKMShopItemTemplet.m_Price);
		}
		else
		{
			SetPrice(m_cNKMShopItemTemplet.m_PriceItemID, realPrice * m_iBuyCount);
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_SHOP_BUY_CONFIRM_BADGE_DISCOUNT_RATE, flag);
		NKCUtil.SetLabelText(m_DISCOUNTRATE_TEXT, $"-{(int)m_cNKMShopItemTemplet.m_DiscountRate}%");
		bool bValue = false;
		if (flag && NKCSynchronizedTime.IsEventTime(m_cNKMShopItemTemplet.discountIntervalId, m_cNKMShopItemTemplet.DiscountStartDateUtc, m_cNKMShopItemTemplet.DiscountEndDateUtc) && m_cNKMShopItemTemplet.HasDiscountDateLimit)
		{
			bValue = true;
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_SHOP_BUY_CONFIRM_BADGE_DISCOUNT_TIME, bValue);
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			if (!nKMUserData.CheckPrice(realPrice, m_cNKMShopItemTemplet.m_PriceItemID))
			{
				NKCUtil.SetLabelTextColor(m_NKM_UI_POPUP_ITEM_BOX_PRICE_TEXT, Color.red);
				NKCUtil.SetLabelTextColor(m_BUY_COUNT_TEXT, Color.red);
			}
			else if (m_iBuyCount > 1)
			{
				NKCUtil.SetLabelTextColor(m_NKM_UI_POPUP_ITEM_BOX_PRICE_TEXT, new Color(1f, 69f / 85f, 0.23137255f));
				NKCUtil.SetLabelTextColor(m_BUY_COUNT_TEXT, new Color(1f, 69f / 85f, 0.23137255f));
			}
			else
			{
				NKCUtil.SetLabelTextColor(m_NKM_UI_POPUP_ITEM_BOX_PRICE_TEXT, Color.white);
				NKCUtil.SetLabelTextColor(m_BUY_COUNT_TEXT, Color.white);
			}
		}
	}

	private void SetPrice(int priceItemID, int Price, bool bSale = false, int oldPrice = 0)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_ITEM_BOX_Discountline, bSale);
		if (bSale)
		{
			NKCUtil.SetLabelText(m_NKM_UI_POPUP_ITEM_BOX_BEFORE, oldPrice.ToString());
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_ITEM_BOX_PRICE_ICON, Price > 0);
		Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(priceItemID);
		NKCUtil.SetImageSprite(m_NKM_UI_POPUP_ITEM_BOX_PRICE_ICON, orLoadMiscItemSmallIcon, bDisableIfSpriteNull: true);
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_ITEM_BOX_PRICE_TEXT, (Price > 0) ? Price.ToString() : NKCUtilString.GET_STRING_SHOP_FREE);
	}

	private void SetInappPurchasePrice(ShopItemTemplet cShopItemTemplet, int price, bool bSale = false, int oldPrice = 0)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_ITEM_BOX_Discountline, bSale);
		if (bSale)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_ITEM_BOX_PRICE_ICON, bValue: false);
			NKCUtil.SetLabelText(m_NKM_UI_POPUP_ITEM_BOX_BEFORE, NKCUtilString.GetInAppPurchasePriceString(oldPrice.ToString("N0"), cShopItemTemplet.m_ProductID));
			NKCUtil.SetLabelText(m_NKM_UI_POPUP_ITEM_BOX_PRICE_TEXT, NKCPublisherModule.InAppPurchase.GetLocalPriceString(cShopItemTemplet.m_MarketID, cShopItemTemplet.m_ProductID));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_ITEM_BOX_PRICE_ICON, bValue: false);
			NKCUtil.SetLabelText(m_NKM_UI_POPUP_ITEM_BOX_PRICE_TEXT, NKCPublisherModule.InAppPurchase.GetLocalPriceString(cShopItemTemplet.m_MarketID, cShopItemTemplet.m_ProductID));
		}
	}

	private void ChangeValue(bool bPlus)
	{
		if (m_cNKMShopItemTemplet == null)
		{
			return;
		}
		if (m_bWasHold)
		{
			m_bWasHold = false;
			return;
		}
		if (bPlus)
		{
			m_iBuyCount++;
		}
		else
		{
			m_iBuyCount--;
			if (m_iBuyCount <= 0 && m_bUseMinusToMax)
			{
				m_iBuyCount = m_iMaxBuyCount;
			}
		}
		m_iBuyCount = Mathf.Clamp(m_iBuyCount, 1, m_iMaxBuyCount);
		UpdateSlot();
		UpdatePriceInfo();
	}

	private void UpdateSlot()
	{
		if (m_cNKMShopItemTemplet == null)
		{
			return;
		}
		NKCUtil.SetLabelText(m_BUY_COUNT_TEXT, m_iBuyCount.ToString());
		bool bShowNumber = IsCountVisible(m_cNKMShopItemTemplet);
		bool bFirstBuy = NKCShopManager.IsFirstBuy(m_cNKMShopItemTemplet.m_ProductID, NKCScenManager.CurrentUserData());
		NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeShopItemData(m_cNKMShopItemTemplet, bFirstBuy);
		slotData.Count *= m_iBuyCount;
		m_PackageIcon.SetData(slotData, bShowName: false, bShowNumber, bEnableLayoutElement: false, null);
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(m_cNKMShopItemTemplet.m_ItemID);
		if (itemMiscTempletByID == null || (itemMiscTempletByID.IsPackageItem && itemMiscTempletByID.m_RewardGroupID == 0))
		{
			Debug.LogError("no rewardgroup! ID : " + m_cNKMShopItemTemplet.m_ItemID);
			return;
		}
		List<NKMRandomBoxItemTemplet> randomBoxItemTempletList = NKCRandomBoxManager.GetRandomBoxItemTempletList(itemMiscTempletByID.m_RewardGroupID);
		if (itemMiscTempletByID.m_RewardGroupID != 0 && randomBoxItemTempletList == null)
		{
			Debug.LogError("rewardgroup null! ID : " + itemMiscTempletByID.m_RewardGroupID);
			return;
		}
		int num = 0;
		if (randomBoxItemTempletList != null)
		{
			num += randomBoxItemTempletList.Count;
		}
		if (itemMiscTempletByID.CustomPackageTemplets != null)
		{
			num += itemMiscTempletByID.CustomPackageTemplets.Count;
		}
		SetSlotCount(num);
		if (m_lstItemSlot.Count <= 0)
		{
			return;
		}
		int num2 = 0;
		if (randomBoxItemTempletList != null)
		{
			for (int i = 0; i < randomBoxItemTempletList.Count; i++)
			{
				if (num2 >= m_lstItemSlot.Count)
				{
					break;
				}
				NKCUISlot nKCUISlot = m_lstItemSlot[num2];
				num2++;
				NKMRandomBoxItemTemplet nKMRandomBoxItemTemplet = randomBoxItemTempletList[i];
				NKCUtil.SetGameobjectActive(nKCUISlot, bValue: true);
				NKCUISlot.SlotData slotData2 = NKCUISlot.SlotData.MakeRewardTypeData(nKMRandomBoxItemTemplet.m_reward_type, nKMRandomBoxItemTemplet.m_RewardID, nKMRandomBoxItemTemplet.TotalQuantity_Max * m_iBuyCount);
				nKCUISlot.TurnOffExtraUI();
				nKCUISlot.SetData(slotData2, bShowName: false, IsCountVisible(slotData2), bEnableLayoutElement: false, null);
				nKCUISlot.SetOnClickAction(NKCUISlot.SlotClickType.RatioList, NKCUISlot.SlotClickType.ChoiceList, NKCUISlot.SlotClickType.Tooltip);
				NKCShopManager.ShowShopItemCashCount(nKCUISlot, slotData2, nKMRandomBoxItemTemplet.FreeQuantity_Max * m_iBuyCount, nKMRandomBoxItemTemplet.PaidQuantity_Max * m_iBuyCount);
			}
		}
		if (itemMiscTempletByID.CustomPackageTemplets != null)
		{
			for (int j = 0; j < itemMiscTempletByID.CustomPackageTemplets.Count; j++)
			{
				if (num2 >= m_lstItemSlot.Count)
				{
					break;
				}
				NKCUISlot nKCUISlot2 = m_lstItemSlot[num2];
				num2++;
				NKCUtil.SetGameobjectActive(nKCUISlot2, bValue: true);
				NKMCustomPackageElement nKMCustomPackageElement = null;
				if (m_lstSelection != null && j < m_lstSelection.Count)
				{
					nKMCustomPackageElement = itemMiscTempletByID.CustomPackageTemplets[j].Get(m_lstSelection[j]);
				}
				if (nKMCustomPackageElement != null)
				{
					NKCUISlot.SlotData slotData3 = NKCUISlot.SlotData.MakeRewardTypeData(nKMCustomPackageElement.RewardType, nKMCustomPackageElement.RewardId, nKMCustomPackageElement.TotalRewardCount * m_iBuyCount);
					nKCUISlot2.SetData(slotData3, bShowName: false, IsCountVisible(slotData3), bEnableLayoutElement: false, null);
					nKCUISlot2.SetOnClickAction(NKCUISlot.SlotClickType.RatioList, NKCUISlot.SlotClickType.ChoiceList, NKCUISlot.SlotClickType.Tooltip);
					nKCUISlot2.SetShowArrowBGText(bSet: true);
					nKCUISlot2.SetArrowBGText(NKCStringTable.GetString("SI_DP_SHOP_SLOT_CHOICE"), new Color(2f / 3f, 0.03137255f, 0.03137255f));
					if (NKCShopManager.WillOverflowOnGain(nKMCustomPackageElement.RewardType, nKMCustomPackageElement.RewardId, nKMCustomPackageElement.TotalRewardCount * m_iBuyCount))
					{
						nKCUISlot2.SetHaveCountString(bShow: true, NKCStringTable.GetString("SI_DP_ICON_SLOT_ALREADY_HAVE"));
					}
					else if (NKCShopManager.IsCustomPackageSelectionHasDuplicate(itemMiscTempletByID, j, m_lstSelection, bIgnoreIfFirstItem: false))
					{
						nKCUISlot2.SetHaveCountString(bShow: true, NKCStringTable.GetString("SI_DP_SHOP_CUSTOM_DUPLICATE"));
					}
					else
					{
						nKCUISlot2.SetHaveCountString(bShow: false, null);
					}
				}
				else
				{
					nKCUISlot2.SetEmpty();
				}
			}
		}
		for (int k = num2; k < m_lstItemSlot.Count; k++)
		{
			NKCUtil.SetGameobjectActive(m_lstItemSlot[k], bValue: false);
		}
	}

	private void OnPolicy()
	{
		NKCPublisherModule.InAppPurchase.OpenPolicy(null);
	}

	private void OnBtnLinkedItem()
	{
		if (m_cNKMShopItemTemplet != null)
		{
			NKCPopupShopLinkPreview.Instance.Open(m_cNKMShopItemTemplet.m_ProductID);
		}
	}

	private void OnBtnLinkedUnitPool()
	{
		if (m_lstCustomPickupContractTemplet.Count > 0)
		{
			NKCUIContractPopupRateV2.Instance.Open(m_lstCustomPickupContractTemplet);
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
			m_fDelay = Mathf.Clamp(m_fDelay, 0.01f, 0.35f);
			m_iBuyCount += m_iChangeValue;
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
			UpdateSlot();
			UpdatePriceInfo();
		}
	}

	public void OnScroll(PointerEventData eventData)
	{
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
		UpdateSlot();
		UpdatePriceInfo();
	}
}
