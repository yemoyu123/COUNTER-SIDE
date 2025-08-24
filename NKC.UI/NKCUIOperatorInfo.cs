using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Math;
using NKC.Templet;
using NKC.UI.Collection;
using NKC.UI.Component;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIOperatorInfo : NKCUIBase
{
	private enum TAB_STATE
	{
		NONE,
		INFO,
		LEVEL_UP
	}

	public class OpenOption
	{
		public readonly List<long> m_lstOperatorUID = new List<long>();

		public readonly List<NKMOperator> m_lstOperatorData = new List<NKMOperator>();

		public int m_SelectSlotIndex;

		public bool m_bShowFierceInfo;

		public OpenOption(List<long> UnitUIDList, int SlotIdx = 0)
		{
			if (UnitUIDList != null && UnitUIDList.Count > 1)
			{
				m_lstOperatorUID = UnitUIDList;
				m_SelectSlotIndex = SlotIdx;
			}
		}

		public OpenOption(List<NKMOperator> lstOperatorData, int SlotIdx = 0)
		{
			if (lstOperatorData != null && lstOperatorData.Count > 1)
			{
				m_lstOperatorData = lstOperatorData;
				m_SelectSlotIndex = SlotIdx;
			}
		}
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_operator_info";

	private const string UI_ASSET_NAME = "NKM_UI_OPERATOR_INFO";

	private static NKCUIOperatorInfo m_Instance;

	public RectTransform m_NKM_UI_UNIT_ILLUST_ROOT;

	[Header("유닛 일러스트")]
	public NKCUIComDragSelectablePanel m_DragCharacterView;

	public EventTrigger m_evtPanel;

	public GameObject m_NKM_UI_OPERATOR_INFO_ARROW;

	public NKCUIComStateButton m_Right;

	public NKCUIComStateButton m_Left;

	public GameObject m_NKM_UI_OPERATOR_INFO_CONTROL;

	public NKCUIComButton m_NKM_UI_UNIT_INFO_CONTROL_VOICE_BG;

	public NKCUIComButton m_NKM_UI_UNIT_INFO_CONTROL_PRACTICE_BG;

	public NKCUIComButton m_NKM_UI_UNIT_INFO_CONTROL_APPRAISAL_BG;

	public NKCUIComButton m_NKM_UI_UNIT_INFO_CONTROL_SKIN_VIEW_BG;

	[Header("우측 메뉴")]
	public GameObject m_NKM_UI_OPERATOR_INFO_DESC_STAT;

	public GameObject m_NKM_UI_OPERATOR_INFO_DESC_SKILL_01;

	public GameObject m_NKM_UI_OPERATOR_INFO_DESC_SKILL_02;

	public GameObject m_NKM_UI_OPERATOR_INFO_DESC_BUTTON;

	public GameObject m_LEVELUP;

	public NKCUIOperatorSummary m_NKM_UI_OPERATOR_INFO_SUMMARY;

	public NKCUIOperatorInfoPopupLevelUp m_NKM_UI_OPERATOR_INFO_POPUP_LEVELUP;

	public GameObject m_NKM_UI_OPERATOR_INFO_CONTROL_BOTTOM;

	public NKCUIComButton NKM_UI_OPERATOR_INFO_ILLUST_CHANGE;

	public NKCUIComToggle m_NKM_UI_OPERATOR_INFO_UNIT_LOCK;

	[Header("능력치")]
	public Text m_NKM_UI_OPERATOR_INFO_SUMMARY_ATTACK_TEXT;

	public Text m_STAT_NAME_HP;

	public Text m_STAT_NAME_ATK;

	public Text m_STAT_NAME_DEF;

	public Text m_STAT_NAME_SKILL_COOL;

	[Header("스킬 패널")]
	public NKCUIOperatorSkill m_MainSkill;

	public NKCUIOperatorSkill m_SubSkill;

	public NKCUIOperatorTacticalSkillCombo m_MainSkillCombo;

	public NKCUIComStateButton m_MainSkillBtn;

	public NKCUIComStateButton m_SubSkillBtn;

	public Text m_lbSkillCool;

	[Header("버튼")]
	public NKCUIComStateButton m_BUTTON_SKILLUP;

	public NKCUIComStateButton m_BUTTON_LEVELUP;

	public NKCUIComStateButton m_BUTTON_PLACENEMT;

	public NKCUIComStateButton m_BUTTON_COLLECTION;

	public NKCUIComStateButton m_BUTTON_SKILLUP_LOCK;

	public NKCUIComStateButton m_BUTTON_LEVELUP_LOCK;

	[Header("애니메이션")]
	public Animator m_NKM_UI_OPERATOR_INFO_ANI;

	[Header("성우")]
	public Text m_lbVoiceActor;

	[Header("event")]
	public GameObject m_objEvent;

	public Text m_lbEventDesc;

	private TAB_STATE m_State;

	private NKMOperator m_OperatorData;

	private string Ani_LvUp = "LvUp";

	private string Ani_LvUpIn = "LvUpIn";

	private string Ani_LvUpOut = "LvUpOut";

	private string Ani_Base = "Base";

	private OpenOption m_OpenOption;

	private bool m_bAppraisal;

	private bool m_bViewMode;

	public static NKCUIOperatorInfo Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIOperatorInfo>("ab_ui_nkm_ui_operator_info", "NKM_UI_OPERATOR_INFO", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIOperatorInfo>();
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	public static bool HasInstance => m_Instance != null;

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

	public override string MenuName => NKCUtilString.GET_STRING_OPERATOR_INFO_MENU_NAME;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string GuideTempletID => "ARTICLE_OPERATOR_INFO";

	public static void CheckInstanceAndClose()
	{
		if (m_Instance != null && m_Instance.IsOpen)
		{
			m_Instance.Close();
		}
	}

	public override void CloseInternal()
	{
		BannerCleanUp();
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
		m_State = TAB_STATE.NONE;
	}

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	private void Init()
	{
		if (m_NKM_UI_OPERATOR_INFO_POPUP_LEVELUP != null)
		{
			m_NKM_UI_OPERATOR_INFO_POPUP_LEVELUP.Init(OnStart);
		}
		NKCUtil.SetBindFunction(m_BUTTON_SKILLUP, OnClickSkillUp);
		NKCUtil.SetBindFunction(m_BUTTON_LEVELUP, OnClickLevelUp);
		if (m_DragCharacterView != null)
		{
			m_DragCharacterView.Init(rotation: true);
			m_DragCharacterView.dOnGetObject += MakeMainBannerListSlot;
			m_DragCharacterView.dOnReturnObject += ReturnMainBannerListSlot;
			m_DragCharacterView.dOnProvideData += ProvideMainBannerListSlotData;
			m_DragCharacterView.dOnIndexChangeListener += SelectCharacter;
			m_DragCharacterView.dOnFocus += Focus;
		}
		if (m_evtPanel != null)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(OnEventPanelClick);
			m_evtPanel.triggers.Add(entry);
		}
		if (m_NKM_UI_OPERATOR_INFO_UNIT_LOCK != null)
		{
			m_NKM_UI_OPERATOR_INFO_UNIT_LOCK.OnValueChanged.RemoveAllListeners();
			m_NKM_UI_OPERATOR_INFO_UNIT_LOCK.OnValueChanged.AddListener(OnClickLock);
		}
		if (null != m_NKM_UI_UNIT_INFO_CONTROL_VOICE_BG)
		{
			m_NKM_UI_UNIT_INFO_CONTROL_VOICE_BG.PointerClick.RemoveAllListeners();
			m_NKM_UI_UNIT_INFO_CONTROL_VOICE_BG.PointerClick.AddListener(OnClickSpeech);
		}
		if (null != m_NKM_UI_UNIT_INFO_CONTROL_APPRAISAL_BG)
		{
			m_NKM_UI_UNIT_INFO_CONTROL_APPRAISAL_BG.PointerClick.RemoveAllListeners();
			m_NKM_UI_UNIT_INFO_CONTROL_APPRAISAL_BG.PointerClick.AddListener(OnClickAppraisal);
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_UNIT_INFO_CONTROL_APPRAISAL_BG, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.UNIT_REVIEW_SYSTEM));
		if (null != NKM_UI_OPERATOR_INFO_ILLUST_CHANGE)
		{
			NKM_UI_OPERATOR_INFO_ILLUST_CHANGE.PointerClick.RemoveAllListeners();
			NKM_UI_OPERATOR_INFO_ILLUST_CHANGE.PointerClick.AddListener(OnClickChangeIllust);
		}
		NKCUtil.SetBindFunction(m_MainSkillBtn, OnClickSkillInfo);
		NKCUtil.SetBindFunction(m_SubSkillBtn, OnClickSkillInfo);
		NKCUtil.SetBindFunction(m_BUTTON_PLACENEMT, OnClickPlacement);
		NKCUtil.SetBindFunction(m_BUTTON_COLLECTION, OnClickCollection);
	}

	public void OnStart(List<MiscItemData> lstMaterials)
	{
		if (m_OperatorData != null)
		{
			NKCPacketSender.Send_NKMPacket_OPERATOR_LEVELUP_REQ(m_OperatorData.uid, lstMaterials);
		}
	}

	public override void OnBackButton()
	{
		if (m_State == TAB_STATE.LEVEL_UP)
		{
			UpdateState(TAB_STATE.INFO);
		}
		else if (m_bViewMode)
		{
			OnClickChangeIllust();
		}
		else
		{
			base.OnBackButton();
		}
	}

	public override void UnHide()
	{
		base.UnHide();
		if (m_State == TAB_STATE.LEVEL_UP)
		{
			UpdateMenuAni(TAB_STATE.LEVEL_UP, bForce: true);
		}
		else if (m_State == TAB_STATE.INFO)
		{
			m_bAppraisal = false;
			UpdateMenuAni(TAB_STATE.INFO, bForce: true);
		}
	}

	public void Open(NKMOperator data, OpenOption option = null)
	{
		m_OperatorData = data;
		bool bValue = false;
		if (option == null)
		{
			if (m_OpenOption == null)
			{
				m_OpenOption = new OpenOption(new List<long> { m_OperatorData.uid });
			}
			m_OpenOption.m_lstOperatorData.Add(data);
			if (m_OpenOption != null)
			{
				m_OpenOption.m_lstOperatorData.Add(data);
			}
		}
		else
		{
			m_OpenOption = option;
			bValue = m_OpenOption.m_lstOperatorData.Count > 1;
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATOR_INFO_ARROW, bValue);
		InitBanner();
		UpdateState(TAB_STATE.INFO);
		UpdateUnitInfo();
		CheckTutorial();
		UIOpened();
	}

	private void InitBanner()
	{
		if (m_OperatorData == null || !(m_DragCharacterView != null) || m_OpenOption == null)
		{
			return;
		}
		if (m_OpenOption.m_lstOperatorData.Count <= 0)
		{
			if (m_OpenOption.m_lstOperatorUID.Count <= 0)
			{
				m_OpenOption.m_lstOperatorUID.Add(m_OperatorData.uid);
			}
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (nKMUserData != null)
			{
				NKMArmyData armyData = nKMUserData.m_ArmyData;
				if (armyData != null)
				{
					for (int i = 0; i < m_OpenOption.m_lstOperatorUID.Count; i++)
					{
						NKMOperator operatorFromUId = armyData.GetOperatorFromUId(m_OpenOption.m_lstOperatorUID[i]);
						if (operatorFromUId != null)
						{
							m_OpenOption.m_lstOperatorData.Add(operatorFromUId);
						}
					}
				}
			}
		}
		for (int j = 0; j < m_OpenOption.m_lstOperatorData.Count; j++)
		{
			if (m_OpenOption.m_lstOperatorData[j].uid == m_OperatorData.uid)
			{
				m_DragCharacterView.TotalCount = m_OpenOption.m_lstOperatorData.Count;
				m_DragCharacterView.SetIndex(j);
				break;
			}
		}
	}

	private void UpdateUnitInfo()
	{
		if (m_OperatorData != null)
		{
			m_NKM_UI_OPERATOR_INFO_SUMMARY.SetData(m_OperatorData);
			m_NKM_UI_OPERATOR_INFO_POPUP_LEVELUP?.SetData(m_OperatorData);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_OperatorData.id);
			UpdateUnitStat();
			UpdateSkillInfo();
			UpdateLockState();
			if (unitTempletBase != null)
			{
				NKCUtil.SetLabelText(m_lbVoiceActor, NKCVoiceActorNameTemplet.FindActorName(unitTempletBase));
			}
			bool bValue = NKCCompanyBuffManager.IsCurrentApplyBuff(NKMConst.Buff.BuffType.OPERATOR_SKILL_ENHANCE_COST_DISCOUNT);
			NKCUtil.SetGameobjectActive(m_objEvent, bValue);
			if (NKCUIUnitPlacement.IsInstanceOpen)
			{
				NKCUIUnitPlacement.Instance.SetData(NKCUIUnitPlacement.UnitType.Operator, m_OperatorData.uid);
			}
		}
	}

	private void UpdateUnitLevelUp(bool bShowLevelUpFX)
	{
		if (m_OperatorData != null)
		{
			m_NKM_UI_OPERATOR_INFO_SUMMARY.SetData(m_OperatorData);
			m_NKM_UI_OPERATOR_INFO_POPUP_LEVELUP?.SetData(m_OperatorData, bShowLevelUpFX);
			UpdateUnitStat();
		}
	}

	private void UpdateUnitStat()
	{
		NKCUtil.SetLabelText(m_NKM_UI_OPERATOR_INFO_SUMMARY_ATTACK_TEXT, m_OperatorData.CalculateOperatorOperationPower().ToString("N0"));
		NKCUtil.SetLabelText(m_STAT_NAME_ATK, NKCOperatorUtil.GetStatPercentageString(m_OperatorData, NKM_STAT_TYPE.NST_ATK));
		NKCUtil.SetLabelText(m_STAT_NAME_DEF, NKCOperatorUtil.GetStatPercentageString(m_OperatorData, NKM_STAT_TYPE.NST_DEF));
		NKCUtil.SetLabelText(m_STAT_NAME_HP, NKCOperatorUtil.GetStatPercentageString(m_OperatorData, NKM_STAT_TYPE.NST_HP));
		NKCUtil.SetLabelText(m_STAT_NAME_SKILL_COOL, NKCOperatorUtil.GetStatPercentageString(m_OperatorData, NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE));
	}

	private void UpdateSkillInfo()
	{
		if (m_OperatorData != null)
		{
			m_MainSkill.SetData(m_OperatorData.mainSkill.id, m_OperatorData.mainSkill.level);
			m_MainSkillCombo.SetData(m_OperatorData.id);
			m_SubSkill.SetData(m_OperatorData.subSkill.id, m_OperatorData.subSkill.level);
			NKMTacticalCommandTemplet tacticalCommandTempletByID = NKMTacticalCommandManager.GetTacticalCommandTempletByID(m_OperatorData.mainSkill.id);
			if (tacticalCommandTempletByID != null)
			{
				NKCUtil.SetLabelText(m_lbSkillCool, string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_SECONDS"), (int)tacticalCommandTempletByID.m_fCoolTime));
			}
		}
	}

	private void UpdateState(TAB_STATE newState)
	{
		if (m_State != newState)
		{
			if (m_State == TAB_STATE.LEVEL_UP)
			{
				m_NKM_UI_OPERATOR_INFO_POPUP_LEVELUP?.Refresh();
			}
			UpdateMenuAni(newState);
			if (m_State == TAB_STATE.LEVEL_UP)
			{
				m_NKM_UI_OPERATOR_INFO_POPUP_LEVELUP?.ResetResourceIcon();
			}
			NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATOR_INFO_DESC_STAT, m_State == TAB_STATE.INFO);
			NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATOR_INFO_DESC_SKILL_01, m_State == TAB_STATE.INFO);
			NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATOR_INFO_DESC_SKILL_02, m_State == TAB_STATE.INFO);
			NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATOR_INFO_DESC_BUTTON, m_State == TAB_STATE.INFO);
			NKCUtil.SetGameobjectActive(m_LEVELUP, m_State == TAB_STATE.LEVEL_UP);
			NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATOR_INFO_CONTROL, m_State == TAB_STATE.INFO);
		}
	}

	private void UpdateMenuAni(TAB_STATE newState, bool bForce = false)
	{
		switch (newState)
		{
		case TAB_STATE.INFO:
			if (bForce)
			{
				m_NKM_UI_OPERATOR_INFO_ANI.SetTrigger(Ani_Base);
			}
			else if (m_State == TAB_STATE.NONE)
			{
				m_NKM_UI_OPERATOR_INFO_ANI.SetTrigger(Ani_Base);
			}
			else
			{
				m_NKM_UI_OPERATOR_INFO_ANI.SetTrigger(Ani_LvUpOut);
			}
			break;
		case TAB_STATE.LEVEL_UP:
			if (bForce)
			{
				m_NKM_UI_OPERATOR_INFO_ANI.SetTrigger(Ani_LvUp);
			}
			else
			{
				m_NKM_UI_OPERATOR_INFO_ANI.SetTrigger(Ani_LvUpIn);
			}
			break;
		}
		m_State = newState;
	}

	private void OnClickSkillUp()
	{
		NKCUIOperatorInfoPopupSkill.Instance.Open(m_OperatorData);
	}

	private void OnClickLevelUp()
	{
		UpdateState(TAB_STATE.LEVEL_UP);
	}

	private RectTransform MakeMainBannerListSlot()
	{
		GameObject obj = new GameObject("Banner", typeof(RectTransform), typeof(LayoutElement));
		LayoutElement component = obj.GetComponent<LayoutElement>();
		component.ignoreLayout = false;
		component.preferredWidth = m_DragCharacterView.m_rtContentRect.GetWidth();
		component.preferredHeight = m_DragCharacterView.m_rtContentRect.GetHeight();
		component.flexibleWidth = 2f;
		component.flexibleHeight = 2f;
		return obj.GetComponent<RectTransform>();
	}

	private void ProvideMainBannerListSlotData(Transform tr, int idx)
	{
		if (m_OpenOption == null || m_OpenOption.m_lstOperatorData == null || m_OpenOption.m_lstOperatorData.Count <= idx)
		{
			return;
		}
		NKMOperator nKMOperator = m_OpenOption.m_lstOperatorData[idx];
		if (nKMOperator != null)
		{
			NKCUICharacterView component = tr.GetComponent<NKCUICharacterView>();
			if (component != null)
			{
				component.SetCharacterIllust(nKMOperator);
				return;
			}
			NKCUICharacterView nKCUICharacterView = tr.gameObject.AddComponent<NKCUICharacterView>();
			nKCUICharacterView.m_rectIllustRoot = tr.GetComponent<RectTransform>();
			nKCUICharacterView.SetCharacterIllust(nKMOperator);
		}
	}

	private void ReturnMainBannerListSlot(Transform go)
	{
		NKCUtil.SetGameobjectActive(go, bValue: false);
		Object.Destroy(go.gameObject);
	}

	public void TouchCharacter(RectTransform rt, PointerEventData eventData)
	{
		if (m_DragCharacterView.GetDragOffset().IsNearlyZero())
		{
			NKCUICharacterView componentInChildren = rt.GetComponentInChildren<NKCUICharacterView>();
			if (componentInChildren != null)
			{
				componentInChildren.OnPointerDown(eventData);
				componentInChildren.OnPointerUp(eventData);
			}
		}
	}

	private void Focus(RectTransform rect, bool bFocus)
	{
		NKCUtil.SetGameobjectActive(rect.gameObject, bFocus);
	}

	private void FocusColor(RectTransform rect, Color ApplyColor)
	{
		NKCUICharacterView componentInChildren = rect.gameObject.GetComponentInChildren<NKCUICharacterView>();
		if (componentInChildren != null)
		{
			componentInChildren.SetColor(ApplyColor);
		}
	}

	public void SelectCharacter(int idx)
	{
		if (m_OpenOption.m_lstOperatorData.Count < idx || idx < 0)
		{
			Debug.LogWarning($"Error - Count : {m_OpenOption.m_lstOperatorData.Count}, Index : {idx}");
			return;
		}
		NKMOperator nKMOperator = m_OpenOption.m_lstOperatorData[idx];
		if (nKMOperator != null)
		{
			ChangeOperator(nKMOperator);
		}
	}

	private void BannerCleanUp()
	{
		if (!(m_DragCharacterView != null))
		{
			return;
		}
		NKCUICharacterView[] componentsInChildren = m_DragCharacterView.gameObject.GetComponentsInChildren<NKCUICharacterView>();
		if (componentsInChildren != null)
		{
			NKCUICharacterView[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].CloseImmediatelyIllust();
			}
		}
	}

	private void OnEventPanelClick(BaseEventData e)
	{
		if (!(m_DragCharacterView != null) || !m_DragCharacterView.GetDragOffset().IsNearlyZero())
		{
			return;
		}
		RectTransform currentItem = m_DragCharacterView.GetCurrentItem();
		if (currentItem != null)
		{
			NKCUICharacterView componentInChildren = currentItem.GetComponentInChildren<NKCUICharacterView>();
			if (componentInChildren != null)
			{
				PointerEventData eventData = new PointerEventData(EventSystem.current);
				componentInChildren.OnPointerDown(eventData);
				componentInChildren.OnPointerUp(eventData);
			}
		}
	}

	private void ChangeOperator(NKMOperator operatorData)
	{
		m_OperatorData = operatorData;
		UpdateUnitInfo();
	}

	public override void OnOperatorUpdate(NKMUserData.eChangeNotifyType eEventType, long uid, NKMOperator operatorData)
	{
		switch (eEventType)
		{
		case NKMUserData.eChangeNotifyType.Update:
			if (m_OperatorData != null && m_OperatorData.uid == uid)
			{
				m_OperatorData = operatorData;
			}
			if (m_State == TAB_STATE.LEVEL_UP)
			{
				if (m_OpenOption != null && m_OpenOption.m_lstOperatorData != null)
				{
					List<NKMOperator> lstOperatorData = m_OpenOption.m_lstOperatorData;
					for (int j = 0; j < lstOperatorData.Count; j++)
					{
						if (uid == lstOperatorData[j].uid)
						{
							m_OpenOption = new OpenOption(lstOperatorData, m_OpenOption.m_SelectSlotIndex);
							break;
						}
					}
				}
			}
			else
			{
				UpdateSkillInfo();
			}
			UpdateUnitLevelUp(m_State == TAB_STATE.LEVEL_UP);
			break;
		case NKMUserData.eChangeNotifyType.Remove:
		{
			int index = 0;
			for (int i = 0; i < m_OpenOption.m_lstOperatorData.Count; i++)
			{
				if (m_OpenOption.m_lstOperatorData[i].uid == uid)
				{
					m_OpenOption.m_lstOperatorData.RemoveAt(i);
					i--;
				}
				else if (m_OpenOption.m_lstOperatorData[i].uid == m_OperatorData.uid)
				{
					index = i;
				}
			}
			m_DragCharacterView.TotalCount = m_OpenOption.m_lstOperatorData.Count;
			m_DragCharacterView.SetIndex(index);
			NKCUtil.SetGameobjectActive(m_NKM_UI_OPERATOR_INFO_ARROW, m_OpenOption.m_lstOperatorData.Count > 1);
			break;
		}
		}
	}

	public void UpdateLockState(long operatorUID)
	{
		if (m_OperatorData == null)
		{
			return;
		}
		if (m_OpenOption != null && m_OpenOption.m_lstOperatorData != null)
		{
			foreach (NKMOperator lstOperatorDatum in m_OpenOption.m_lstOperatorData)
			{
				if (lstOperatorDatum.uid == operatorUID)
				{
					lstOperatorDatum.bLock = m_OperatorData.bLock;
					break;
				}
			}
		}
		if (m_OperatorData.uid == operatorUID)
		{
			UpdateLockState();
		}
	}

	private void UpdateLockState()
	{
		if (m_OperatorData != null)
		{
			bool bSelect = NKCOperatorUtil.IsLock(m_OperatorData.uid);
			m_NKM_UI_OPERATOR_INFO_UNIT_LOCK.Select(bSelect, bForce: true);
		}
	}

	private void OnClickLock(bool bSet)
	{
		NKCPacketSender.Send_NKMPacket_OPERATOR_LOCK_REQ(m_OperatorData.uid, bSet);
	}

	private void OnClickSkillInfo()
	{
		NKCUIOperatorPopUpSkill.Instance.Open(m_OperatorData.uid);
	}

	private void OnClickSpeech()
	{
		NKCUIPopupVoice.Instance.Open(m_OperatorData.id, 0, bLifetime: false);
	}

	private void OnClickAppraisal()
	{
		if (!m_bViewMode)
		{
			NKCUIUnitReview.Instance.OpenUI(m_OperatorData.id);
			m_bAppraisal = true;
		}
	}

	private void OnClickChangeIllust()
	{
		if (!m_bAppraisal)
		{
			NKCUIPopupIllustView.Instance.Open(m_OperatorData);
		}
	}

	private void OnClickPlacement()
	{
		if (NKCUIUnitPlacement.IsInstanceOpen)
		{
			NKCUIUnitPlacement.Instance.Close();
		}
		else
		{
			NKCUIUnitPlacement.Instance.Open(NKCUIUnitPlacement.UnitType.Operator, m_OperatorData.uid);
		}
	}

	private void OnClickCollection()
	{
		NKMOperator dummyOperator = NKCOperatorUtil.GetDummyOperator(m_OperatorData.id, bSetMaximum: true);
		if (dummyOperator != null)
		{
			NKCUICollectionOperatorInfoV2.CheckInstanceAndOpen(dummyOperator, null);
		}
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		m_NKM_UI_OPERATOR_INFO_POPUP_LEVELUP.OnInventoryChange(itemData);
	}

	public void CheckTutorial()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.OperatorEnhance);
	}
}
