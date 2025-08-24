using System.Collections;
using System.Collections.Generic;
using NKC.UI;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCOperatorInfoPopupSkillResult : NKCUIBase
{
	private enum RESULT_EVENT_TYPE
	{
		NONE,
		MAIN_SKILL_UP_OK,
		MAIN_SKILL_UP_FAIL,
		SUB_SKILL_UP_OK,
		SUB_SKILL_UP_FAIL,
		SUB_SKILL_TRANSFER_OK,
		SUB_SKILL_TRANSFER_FAIL
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_operator_info";

	private const string UI_ASSET_NAME = "NKM_UI_OPERATOR_INFO_POPUP_SKILL_RESULT";

	private static NKCOperatorInfoPopupSkillResult m_Instance;

	public NKCUIComStateButton m_Confirm;

	public GameObject m_Success;

	public GameObject m_Fail;

	public NKCUIOperatorSkill m_OriginalSkill;

	public NKCUIOperatorSkill m_UpgradeSkill;

	public NKCUIOperatorSkill m_FailedSkill;

	public Text m_lbResult;

	public Animator m_Ani;

	private bool m_bPlaySkillUpVoice;

	private List<RESULT_EVENT_TYPE> m_lstEvent = new List<RESULT_EVENT_TYPE>();

	private NKMOperator m_Operator;

	private int m_preSubSkillID;

	private int m_preSubSkillLevel;

	private string Success = "Success";

	private string Fail = "Fail";

	private string Implant = "Implant";

	private string SuccessOutro = "SuccessOutro";

	private string FailOutro = "FailOutro";

	private string ImplantOutro = "ImplantOutro";

	private bool m_bClickBlock;

	private bool m_bPlayOutro;

	[Header("애니메이션 딜레이 갭")]
	public float m_fDelayGap;

	private RESULT_EVENT_TYPE m_lastResultEventType;

	[Header("보이스 재생 딜레이시간")]
	public float m_fVoiceDelayTime = 0.3f;

	public static NKCOperatorInfoPopupSkillResult Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCOperatorInfoPopupSkillResult>("ab_ui_nkm_ui_operator_info", "NKM_UI_OPERATOR_INFO_POPUP_SKILL_RESULT", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCOperatorInfoPopupSkillResult>();
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

	public override string MenuName => NKCUtilString.GET_STRING_OPERATOR_SKILL_RESULT_TITLE;

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
		NKCUtil.SetBindFunction(m_Confirm, OnStartResult);
		NKCUtil.SetHotkey(m_Confirm, HotkeyEventType.Confirm);
	}

	public void Open(long targetUID, bool bTryMainSkill, bool bMainSkillLvUp, bool bTrySubskill, bool bSubskillLvUp, bool bTryImplantSubSkill, bool bImplantSubskill, int oldSubSkillID, int oldSubSkillLv)
	{
		m_lstEvent.Clear();
		m_Operator = NKCOperatorUtil.GetOperatorData(targetUID);
		if (m_Operator == null)
		{
			return;
		}
		if (bTryMainSkill)
		{
			if (bMainSkillLvUp)
			{
				m_lstEvent.Add(RESULT_EVENT_TYPE.MAIN_SKILL_UP_OK);
			}
			else
			{
				m_lstEvent.Add(RESULT_EVENT_TYPE.MAIN_SKILL_UP_FAIL);
			}
		}
		if (bTrySubskill)
		{
			if (bSubskillLvUp)
			{
				m_lstEvent.Add(RESULT_EVENT_TYPE.SUB_SKILL_UP_OK);
			}
			else
			{
				m_lstEvent.Add(RESULT_EVENT_TYPE.SUB_SKILL_UP_FAIL);
			}
		}
		if (bTryImplantSubSkill)
		{
			if (bImplantSubskill)
			{
				m_lstEvent.Add(RESULT_EVENT_TYPE.SUB_SKILL_TRANSFER_OK);
			}
			else
			{
				m_lstEvent.Add(RESULT_EVENT_TYPE.SUB_SKILL_TRANSFER_FAIL);
			}
		}
		m_preSubSkillID = oldSubSkillID;
		m_preSubSkillLevel = oldSubSkillLv;
		m_lastResultEventType = RESULT_EVENT_TYPE.NONE;
		m_bClickBlock = false;
		m_bPlayOutro = true;
		m_bPlaySkillUpVoice = false;
		UIOpened();
		OnStartResult();
	}

	private void OnStartResult()
	{
		if (!m_bClickBlock)
		{
			if (!m_bPlayOutro)
			{
				OnPlayOutro();
				return;
			}
			if (m_lstEvent.Count <= 0)
			{
				Close();
				return;
			}
			OnResult(m_lstEvent[0]);
			m_lstEvent.RemoveAt(0);
		}
	}

	private void OnPlayOutro()
	{
		switch (m_lastResultEventType)
		{
		case RESULT_EVENT_TYPE.MAIN_SKILL_UP_OK:
		case RESULT_EVENT_TYPE.SUB_SKILL_UP_OK:
			m_Ani.SetTrigger(SuccessOutro);
			break;
		case RESULT_EVENT_TYPE.MAIN_SKILL_UP_FAIL:
		case RESULT_EVENT_TYPE.SUB_SKILL_UP_FAIL:
		case RESULT_EVENT_TYPE.SUB_SKILL_TRANSFER_FAIL:
			m_Ani.SetTrigger(FailOutro);
			break;
		case RESULT_EVENT_TYPE.SUB_SKILL_TRANSFER_OK:
			m_Ani.SetTrigger(ImplantOutro);
			break;
		}
		m_bPlayOutro = true;
		StartCoroutine(StartDelayAni(m_Ani.GetCurrentAnimatorStateInfo(0).length - m_fDelayGap, m_lstEvent.Count > 0, m_lstEvent.Count <= 0));
	}

	private IEnumerator StartDelayAni(float delay, bool autoOpen = false, bool autoClose = false)
	{
		m_bClickBlock = true;
		yield return new WaitForSeconds(delay);
		m_bClickBlock = false;
		if (autoOpen)
		{
			OnStartResult();
		}
		if (autoClose)
		{
			Close();
		}
	}

	private void OnResult(RESULT_EVENT_TYPE type)
	{
		bool flag = false;
		string msg = "";
		m_lastResultEventType = type;
		switch (m_lastResultEventType)
		{
		case RESULT_EVENT_TYPE.MAIN_SKILL_UP_OK:
			m_OriginalSkill.SetData(m_Operator.mainSkill.id, m_Operator.mainSkill.level - 1);
			m_UpgradeSkill.SetData(m_Operator.mainSkill.id, m_Operator.mainSkill.level);
			msg = NKCUtilString.GET_STRING_OPERATOR_MAIN_SKILL_SUCCESS;
			flag = true;
			break;
		case RESULT_EVENT_TYPE.SUB_SKILL_TRANSFER_OK:
			m_OriginalSkill.SetData(m_preSubSkillID, m_preSubSkillLevel);
			m_UpgradeSkill.SetData(m_Operator.subSkill.id, m_Operator.subSkill.level);
			msg = NKCUtilString.GET_STRING_OPERATOR_PASSIVE_SKILL_IMPLANT_SUCCESS;
			flag = true;
			break;
		case RESULT_EVENT_TYPE.SUB_SKILL_UP_OK:
			m_OriginalSkill.SetData(m_preSubSkillID, m_preSubSkillLevel);
			m_UpgradeSkill.SetData(m_Operator.subSkill.id, m_Operator.subSkill.level);
			msg = NKCUtilString.GET_STRING_OPERATOR_PASSIVE_SKILL_SUCCESS;
			flag = true;
			break;
		case RESULT_EVENT_TYPE.MAIN_SKILL_UP_FAIL:
			m_FailedSkill.SetData(m_Operator.mainSkill.id, m_Operator.mainSkill.level);
			msg = NKCUtilString.GET_STRING_OPERATOR_MAIN_SKILL_FAIL;
			break;
		case RESULT_EVENT_TYPE.SUB_SKILL_TRANSFER_FAIL:
			m_FailedSkill.SetData(m_Operator.subSkill.id, m_Operator.subSkill.level);
			msg = NKCUtilString.GET_STRING_OPERATOR_PASSIVE_SKILL_IMPLANT_FAIL;
			break;
		case RESULT_EVENT_TYPE.SUB_SKILL_UP_FAIL:
			m_FailedSkill.SetData(m_Operator.subSkill.id, m_Operator.subSkill.level);
			msg = NKCUtilString.GET_STRING_OPERATOR_PASSIVE_SKILL_FAIL;
			break;
		}
		if (!m_bPlaySkillUpVoice && (m_lastResultEventType == RESULT_EVENT_TYPE.MAIN_SKILL_UP_OK || m_lastResultEventType == RESULT_EVENT_TYPE.SUB_SKILL_TRANSFER_OK || m_lastResultEventType == RESULT_EVENT_TYPE.SUB_SKILL_UP_OK))
		{
			m_bPlaySkillUpVoice = true;
			StartCoroutine(PlaySkillGrowthVoice());
		}
		if (!flag)
		{
			m_Ani.SetTrigger(Fail);
		}
		else if (type == RESULT_EVENT_TYPE.MAIN_SKILL_UP_OK || type == RESULT_EVENT_TYPE.SUB_SKILL_UP_OK)
		{
			m_Ani.SetTrigger(Success);
		}
		else
		{
			m_Ani.SetTrigger(Implant);
		}
		m_bPlayOutro = false;
		StartCoroutine(StartDelayAni(m_Ani.GetCurrentAnimatorStateInfo(0).length - m_fDelayGap));
		NKCUtil.SetGameobjectActive(m_Success, flag);
		NKCUtil.SetGameobjectActive(m_Fail, !flag);
		NKCUtil.SetLabelText(m_lbResult, msg);
	}

	private IEnumerator PlaySkillGrowthVoice()
	{
		yield return new WaitForSeconds(m_fVoiceDelayTime);
		if (m_Operator != null)
		{
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_GROWTH_SKILL, m_Operator);
		}
	}
}
