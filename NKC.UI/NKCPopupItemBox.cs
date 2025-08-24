using System;
using NKM;
using NKM.Shop;
using NKM.Templet;
using NKM.Templet.Office;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupItemBox : NKCUIBase
{
	public delegate void OnButton();

	public enum eMode
	{
		Normal,
		OKCancel,
		ItemBoxOpen,
		ShopBuy,
		Choice,
		MoveToShop
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_popup_ok_cancel_box";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_ITEM_BOX";

	private static NKCPopupItemBox m_Instance;

	public Text m_lbItemName;

	public Text m_lbItemType;

	public Text m_lbItemCount;

	public Text m_lbItemDesc;

	public GameObject m_objNormalIcons;

	public NKCUISlot m_NKCUIItemSlot;

	public GameObject m_objEmoticonComment;

	public NKCGameHudEmoticonComment m_NKCGameHudEmoticonComment;

	public GameObject m_objEmoticonSD;

	public NKCPopupEmoticonSlotSD m_NKCPopupEmoticonSlotSD;

	public NKCUIComItemDropInfo m_ItemDropInfo;

	public EventTrigger m_etBG;

	public NKCUIComButton m_btnClose;

	[Header("가격")]
	public GameObject m_objPriceRoot;

	public NKCUIPriceTag m_priceTag;

	public GameObject m_objSalePriceRoot;

	public Text m_lbOldPrice;

	public Image m_imgOldPrice;

	[Header("하단 버튼")]
	public NKCUIComButton m_btnCancel;

	public NKCUIComButton m_btnAction1;

	public NKCUIComButton m_btnAction2;

	public Text m_txtAction1;

	public Text m_txtAction2;

	public NKCUIComStateButton m_btnAdOn;

	public NKCUIComStateButton m_btnAdOff;

	public Text m_lbAdLeftCount;

	public Text m_lbAdCoolTime;

	[Header("기간제 아이템 표시")]
	public GameObject m_objTimeInterval;

	public Text m_lbTimeLeft;

	private NKCUISlot.SlotData currentItemData;

	private const int ITEM_MULTIPLE_USE_MAX_COUNT = 10000;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	private bool m_bReservedPlayComment;

	private int m_ReservedCommentEmoticonID = -1;

	private OnButton dOnOKButton;

	private OnButton dOnOKButton2;

	public static NKCPopupItemBox Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCPopupItemBox>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_ITEM_BOX", NKCUIManager.eUIBaseRect.UIFrontPopup, CleanupInstance).GetInstance<NKCPopupItemBox>();
				m_Instance.InitUI();
			}
			return m_Instance;
		}
	}

	public static bool IsInstanceOpen
	{
		get
		{
			if (m_Instance != null)
			{
				return m_Instance.IsOpen;
			}
			return false;
		}
	}

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_POPUP_ITEM_BOX;

	public static NKCPopupItemBox OpenNewInstance()
	{
		NKCPopupItemBox instance = NKCUIManager.OpenNewInstance<NKCPopupItemBox>("ab_ui_nkm_ui_popup_ok_cancel_box", "NKM_UI_POPUP_ITEM_BOX", NKCUIManager.eUIBaseRect.UIFrontPopup, null).GetInstance<NKCPopupItemBox>();
		if (instance != null)
		{
			instance.InitUI();
		}
		return instance;
	}

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
		m_ItemDropInfo?.Init();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.callback.AddListener(delegate
		{
			Close();
		});
		m_etBG.triggers.Add(entry);
		m_btnClose.PointerClick.RemoveAllListeners();
		m_btnClose.PointerClick.AddListener(base.Close);
		m_btnCancel.PointerClick.RemoveAllListeners();
		m_btnCancel.PointerClick.AddListener(base.Close);
		m_btnAction1.PointerClick.RemoveAllListeners();
		m_btnAction1.PointerClick.AddListener(OnOK);
		m_btnAction2.PointerClick.RemoveAllListeners();
		m_btnAction2.PointerClick.AddListener(OnAction2);
		NKCUtil.SetButtonClickDelegate(m_btnAdOn, OnAdWatch);
		m_NKCPopupEmoticonSlotSD.SetClickEvent(delegate
		{
			m_NKCPopupEmoticonSlotSD.PlaySDAni();
		});
		base.gameObject.SetActive(value: false);
	}

	public void OpenItemBox(int itemMiscID, eMode mode = eMode.Normal, OnButton onOkButton = null)
	{
		NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeMiscItemData(itemMiscID, 1L);
		Open(mode, data, onOkButton);
	}

	public void OpenMoldItemBox(int itemMoldID, eMode mode = eMode.Normal, OnButton onOkButton = null)
	{
		NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeMoldItemData(itemMoldID, 1L);
		Open(mode, data, onOkButton);
	}

	public void OpenUnitBox(int unitID, eMode mode = eMode.Normal, OnButton onOkButton = null)
	{
		NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeUnitData(unitID, 1);
		Open(mode, data, onOkButton);
	}

	public void OpenEmoticonBox(int emoticonID, eMode mode = eMode.Normal, OnButton onOkButton = null)
	{
		NKCUISlot.SlotData data = NKCUISlot.SlotData.MakeEmoticonData(emoticonID);
		Open(mode, data, onOkButton);
	}

	public void Open(ShopItemTemplet productTemplet, OnButton onOkButton)
	{
		bool bShowCount = productTemplet != null && IsCountVisible(productTemplet.m_ItemType);
		bool bFirstBuy = productTemplet != null && NKCShopManager.IsFirstBuy(productTemplet.m_ProductID, NKCScenManager.CurrentUserData());
		Open(eMode.ShopBuy, NKCUISlot.SlotData.MakeShopItemData(productTemplet, bFirstBuy), onOkButton, singleOpenOnly: false, bShowCount);
		Instance.m_lbItemDesc.text = NKCUtilString.GetShopDescriptionText(productTemplet.GetItemDescPopup(), bFirstBuy);
		if (productTemplet.m_PriceItemID == 0)
		{
			Instance.m_priceTag.SetData(productTemplet, showMinus: false, changeColor: false);
		}
		else
		{
			int realPrice = NKCScenManager.CurrentUserData().m_ShopData.GetRealPrice(productTemplet);
			Instance.m_priceTag.SetData(productTemplet.m_PriceItemID, realPrice, showMinus: false, changeColor: false, bHidePriceIcon: true);
		}
		SetOldPrice(productTemplet);
	}

	public void Open(NKMShopRandomListData randomProductData, bool showDropInfo, OnButton onOkButton)
	{
		bool bShowCount = randomProductData != null && IsCountVisible(randomProductData.itemType);
		Open(eMode.ShopBuy, NKCUISlot.SlotData.MakeShopItemData(randomProductData), onOkButton, singleOpenOnly: false, bShowCount, showDropInfo);
		int price = randomProductData.GetPrice();
		Instance.m_priceTag.SetData(randomProductData.priceItemId, price, showMinus: false, changeColor: false, bHidePriceIcon: true);
		SetOldPrice(randomProductData.priceItemId, price, randomProductData.price);
	}

	public void Open(eMode mode, NKCUISlot.SlotData data, OnButton onOkButton = null, bool singleOpenOnly = false, bool bShowCount = false, bool showDropInfo = true)
	{
		if (data != null)
		{
			if (data.eType == NKCUISlot.eSlotMode.Emoticon)
			{
				EmoticonOpenProcess(data, mode, bShowCount, showDropInfo);
			}
			else
			{
				NormalOpenProcess(data, mode, bShowCount, showDropInfo);
			}
			SetButton(mode, data.Count, onOkButton, singleOpenOnly);
			NKCAdManager.SetItemRewardAdButtonState(mode, data.ID, m_btnAdOn, m_btnAdOff, m_lbAdLeftCount);
		}
	}

	private void NormalOpenProcess(NKCUISlot.SlotData data, eMode mode, bool bShowNumber, bool showDropInfo)
	{
		NKCUtil.SetGameobjectActive(m_objEmoticonComment, bValue: false);
		NKCUtil.SetGameobjectActive(m_objEmoticonSD, bValue: false);
		NKCUtil.SetGameobjectActive(m_objNormalIcons, bValue: true);
		m_NKCUIItemSlot.SetData(data, bShowName: false, bShowNumber, bEnableLayoutElement: false, null);
		m_NKCUIItemSlot.SetOnClickAction(NKCUISlot.SlotClickType.RatioList, NKCUISlot.SlotClickType.ChoiceList, NKCUISlot.SlotClickType.BoxList);
		CommonOpenProcess(data, mode, bShowNumber, showDropInfo);
	}

	private void CommonOpenProcess(NKCUISlot.SlotData data, eMode mode, bool bShowNumber, bool showDropInfo)
	{
		base.gameObject.SetActive(value: true);
		m_bReservedPlayComment = false;
		m_ReservedCommentEmoticonID = -1;
		SetTextFromSlotdata(data);
		m_NKCUIOpenAnimator.PlayOpenAni();
		currentItemData = data;
		NKCUtil.SetGameobjectActive(m_objPriceRoot, mode == eMode.ShopBuy);
		m_ItemDropInfo?.SetData(showDropInfo ? data : null);
		SetIntervalItemTimeLeft(data);
		UIOpened();
	}

	private void EmoticonOpenProcess(NKCUISlot.SlotData data, eMode mode, bool bShowNumber, bool showDropInfo)
	{
		NKMEmoticonTemplet nKMEmoticonTemplet = NKMEmoticonTemplet.Find(data.ID);
		if (nKMEmoticonTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_objEmoticonComment, nKMEmoticonTemplet.m_EmoticonType == NKM_EMOTICON_TYPE.NET_TEXT);
			NKCUtil.SetGameobjectActive(m_objEmoticonSD, nKMEmoticonTemplet.m_EmoticonType == NKM_EMOTICON_TYPE.NET_ANI);
			NKCUtil.SetGameobjectActive(m_objNormalIcons, bValue: false);
			CommonOpenProcess(data, mode, bShowNumber, showDropInfo);
			if (nKMEmoticonTemplet.m_EmoticonType == NKM_EMOTICON_TYPE.NET_ANI)
			{
				m_NKCPopupEmoticonSlotSD.SetUI(data.ID);
				m_NKCPopupEmoticonSlotSD.PlaySDAni();
			}
			else if (nKMEmoticonTemplet.m_EmoticonType == NKM_EMOTICON_TYPE.NET_TEXT)
			{
				m_bReservedPlayComment = true;
				m_ReservedCommentEmoticonID = data.ID;
			}
		}
	}

	private void SetButton(eMode mode, long count, OnButton onOkButton1 = null, bool singleOpenOnly = false)
	{
		if (singleOpenOnly)
		{
			count = 1L;
		}
		switch (mode)
		{
		case eMode.Normal:
			NKCUtil.SetGameobjectActive(m_btnCancel, bValue: false);
			NKCUtil.SetGameobjectActive(m_btnAction1, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnAction2, bValue: false);
			NKCUtil.SetHotkey(m_btnAction1, HotkeyEventType.Confirm);
			NKCUtil.SetHotkey(m_btnAction2, HotkeyEventType.None);
			NKCUtil.SetLabelText(m_txtAction1, NKCUtilString.GET_STRING_CONFIRM);
			dOnOKButton = onOkButton1;
			dOnOKButton2 = null;
			break;
		case eMode.ItemBoxOpen:
		{
			NKCUtil.SetGameobjectActive(m_btnCancel, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnAction1, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnAction2, count > 1);
			if (count > 1)
			{
				NKCUtil.SetHotkey(m_btnAction1, HotkeyEventType.None);
				NKCUtil.SetHotkey(m_btnAction2, HotkeyEventType.Confirm);
			}
			else
			{
				NKCUtil.SetHotkey(m_btnAction1, HotkeyEventType.Confirm);
				NKCUtil.SetHotkey(m_btnAction2, HotkeyEventType.None);
			}
			long num = Math.Min(count, 10000L);
			NKCUtil.SetLabelText(m_txtAction1, string.Format(NKCUtilString.GET_STRING_USE_ONE_PARAM, 1));
			NKCUtil.SetLabelText(m_txtAction2, string.Format(NKCUtilString.GET_STRING_USE_ONE_PARAM, num));
			dOnOKButton = OnUseSingle;
			dOnOKButton2 = OnUseMany;
			break;
		}
		case eMode.ShopBuy:
		case eMode.MoveToShop:
			NKCUtil.SetGameobjectActive(m_btnCancel, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnAction1, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnAction2, bValue: false);
			NKCUtil.SetHotkey(m_btnAction1, HotkeyEventType.Confirm);
			NKCUtil.SetHotkey(m_btnAction2, HotkeyEventType.None);
			NKCUtil.SetLabelText(m_txtAction1, NKCUtilString.GET_STRING_SHOP_PURCHASE);
			dOnOKButton = onOkButton1;
			dOnOKButton2 = null;
			break;
		case eMode.OKCancel:
			NKCUtil.SetGameobjectActive(m_btnCancel, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnAction1, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnAction2, bValue: false);
			NKCUtil.SetHotkey(m_btnAction1, HotkeyEventType.Confirm);
			NKCUtil.SetHotkey(m_btnAction2, HotkeyEventType.None);
			NKCUtil.SetLabelText(m_txtAction1, NKCUtilString.GET_STRING_CONFIRM);
			dOnOKButton = onOkButton1;
			dOnOKButton2 = null;
			break;
		case eMode.Choice:
			NKCUtil.SetGameobjectActive(m_btnCancel, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnAction1, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnAction2, bValue: false);
			NKCUtil.SetHotkey(m_btnAction1, HotkeyEventType.Confirm);
			NKCUtil.SetHotkey(m_btnAction2, HotkeyEventType.None);
			NKCUtil.SetLabelText(m_txtAction1, NKCUtilString.GET_STRING_USE_CHOICE);
			dOnOKButton = OnUseChoice;
			dOnOKButton2 = null;
			break;
		default:
			NKCUtil.SetGameobjectActive(m_btnCancel, bValue: true);
			NKCUtil.SetGameobjectActive(m_btnAction1, bValue: false);
			NKCUtil.SetGameobjectActive(m_btnAction2, bValue: false);
			NKCUtil.SetHotkey(m_btnAction1, HotkeyEventType.Confirm);
			NKCUtil.SetHotkey(m_btnAction2, HotkeyEventType.None);
			dOnOKButton = null;
			dOnOKButton2 = null;
			break;
		}
	}

	private void SetOldPrice(ShopItemTemplet productTemplet)
	{
		if (productTemplet == null)
		{
			NKCUtil.SetGameobjectActive(m_objSalePriceRoot, bValue: false);
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			int realPrice = nKMUserData.m_ShopData.GetRealPrice(productTemplet);
			SetOldPrice(productTemplet.m_PriceItemID, realPrice, productTemplet.m_Price);
		}
	}

	private void SetOldPrice(int priceItemId, int realPrice, int oldPrice)
	{
		NKCUtil.SetGameobjectActive(m_objSalePriceRoot, oldPrice > realPrice);
		if (oldPrice > realPrice)
		{
			Sprite orLoadMiscItemSmallIcon = NKCResourceUtility.GetOrLoadMiscItemSmallIcon(priceItemId);
			NKCUtil.SetImageSprite(m_imgOldPrice, orLoadMiscItemSmallIcon, bDisableIfSpriteNull: true);
			NKCUtil.SetLabelText(m_lbOldPrice, oldPrice.ToString());
		}
	}

	private bool IsCountVisible(NKM_REWARD_TYPE type)
	{
		switch (type)
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

	private void SetTextFromSlotdata(NKCUISlot.SlotData data)
	{
		NKCUtil.SetGameobjectActive(m_lbItemType, bValue: false);
		NKCUtil.SetGameobjectActive(m_lbItemCount, bValue: false);
		switch (data.eType)
		{
		case NKCUISlot.eSlotMode.ItemMisc:
		{
			NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(data.ID);
			NKMOfficeInteriorTemplet nKMOfficeInteriorTemplet = NKMItemMiscTemplet.FindInterior(data.ID);
			if (nKMOfficeInteriorTemplet != null)
			{
				m_lbItemName.text = itemMiscTempletByID.GetItemName();
				switch (nKMOfficeInteriorTemplet.InteriorCategory)
				{
				case InteriorCategory.DECO:
					NKCUtil.SetGameobjectActive(m_lbItemCount, bValue: false);
					break;
				case InteriorCategory.FURNITURE:
				{
					NKCUtil.SetGameobjectActive(m_lbItemCount, bValue: true);
					NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
					NKCUtil.SetLabelText(m_lbItemCount, NKCStringTable.GetString("SI_DP_INTERIOR_COUNT_TWO_PARAM", nKMUserData.OfficeData.GetFreeInteriorCount(data.ID).ToString("N0"), nKMUserData.OfficeData.GetInteriorCount(data.ID).ToString("N0")));
					break;
				}
				}
				m_lbItemDesc.text = itemMiscTempletByID.GetItemDesc();
			}
			else if (itemMiscTempletByID != null)
			{
				m_lbItemName.text = itemMiscTempletByID.GetItemName();
				m_lbItemDesc.text = itemMiscTempletByID.GetItemDesc();
				NKCUtil.SetGameobjectActive(m_lbItemCount, itemMiscTempletByID.m_ItemMiscType != NKM_ITEM_MISC_TYPE.IMT_VIEW);
				long num = 0L;
				num = ((data.ID == 203) ? NKMMissionManager.GetRepeatMissionDataTimes(NKM_MISSION_TYPE.REPEAT_DAILY) : ((data.ID == 204) ? NKMMissionManager.GetRepeatMissionDataTimes(NKM_MISSION_TYPE.REPEAT_WEEKLY) : ((data.ID != 202) ? NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(data.ID) : NKCScenManager.CurrentUserData().GetMissionAchievePoint())));
				NKCUtil.SetLabelText(m_lbItemCount, NKCUtilString.GET_STRING_ITEM_COUNT_ONE_PARAM, num.ToString("N0"));
			}
			else
			{
				Debug.LogError("ItemTemplet Not Found. ItemID : " + data.ID);
				m_lbItemName.text = "";
				m_lbItemDesc.text = "";
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
			}
			else
			{
				Debug.LogError("NKMItemMoldTemplet Not Found. moldItemID : " + data.ID);
				m_lbItemName.text = "";
				m_lbItemDesc.text = "";
			}
			break;
		}
		case NKCUISlot.eSlotMode.Unit:
		case NKCUISlot.eSlotMode.UnitCount:
		{
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(data.ID);
			if (unitTempletBase2 != null)
			{
				m_lbItemName.text = unitTempletBase2.GetUnitName();
				NKCUtil.SetGameobjectActive(m_lbItemType, bValue: true);
				m_lbItemType.text = unitTempletBase2.GetUnitTitle();
				m_lbItemDesc.text = unitTempletBase2.GetUnitDesc();
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
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(data.ID);
			if (equipTemplet != null)
			{
				if (data.Count > 0)
				{
					m_lbItemName.text = $"{NKCUtilString.GetItemEquipNameWithTier(equipTemplet)} +{data.Count}";
				}
				else
				{
					m_lbItemName.text = NKCUtilString.GetItemEquipNameWithTier(equipTemplet);
				}
				NKCUtil.SetGameobjectActive(m_lbItemType, bValue: true);
				m_lbItemType.text = NKCUtilString.GetEquipTypeString(equipTemplet);
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
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(skinTemplet.m_SkinEquipUnitID);
				m_lbItemName.text = skinTemplet.GetTitle();
				m_lbItemDesc.text = skinTemplet.GetSkinDesc();
				NKCUtil.SetGameobjectActive(m_lbItemType, bValue: true);
				m_lbItemType.text = string.Format(NKCUtilString.GET_STRING_SKIN_ONE_PARAM, unitTempletBase.GetUnitName());
			}
			break;
		}
		case NKCUISlot.eSlotMode.Buff:
		{
			NKMCompanyBuffTemplet companyBuffTemplet = NKMCompanyBuffManager.GetCompanyBuffTemplet(data.ID);
			m_lbItemName.text = companyBuffTemplet.GetBuffName();
			m_lbItemDesc.text = companyBuffTemplet.GetBuffDescForItemPopup();
			break;
		}
		case NKCUISlot.eSlotMode.Emoticon:
		{
			NKMEmoticonTemplet nKMEmoticonTemplet = NKMEmoticonTemplet.Find(data.ID);
			if (nKMEmoticonTemplet != null)
			{
				m_lbItemName.text = nKMEmoticonTemplet.GetEmoticonName();
				m_lbItemDesc.text = nKMEmoticonTemplet.GetEmoticonDesc();
			}
			break;
		}
		default:
			Debug.LogError("Undefined type");
			m_lbItemName.text = "";
			m_lbItemDesc.text = "";
			break;
		}
	}

	private void SetIntervalItemTimeLeft(NKCUISlot.SlotData slotData)
	{
		bool flag = false;
		if (slotData.eType == NKCUISlot.eSlotMode.ItemMisc)
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

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		if (IsInstanceOpen)
		{
			SetTextFromSlotdata(currentItemData);
			if (m_ItemDropInfo != null && m_ItemDropInfo.gameObject.activeSelf)
			{
				m_ItemDropInfo.SetData(currentItemData, initScrollPosition: false);
			}
		}
	}

	public void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
			if (m_bReservedPlayComment)
			{
				m_bReservedPlayComment = false;
				m_NKCGameHudEmoticonComment.PlayPreview(m_ReservedCommentEmoticonID);
				m_ReservedCommentEmoticonID = -1;
			}
			if (currentItemData != null)
			{
				NKCAdManager.UpdateItemRewardAdCoolTime(currentItemData.ID, m_btnAdOn, m_btnAdOff, m_lbAdCoolTime, m_lbAdLeftCount);
			}
		}
	}

	public void OnOK()
	{
		Close();
		if (dOnOKButton != null)
		{
			dOnOKButton();
		}
	}

	public void OnAction2()
	{
		Close();
		if (dOnOKButton2 != null)
		{
			dOnOKButton2();
		}
	}

	public void OnUseSingle()
	{
		if (currentItemData != null)
		{
			NKCPacketSender.Send_NKMPacket_RANDOM_ITEM_BOX_OPEN_REQ(currentItemData.ID, 1);
		}
	}

	public void OnUseMany()
	{
		if (currentItemData != null)
		{
			long num = Math.Min(currentItemData.Count, 10000L);
			NKCPacketSender.Send_NKMPacket_RANDOM_ITEM_BOX_OPEN_REQ(currentItemData.ID, (int)num);
		}
	}

	public void OnBuy()
	{
	}

	public void OnUseChoice()
	{
		if (currentItemData == null)
		{
			return;
		}
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(currentItemData.ID);
		if (itemMiscTempletByID != null)
		{
			switch (itemMiscTempletByID.m_ItemMiscType)
			{
			case NKM_ITEM_MISC_TYPE.IMT_CHOICE_UNIT:
			case NKM_ITEM_MISC_TYPE.IMT_CHOICE_SHIP:
				NKCUISelection.Instance.Open(itemMiscTempletByID);
				break;
			case NKM_ITEM_MISC_TYPE.IMT_CHOICE_OPERATOR:
				NKCUISelectionOperator.Instance.Open(itemMiscTempletByID);
				break;
			case NKM_ITEM_MISC_TYPE.IMT_CHOICE_EQUIP:
				NKCUISelectionEquip.Instance.Open(itemMiscTempletByID);
				break;
			case NKM_ITEM_MISC_TYPE.IMT_CHOICE_MISC:
				NKCUISelectionMisc.Instance.Open(itemMiscTempletByID);
				break;
			case NKM_ITEM_MISC_TYPE.IMT_CHOICE_MOLD:
				Debug.LogError("필요시 추가해야함");
				break;
			case NKM_ITEM_MISC_TYPE.IMT_CHOICE_SKIN:
				NKCUISelectionSkin.Instance.Open(itemMiscTempletByID);
				break;
			}
		}
	}

	public void OnAdWatch()
	{
		Close();
		if (currentItemData != null)
		{
			NKCAdManager.WatchItemRewardAd(currentItemData.ID);
		}
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
	}
}
