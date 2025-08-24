using System.Collections.Generic;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCPopupItemEquipBox : NKCUIBase
{
	public enum EQUIP_BOX_BOTTOM_MENU_TYPE
	{
		EBBMT_NONE,
		EBBMT_ENFORCE_AND_EQUIP,
		EBBMT_CHANGE,
		EBBMT_PRESET_CHANGE,
		EBBMT_OK
	}

	public delegate void OnButton();

	private const string ASSET_BUNDLE_NAME = "AB_UI_ITEM_EQUIP_SLOT_CARD";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_ITEM_EQUIP_BOX";

	private static NKCPopupItemEquipBox m_Popup;

	[Header("Contents/Title")]
	public Text m_lbTitle;

	[Header("Contents/Slots")]
	public GameObject m_NKM_UI_POPUP_ITEM_EQUIP_UNIT;

	public NKCUIUnitSelectListSlot m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT;

	public NKCUIInvenEquipSlot m_NKCUIInvenEquipSlot;

	public GameObject m_NKM_UI_POPUP_ITEM_SLOT_01_NUMBER;

	public NKCUIInvenEquipSlot m_NKCUIInvenEquipSlot2;

	public GameObject m_NKM_UI_POPUP_ITEM_SLOT_02_NUMBER;

	public GameObject m_NKM_UI_POPUP_ITEM_ICON_02;

	public NKCUIComItemDropInfo m_DropInfo;

	[Header("Buttons")]
	public GameObject m_NKM_UI_POPUP_OK_ROOT;

	public NKCUIComButton m_UnEquipButton;

	public NKCUIComButton m_ReinforceButton;

	public Image m_imgNKM_UI_POPUP_OK_BOX_REINFORCE;

	public Text m_txtNKM_UI_POPUP_REINFORCE_TEXT;

	public NKCUIComButton m_ReinforceButtonLock;

	public NKCUIComButton m_EquipButton;

	public NKCUIComButton m_ChangeButton;

	public NKCUIComButton m_OkButton;

	public NKCUIComStateButton m_NKM_UI_POPUP_OK_BOX_CANCEL;

	public NKCUIComButton m_NKM_UI_POPUP_OK_BOX_SELECT_1;

	public Text m_NKM_UI_POPUP_CHANGE_TEXT_1;

	public NKCUIComButton m_NKM_UI_POPUP_OK_BOX_SELECT_2;

	public Text m_NKM_UI_POPUP_CHANGE_TEXT_2;

	[Header("업그레이드 완료창 전용")]
	public GameObject m_objUpgradeEffect;

	private OnButton dOnEquipButton;

	public OnButton m_dOnClose;

	private NKCUIOpenAnimator m_NKCUIOpenAnimator;

	private long m_UnitUIDForChange;

	private ITEM_EQUIP_POSITION m_ITEM_EQUIP_POSITION_For_Change;

	private long m_SelectedItemUID;

	private bool m_bShowFierceInfo;

	private int m_iPresetIndex = -1;

	private List<long> m_listPresetEquip;

	public override eMenutype eUIType => eMenutype.Popup;

	public override string MenuName => NKCUtilString.GET_STRING_POPUP_ITEM_EQUIP_BOX;

	public static NKCPopupItemEquipBox OpenInstance()
	{
		NKCPopupItemEquipBox instance = NKCUIManager.OpenNewInstance<NKCPopupItemEquipBox>("AB_UI_ITEM_EQUIP_SLOT_CARD", "NKM_UI_POPUP_ITEM_EQUIP_BOX", NKCUIManager.eUIBaseRect.UIFrontPopup, null).GetInstance<NKCPopupItemEquipBox>();
		if ((object)instance != null)
		{
			instance.InitUI();
			return instance;
		}
		return instance;
	}

	public void InitUI()
	{
		m_NKCUIOpenAnimator = new NKCUIOpenAnimator(base.gameObject);
		m_UnEquipButton.PointerClick.RemoveAllListeners();
		m_UnEquipButton.PointerClick.AddListener(OnClickUnEquip);
		m_ReinforceButton.PointerClick.RemoveAllListeners();
		m_ReinforceButton.PointerClick.AddListener(OnClickEquipEnhance);
		m_ReinforceButtonLock.PointerClick.RemoveAllListeners();
		m_ReinforceButtonLock.PointerClick.AddListener(OnClickEquipEnhance);
		m_EquipButton.PointerClick.RemoveAllListeners();
		m_EquipButton.PointerClick.AddListener(OnClickEquipBtn);
		NKCUtil.SetHotkey(m_EquipButton, HotkeyEventType.Confirm);
		m_ChangeButton.PointerClick.RemoveAllListeners();
		m_ChangeButton.PointerClick.AddListener(OnClickEquipBtn);
		NKCUtil.SetHotkey(m_ChangeButton, HotkeyEventType.Confirm);
		m_OkButton.PointerClick.RemoveAllListeners();
		m_OkButton.PointerClick.AddListener(base.Close);
		NKCUtil.SetHotkey(m_OkButton, HotkeyEventType.Confirm);
		m_NKM_UI_POPUP_OK_BOX_CANCEL.PointerClick.RemoveAllListeners();
		m_NKM_UI_POPUP_OK_BOX_CANCEL.PointerClick.AddListener(base.Close);
		base.gameObject.SetActive(value: false);
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_CHANGE_TEXT_1, NKCUtilString.GET_STRING_EQUIP_SELECT_ACC_1);
		NKCUtil.SetLabelText(m_NKM_UI_POPUP_CHANGE_TEXT_2, NKCUtilString.GET_STRING_EQUIP_SELECT_ACC_2);
		m_DropInfo.Init();
	}

	public static void Open(NKMEquipItemData cNKMEquipItemData, EQUIP_BOX_BOTTOM_MENU_TYPE bottomMenuType = EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_ENFORCE_AND_EQUIP, OnButton dOnEquipButton = null)
	{
		if (cNKMEquipItemData != null)
		{
			if (m_Popup == null)
			{
				m_Popup = OpenInstance();
			}
			m_Popup.ResetUI();
			m_Popup.dOnEquipButton = dOnEquipButton;
			m_Popup.m_ChangeButton.PointerClick.RemoveAllListeners();
			m_Popup.m_ChangeButton.PointerClick.AddListener(m_Popup.OnClickEquipBtn);
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(cNKMEquipItemData.m_ItemEquipID);
			string itemID = ((equipTemplet != null) ? equipTemplet.m_ItemEquipStrID : string.Empty);
			if (equipTemplet != null)
			{
				NKCUtil.SetLabelTextColor(m_Popup.m_txtNKM_UI_POPUP_REINFORCE_TEXT, NKCUtil.GetColor("#FFFFFF"));
				NKCUtil.SetImageSprite(m_Popup.m_imgNKM_UI_POPUP_OK_BOX_REINFORCE, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_BLUE));
			}
			m_Popup.m_NKCUIInvenEquipSlot.SetData(cNKMEquipItemData);
			m_Popup.SetTextFromSlotdata(cNKMEquipItemData, bottomMenuType);
			m_Popup.m_DropInfo.SetData(itemID);
			m_Popup.gameObject.SetActive(value: true);
			m_Popup.m_NKCUIOpenAnimator.PlayOpenAni();
			m_Popup.UIOpened();
		}
	}

	public static void Open(NKMUnitReactorTemplet reactorTemplet, int iReactorLevel)
	{
		if (reactorTemplet != null)
		{
			if (m_Popup == null)
			{
				m_Popup = OpenInstance();
			}
			m_Popup.ResetUI();
			m_Popup.SetSlotData(reactorTemplet, iReactorLevel);
			m_Popup.gameObject.SetActive(value: true);
			m_Popup.m_NKCUIOpenAnimator.PlayOpenAni();
			m_Popup.UIOpened();
		}
	}

	public static void OpenForConfirm(NKMEquipItemData cNKMEquipItemData, UnityAction dOnEquipButton = null, bool bFierceInfo = false, bool bShowCancel = true, OnButton dOnClose = null)
	{
		if (cNKMEquipItemData != null)
		{
			if (m_Popup == null)
			{
				m_Popup = OpenInstance();
			}
			m_Popup.ResetUI();
			if (dOnEquipButton != null)
			{
				m_Popup.m_OkButton.PointerClick.RemoveAllListeners();
				m_Popup.m_OkButton.PointerClick.AddListener(m_Popup.Close);
				m_Popup.m_OkButton.PointerClick.AddListener(dOnEquipButton);
			}
			m_Popup.m_dOnClose = dOnClose;
			m_Popup.m_NKCUIInvenEquipSlot.SetData(cNKMEquipItemData, bFierceInfo);
			m_Popup.SetTextFromSlotdata(cNKMEquipItemData, EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE, bInitOKBtn: false);
			NKCUtil.SetGameobjectActive(m_Popup.m_NKM_UI_POPUP_OK_BOX_CANCEL.gameObject, bShowCancel);
			m_Popup.gameObject.SetActive(value: true);
			m_Popup.m_NKCUIOpenAnimator.PlayOpenAni();
			m_Popup.UIOpened();
		}
	}

	public static void OpenForPresetChange(long unitUID, long equipItemUId, ITEM_EQUIP_POSITION equipPosition, int presetIndex, List<long> presetEquipList, bool bShowFierceInfo = false)
	{
		NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(equipItemUId);
		if (itemEquip != null)
		{
			if (m_Popup == null)
			{
				m_Popup = OpenInstance();
			}
			m_Popup.ResetUI();
			m_Popup.m_ITEM_EQUIP_POSITION_For_Change = equipPosition;
			m_Popup.m_SelectedItemUID = itemEquip.m_ItemUid;
			m_Popup.m_UnitUIDForChange = unitUID;
			m_Popup.m_bShowFierceInfo = bShowFierceInfo;
			m_Popup.m_iPresetIndex = presetIndex;
			m_Popup.m_listPresetEquip = presetEquipList;
			m_Popup.m_ChangeButton.PointerClick.RemoveAllListeners();
			m_Popup.m_ChangeButton.PointerClick.AddListener(m_Popup.OnClickPresetChange);
			m_Popup.m_NKCUIInvenEquipSlot.SetData(itemEquip, bShowFierceInfo);
			m_Popup.SetTextFromSlotdata(itemEquip, EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_PRESET_CHANGE);
			m_Popup.m_UnEquipButton.PointerClick.RemoveAllListeners();
			m_Popup.m_UnEquipButton.PointerClick.AddListener(m_Popup.OnClickPresetUnEquip);
			m_Popup.gameObject.SetActive(value: true);
			m_Popup.m_NKCUIOpenAnimator.PlayOpenAni();
			m_Popup.UIOpened();
		}
	}

	public static void OpenForSelectItem(long itemUID1, long itemUID2, UnityAction func1, UnityAction func2)
	{
		if (m_Popup == null)
		{
			m_Popup = OpenInstance();
		}
		m_Popup.ResetUI();
		NKMEquipItemData itemEquip = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(itemUID1);
		NKMEquipItemData itemEquip2 = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(itemUID2);
		if (itemEquip == null || itemEquip2 == null)
		{
			return;
		}
		if (itemEquip.m_OwnerUnitUID > 0)
		{
			NKCScenManager.CurrentUserData();
			NKMUnitData unitFromUID = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(itemEquip.m_OwnerUnitUID);
			if (unitFromUID != null)
			{
				m_Popup.m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT?.SetData(unitFromUID, NKMDeckIndex.None, bEnableLayoutElement: false, null);
				m_Popup.m_NKM_UI_UNIT_SELECT_LIST_UNIT_SLOT?.SetSlotState(NKCUnitSortSystem.eUnitState.NONE);
			}
		}
		m_Popup.m_NKCUIInvenEquipSlot?.SetData(itemEquip);
		m_Popup.m_NKCUIInvenEquipSlot2?.SetData(itemEquip2);
		if (m_Popup.m_NKM_UI_POPUP_OK_BOX_SELECT_1 != null)
		{
			NKCUtil.SetGameobjectActive(m_Popup.m_NKM_UI_POPUP_OK_BOX_SELECT_1, bValue: true);
			m_Popup.m_NKM_UI_POPUP_OK_BOX_SELECT_1.PointerClick.RemoveAllListeners();
			m_Popup.m_NKM_UI_POPUP_OK_BOX_SELECT_1.PointerClick.AddListener(func1);
		}
		if (m_Popup.m_NKM_UI_POPUP_OK_BOX_SELECT_2 != null)
		{
			NKCUtil.SetGameobjectActive(m_Popup.m_NKM_UI_POPUP_OK_BOX_SELECT_2, bValue: true);
			m_Popup.m_NKM_UI_POPUP_OK_BOX_SELECT_2.PointerClick.RemoveAllListeners();
			m_Popup.m_NKM_UI_POPUP_OK_BOX_SELECT_2.PointerClick.AddListener(func2);
		}
		NKCUtil.SetGameobjectActive(m_Popup.m_NKM_UI_POPUP_ITEM_ICON_02, bValue: true);
		NKCUtil.SetGameobjectActive(m_Popup.m_NKM_UI_POPUP_ITEM_SLOT_01_NUMBER, bValue: true);
		NKCUtil.SetGameobjectActive(m_Popup.m_NKM_UI_POPUP_ITEM_SLOT_02_NUMBER, bValue: true);
		NKCUtil.SetGameobjectActive(m_Popup.m_NKM_UI_POPUP_OK_BOX_CANCEL, bValue: true);
		NKCUtil.SetGameobjectActive(m_Popup.m_NKM_UI_POPUP_ITEM_EQUIP_UNIT, itemEquip.m_OwnerUnitUID > 0);
		ShowTitle(NKCStringTable.GetString("SI_PF_POPUP_ITEM_EQUIP_BOX_ACC_CHOICE_TITLE"));
		m_Popup.gameObject.SetActive(value: true);
		m_Popup.m_NKCUIOpenAnimator.PlayOpenAni();
		m_Popup.UIOpened();
	}

	private void ResetUI()
	{
		NKCUtil.SetGameobjectActive(m_lbTitle, bValue: false);
		NKCUtil.SetGameobjectActive(m_DropInfo, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_ITEM_EQUIP_UNIT, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_ITEM_SLOT_01_NUMBER, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_ITEM_SLOT_02_NUMBER, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_ITEM_ICON_02, bValue: false);
		NKCUtil.SetGameobjectActive(m_objUpgradeEffect, bValue: false);
		NKCUtil.SetGameobjectActive(m_UnEquipButton, bValue: false);
		NKCUtil.SetGameobjectActive(m_ReinforceButton, bValue: false);
		NKCUtil.SetGameobjectActive(m_ReinforceButtonLock, bValue: false);
		NKCUtil.SetGameobjectActive(m_EquipButton, bValue: false);
		NKCUtil.SetGameobjectActive(m_ChangeButton, bValue: false);
		NKCUtil.SetGameobjectActive(m_OkButton, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_OK_BOX_SELECT_1, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_OK_BOX_SELECT_2, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_OK_BOX_CANCEL, bValue: false);
	}

	private void OnClickEmptySlot(NKCUISlotEquip cItemSlot, NKMEquipItemData equipData)
	{
		if (m_NKCUIInvenEquipSlot.GetNKMEquipTemplet() != null && m_NKCUIInvenEquipSlot.GetNKMEquipItemData() != null)
		{
			NKMItemManager.UnEquip(m_NKCUIInvenEquipSlot.GetNKMEquipItemData().m_ItemUid);
		}
	}

	public void OnClickChange()
	{
		NKCUtil.ChangeEquip(m_UnitUIDForChange, m_ITEM_EQUIP_POSITION_For_Change, OnClickEmptySlot, m_SelectedItemUID, m_bShowFierceInfo);
	}

	public void OnClickPresetChange()
	{
		if (m_NKCUIInvenEquipSlot.GetNKMEquipTemplet() != null)
		{
			int iPresetIndex = m_iPresetIndex;
			NKCUtil.ChangePresetEquip(m_UnitUIDForChange, m_iPresetIndex, m_SelectedItemUID, m_listPresetEquip, m_ITEM_EQUIP_POSITION_For_Change, m_NKCUIInvenEquipSlot.GetNKMEquipTemplet().m_EquipUnitStyleType, m_bShowFierceInfo, delegate
			{
				NKCPacketSender.Send_NKMPacket_EQUIP_PRESET_REGISTER_REQ(iPresetIndex, m_ITEM_EQUIP_POSITION_For_Change, 0L);
			});
		}
	}

	private void OnClickEquipBtn()
	{
		if (dOnEquipButton != null)
		{
			dOnEquipButton();
			dOnEquipButton = null;
		}
	}

	public void OnClickEquipEnhance()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BASE_FACTORY))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.BASE_FACTORY);
		}
		else if (!NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_ENCHANT))
		{
			NKCContentManager.ShowLockedMessagePopup(ContentsType.FACTORY_ENCHANT);
		}
		else if (m_NKCUIInvenEquipSlot != null && m_NKCUIInvenEquipSlot.GetNKMEquipItemData() != null)
		{
			NKM_ERROR_CODE nKM_ERROR_CODE = NKMItemManager.CanEnchantItem(NKCScenManager.GetScenManager().GetMyUserData(), m_NKCUIInvenEquipSlot.GetNKMEquipItemData());
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString(nKM_ERROR_CODE.ToString()));
				return;
			}
			Close();
			NKCUIForge.Instance.Open(NKCUIForge.NKC_FORGE_TAB.NFT_ENCHANT, m_NKCUIInvenEquipSlot.GetNKMEquipItemData().m_ItemUid);
		}
	}

	public void OnClickUnEquip()
	{
		NKMItemManager.UnEquip(m_NKCUIInvenEquipSlot.GetEquipItemUID());
	}

	public void OnClickPresetUnEquip()
	{
		NKCPacketSender.Send_NKMPacket_EQUIP_PRESET_REGISTER_REQ(m_iPresetIndex, m_ITEM_EQUIP_POSITION_For_Change, 0L);
	}

	private void SetTextFromSlotdata(NKMEquipItemData cNKMEquipItemData, EQUIP_BOX_BOTTOM_MENU_TYPE bottomMenuType = EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_ENFORCE_AND_EQUIP, bool bInitOKBtn = true)
	{
		if (cNKMEquipItemData == null)
		{
			return;
		}
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(cNKMEquipItemData.m_ItemEquipID);
		if (equipTemplet != null)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_POPUP_OK_ROOT, bValue: true);
			switch (bottomMenuType)
			{
			case EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE:
				NKCUtil.SetGameobjectActive(m_OkButton, bValue: true);
				break;
			case EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_PRESET_CHANGE:
				NKCUtil.SetGameobjectActive(m_ReinforceButton, NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_ENCHANT) && equipTemplet.m_ItemEquipPosition != ITEM_EQUIP_POSITION.IEP_ENCHANT);
				NKCUtil.SetGameobjectActive(m_ReinforceButtonLock, !NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_ENCHANT) && equipTemplet.m_ItemEquipPosition != ITEM_EQUIP_POSITION.IEP_ENCHANT);
				NKCUtil.SetGameobjectActive(m_UnEquipButton, bValue: true);
				NKCUtil.SetGameobjectActive(m_ChangeButton, bValue: true);
				break;
			default:
				NKCUtil.SetGameobjectActive(m_UnEquipButton, cNKMEquipItemData.m_OwnerUnitUID > 0);
				NKCUtil.SetGameobjectActive(m_ReinforceButton, NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_ENCHANT) && equipTemplet.m_ItemEquipPosition != ITEM_EQUIP_POSITION.IEP_ENCHANT);
				NKCUtil.SetGameobjectActive(m_ReinforceButtonLock, !NKCContentManager.IsContentsUnlocked(ContentsType.FACTORY_ENCHANT) && equipTemplet.m_ItemEquipPosition != ITEM_EQUIP_POSITION.IEP_ENCHANT);
				NKCUtil.SetGameobjectActive(m_EquipButton, cNKMEquipItemData.m_OwnerUnitUID <= 0 && equipTemplet.m_ItemEquipPosition != ITEM_EQUIP_POSITION.IEP_ENCHANT);
				NKCUtil.SetGameobjectActive(m_ChangeButton, cNKMEquipItemData.m_OwnerUnitUID > 0);
				NKCUtil.SetGameobjectActive(m_OkButton, equipTemplet.m_ItemEquipPosition == ITEM_EQUIP_POSITION.IEP_ENCHANT);
				break;
			}
			if (bInitOKBtn)
			{
				m_OkButton.PointerClick.RemoveAllListeners();
				m_OkButton.PointerClick.AddListener(base.Close);
			}
		}
	}

	public static void ShowTitle(string title)
	{
		if (m_Popup != null)
		{
			NKCUtil.SetGameobjectActive(m_Popup.m_lbTitle, bValue: true);
			NKCUtil.SetLabelText(m_Popup.m_lbTitle, title);
		}
	}

	public static void ShowUpgradeCompleteEffect()
	{
		if (m_Popup != null)
		{
			NKCUtil.SetGameobjectActive(m_Popup.m_objUpgradeEffect, bValue: true);
		}
	}

	private void SetSlotData(NKMUnitReactorTemplet reactorTemplet, int reactorLv)
	{
		if (reactorTemplet != null)
		{
			m_NKCUIInvenEquipSlot.SetData(reactorTemplet, reactorLv);
			NKCUtil.SetGameobjectActive(m_OkButton, bValue: true);
			NKCUtil.SetBindFunction(m_OkButton, base.Close);
		}
	}

	public void Update()
	{
		if (base.IsOpen)
		{
			m_NKCUIOpenAnimator.Update();
		}
	}

	public static void CloseItemBox()
	{
		if (m_Popup != null && m_Popup.IsOpen)
		{
			m_Popup.Close();
		}
	}

	public void OnOK()
	{
		Close();
	}

	public override void CloseInternal()
	{
		m_UnEquipButton.PointerClick.RemoveAllListeners();
		m_UnEquipButton.PointerClick.AddListener(OnClickUnEquip);
		m_iPresetIndex = -1;
		m_listPresetEquip = null;
		base.gameObject.SetActive(value: false);
		if (m_dOnClose != null)
		{
			m_dOnClose();
			m_dOnClose = null;
		}
	}
}
