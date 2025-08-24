using NKM;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperatorPopupChange : NKCUIBase
{
	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_unit_change_popup";

	private const string UI_ASSET_NAME = "NKM_UI_OPERATOR_CHANGE_POPUP";

	private static NKCUIOperatorPopupChange m_Instance;

	public EventTrigger m_CloseEvent;

	public NKCUIComButton m_Ok;

	public NKCUIComButton m_Cancel;

	public NKCUIOperatorSelectListSlot m_BeforeOperator;

	public NKCUIOperatorSelectListSlot m_AfterOperator;

	public NKCUIOperatorSkill m_BeforeMainSkill;

	public NKCUIOperatorSkill m_BeforeSubSkill;

	public Text m_BeforeStatHP;

	public Text m_BeforeStatATK;

	public Text m_BeforeStatDEF;

	public Text m_BeforeStatSKILL;

	public NKCUIOperatorSkill m_AfterMainSkill;

	public NKCUIOperatorSkill m_AfterSubSkill;

	public Text m_AfterStatHP;

	public Text m_AfterStatATK;

	public Text m_AfterStatDEF;

	public Text m_AfterStatSKILL;

	public Text m_ChangeStatHP;

	public Text m_ChangeStatATK;

	public Text m_ChangeStatDEF;

	public Text m_ChangeStatSKILL;

	private NKMDeckIndex m_curDeckIdx;

	private NKMOperator m_BeforeOperatorData;

	private NKMOperator m_AfterOperatorData;

	private UnityAction m_CallBack;

	public static NKCUIOperatorPopupChange Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIOperatorPopupChange>("ab_ui_nkm_ui_unit_change_popup", "NKM_UI_OPERATOR_CHANGE_POPUP", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIOperatorPopupChange>();
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

	public override string MenuName => NKCUtilString.GET_STRING_OPERATOR_SKILL_POPUP;

	public override eMenutype eUIType => eMenutype.Popup;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	public override void CloseInternal()
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void Init()
	{
		if (m_CloseEvent != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(OnEventPanelClick);
			m_CloseEvent.triggers.Add(entry);
		}
		if (m_Cancel != null)
		{
			m_Cancel.PointerClick.RemoveAllListeners();
			m_Cancel.PointerClick.AddListener(base.Close);
		}
		if (m_Ok != null)
		{
			m_Ok.PointerClick.RemoveAllListeners();
			m_Ok.PointerClick.AddListener(OnConfirm);
			NKCUtil.SetHotkey(m_Ok, HotkeyEventType.Confirm);
		}
	}

	private void OnEventPanelClick(BaseEventData e)
	{
		Close();
	}

	public void Open(NKMDeckIndex deckIdx, long BeforeOperatorUID, long AfterOperatorUID, UnityAction callBack = null)
	{
		m_curDeckIdx = deckIdx;
		m_BeforeOperatorData = NKCOperatorUtil.GetOperatorData(BeforeOperatorUID);
		if (m_BeforeOperatorData != null)
		{
			m_BeforeOperator.SetData(m_BeforeOperatorData, deckIdx, bEnableLayoutElement: true, null);
		}
		m_AfterOperatorData = NKCOperatorUtil.GetOperatorData(AfterOperatorUID);
		if (m_AfterOperatorData != null)
		{
			m_AfterOperator.SetData(m_AfterOperatorData, NKMDeckIndex.None, bEnableLayoutElement: true, null);
		}
		if (m_BeforeOperatorData != null && m_AfterOperatorData != null)
		{
			m_BeforeMainSkill.SetData(m_BeforeOperatorData.mainSkill.id, m_BeforeOperatorData.mainSkill.level);
			m_BeforeSubSkill.SetData(m_BeforeOperatorData.subSkill.id, m_BeforeOperatorData.subSkill.level);
			m_AfterMainSkill.SetData(m_AfterOperatorData.mainSkill.id, m_AfterOperatorData.mainSkill.level);
			m_AfterSubSkill.SetData(m_AfterOperatorData.subSkill.id, m_AfterOperatorData.subSkill.level);
			float stateValue = NKCOperatorUtil.GetStateValue(m_BeforeOperatorData, NKM_STAT_TYPE.NST_HP);
			float stateValue2 = NKCOperatorUtil.GetStateValue(m_BeforeOperatorData, NKM_STAT_TYPE.NST_ATK);
			float stateValue3 = NKCOperatorUtil.GetStateValue(m_BeforeOperatorData, NKM_STAT_TYPE.NST_DEF);
			float stateValue4 = NKCOperatorUtil.GetStateValue(m_BeforeOperatorData, NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE);
			float stateValue5 = NKCOperatorUtil.GetStateValue(m_AfterOperatorData, NKM_STAT_TYPE.NST_HP);
			float stateValue6 = NKCOperatorUtil.GetStateValue(m_AfterOperatorData, NKM_STAT_TYPE.NST_ATK);
			float stateValue7 = NKCOperatorUtil.GetStateValue(m_AfterOperatorData, NKM_STAT_TYPE.NST_DEF);
			float stateValue8 = NKCOperatorUtil.GetStateValue(m_AfterOperatorData, NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE);
			NKCUtil.SetLabelText(m_BeforeStatHP, NKCOperatorUtil.GetStatPercentageString(stateValue));
			NKCUtil.SetLabelText(m_BeforeStatATK, NKCOperatorUtil.GetStatPercentageString(stateValue2));
			NKCUtil.SetLabelText(m_BeforeStatDEF, NKCOperatorUtil.GetStatPercentageString(stateValue3));
			NKCUtil.SetLabelText(m_BeforeStatSKILL, NKCOperatorUtil.GetStatPercentageString(stateValue4));
			NKCUtil.SetLabelText(m_AfterStatHP, NKCOperatorUtil.GetStatPercentageString(stateValue5));
			NKCUtil.SetLabelText(m_AfterStatATK, NKCOperatorUtil.GetStatPercentageString(stateValue6));
			NKCUtil.SetLabelText(m_AfterStatDEF, NKCOperatorUtil.GetStatPercentageString(stateValue7));
			NKCUtil.SetLabelText(m_AfterStatSKILL, NKCOperatorUtil.GetStatPercentageString(stateValue8));
			SetChangeStatString(ref m_ChangeStatHP, stateValue, stateValue5);
			SetChangeStatString(ref m_ChangeStatATK, stateValue2, stateValue6);
			SetChangeStatString(ref m_ChangeStatDEF, stateValue3, stateValue7);
			SetChangeStatString(ref m_ChangeStatSKILL, stateValue4, stateValue8);
		}
		m_CallBack = callBack;
		UIOpened();
	}

	private void SetChangeStatString(ref Text str, float beforeValue, float AfterValue)
	{
		NKCUtil.SetGameobjectActive(str, beforeValue != AfterValue);
		if (beforeValue > AfterValue)
		{
			NKCUtil.SetLabelText(str, string.Format(NKCUtilString.GET_STRING_OPERATOR_POPUP_CHANGE_STAT_MINUS_DESC_01, NKCOperatorUtil.GetStatPercentageString(beforeValue - AfterValue)));
			NKCUtil.SetLabelTextColor(str, NKCUtil.GetColor("#FF3D40"));
		}
		else
		{
			NKCUtil.SetLabelText(str, string.Format(NKCUtilString.GET_STRING_OPERATOR_POPUP_CHANGE_STAT_PLUS_DESC_01, NKCOperatorUtil.GetStatPercentageString(AfterValue - beforeValue)));
			NKCUtil.SetLabelTextColor(str, NKCUtil.GetColor("#A3FF66"));
		}
	}

	private void OnConfirm()
	{
		m_CallBack?.Invoke();
		Close();
	}
}
