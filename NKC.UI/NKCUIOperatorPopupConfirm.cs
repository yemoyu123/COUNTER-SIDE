using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperatorPopupConfirm : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_operator_info";

	private const string UI_ASSET_NAME = "NKM_UI_POPUP_OPERATOR_CONFIRM";

	private static NKCUIOperatorPopupConfirm m_Instance;

	public Text m_lbTitle;

	[Header("오퍼레이터 재료")]
	public GameObject m_ObjOperatorPanel;

	public NKCUIOperatorSelectListSlot m_OperatorSlot;

	public Text m_lbStatHP;

	public Text m_lbStatAtk;

	public Text m_lbStatDef;

	public Text m_lbSkillCollReduce;

	public NKCUIOperatorSkill m_MainSkill;

	public NKCUIOperatorSkill m_Subskill;

	public GameObject m_CostItem;

	public NKCUISlot m_CostItemSlot;

	[Header("토큰 재료")]
	public GameObject m_ObjTokenPanel;

	public NKCUISlot m_TokenCostitem;

	public NKCUISlot m_TokenSkill;

	public NKCUIComStateButton m_Confirm;

	public NKCUIComStateButton m_Cancel;

	private UnityAction m_OK;

	private bool m_bSubSkillTransferConfim;

	public static NKCUIOperatorPopupConfirm Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIOperatorPopupConfirm>("ab_ui_nkm_ui_operator_info", "NKM_UI_POPUP_OPERATOR_CONFIRM", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIOperatorPopupConfirm>();
				m_Instance.Init();
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

	public override string MenuName => "POPUP_OPERATOR_CONFIRM";

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

	public override void CloseInternal()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void Init()
	{
		NKCUtil.SetBindFunction(m_Confirm, OnConfirm);
		NKCUtil.SetHotkey(m_Confirm, HotkeyEventType.Confirm);
		NKCUtil.SetBindFunction(m_Cancel, OnCancel);
	}

	public void Open(long targetUID, int BaseUnitID = 0, UnityAction callBack = null)
	{
		NKMOperator operatorData = NKCOperatorUtil.GetOperatorData(targetUID);
		if (operatorData != null)
		{
			NKCUtil.SetGameobjectActive(m_ObjTokenPanel, bValue: false);
			NKCUtil.SetGameobjectActive(m_ObjOperatorPanel, bValue: true);
			NKMOperator operatorData2 = NKCOperatorUtil.GetOperatorData(targetUID);
			if (operatorData2 != null)
			{
				m_OperatorSlot.SetData(operatorData2, NKMDeckIndex.None, bEnableLayoutElement: true, null);
			}
			NKCUtil.SetLabelText(m_lbStatHP, NKCOperatorUtil.GetStatPercentageString(operatorData, NKM_STAT_TYPE.NST_HP));
			NKCUtil.SetLabelText(m_lbStatAtk, NKCOperatorUtil.GetStatPercentageString(operatorData, NKM_STAT_TYPE.NST_ATK));
			NKCUtil.SetLabelText(m_lbStatDef, NKCOperatorUtil.GetStatPercentageString(operatorData, NKM_STAT_TYPE.NST_DEF));
			NKCUtil.SetLabelText(m_lbSkillCollReduce, NKCOperatorUtil.GetStatPercentageString(operatorData, NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE));
			m_MainSkill.SetData(operatorData.mainSkill.id, operatorData.mainSkill.level);
			m_Subskill.SetData(operatorData.subSkill.id, operatorData.subSkill.level);
			m_bSubSkillTransferConfim = false;
			int num = 0;
			if (operatorData2.level > 1)
			{
				num++;
			}
			if (operatorData2.mainSkill.level > 1)
			{
				num++;
			}
			if (operatorData2.subSkill.level > 1)
			{
				num++;
			}
			if (num > 0)
			{
				m_bSubSkillTransferConfim = true;
			}
			NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_OPERATOR_CONFIRM_POPUP_TITLE_TRANSFER);
			SetCostItem(BaseUnitID);
			UIOpened();
			m_OK = callBack;
		}
	}

	public void Open(long targetUID, UnityAction callBack = null)
	{
		NKMOperator operatorData = NKCOperatorUtil.GetOperatorData(targetUID);
		if (operatorData != null)
		{
			NKCUtil.SetGameobjectActive(m_ObjTokenPanel, bValue: false);
			NKCUtil.SetGameobjectActive(m_ObjOperatorPanel, bValue: true);
			NKMOperator operatorData2 = NKCOperatorUtil.GetOperatorData(targetUID);
			if (operatorData2 != null)
			{
				m_OperatorSlot.SetData(operatorData2, NKMDeckIndex.None, bEnableLayoutElement: true, null);
			}
			NKCUtil.SetLabelText(m_lbStatHP, NKCOperatorUtil.GetStatPercentageString(operatorData, NKM_STAT_TYPE.NST_HP));
			NKCUtil.SetLabelText(m_lbStatAtk, NKCOperatorUtil.GetStatPercentageString(operatorData, NKM_STAT_TYPE.NST_ATK));
			NKCUtil.SetLabelText(m_lbStatDef, NKCOperatorUtil.GetStatPercentageString(operatorData, NKM_STAT_TYPE.NST_DEF));
			NKCUtil.SetLabelText(m_lbSkillCollReduce, NKCOperatorUtil.GetStatPercentageString(operatorData, NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE));
			m_MainSkill.SetData(operatorData.mainSkill.id, operatorData.mainSkill.level);
			m_Subskill.SetData(operatorData.subSkill.id, operatorData.subSkill.level);
			NKCUtil.SetLabelText(m_lbTitle, NKCUtilString.GET_STRING_OPERATOR_CONFIRM_POPUP_TITLE_SELECT);
			NKCUtil.SetGameobjectActive(m_CostItem, bValue: false);
			UIOpened();
			m_OK = callBack;
		}
	}

	public void OpenForToken(int TokenItemID, int BaseUnitID, UnityAction callBack = null)
	{
		NKCUtil.SetGameobjectActive(m_ObjTokenPanel, bValue: true);
		NKCUtil.SetGameobjectActive(m_ObjOperatorPanel, bValue: false);
		NKCUISlot.SlotData slotData = NKCUISlot.SlotData.MakeMiscItemData(TokenItemID, 1L);
		m_TokenSkill.SetMiscItemData(slotData, bShowName: false, bShowCount: true, bEnableLayoutElement: true, null);
		SetCostItem(BaseUnitID, bForTokenUI: true);
		UIOpened();
		m_OK = callBack;
	}

	private void SetCostItem(int BaseUnitID, bool bForTokenUI = false)
	{
		bool bValue = false;
		if (BaseUnitID != 0)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(BaseUnitID);
			if (unitTempletBase != null)
			{
				NKMOperatorConstTemplet.HostUnit hostUnit = NKMCommonConst.OperatorConstTemplet.hostUnits.Find((NKMOperatorConstTemplet.HostUnit e) => e.m_NKM_UNIT_GRADE == unitTempletBase.m_NKM_UNIT_GRADE);
				if (hostUnit != null)
				{
					int info = hostUnit.itemCount;
					int bounsRatio = 0;
					NKCCompanyBuff.SetDiscountOfOperatorSkillEnhanceCost(NKCScenManager.GetScenManager().GetMyUserData().m_companyBuffDataList, ref info, ref bounsRatio);
					if (!bForTokenUI)
					{
						m_CostItemSlot.SetData(NKCUISlot.SlotData.MakeMiscItemData(hostUnit.itemId, info), bShowName: false, bShowNumber: true, bEnableLayoutElement: true, null);
					}
					else
					{
						m_TokenCostitem.SetData(NKCUISlot.SlotData.MakeMiscItemData(hostUnit.itemId, info), bShowName: false, bShowNumber: true, bEnableLayoutElement: true, null);
					}
					bValue = true;
				}
			}
		}
		NKCUtil.SetGameobjectActive(m_CostItem, bValue);
	}

	private void OnCancel()
	{
		Close();
	}

	private void OnConfirm()
	{
		if (m_bSubSkillTransferConfim)
		{
			NKCPopupInputText.Instance.OpenOKCancelBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_OPERATOR_CONFIRM_POPUP_WARNING, NKCUtilString.GET_STRING_OPERATOR_CONFIRM_FUSION_TEXT, NKCUtilString.GET_STRING_OPERATOR_CONFIRM_FUSION_TEXT, OnEditText, null, 0);
		}
		else
		{
			OnConfirmOperatorMix();
		}
	}

	private void OnEditText(string inputString)
	{
		if (string.Equals(inputString.ToLower(), NKCUtilString.GET_STRING_OPERATOR_CONFIRM_FUSION_TEXT.ToLower()))
		{
			OnConfirmOperatorMix();
		}
		else
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_OPTION_SIGN_OUT_MESSAGE_MISS_MATCHED);
		}
	}

	private void OnConfirmOperatorMix()
	{
		m_OK?.Invoke();
		Close();
	}
}
