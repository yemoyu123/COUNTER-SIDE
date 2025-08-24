using System;
using System.Collections;
using ClientPacket.Common;
using ClientPacket.Item;
using NKC.PacketHandler;
using NKC.UI.Component;
using NKC.UI.Trim;
using NKM;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIForgeTuning : MonoBehaviour
{
	public enum NKC_TUNING_TAB
	{
		NTT_PRECISION,
		NTT_OPTION_CHANGE,
		NTT_SET_OPTION_CHANGE
	}

	public delegate void OnSelectSlot(int idx);

	private enum SET_OPTION_UI_STATE
	{
		NOT_SELECTED,
		FIRST_FREE_CHANGE,
		CAN_POSSIBLE_CHANGE,
		FIXED_SET_OPTION,
		DONT_HAVE_SET_OPTION_DATA
	}

	private NKC_TUNING_TAB m_NKC_TUNING_TAB;

	private long m_LeftEquipUID;

	public NKCUIRectMove m_rmTuningRoot;

	[Header("panel")]
	public GameObject m_NKM_UI_FACTORY_TUNING_PRECISION_panel;

	public GameObject m_NKM_UI_FACTORY_TUNING_OPTION_CHANGE_panel;

	public GameObject m_NKM_UI_FACTORY_TUNING_SET_CHANGE_panel;

	[Header("정밀화 버튼")]
	public Image m_NKM_UI_FACTORY_TUNING_BUTTON_PRECISION;

	public GameObject m_NKM_UI_FACTORY_TUNING_BUTTON_PRECISION_LIGHT;

	public NKCUIComButton m_btnNKM_UI_FACTORY_TUNING_BUTTON_PRECISION;

	[Header("옵션 변경 경고 메시지")]
	public GameObject m_NKM_UI_FACTORY_TUNING_OPTION_CHANGE_WARNING;

	[Header("탭 토글")]
	public NKCUIComToggle m_RefineToggleBtn;

	public NKCUIComToggle m_ChangeToggleBtn;

	public NKCUIComToggle m_SetOptionChangeToggleBtn;

	[Header("튜닝 버튼")]
	public Image m_NKM_UI_FACTORY_TUNING_BUTTON_OK;

	public NKCUIComButton m_btnNKM_UI_FACTORY_TUNING_BUTTON_OK;

	public NKCUIComButton m_btnNKM_UI_FACTORY_TUNING_BUTTON_CHANGE;

	public Image m_img_NKM_UI_FACTORY_TUNING_PRECISION_ICON;

	public Text m_txt_NKM_UI_FACTORY_TUNING_PRECISION_TEXT;

	public Image m_img_NKM_UI_FACTORY_TUNING_OK_ICON;

	public Text m_txt_NKM_UI_FACTORY_TUNING_OK_TEXT;

	public Image m_img_NKM_UI_FACTORY_TUNING_BUTTON_CHANGE;

	public Text m_txt_NKM_UI_FACTORY_TUNING_BUTTON_CHANGE;

	public Image m_img_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_CHANGE;

	public NKCUIComStateButton m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_CHANGE;

	public Text m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_CHANGE_TEXT;

	public Image m_img_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_OK;

	public Image m_img_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_OK_ICON;

	public NKCUIComStateButton m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_OK;

	public Text m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_OK_TEXT;

	public NKCUIComStateButton m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_FREE;

	public NKCUIComToggle m_firstOptionBtn;

	public NKCUIComToggle m_secondOptionBtn;

	private int m_iSelectOptionIdx = 1;

	public NKCUIComStateButton m_csbtn_TUNING_RATE_BUTTON;

	[Header("소모 재화 슬롯")]
	public NKCUIItemCostSlot m_MaterialSlot1;

	public NKCUIItemCostSlot m_MaterialSlot2;

	[Header("리뉴얼 옵션(최대2개)")]
	public NKCUIForgeTuningOptionSlot[] m_NKM_UI_FACTORY_TUNING_OPTION_SLOT;

	public NKCUIComToggleGroup m_NKM_UI_FACTORY_TUNING_OPTION_SLOT_toggle;

	[Header("옵션 목록 보기")]
	public NKCUIComStateButton m_NKM_UI_FACTORY_TUNING_OPTION_CHANGE_LIST_BUTTON;

	public NKCUIComStateButton m_NKM_UI_FACTORY_TUNING_SET_OPTION_CHANGE_BUTTON;

	[Header("세트 옵션 변경")]
	public GameObject m_NowOption_SET_None_Text;

	public Text m_txtNowOption_SET_None_Text;

	public Image m_NowOption_SET_ICON;

	public Text m_NowOptionSET_NAME;

	public GameObject m_NowOptionOption1;

	public Text m_NowOption_STAT_TEXT_1;

	public GameObject m_NowOptionOption2;

	public Text m_NowOption_STAT_TEXT_2;

	[Space]
	public GameObject m_NewOption_SET_None_Text;

	public GameObject m_NewOption_BEFORE;

	public GameObject m_NewOption_AFTER;

	public Text m_NewOption_BEFORE_SET_None_Text;

	public Image m_NewOption_SET_ICON;

	public Text m_NewOption_SET_NAME;

	public GameObject m_NewOptionOption1;

	public Text m_NewOption_STAT_TEXT_1;

	public GameObject m_NewOptionOption2;

	public Text m_NewOption_STAT_TEXT_2;

	[Header("이펙트")]
	public Animator m_AB_FX_UI_FACTORY_EQUIP_SLOT;

	public Animator m_AB_FX_UI_FACTORY_SMALL_SLOT_OPTION_BEFORE;

	public Animator m_AB_FX_UI_FACTORY_SMALL_SLOT_OPTION_AFTER;

	public Animator m_AB_FX_UI_FACTORY_SMALL_SLOT_AFTER;

	[Header("확정 변경")]
	public GameObject m_objBinaryCount;

	public GameObject m_objBinaryCountON;

	public GameObject m_objBinaryCountOFF;

	public Image m_imgConfirmOptionIcon;

	public NKCComTMPUIText m_lbConfirmOptionCount;

	public Slider m_sdBinaryGauge;

	public Image m_imgConfirmGauge;

	public NKCComTMPUIText m_lbBinaryText;

	public NKCComTMPUIText m_lbBinaryBtnTextON;

	public NKCComTMPUIText m_lbBinaryBtnTextOFF;

	private OnSelectSlot dOnSelectSlot;

	private int m_PrecisionReqCredit = 150;

	private int m_PrecisionReqMaterial = 1;

	private int m_RandomStatReqCredit = 150;

	private int m_RandomStatReqMaterial = 1;

	private bool m_bMaxTuningBonusCount;

	private bool m_bActiveTuningBonus;

	private SET_OPTION_UI_STATE m_curSetOptionState;

	private NKCPopupEquipOptionList m_NKCPopupEquipOption;

	private NKCPopupEquipSetOptionList m_NKCPopupEquipSetOption;

	private const string ACTIVE_OPTION = "ACTIVE_OPTION";

	private const string ACTIVE_SET_OPTION = "ACTIVE_SET_OPTION";

	private GameObject UnActiveReservedGameObject;

	private NKCPopupEquipOptionList NKCPopupEquipOption
	{
		get
		{
			if (m_NKCPopupEquipOption == null)
			{
				NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupEquipOptionList>("AB_UI_NKM_UI_FACTORY", "NKM_UI_FACTORY_EQUIP_OPTION_POPUP", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), null);
				m_NKCPopupEquipOption = loadedUIData.GetInstance<NKCPopupEquipOptionList>();
				if (m_NKCPopupEquipOption != null)
				{
					m_NKCPopupEquipOption.InitUI();
				}
			}
			return m_NKCPopupEquipOption;
		}
	}

	private NKCPopupEquipSetOptionList NKCPopupEquipSetOption
	{
		get
		{
			if (m_NKCPopupEquipSetOption == null)
			{
				NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupEquipSetOptionList>("AB_UI_NKM_UI_FACTORY", "NKM_UI_FACTORY_EQUIP_SET_LIST_POPUP", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), null);
				m_NKCPopupEquipSetOption = loadedUIData.GetInstance<NKCPopupEquipSetOptionList>();
				if (m_NKCPopupEquipSetOption != null)
				{
					m_NKCPopupEquipSetOption.InitUI();
				}
			}
			return m_NKCPopupEquipSetOption;
		}
	}

	public void InitUI(OnSelectSlot selectSlotIdx = null)
	{
		NKCUtil.SetBindFunction(m_btnNKM_UI_FACTORY_TUNING_BUTTON_PRECISION, OnClickRefine);
		NKCUtil.SetHotkey(m_btnNKM_UI_FACTORY_TUNING_BUTTON_PRECISION, HotkeyEventType.Confirm);
		NKCUtil.SetBindFunction(m_btnNKM_UI_FACTORY_TUNING_BUTTON_CHANGE, OnClickOptionChange);
		NKCUtil.SetBindFunction(m_btnNKM_UI_FACTORY_TUNING_BUTTON_OK, OnClickOptionConfirm);
		NKCUtil.SetToggleValueChangedDelegate(m_RefineToggleBtn, OnClickPrecision);
		NKCUtil.SetToggleValueChangedDelegate(m_ChangeToggleBtn, OnClickOptionChange);
		NKCUtil.SetToggleValueChangedDelegate(m_SetOptionChangeToggleBtn, OnClickSetOptionChange);
		NKCUtil.SetToggleValueChangedDelegate(m_firstOptionBtn, OnClickFirstOption);
		NKCUtil.SetToggleValueChangedDelegate(m_secondOptionBtn, OnClickSecondOption);
		if (m_csbtn_TUNING_RATE_BUTTON != null)
		{
			m_csbtn_TUNING_RATE_BUTTON.PointerDown.RemoveAllListeners();
			m_csbtn_TUNING_RATE_BUTTON.PointerDown.AddListener(OnButtonDownOptionRateInfo);
		}
		NKCUtil.SetBindFunction(m_NKM_UI_FACTORY_TUNING_OPTION_CHANGE_LIST_BUTTON, OpenOptionList);
		NKCUtil.SetBindFunction(m_NKM_UI_FACTORY_TUNING_SET_OPTION_CHANGE_BUTTON, OpenSetOptionList);
		NKCUtil.SetBindFunction(m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_FREE, delegate
		{
			NKCPacketSender.Send_NKMPacket_EQUIP_ITEM_FIRST_SET_OPTION_REQ(m_LeftEquipUID);
		});
		m_MaterialSlot1?.SetData(0, 0, 0L);
		m_MaterialSlot2?.SetData(0, 0, 0L);
		SetActiveEffect(bActive: false);
		dOnSelectSlot = selectSlotIdx;
	}

	public void SetActiveEffect(bool bActive)
	{
		NKCUtil.SetGameobjectActive(m_AB_FX_UI_FACTORY_EQUIP_SLOT.gameObject, bActive);
		NKCUtil.SetGameobjectActive(m_AB_FX_UI_FACTORY_SMALL_SLOT_OPTION_BEFORE.gameObject, bActive);
		NKCUtil.SetGameobjectActive(m_AB_FX_UI_FACTORY_SMALL_SLOT_OPTION_AFTER.gameObject, bActive);
		NKCUtil.SetGameobjectActive(m_AB_FX_UI_FACTORY_SMALL_SLOT_AFTER.gameObject, bActive);
	}

	public void SetLeftEquipUID(long uid)
	{
		m_LeftEquipUID = uid;
		EnableUI(m_LeftEquipUID != 0);
		SetTuningRequirementResourceCount();
	}

	private void SetTuningRequirementResourceCount()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMEquipItemData itemEquip = myUserData.m_InventoryData.GetItemEquip(m_LeftEquipUID);
		if (itemEquip == null)
		{
			return;
		}
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
		if (equipTemplet != null)
		{
			m_PrecisionReqCredit = equipTemplet.m_PrecisionReqResource;
			m_PrecisionReqMaterial = equipTemplet.m_PrecisionReqItem;
			m_RandomStatReqCredit = equipTemplet.m_RandomStatReqResource;
			m_RandomStatReqMaterial = equipTemplet.m_RandomStatReqItem;
			if (NKCCompanyBuff.NeedShowEventMark(myUserData.m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_ENCHANT_TUNING_CREDIT_DISCOUNT))
			{
				NKCCompanyBuff.SetDiscountOfCreditInEnchantTuning(myUserData.m_companyBuffDataList, ref m_PrecisionReqCredit);
				NKCCompanyBuff.SetDiscountOfCreditInEnchantTuning(myUserData.m_companyBuffDataList, ref m_RandomStatReqCredit);
			}
		}
	}

	public void SetOut()
	{
		m_rmTuningRoot.Set("Out");
		SetActiveEffect(bActive: false);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	public void AnimateOutToIn()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		m_rmTuningRoot.Set("Out");
		m_rmTuningRoot.Transit("In");
	}

	public void ResetUI(bool bForce = true, bool bMoveToTabBeingTuned = false)
	{
		if (bMoveToTabBeingTuned)
		{
			NKMEquipTuningCandidate tuiningData = NKCScenManager.CurrentUserData().GetTuiningData();
			if (tuiningData != null)
			{
				if (tuiningData.option1 != NKM_STAT_TYPE.NST_RANDOM || tuiningData.option2 != NKM_STAT_TYPE.NST_RANDOM)
				{
					m_ChangeToggleBtn.Select(bSelect: true);
					m_NKC_TUNING_TAB = NKC_TUNING_TAB.NTT_OPTION_CHANGE;
					m_iSelectOptionIdx = ((tuiningData.option1 != NKM_STAT_TYPE.NST_RANDOM) ? 1 : 2);
				}
				else if (tuiningData.setOptionId != 0)
				{
					m_NKC_TUNING_TAB = NKC_TUNING_TAB.NTT_SET_OPTION_CHANGE;
					m_SetOptionChangeToggleBtn.Select(bSelect: true);
				}
			}
		}
		SetActiveEffect(bActive: false);
		SetTab(m_NKC_TUNING_TAB, bForce);
		m_firstOptionBtn.Select(m_iSelectOptionIdx == 1);
		m_secondOptionBtn.Select(m_iSelectOptionIdx == 2);
		dOnSelectSlot(m_iSelectOptionIdx);
	}

	public int GetSelectOption()
	{
		return m_iSelectOptionIdx;
	}

	public void OnClickPrecision(bool bSet)
	{
		if (bSet)
		{
			SetTab(NKC_TUNING_TAB.NTT_PRECISION, bForce: true);
		}
	}

	public void OnClickOptionChange(bool bSet)
	{
		if (!bSet)
		{
			return;
		}
		if (NKCScenManager.GetScenManager().GetMyUserData().hasReservedSetOptionData())
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_SET_OPTION_TUNING_EXIT_CONFIRM, delegate
			{
				NKCPacketSender.Send_NKMPacket_Equip_Tuning_Cancel_REQ();
				SetTab(NKC_TUNING_TAB.NTT_OPTION_CHANGE, bForce: true);
			}, delegate
			{
				SetTab(m_NKC_TUNING_TAB, bForce: true);
			});
		}
		else
		{
			SetTab(NKC_TUNING_TAB.NTT_OPTION_CHANGE, bForce: true);
		}
	}

	public void OnClickSetOptionChange(bool bSet)
	{
		if (!bSet)
		{
			return;
		}
		if (NKCScenManager.GetScenManager().GetMyUserData().hasReservedEquipTuningData())
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_FORGE_TUNING_EXIT_CONFIRM, delegate
			{
				NKCPacketSender.Send_NKMPacket_Equip_Tuning_Cancel_REQ();
				SetTab(NKC_TUNING_TAB.NTT_SET_OPTION_CHANGE, bForce: true);
			}, delegate
			{
				SetTab(m_NKC_TUNING_TAB, bForce: true);
			});
		}
		else
		{
			SetTab(NKC_TUNING_TAB.NTT_SET_OPTION_CHANGE, bForce: true);
		}
	}

	public void OnClickFirstOption(bool bSet)
	{
		if (bSet)
		{
			SelectTuningOption(1);
		}
	}

	public void OnClickSecondOption(bool bSet)
	{
		if (bSet)
		{
			SelectTuningOption(2);
		}
	}

	public void OnButtonDownOptionRateInfo(PointerEventData pointEventData)
	{
		if (m_LeftEquipUID != 0L)
		{
			NKCUITrimToolTip.Instance.Open(NKCStringTable.GetString("SI_DP_POPUP_EQUIP_PRECISION_CHANCE_INFO"), pointEventData.position, 32);
		}
	}

	private void SelectTuningOption(int idx)
	{
		if (m_LeftEquipUID == 0L)
		{
			m_NKM_UI_FACTORY_TUNING_OPTION_SLOT_toggle.SetAllToggleUnselected();
		}
		else if (m_iSelectOptionIdx != idx)
		{
			m_iSelectOptionIdx = idx;
			dOnSelectSlot(m_iSelectOptionIdx);
			SetTab(m_NKC_TUNING_TAB);
		}
	}

	public void OnClickRefine()
	{
		if (!CanTuning(m_PrecisionReqCredit, m_PrecisionReqMaterial, out var needItemID, out var needCount))
		{
			NKCShopManager.OpenItemLackPopup(needItemID, needCount);
			return;
		}
		NKMPacket_EQUIP_TUNING_REFINE_REQ nKMPacket_EQUIP_TUNING_REFINE_REQ = new NKMPacket_EQUIP_TUNING_REFINE_REQ();
		nKMPacket_EQUIP_TUNING_REFINE_REQ.equipOptionID = m_iSelectOptionIdx;
		nKMPacket_EQUIP_TUNING_REFINE_REQ.equipUID = m_LeftEquipUID;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EQUIP_TUNING_REFINE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public void OnClickOptionChange()
	{
		if (!CanTuning(m_RandomStatReqCredit, m_RandomStatReqMaterial, out var needItemID, out var needCount))
		{
			NKCShopManager.OpenItemLackPopup(needItemID, needCount);
			return;
		}
		NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(m_LeftEquipUID);
		if (itemEquip != null && NKCScenManager.CurrentUserData().hasReservedEquipCandidate())
		{
			NKM_ERROR_CODE nKM_ERROR_CODE = NKMItemManager.CanEnchantItem(NKCScenManager.CurrentUserData(), itemEquip);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				GoToHomeAfterImpossibleTuningPopupMsgBox(nKM_ERROR_CODE);
				return;
			}
		}
		if (m_bMaxTuningBonusCount)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("SI_DP_FACTORY_TUNING_OPTION_BONUS_FULL_POPUP"));
			return;
		}
		NKMPacket_EQUIP_TUNING_STAT_CHANGE_REQ nKMPacket_EQUIP_TUNING_STAT_CHANGE_REQ = new NKMPacket_EQUIP_TUNING_STAT_CHANGE_REQ();
		nKMPacket_EQUIP_TUNING_STAT_CHANGE_REQ.equipOptionID = m_iSelectOptionIdx;
		nKMPacket_EQUIP_TUNING_STAT_CHANGE_REQ.equipUID = m_LeftEquipUID;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EQUIP_TUNING_STAT_CHANGE_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	private void GoToHomeAfterImpossibleTuningPopupMsgBox(NKM_ERROR_CODE error)
	{
		string text = "";
		switch (error)
		{
		case NKM_ERROR_CODE.NEC_FAIL_WARFARE_DOING:
			text = NKCUtilString.GET_STRING_IMPOSSIBLE_TUNING_BY_WARFARE;
			break;
		case NKM_ERROR_CODE.NEC_FAIL_DIVE_DOING:
			text = NKCUtilString.GET_STRING_IMPOSSIBLE_TUNING_BY_DIVE;
			break;
		default:
			return;
		}
		NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, text, delegate
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
		});
	}

	private bool CanTuning(int creditCost, int itemCost, out int needItemID, out int needCount)
	{
		needItemID = 0;
		needCount = 0;
		long num = 0L;
		NKMItemMiscData itemMisc = NKCScenManager.CurrentUserData().m_InventoryData.GetItemMisc(1013);
		if (itemMisc != null)
		{
			num = itemMisc.TotalCount;
		}
		if (num < itemCost)
		{
			needItemID = 1013;
			needCount = itemCost;
			return false;
		}
		if (NKCScenManager.CurrentUserData().GetCredit() < creditCost)
		{
			needItemID = 1;
			needCount = creditCost;
			return false;
		}
		return true;
	}

	private void SendOptionConfirmPacket()
	{
		NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(m_LeftEquipUID);
		if (itemEquip != null && NKCScenManager.CurrentUserData().hasReservedEquipCandidate())
		{
			NKM_ERROR_CODE nKM_ERROR_CODE = NKMItemManager.CanEnchantItem(NKCScenManager.CurrentUserData(), itemEquip);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				GoToHomeAfterImpossibleTuningPopupMsgBox(nKM_ERROR_CODE);
				return;
			}
		}
		NKMPacket_EQUIP_TUNING_STAT_CHANGE_CONFIRM_REQ nKMPacket_EQUIP_TUNING_STAT_CHANGE_CONFIRM_REQ = new NKMPacket_EQUIP_TUNING_STAT_CHANGE_CONFIRM_REQ();
		nKMPacket_EQUIP_TUNING_STAT_CHANGE_CONFIRM_REQ.equipOptionID = m_iSelectOptionIdx;
		nKMPacket_EQUIP_TUNING_STAT_CHANGE_CONFIRM_REQ.equipUID = m_LeftEquipUID;
		NKCScenManager.GetScenManager().GetConnectGame().Send(nKMPacket_EQUIP_TUNING_STAT_CHANGE_CONFIRM_REQ, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_NORMAL);
	}

	public void OnClickOptionConfirm()
	{
		if (m_bActiveTuningBonus)
		{
			NKCUISelectionEquipDetail.Instance.Open(m_LeftEquipUID, m_iSelectOptionIdx, SetOption: false, OnConfirmOptionTuningBouns, OnConfirmSetOptionTuningBouns);
		}
		else if (IsOptionChangePossible())
		{
			string format = (m_bMaxTuningBonusCount ? NKCUtilString.GET_STRING_FORGE_TUNING_CONFIRM_DESC_TWO_PARAM_FULL : NKCUtilString.GET_STRING_FORGE_TUNING_CONFIRM_DESC_TWO_PARAM);
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_FORGE_TUNING_CONFIRM_TITLE, string.Format(format, GetTuningOptionText(m_iSelectOptionIdx - 1, Before: true), GetTuningOptionText(m_iSelectOptionIdx - 1)), SendOptionConfirmPacket);
		}
	}

	private bool IsOptionChangePossible()
	{
		if (NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(m_LeftEquipUID) == null)
		{
			return false;
		}
		return NKCScenManager.GetScenManager().GetMyUserData().IsPossibleTuning(m_LeftEquipUID, m_iSelectOptionIdx - 1);
	}

	private string GetTuningOptionText(int idx, bool Before = false)
	{
		if (m_NKM_UI_FACTORY_TUNING_OPTION_SLOT.Length > idx)
		{
			return m_NKM_UI_FACTORY_TUNING_OPTION_SLOT[idx].GetStatText(Before);
		}
		return "";
	}

	public NKC_TUNING_TAB GetCurTuningTab()
	{
		return m_NKC_TUNING_TAB;
	}

	public void SetTab(NKC_TUNING_TAB eNKC_TUNING_TAB, bool bForce = false)
	{
		if (bForce)
		{
			m_RefineToggleBtn.Select(eNKC_TUNING_TAB == NKC_TUNING_TAB.NTT_PRECISION, bForce: true);
			m_ChangeToggleBtn.Select(eNKC_TUNING_TAB == NKC_TUNING_TAB.NTT_OPTION_CHANGE, bForce: true);
			m_SetOptionChangeToggleBtn.Select(eNKC_TUNING_TAB == NKC_TUNING_TAB.NTT_SET_OPTION_CHANGE, bForce: true);
		}
		NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(m_LeftEquipUID);
		m_NKC_TUNING_TAB = eNKC_TUNING_TAB;
		NKCUIManager.UpdateUpsideMenu();
		UpdateSubUI();
		if (itemEquip != null && m_NKC_TUNING_TAB != NKC_TUNING_TAB.NTT_SET_OPTION_CHANGE)
		{
			for (int i = 0; i < m_NKM_UI_FACTORY_TUNING_OPTION_SLOT.Length; i++)
			{
				m_NKM_UI_FACTORY_TUNING_OPTION_SLOT[i]?.SetData(m_NKC_TUNING_TAB, i, itemEquip);
			}
			UpdateOptionSlotStat(bForce);
		}
		m_MaterialSlot1.SetData(0, 0, 0L);
		m_MaterialSlot2.SetData(0, 0, 0L);
		UpdateRequireItemUI();
	}

	private void UpdateSubUI()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_TUNING_OPTION_SLOT_toggle.gameObject, m_NKC_TUNING_TAB != NKC_TUNING_TAB.NTT_SET_OPTION_CHANGE);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_TUNING_PRECISION_panel, m_NKC_TUNING_TAB == NKC_TUNING_TAB.NTT_PRECISION);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_TUNING_OPTION_CHANGE_panel, m_NKC_TUNING_TAB == NKC_TUNING_TAB.NTT_OPTION_CHANGE);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_TUNING_SET_CHANGE_panel, m_NKC_TUNING_TAB == NKC_TUNING_TAB.NTT_SET_OPTION_CHANGE);
		NKCUtil.SetGameobjectActive(m_csbtn_TUNING_RATE_BUTTON, m_NKC_TUNING_TAB == NKC_TUNING_TAB.NTT_PRECISION && NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.OPEN_TAG_RATE_INFO));
		NKCUtil.SetGameobjectActive(m_objBinaryCount, m_NKC_TUNING_TAB != NKC_TUNING_TAB.NTT_PRECISION && m_LeftEquipUID != 0);
		if (m_LeftEquipUID == 0L)
		{
			ClearAllUI();
		}
		else if (m_NKC_TUNING_TAB == NKC_TUNING_TAB.NTT_OPTION_CHANGE)
		{
			NKCUtil.SetBindFunction(m_NKM_UI_FACTORY_TUNING_OPTION_CHANGE_LIST_BUTTON, OpenOptionList);
			NKCUtil.SetImageSprite(m_imgConfirmOptionIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_INVEN_ICON_ITEM_MISC_SMALL", "AB_INVEN_ICON_IMI_ITEM_MISC_TUNING_MATERIAL"));
			NKCUtil.SetImageSprite(m_imgConfirmGauge, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_FACTORY_SPRITE", "AB_UI_NKM_UI_FACTORY_GAUGE_TUNING"));
			NKCUtil.SetLabelText(m_lbBinaryText, NKCUtilString.GET_STRING_TUNING_OPTION_CHANGE_BONUS_TITLE);
			NKCUtil.SetLabelText(m_lbBinaryBtnTextON, NKCUtilString.GET_STRING_TUNING_OPTION_CHANGE_BONUS_ACTIVE);
			NKCUtil.SetLabelText(m_lbBinaryBtnTextOFF, NKCUtilString.GET_STRING_TUNING_OPTION_CHANGE_BONUS_ACTIVE);
		}
		else
		{
			if (m_NKC_TUNING_TAB != NKC_TUNING_TAB.NTT_SET_OPTION_CHANGE)
			{
				return;
			}
			bool flag = false;
			NKMEquipItemData itemEquip = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(m_LeftEquipUID);
			if (itemEquip != null)
			{
				NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
				if (equipTemplet != null && equipTemplet.SetGroupList != null && equipTemplet.SetGroupList.Count > 1)
				{
					flag = true;
				}
			}
			if (flag)
			{
				NKCUtil.SetBindFunction(m_NKM_UI_FACTORY_TUNING_SET_OPTION_CHANGE_BUTTON, OpenSetOptionList);
				NKCUtil.SetBindFunction(m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_OK, OnSetOptionConfirm);
				NKCUtil.SetBindFunction(m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_CHANGE, OnSetOptionChange);
				NKCUtil.SetImageSprite(m_img_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_CHANGE, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_BLUE));
				NKCUtil.SetLabelTextColor(m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_CHANGE_TEXT, Color.white);
			}
			else
			{
				NKCUtil.SetBindFunction(m_NKM_UI_FACTORY_TUNING_SET_OPTION_CHANGE_BUTTON);
				NKCUtil.SetBindFunction(m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_OK);
				NKCUtil.SetBindFunction(m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_CHANGE);
				NKCUtil.SetImageSprite(m_img_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_CHANGE, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY));
				NKCUtil.SetLabelTextColor(m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_CHANGE_TEXT, NKCUtil.GetUITextColor(bActive: false));
			}
			NKCUtil.SetImageSprite(m_imgConfirmOptionIcon, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_INVEN_ICON_ITEM_MISC_SMALL", "AB_INVEN_ICON_IMI_ITEM_MISC_SET_MATERIAL"));
			NKCUtil.SetImageSprite(m_imgConfirmGauge, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_FACTORY_SPRITE", "AB_UI_NKM_UI_FACTORY_GAUGE_SET"));
			NKCUtil.SetLabelText(m_lbBinaryText, NKCUtilString.GET_STRING_TUNING_SET_OPTION_CHANGE_BONUS_TITLE);
			NKCUtil.SetLabelText(m_lbBinaryBtnTextON, NKCUtilString.GET_STRING_TUNING_SET_OPTION_CHANGE_BONUS_ACTIVE);
			NKCUtil.SetLabelText(m_lbBinaryBtnTextOFF, NKCUtilString.GET_STRING_TUNING_SET_OPTION_CHANGE_BONUS_ACTIVE);
		}
	}

	private void OnSetOptionChange()
	{
		bool flag = true;
		int num = 0;
		int num2 = 0;
		NKMInventoryData inventoryData = NKCScenManager.CurrentUserData().m_InventoryData;
		NKMEquipItemData itemEquip = inventoryData.GetItemEquip(m_LeftEquipUID);
		if (itemEquip != null)
		{
			if (NKCScenManager.CurrentUserData().hasReservedEquipCandidate())
			{
				NKM_ERROR_CODE nKM_ERROR_CODE = NKMItemManager.CanEnchantItem(NKCScenManager.CurrentUserData(), itemEquip);
				if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
				{
					GoToHomeAfterImpossibleTuningPopupMsgBox(nKM_ERROR_CODE);
					return;
				}
			}
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
			if (equipTemplet != null)
			{
				if (equipTemplet.m_RandomSetReqItemValue > inventoryData.GetCountMiscItem(equipTemplet.m_RandomSetReqItemID))
				{
					num = equipTemplet.m_RandomSetReqItemID;
					num2 = equipTemplet.m_RandomSetReqItemValue;
					flag = false;
				}
				else if (equipTemplet.m_RandomSetReqResource > inventoryData.GetCountMiscItem(1))
				{
					int credit = equipTemplet.m_RandomSetReqResource;
					if (NKCCompanyBuff.NeedShowEventMark(NKCScenManager.CurrentUserData().m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_ENCHANT_TUNING_CREDIT_DISCOUNT))
					{
						NKCCompanyBuff.SetDiscountOfCreditInEnchantTuning(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref credit);
					}
					if (credit > inventoryData.GetCountMiscItem(1))
					{
						num = 1;
						num2 = credit;
						flag = false;
					}
				}
			}
		}
		if (m_bMaxTuningBonusCount)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCStringTable.GetString("SI_DP_FACTORY_TUNING_SET_BONUS_FULL_POPUP"));
		}
		else if (!flag && num != 0 && num2 != 0)
		{
			NKCShopManager.OpenItemLackPopup(num, num2);
		}
		else
		{
			NKCPacketSender.Send_NKMPacket_EQUIP_ITEM_CHANGE_SET_OPTION_REQ(m_LeftEquipUID);
		}
	}

	private void OnSetOptionConfirm()
	{
		NKMEquipItemData itemEquip = NKCScenManager.CurrentUserData().m_InventoryData.GetItemEquip(m_LeftEquipUID);
		if (itemEquip == null)
		{
			return;
		}
		if (NKCScenManager.CurrentUserData().hasReservedEquipCandidate())
		{
			NKM_ERROR_CODE nKM_ERROR_CODE = NKMItemManager.CanEnchantItem(NKCScenManager.CurrentUserData(), itemEquip);
			if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
			{
				GoToHomeAfterImpossibleTuningPopupMsgBox(nKM_ERROR_CODE);
				return;
			}
		}
		NKCPacketSender.Send_NKMPacket_EQUIP_ITEM_CONFIRM_SET_OPTION_REQ(m_LeftEquipUID);
	}

	private void OnBounsSetOptionConfirm()
	{
		if (!m_bActiveTuningBonus)
		{
			return;
		}
		NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(m_LeftEquipUID);
		if (itemEquip != null)
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
			if (equipTemplet != null && equipTemplet.SetGroupList != null && equipTemplet.SetGroupList.Count <= 1)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCPacketHandlers.GetErrorMessage(NKM_ERROR_CODE.NEC_FAIL_INVALID_EQUIP_OPTION_ID));
				return;
			}
		}
		NKCUISelectionEquipDetail.Instance.Open(m_LeftEquipUID, -1, SetOption: true, OnConfirmOptionTuningBouns, OnConfirmSetOptionTuningBouns);
	}

	private void UpdateUIPrecision()
	{
		if (m_LeftEquipUID == 0L)
		{
			return;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		NKMEquipItemData itemEquip = myUserData.m_InventoryData.GetItemEquip(m_LeftEquipUID);
		if (itemEquip == null)
		{
			return;
		}
		bool flag = true;
		int num = ((m_iSelectOptionIdx == 1) ? itemEquip.m_Precision : itemEquip.m_Precision2);
		if (num >= 100)
		{
			m_NKM_UI_FACTORY_TUNING_BUTTON_PRECISION.material = NKCResourceUtility.GetOrLoadAssetResource<Material>("AB_UI_NKM_UI_OPERATION_EP_THUMBNAIL", "EP_THUMBNAIL_BLACK_AND_WHITE");
		}
		else
		{
			if (myUserData.m_InventoryData.GetCountMiscItem(1013) < m_PrecisionReqMaterial || myUserData.m_InventoryData.GetCountMiscItem(1) < m_PrecisionReqCredit)
			{
				flag = false;
			}
			SetSlotMaterialData(ref m_MaterialSlot1, 1013, m_PrecisionReqMaterial);
			SetSlotMaterialData(ref m_MaterialSlot2, 1, m_PrecisionReqCredit);
			m_NKM_UI_FACTORY_TUNING_BUTTON_PRECISION.material = null;
		}
		if (num >= 100 || !flag)
		{
			EnableUI(bActive: false);
		}
		else
		{
			EnableUI(bActive: true);
		}
	}

	public void UpdateRequireItemUI()
	{
		switch (m_NKC_TUNING_TAB)
		{
		case NKC_TUNING_TAB.NTT_PRECISION:
			UpdateUIPrecision();
			break;
		case NKC_TUNING_TAB.NTT_OPTION_CHANGE:
			UpdateUIOptionChange();
			break;
		case NKC_TUNING_TAB.NTT_SET_OPTION_CHANGE:
			UpdateUISetOptionChange();
			break;
		default:
			Debug.LogError("설정되지 않은 옵션(" + m_NKC_TUNING_TAB.ToString() + ")입니다.");
			break;
		}
	}

	private void UpdateUIOptionChange()
	{
		if (m_LeftEquipUID == 0L)
		{
			return;
		}
		NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(m_LeftEquipUID);
		if (itemEquip == null)
		{
			return;
		}
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
		if (equipTemplet == null)
		{
			return;
		}
		int optionState = GetOptionState();
		int statGroupID = ((optionState == 1 || optionState == 3) ? equipTemplet.m_StatGroupID : equipTemplet.m_StatGroupID_2);
		if (NKMEquipTuningManager.GetEquipRandomStatGroupList(statGroupID) == null || NKMEquipTuningManager.GetEquipRandomStatGroupList(statGroupID).Count == 1)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_TUNING_OPTION_CHANGE_WARNING, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_TUNING_OPTION_CHANGE_WARNING, bValue: true);
			SetSlotMaterialData(ref m_MaterialSlot1, 1013, m_RandomStatReqMaterial);
			SetSlotMaterialData(ref m_MaterialSlot2, 1, m_RandomStatReqCredit);
			if (NKCScenManager.GetScenManager().GetMyUserData().IsPossibleTuning(m_LeftEquipUID, m_iSelectOptionIdx - 1))
			{
				m_NKM_UI_FACTORY_TUNING_BUTTON_OK.material = null;
			}
		}
		if (m_LeftEquipUID != 0L)
		{
			float value = (float)NKMItemManager.GetRemainResetCount(NKMCommonConst.TuningBonusResetGroupID) / (float)NKMCommonConst.TuningBonusCount;
			m_sdBinaryGauge.value = value;
			float num = Mathf.Max(NKMItemManager.GetRemainResetCount(NKMCommonConst.TuningBonusResetGroupID), 0);
			NKCUtil.SetLabelText(m_lbConfirmOptionCount, $"{num}/{NKMCommonConst.TuningBonusCount}");
			m_bMaxTuningBonusCount = NKMItemManager.GetRemainResetCount(NKMCommonConst.TuningBonusResetGroupID) >= NKMCommonConst.TuningBonusCount;
			m_bActiveTuningBonus = m_bMaxTuningBonusCount && !NKCScenManager.GetScenManager().GetMyUserData().hasReservedEquipTuningData();
			NKCUtil.SetGameobjectActive(m_objBinaryCountON, m_bActiveTuningBonus);
			NKCUtil.SetGameobjectActive(m_objBinaryCountOFF, !m_bActiveTuningBonus);
		}
		EnableUI(bActive: true);
	}

	private void UpdateUISetOptionChange()
	{
		NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(m_LeftEquipUID);
		NKMEquipTemplet nKMEquipTemplet = null;
		m_curSetOptionState = SET_OPTION_UI_STATE.NOT_SELECTED;
		NKCUtil.SetGameobjectActive(m_objBinaryCountON, bValue: false);
		NKCUtil.SetGameobjectActive(m_objBinaryCountOFF, bValue: false);
		if (m_LeftEquipUID != 0L && itemEquip != null)
		{
			nKMEquipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
			NKMItemEquipSetOptionTemplet nowSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(itemEquip.m_SetOptionId);
			if (nKMEquipTemplet != null)
			{
				if (nowSetOptionTemplet != null)
				{
					if (nKMEquipTemplet.SetGroupList != null)
					{
						if (nKMEquipTemplet.SetGroupList.Count > 1)
						{
							m_curSetOptionState = SET_OPTION_UI_STATE.CAN_POSSIBLE_CHANGE;
						}
						else if (nKMEquipTemplet.SetGroupList.Count == 1)
						{
							m_curSetOptionState = SET_OPTION_UI_STATE.FIXED_SET_OPTION;
						}
						else
						{
							m_curSetOptionState = SET_OPTION_UI_STATE.DONT_HAVE_SET_OPTION_DATA;
						}
					}
					else
					{
						m_curSetOptionState = SET_OPTION_UI_STATE.FIXED_SET_OPTION;
					}
				}
				else if (nKMEquipTemplet.SetGroupList != null && nKMEquipTemplet.SetGroupList.Count > 0)
				{
					m_curSetOptionState = SET_OPTION_UI_STATE.FIRST_FREE_CHANGE;
				}
				else
				{
					m_curSetOptionState = SET_OPTION_UI_STATE.DONT_HAVE_SET_OPTION_DATA;
				}
				float value = (float)NKMItemManager.GetRemainResetCount(NKMCommonConst.SetBonusResetGroupID) / (float)NKMCommonConst.SetBonusCount;
				m_sdBinaryGauge.value = value;
				float num = Mathf.Max(NKMItemManager.GetRemainResetCount(NKMCommonConst.SetBonusResetGroupID), 0);
				NKCUtil.SetLabelText(m_lbConfirmOptionCount, $"{num}/{NKMCommonConst.SetBonusCount}");
				m_bMaxTuningBonusCount = NKMItemManager.GetRemainResetCount(NKMCommonConst.SetBonusResetGroupID) >= NKMCommonConst.SetBonusCount;
				m_bActiveTuningBonus = m_bMaxTuningBonusCount && !NKCScenManager.GetScenManager().GetMyUserData().hasReservedSetOptionData();
				NKCUtil.SetGameobjectActive(m_objBinaryCountON, m_bActiveTuningBonus);
				NKCUtil.SetGameobjectActive(m_objBinaryCountOFF, !m_bActiveTuningBonus);
			}
			if (nowSetOptionTemplet != null)
			{
				NKCUtil.SetImageSprite(m_NowOption_SET_ICON, NKCUtil.GetSpriteEquipSetOptionIcon(nowSetOptionTemplet));
				int num2 = 0;
				if (itemEquip.m_OwnerUnitUID > 0)
				{
					num2 = NKMItemManager.GetMatchingSetOptionItem(itemEquip);
				}
				NKCUtil.SetLabelText(m_NowOptionSET_NAME, $"{NKCStringTable.GetString(nowSetOptionTemplet.m_EquipSetName)} ({num2}/{nowSetOptionTemplet.m_EquipSetPart})");
				NKCUtil.SetGameobjectActive(m_NowOption_SET_None_Text, bValue: false);
				NKCUtil.SetGameobjectActive(m_NowOptionOption1, nowSetOptionTemplet.m_StatType_1 != NKM_STAT_TYPE.NST_RANDOM);
				NKCUtil.SetGameobjectActive(m_NowOptionOption2, nowSetOptionTemplet.m_StatType_2 != NKM_STAT_TYPE.NST_RANDOM);
				string setOptionDescription = NKMItemManager.GetSetOptionDescription(nowSetOptionTemplet.m_StatType_1, nowSetOptionTemplet.m_StatValue_1);
				NKCUtil.SetLabelText(m_NowOption_STAT_TEXT_1, setOptionDescription);
				if (nowSetOptionTemplet.m_StatType_2 != NKM_STAT_TYPE.NST_RANDOM)
				{
					string setOptionDescription2 = NKMItemManager.GetSetOptionDescription(nowSetOptionTemplet.m_StatType_2, nowSetOptionTemplet.m_StatValue_2);
					NKCUtil.SetLabelText(m_NowOption_STAT_TEXT_2, setOptionDescription2);
				}
			}
			NKCUtil.SetGameobjectActive(m_NowOption_SET_None_Text, nowSetOptionTemplet == null);
			NKCUtil.SetGameobjectActive(m_NowOption_SET_ICON.gameObject, nowSetOptionTemplet != null);
			NKCUtil.SetGameobjectActive(m_NowOptionSET_NAME.gameObject, nowSetOptionTemplet != null);
			int reservedSetOption = NKCScenManager.CurrentUserData().GetReservedSetOption(m_LeftEquipUID);
			NKMItemEquipSetOptionTemplet newSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(reservedSetOption);
			if (newSetOptionTemplet != null)
			{
				NKCUtil.SetImageSprite(m_NewOption_SET_ICON, NKCUtil.GetSpriteEquipSetOptionIcon(newSetOptionTemplet));
				int num3 = 0;
				if (itemEquip.m_OwnerUnitUID > 0)
				{
					num3 = NKMItemManager.GetExpactSetOptionMatchingCnt(itemEquip, reservedSetOption);
				}
				NKCUtil.SetLabelText(m_NewOption_SET_NAME, $"{NKCStringTable.GetString(newSetOptionTemplet.m_EquipSetName)} ({num3}/{newSetOptionTemplet.m_EquipSetPart})");
				NKCUtil.SetGameobjectActive(m_NewOptionOption1, newSetOptionTemplet.m_StatType_1 != NKM_STAT_TYPE.NST_RANDOM);
				NKCUtil.SetGameobjectActive(m_NewOptionOption2, newSetOptionTemplet.m_StatType_2 != NKM_STAT_TYPE.NST_RANDOM);
				string setOptionDescription3 = NKMItemManager.GetSetOptionDescription(newSetOptionTemplet.m_StatType_1, newSetOptionTemplet.m_StatValue_1);
				NKCUtil.SetLabelText(m_NewOption_STAT_TEXT_1, setOptionDescription3);
				if (newSetOptionTemplet.m_StatType_2 != NKM_STAT_TYPE.NST_RANDOM)
				{
					string setOptionDescription4 = NKMItemManager.GetSetOptionDescription(newSetOptionTemplet.m_StatType_2, newSetOptionTemplet.m_StatValue_2);
					NKCUtil.SetLabelText(m_NewOption_STAT_TEXT_2, setOptionDescription4);
				}
				if (nowSetOptionTemplet != null)
				{
					string _ConfirmDesc = (m_bMaxTuningBonusCount ? NKCUtilString.GET_STRING_FORGE_SET_OPTION_CHANGE_POPUP_CONFIRM_DESC_FULL : NKCUtilString.GET_STRING_FORGE_SET_OPTION_CHANGE_POPUP_CONFIRM_DESC);
					NKCUtil.SetBindFunction(m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_OK, delegate
					{
						NKCPopupChangeConfirm.Instance.Open(NKCUtilString.GET_STRING_FORGE_SET_OPTION_CHANGE_POPUP_CONFIRM_TITLE, NKCStringTable.GetString(nowSetOptionTemplet.m_EquipSetName), NKCStringTable.GetString(newSetOptionTemplet.m_EquipSetName), _ConfirmDesc, OnSetOptionConfirm);
					});
				}
			}
		}
		UpdateSetOptionUI(nKMEquipTemplet);
		EnableUI(bActive: true);
	}

	private void UpdateSetOptionUI(NKMEquipTemplet equipTemplet)
	{
		switch (m_curSetOptionState)
		{
		case SET_OPTION_UI_STATE.NOT_SELECTED:
			NKCUtil.SetGameobjectActive(m_NowOption_SET_None_Text, bValue: true);
			NKCUtil.SetLabelText(m_txtNowOption_SET_None_Text, NKCUtilString.GET_STRING_FORGE_TUNING_SET_NO_OPTION);
			NKCUtil.SetGameobjectActive(m_NewOption_BEFORE, bValue: true);
			NKCUtil.SetLabelText(m_NewOption_BEFORE_SET_None_Text, "-");
			NKCUtil.SetGameobjectActive(m_NewOption_AFTER, bValue: false);
			NKCUtil.SetGameobjectActive(m_NowOption_SET_ICON.gameObject, bValue: false);
			NKCUtil.SetGameobjectActive(m_NowOptionSET_NAME.gameObject, bValue: false);
			NKCUtil.SetGameobjectActive(m_NowOptionOption1, bValue: false);
			NKCUtil.SetGameobjectActive(m_NowOptionOption2, bValue: false);
			NKCUtil.SetImageColor(m_img_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_OK_ICON, NKCUtil.GetUITextColor(bActive: false));
			NKCUtil.SetImageSprite(m_img_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_OK, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY));
			NKCUtil.SetLabelTextColor(m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_OK_TEXT, NKCUtil.GetUITextColor(bActive: false));
			NKCUtil.SetBindFunction(m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_OK);
			NKCUtil.SetImageSprite(m_img_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_CHANGE, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY));
			NKCUtil.SetLabelTextColor(m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_CHANGE_TEXT, NKCUtil.GetUITextColor(bActive: false));
			NKCUtil.SetBindFunction(m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_CHANGE);
			break;
		case SET_OPTION_UI_STATE.DONT_HAVE_SET_OPTION_DATA:
			NKCUtil.SetGameobjectActive(m_NowOption_SET_ICON.gameObject, bValue: false);
			NKCUtil.SetGameobjectActive(m_NowOptionSET_NAME.gameObject, bValue: false);
			NKCUtil.SetLabelText(m_txtNowOption_SET_None_Text, NKCUtilString.GET_STRING_FORGE_TUNING_SET_OPTION_CANNOT_CHANGE);
			NKCUtil.SetGameobjectActive(m_NowOptionOption1, bValue: false);
			NKCUtil.SetGameobjectActive(m_NowOptionOption2, bValue: false);
			break;
		case SET_OPTION_UI_STATE.FIRST_FREE_CHANGE:
			NKCUtil.SetGameobjectActive(m_NowOptionOption1, bValue: false);
			NKCUtil.SetGameobjectActive(m_NowOptionOption2, bValue: false);
			NKCUtil.SetLabelText(m_txtNowOption_SET_None_Text, NKCUtilString.GET_STRING_FORGE_TUNING_SET_NO_OPTION);
			if (equipTemplet != null)
			{
				SetSlotMaterialData(ref m_MaterialSlot1, equipTemplet.m_RandomSetReqItemID, 0);
				SetSlotMaterialData(ref m_MaterialSlot2, 1, 0);
			}
			break;
		case SET_OPTION_UI_STATE.CAN_POSSIBLE_CHANGE:
			if (equipTemplet != null)
			{
				SetSlotMaterialData(ref m_MaterialSlot1, equipTemplet.m_RandomSetReqItemID, equipTemplet.m_RandomSetReqItemValue);
				NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
				int credit = equipTemplet.m_RandomSetReqResource;
				if (NKCCompanyBuff.NeedShowEventMark(nKMUserData?.m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_ENCHANT_TUNING_CREDIT_DISCOUNT))
				{
					NKCCompanyBuff.SetDiscountOfCreditInEnchantTuning(nKMUserData?.m_companyBuffDataList, ref credit);
				}
				SetSlotMaterialData(ref m_MaterialSlot2, 1, credit);
			}
			break;
		}
		if (m_curSetOptionState == SET_OPTION_UI_STATE.FIXED_SET_OPTION)
		{
			NKCUtil.SetLabelText(m_NewOption_BEFORE_SET_None_Text, NKCUtilString.GET_STRING_FORGE_TUNING_SET_OPTION_CANNOT_CHANGE);
		}
		else
		{
			NKCUtil.SetLabelText(m_NewOption_BEFORE_SET_None_Text, "-");
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_FREE.gameObject, SET_OPTION_UI_STATE.FIRST_FREE_CHANGE == m_curSetOptionState);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_OK.gameObject, SET_OPTION_UI_STATE.FIRST_FREE_CHANGE != m_curSetOptionState);
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_CHANGE.gameObject, SET_OPTION_UI_STATE.FIRST_FREE_CHANGE != m_curSetOptionState);
		NKMItemEquipSetOptionTemplet equipSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(NKCScenManager.CurrentUserData().GetReservedSetOption(m_LeftEquipUID));
		if (equipSetOptionTemplet == null)
		{
			NKCUtil.SetGameobjectActive(m_NewOptionOption1, bValue: false);
			NKCUtil.SetGameobjectActive(m_NewOptionOption2, bValue: false);
			NKCUtil.SetImageColor(m_img_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_OK_ICON, NKCUtil.GetUITextColor(bActive: false));
			NKCUtil.SetImageSprite(m_img_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_OK, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY));
			NKCUtil.SetLabelTextColor(m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_OK_TEXT, NKCUtil.GetUITextColor(bActive: false));
			NKCUtil.SetBindFunction(m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_OK);
		}
		else
		{
			NKCUtil.SetImageColor(m_img_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_OK_ICON, NKCUtil.GetUITextColor());
			NKCUtil.SetImageSprite(m_img_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_OK, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_YELLOW));
			NKCUtil.SetLabelTextColor(m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_OK_TEXT, NKCUtil.GetUITextColor());
		}
		if (m_bActiveTuningBonus)
		{
			NKCUtil.SetImageSprite(m_img_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_OK, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_YELLOW));
			NKCUtil.SetBindFunction(m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_OK, OnBounsSetOptionConfirm);
			NKCUtil.SetImageSprite(m_img_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_CHANGE, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY));
			NKCUtil.SetLabelTextColor(m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_CHANGE_TEXT, NKCUtil.GetUITextColor(bActive: false));
		}
		NKCUtil.SetGameobjectActive(m_NewOption_BEFORE, equipSetOptionTemplet == null);
		NKCUtil.SetGameobjectActive(m_NewOption_AFTER, equipSetOptionTemplet != null);
		NKCUtil.SetGameobjectActive(m_NewOption_SET_ICON.gameObject, equipSetOptionTemplet != null);
		NKCUtil.SetGameobjectActive(m_NewOption_SET_NAME.gameObject, equipSetOptionTemplet != null);
	}

	private void SetSlotMaterialData(ref NKCUIItemCostSlot materialSlot, int itemID, int ReqCnt)
	{
		if (!(materialSlot == null))
		{
			long countMiscItem = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetCountMiscItem(itemID);
			bool bShowEvent = itemID == 1 && NKCCompanyBuff.NeedShowEventMark(NKCScenManager.CurrentUserData().m_companyBuffDataList, NKMConst.Buff.BuffType.BASE_FACTORY_ENCHANT_TUNING_CREDIT_DISCOUNT);
			materialSlot.SetData(itemID, ReqCnt, countMiscItem, bShowTooltip: true, bShowBG: true, bShowEvent);
		}
	}

	private void UpdateOptionSlotStat(bool bForce)
	{
		int num = ((m_NKC_TUNING_TAB == NKC_TUNING_TAB.NTT_PRECISION) ? GetSlotState() : GetOptionState());
		switch (num)
		{
		case 0:
			m_firstOptionBtn.m_bLock = true;
			m_secondOptionBtn.m_bLock = true;
			m_NKM_UI_FACTORY_TUNING_OPTION_SLOT[0].ClearUI(m_NKC_TUNING_TAB == NKC_TUNING_TAB.NTT_PRECISION);
			m_NKM_UI_FACTORY_TUNING_OPTION_SLOT[1].ClearUI(m_NKC_TUNING_TAB == NKC_TUNING_TAB.NTT_PRECISION);
			break;
		case 1:
			m_firstOptionBtn.m_bLock = false;
			m_secondOptionBtn.m_bLock = true;
			m_NKM_UI_FACTORY_TUNING_OPTION_SLOT[1].ClearUI(m_NKC_TUNING_TAB == NKC_TUNING_TAB.NTT_PRECISION);
			break;
		case 2:
			m_firstOptionBtn.m_bLock = true;
			m_secondOptionBtn.m_bLock = false;
			m_NKM_UI_FACTORY_TUNING_OPTION_SLOT[0].ClearUI(m_NKC_TUNING_TAB == NKC_TUNING_TAB.NTT_PRECISION);
			break;
		case 3:
			m_firstOptionBtn.m_bLock = false;
			m_secondOptionBtn.m_bLock = false;
			break;
		}
		if (bForce)
		{
			int idx = (m_iSelectOptionIdx = ((num == 1 || num == 3) ? 1 : 2));
			dOnSelectSlot(idx);
			m_firstOptionBtn.Select(num == 1 || num == 3, bForce);
			m_secondOptionBtn.Select(num == 2, bForce);
		}
	}

	public void DoAfterRefine(NKMEquipItemData orgData, int changedSlotNum)
	{
		if (orgData == null)
		{
			return;
		}
		NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(orgData.m_ItemUid);
		if (itemEquip == null || itemEquip.m_Precision >= 100 || itemEquip.m_Precision2 >= 100)
		{
			SetEffect(0, changedSlotNum);
		}
		else if (m_NKC_TUNING_TAB == NKC_TUNING_TAB.NTT_PRECISION)
		{
			int num = m_iSelectOptionIdx - 1;
			if (m_NKM_UI_FACTORY_TUNING_OPTION_SLOT.Length > num)
			{
				m_NKM_UI_FACTORY_TUNING_OPTION_SLOT[num].SetPrecisionRate(orgData, num);
			}
		}
	}

	public void Close()
	{
		m_LeftEquipUID = 0L;
		ClearAllUI();
		m_RefineToggleBtn.Select(bSelect: true);
	}

	public void ClearAllUI()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_TUNING_OPTION_CHANGE_WARNING, bValue: false);
		NKCUtil.SetBindFunction(m_NKM_UI_FACTORY_TUNING_OPTION_CHANGE_LIST_BUTTON);
		NKCUtil.SetBindFunction(m_NKM_UI_FACTORY_TUNING_SET_OPTION_CHANGE_BUTTON);
		NKCUtil.SetBindFunction(m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_CHANGE);
		NKCUtil.SetBindFunction(m_NKM_UI_FACTORY_TUNING_SET_OPTION_BUTTON_OK);
		m_MaterialSlot1.SetData(0, 0, 0L);
		m_MaterialSlot2.SetData(0, 0, 0L);
		for (int i = 0; i < m_NKM_UI_FACTORY_TUNING_OPTION_SLOT.Length; i++)
		{
			m_NKM_UI_FACTORY_TUNING_OPTION_SLOT[i].ClearPrecisionRate();
			m_NKM_UI_FACTORY_TUNING_OPTION_SLOT[i]?.ClearUI(bForce: true);
		}
		EnableUI(bActive: false);
	}

	private void EnableUI(bool bActive)
	{
		if (!bActive)
		{
			m_NKM_UI_FACTORY_TUNING_BUTTON_PRECISION.sprite = NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY);
		}
		else
		{
			m_NKM_UI_FACTORY_TUNING_BUTTON_PRECISION.sprite = NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_YELLOW);
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_FACTORY_TUNING_BUTTON_PRECISION_LIGHT, bActive);
		NKCUtil.SetImageColor(m_img_NKM_UI_FACTORY_TUNING_PRECISION_ICON, NKCUtil.GetUITextColor(bActive));
		NKCUtil.SetLabelTextColor(m_txt_NKM_UI_FACTORY_TUNING_PRECISION_TEXT, NKCUtil.GetUITextColor(bActive));
		bool flag = bActive && m_NKC_TUNING_TAB == NKC_TUNING_TAB.NTT_OPTION_CHANGE && (IsOptionChangePossible() || m_bActiveTuningBonus);
		if (flag)
		{
			m_NKM_UI_FACTORY_TUNING_BUTTON_OK.sprite = NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_YELLOW);
		}
		else
		{
			m_NKM_UI_FACTORY_TUNING_BUTTON_OK.sprite = NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY);
		}
		NKCUtil.SetImageColor(m_img_NKM_UI_FACTORY_TUNING_OK_ICON, NKCUtil.GetUITextColor(flag));
		NKCUtil.SetLabelTextColor(m_txt_NKM_UI_FACTORY_TUNING_OK_TEXT, NKCUtil.GetUITextColor(flag));
		if (!bActive || m_bMaxTuningBonusCount)
		{
			NKCUtil.SetImageSprite(m_img_NKM_UI_FACTORY_TUNING_BUTTON_CHANGE, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_GRAY));
			m_txt_NKM_UI_FACTORY_TUNING_BUTTON_CHANGE.color = NKCUtil.GetUITextColor(bActive: false);
		}
		else
		{
			NKCUtil.SetImageSprite(m_img_NKM_UI_FACTORY_TUNING_BUTTON_CHANGE, NKCUtil.GetButtonSprite(NKCUtil.ButtonColor.BC_BLUE));
			m_txt_NKM_UI_FACTORY_TUNING_BUTTON_CHANGE.color = Color.white;
		}
	}

	public void OnConfirmOptionTuningBouns(NKM_STAT_TYPE newStat)
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_FORGE_TUNING_CONFIRM_TITLE, string.Format(NKCUtilString.GET_STRING_FORGE_TUNING_CONFIRM_DESC_TWO_PARAM, GetTuningOptionText(m_iSelectOptionIdx - 1, Before: true), GetTuningOptionConfirmText(m_iSelectOptionIdx - 1, newStat)), delegate
		{
			NKCPacketSender.Send_NKMPacket_EQUIP_TUNING_STAT_CHANGE_BONUS_CONFIRM_REQ(m_LeftEquipUID, m_iSelectOptionIdx, newStat);
		});
	}

	private string GetTuningOptionConfirmText(int slotIndex, NKM_STAT_TYPE statType)
	{
		NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(m_LeftEquipUID);
		if (itemEquip != null)
		{
			NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
			if (equipTemplet != null)
			{
				int statGroupID = ((slotIndex == 0) ? equipTemplet.m_StatGroupID : equipTemplet.m_StatGroupID_2);
				int precision = ((slotIndex == 1) ? itemEquip.m_Precision2 : itemEquip.m_Precision);
				float value = 0f;
				NKMEquipRandomStatTemplet equipRandomStat = NKMEquipTuningManager.GetEquipRandomStat(statGroupID, statType);
				if (equipRandomStat != null)
				{
					value = equipRandomStat.CalcResultStat(precision);
				}
				if (IsPercentStat(equipRandomStat))
				{
					decimal num = new decimal(value);
					num = Math.Round(num * 1000m) / 1000m;
					return NKCUtilString.GetStatShortString("<color=#ffdb00>{0} {1:P1}</color>", statType, num);
				}
				return NKCUtilString.GetStatShortString("<color=#ffdb00>{0} {1:+#;-#;''}</color>", statType, value);
			}
		}
		return "";
	}

	public void OnConfirmSetOptionTuningBouns(int newSetID)
	{
		NKMEquipItemData itemEquip = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetItemEquip(m_LeftEquipUID);
		if (itemEquip == null)
		{
			return;
		}
		NKMItemEquipSetOptionTemplet equipSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(itemEquip.m_SetOptionId);
		NKMItemEquipSetOptionTemplet equipSetOptionTemplet2 = NKMItemManager.GetEquipSetOptionTemplet(newSetID);
		if (equipSetOptionTemplet2 == null)
		{
			Debug.Log($"<color=red>NKCUIForgeTuning::OnConfirmSetOptionTuningBouns - Warning Set Option ID : {newSetID}</color>");
			return;
		}
		NKCPopupChangeConfirm.Instance.Open(NKCUtilString.GET_STRING_FORGE_SET_OPTION_CHANGE_POPUP_CONFIRM_TITLE, NKCStringTable.GetString(equipSetOptionTemplet.m_EquipSetName), NKCStringTable.GetString(equipSetOptionTemplet2.m_EquipSetName), NKCUtilString.GET_STRING_FORGE_SET_OPTION_CHANGE_POPUP_CONFIRM_DESC, delegate
		{
			NKCPacketSender.Send_NKMPacket_EQUIP_ITEM_BONUS_CONFIRM_SET_OPTION_REQ(m_LeftEquipUID, newSetID);
		});
	}

	public void OpenOptionList()
	{
		if (NKCPopupEquipOption != null)
		{
			NKCPopupEquipOption.Open(m_LeftEquipUID, GetOptionState(), NKCUtilString.GET_STRING_SETOPTION_CHANGE_NOTICE);
		}
	}

	public void OpenSetOptionList()
	{
		if (NKCPopupEquipSetOption != null)
		{
			NKCPopupEquipSetOption.Open(m_LeftEquipUID, NKCUtilString.GET_STRING_SETOPTION_CHANGE_NOTICE);
		}
	}

	private int GetOptionState()
	{
		int num = 0;
		if (m_LeftEquipUID == 0L)
		{
			return num;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return num;
		}
		NKMEquipItemData itemEquip = myUserData.m_InventoryData.GetItemEquip(m_LeftEquipUID);
		if (itemEquip == null)
		{
			return num;
		}
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
		if (equipTemplet == null)
		{
			return num;
		}
		if (NKMEquipTuningManager.IsChangeableStatGroup(equipTemplet.m_StatGroupID))
		{
			num++;
		}
		if (NKMEquipTuningManager.IsChangeableStatGroup(equipTemplet.m_StatGroupID_2))
		{
			num += 2;
		}
		return num;
	}

	private int GetSlotState()
	{
		int num = 0;
		if (m_LeftEquipUID == 0L)
		{
			return num;
		}
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		if (myUserData == null)
		{
			return num;
		}
		NKMEquipItemData itemEquip = myUserData.m_InventoryData.GetItemEquip(m_LeftEquipUID);
		if (itemEquip == null)
		{
			return num;
		}
		NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(itemEquip.m_ItemEquipID);
		if (equipTemplet == null)
		{
			return num;
		}
		if (equipTemplet.m_StatGroupID != 0)
		{
			num++;
		}
		if (equipTemplet.m_StatGroupID_2 != 0)
		{
			num += 2;
		}
		return num;
	}

	public void SetEffect(int effectID, int slotID = 0)
	{
		switch (effectID)
		{
		case 0:
			switch (slotID)
			{
			case 1:
				SetAutoUnActiveEffectObject(ref m_AB_FX_UI_FACTORY_SMALL_SLOT_OPTION_BEFORE, "ACTIVE_OPTION");
				break;
			case 2:
				SetAutoUnActiveEffectObject(ref m_AB_FX_UI_FACTORY_SMALL_SLOT_OPTION_AFTER, "ACTIVE_OPTION");
				break;
			}
			if (slotID == 1 || slotID == 2)
			{
				NKCSoundManager.PlaySound("FX_UI_UNIT_GET_STAR", 1f, 0f, 0f);
			}
			break;
		case 1:
			switch (slotID)
			{
			case 1:
				SetAutoUnActiveEffectObject(ref m_AB_FX_UI_FACTORY_SMALL_SLOT_OPTION_BEFORE, "ACTIVE_OPTION");
				break;
			case 2:
				SetAutoUnActiveEffectObject(ref m_AB_FX_UI_FACTORY_SMALL_SLOT_OPTION_AFTER, "ACTIVE_OPTION");
				break;
			}
			NKCSoundManager.PlaySound("FX_UI_UNIT_GET_MAIN", 1f, 0f, 0f);
			break;
		case 2:
			SetAutoUnActiveEffectObject(ref m_AB_FX_UI_FACTORY_SMALL_SLOT_AFTER, "ACTIVE_SET_OPTION");
			NKCSoundManager.PlaySound("FX_UI_UNIT_GET_MAIN", 1f, 0f, 0f);
			break;
		case 3:
			SetAutoUnActiveEffectObject(ref m_AB_FX_UI_FACTORY_EQUIP_SLOT, "ACTIVE_OPTION");
			NKCSoundManager.PlaySound("FX_UI_UNIT_GET_STAR", 1f, 0f, 0f);
			break;
		case 4:
			SetAutoUnActiveEffectObject(ref m_AB_FX_UI_FACTORY_EQUIP_SLOT, "ACTIVE_SET_OPTION");
			NKCSoundManager.PlaySound("FX_UI_UNIT_GET_STAR", 1f, 0f, 0f);
			break;
		}
	}

	private void SetAutoUnActiveEffectObject(ref Animator ani, string key)
	{
		if (!(ani != null))
		{
			return;
		}
		NKCUtil.SetGameobjectActive(ani.gameObject, bValue: true);
		ani.SetTrigger(key);
		RuntimeAnimatorController runtimeAnimatorController = ani.runtimeAnimatorController;
		if (runtimeAnimatorController != null)
		{
			UnActiveReservedGameObject = null;
			int num = -1;
			if (string.Equals(key, "ACTIVE_OPTION"))
			{
				num = 0;
			}
			else if (string.Equals(key, "ACTIVE_SET_OPTION"))
			{
				num = 1;
			}
			if (num > -1 && runtimeAnimatorController.animationClips.Length >= num)
			{
				float length = runtimeAnimatorController.animationClips[num].length;
				StartCoroutine(UnActiveEffect(length));
				UnActiveReservedGameObject = ani.gameObject;
				Debug.Log($"test : {length}");
			}
		}
	}

	private IEnumerator UnActiveEffect(float closedTime)
	{
		yield return new WaitForSeconds(closedTime);
		NKCUtil.SetGameobjectActive(UnActiveReservedGameObject, bValue: false);
	}

	public static bool IsPercentStat(NKMEquipRandomStatTemplet statTemplet)
	{
		if (statTemplet != null)
		{
			return NKMUnitStatManager.IsPercentStat(statTemplet.m_StatType);
		}
		return false;
	}

	public static string GetTuningOptionStatString(NKMEquipItemData equipData, int StatGroupIdx = -1)
	{
		string result = "";
		bool bPercentStat = false;
		if (equipData != null)
		{
			EQUIP_ITEM_STAT eQUIP_ITEM_STAT = null;
			if (equipData.m_Stat.Count > 0 && equipData.m_Stat.Count > StatGroupIdx)
			{
				eQUIP_ITEM_STAT = equipData.m_Stat[StatGroupIdx];
			}
			if (eQUIP_ITEM_STAT != null)
			{
				NKMEquipTemplet equipTemplet = NKMItemManager.GetEquipTemplet(equipData.m_ItemEquipID);
				if (equipTemplet != null)
				{
					foreach (NKMEquipRandomStatTemplet equipRandomStatGroup in NKMEquipTuningManager.GetEquipRandomStatGroupList((StatGroupIdx == 1) ? equipTemplet.m_StatGroupID : equipTemplet.m_StatGroupID_2))
					{
						if (equipRandomStatGroup.m_StatType == eQUIP_ITEM_STAT.type)
						{
							bPercentStat = IsPercentStat(equipRandomStatGroup);
						}
					}
				}
				return GetTuningOptionStatString(eQUIP_ITEM_STAT, equipData, bPercentStat);
			}
		}
		return result;
	}

	public static string GetTuningOptionStatString(EQUIP_ITEM_STAT statData, NKMEquipItemData equipData, bool bPercentStat)
	{
		if (bPercentStat)
		{
			decimal num = new decimal(statData.stat_value + (float)equipData.m_EnchantLevel * statData.stat_level_value);
			num = Math.Round(num * 1000m) / 1000m;
			return NKCUtilString.GetStatShortString("{0} {1:P1}", statData.type, num);
		}
		return NKCUtilString.GetStatShortString("{0} {1:+#;-#;''}", statData.type, statData.stat_value + (float)equipData.m_EnchantLevel * statData.stat_level_value);
	}
}
