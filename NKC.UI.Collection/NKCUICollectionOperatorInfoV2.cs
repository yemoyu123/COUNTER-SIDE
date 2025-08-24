using System.Collections.Generic;
using Cs.Protocol;
using NKC.Templet;
using NKC.UI.Component;
using NKM;
using NKM.Templet;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionOperatorInfoV2 : NKCUIBase
{
	public enum eCollectionState
	{
		CS_NONE,
		CS_PROFILE,
		CS_INFOMATION,
		CS_VOICE,
		CS_STATUS
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_collection";

	private const string UI_ASSET_NAME = "AB_UI_COLLECTION_INFO";

	private const string UI_ASSET_NAME_OTHER = "NKM_UI_COLLECTION_UNIT_INFO_OTHER";

	private static bool m_bWillCloseUnderPopupOnOpen = true;

	private static NKCUICollectionOperatorInfoV2 m_Instance;

	private static NKCUIManager.LoadedUIData m_loadedUIData;

	private NKCUIUpsideMenu.eMode m_UpsideMenuMode = NKCUIUpsideMenu.eMode.Normal;

	private NKMOperator m_Operator;

	public GameObject m_objUnitIdRoot;

	public Text m_lbUnitId;

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffdâ")]
	public NKCUIOperatorSummary m_NKCUIOperInfoSummary;

	public NKCUICharInfoSummary m_NKCCharInfoSummary;

	public NKCUIShipInfoSummary m_NKCUIShipInfoSummary;

	[Header("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffdâ")]
	public GameObject m_objEquipSlotParent;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public ScrollRect m_srUnitIntroduce;

	public Text m_NKM_UI_COLLECTION_UNIT_PROFILE_UNIT_INTRODUCE_TEXT;

	[Header("\ufffdɷ\ufffdġ")]
	public GameObject m_objUnitStatsList;

	public GameObject m_objShipStatsList;

	public GameObject m_objOperatorStatsList;

	[Space]
	public TMP_Text m_lbUnitPowerText;

	public NKCUIUnitStatSlot m_slotHP;

	public NKCUIUnitStatSlot m_slotAttack;

	public NKCUIUnitStatSlot m_slotDefense;

	public NKCUIUnitStatSlot m_slotCoolTime;

	[Header("\ufffd\ufffd")]
	public NKCUICollectionInfoTab m_UnitProfileTab;

	public NKCUICollectionUnitProfile m_UnitProfile;

	public NKCUICollectionInfoTab m_UnitInformationTab;

	public NKCUICollectionUnitDesc m_UnitInfomation;

	public NKCUICollectionInfoTab m_UnitVoiceTab;

	public NKCUICollectionUnitVoice m_UnitVoice;

	public NKCUICollectionInfoTab m_UnitStatusTab;

	public GameObject m_objUnitStatus;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffdϷ\ufffd\ufffd\ufffdƮ")]
	public NKCUICharacterView m_CharacterView;

	[Header("\ufffdϷ\ufffd\ufffd\ufffdƮ \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd忡\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\u0334\ufffd Rect\ufffd\ufffd. Base/ViewMode \ufffd\ufffd \ufffd\u0338\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public Animator m_ani_NKM_UI_COLLECTION_UNIT_INFO_CONTENT;

	[Header("\ufffd\ufffdŸ \ufffd\ufffdư")]
	public NKCUIComStateButton m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_SKIN;

	public NKCUIComStateButton m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_PRACTICE;

	public NKCUIComStateButton m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_APPRAISAL;

	public NKCUIComStateButton m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_ILLUST_CHANGE;

	public NKCUIComStateButton m_NKM_UI_COLLECTION_UNIT_INFO_LOYALTY;

	public NKCUIComStateButton m_NKM_UI_COLLECTION_UNIT_INFO_ACHIEVEMENT;

	public NKCUIComToggle m_tglUnitInfoDetailPopup;

	[Space]
	public GameObject m_NKM_UI_COLLECTION_UNIT_INFO_NOTICE_NOTGET;

	[Header("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public NKCUIComStateButton m_NKM_UI_COLLECTION_UNIT_ACHIEVEMENT_BUTTON;

	public GameObject m_NKM_UI_COLLECTION_UNIT_ACHIEVEMENT_BUTTON_NEW;

	public GameObject m_UNIT_ACHIEVEMENT_COMPLETE;

	[Header("\ufffd\ufffdų \ufffdг\ufffd")]
	public NKCUIOperatorSkill m_OperatorSkill;

	public NKCUIOperatorTacticalSkillCombo m_OperatorTacticalSkillCombo;

	[Header("\ufffd\ufffd\ufffd\ufffd")]
	public GameObject m_objVoiceActor;

	public Text m_lbVoiceActorName;

	[Header("ĳ\ufffd\ufffd\ufffd\ufffd \ufffdǳ\ufffd")]
	public NKCUIComDragSelectablePanel m_DragCharacterView;

	public EventTrigger m_evtPanel;

	private eCollectionState m_eCurrentState = eCollectionState.CS_INFOMATION;

	private NKCUIOperatorInfo.OpenOption m_OpenOption;

	private static NKCUIOperatorInfo.OpenOption m_OpenOptionReserved = null;

	private bool m_bViewMode;

	[Header("SDĳ\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd")]
	public RectTransform m_rtSDRoot;

	private NKCASUIUnitIllust m_spineSD;

	public float m_fSDScale = 1.2f;

	private int m_CurUnitID = -1;

	public override bool WillCloseUnderPopupOnOpen => m_bWillCloseUnderPopupOnOpen;

	public static NKCUICollectionOperatorInfoV2 Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_loadedUIData = NKCUIManager.OpenNewInstance<NKCUICollectionOperatorInfoV2>("ab_ui_collection", "AB_UI_COLLECTION_INFO", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
				m_Instance = m_loadedUIData.GetInstance<NKCUICollectionOperatorInfoV2>();
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

	public override string MenuName => NKCUtilString.GET_STRING_UNIT_INFO;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => m_UpsideMenuMode;

	private static void CleanupInstance()
	{
		m_Instance = null;
	}

	public static void CheckInstanceAndClose()
	{
		if (m_loadedUIData != null)
		{
			m_loadedUIData.CloseInstance();
			m_loadedUIData = null;
		}
	}

	private void OnDestroy()
	{
		CheckInstanceAndClose();
	}

	public void Init()
	{
		InitUI();
	}

	public void InitUI()
	{
		NKCUtil.SetButtonClickDelegate(m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_ILLUST_CHANGE, OnClickChangeIllust);
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_SKIN, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_PRACTICE, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_APPRAISAL, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_ILLUST_CHANGE, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_LOYALTY, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_ACHIEVEMENT, bValue: false);
		NKCUtil.SetGameobjectActive(m_tglUnitInfoDetailPopup, bValue: false);
		NKCUtil.SetToggleValueChangedDelegate(m_tglUnitInfoDetailPopup, OpenUnitInfoDetailPopup);
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
		if (null != m_UnitProfileTab)
		{
			NKCUtil.SetToggleValueChangedDelegate(m_UnitProfileTab.m_tglToggle, delegate
			{
				ChangeState(eCollectionState.CS_PROFILE);
			});
		}
		if (null != m_UnitInformationTab)
		{
			NKCUtil.SetToggleValueChangedDelegate(m_UnitInformationTab.m_tglToggle, delegate
			{
				ChangeState(eCollectionState.CS_INFOMATION);
			});
		}
		if (null != m_UnitVoiceTab)
		{
			NKCUtil.SetToggleValueChangedDelegate(m_UnitVoiceTab.m_tglToggle, delegate
			{
				ChangeState(eCollectionState.CS_VOICE);
			});
		}
		if (null != m_UnitStatusTab)
		{
			NKCUtil.SetToggleValueChangedDelegate(m_UnitStatusTab.m_tglToggle, delegate
			{
				ChangeState(eCollectionState.CS_STATUS);
			});
		}
		NKCUtil.SetButtonClickDelegate(m_NKM_UI_COLLECTION_UNIT_ACHIEVEMENT_BUTTON, OnClickUnitAchievement);
		m_UnitProfile?.Init();
		m_UnitInfomation?.Init();
		m_UnitVoice?.InitUI();
		base.gameObject.SetActive(value: false);
	}

	private void OnEventPanelClick(BaseEventData e)
	{
		if (!(m_DragCharacterView != null) || m_DragCharacterView.GetDragOffset() != 0f)
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

	private void ChangeOperator(NKMOperator cOperatorData)
	{
		m_NKCUIOperInfoSummary.SetData(cOperatorData);
		SetData(cOperatorData);
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
		if (m_OpenOption == null || m_OpenOption.m_lstOperatorData == null)
		{
			return;
		}
		NKMOperator nKMOperator = m_OpenOption.m_lstOperatorData[idx];
		Debug.Log($"<color=yellow>target : {tr.name}, idx : {idx}, </color>");
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
		if (m_DragCharacterView.GetDragOffset() == 0f)
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

	public void SelectCharacter(int idx)
	{
		if (m_OpenOption.m_lstOperatorData.Count >= idx && idx >= 0)
		{
			NKMOperator nKMOperator = m_OpenOption.m_lstOperatorData[idx];
			if (nKMOperator != null)
			{
				ChangeOperator(nKMOperator);
			}
		}
	}

	private void BannerCleanUp()
	{
		if (m_DragCharacterView != null)
		{
			NKCUICharacterView[] componentsInChildren = m_DragCharacterView.gameObject.GetComponentsInChildren<NKCUICharacterView>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].CloseImmediatelyIllust();
			}
		}
	}

	public static void CheckInstanceAndOpen(NKMOperator cOperatorData, NKCUIOperatorInfo.OpenOption openOption, eCollectionState eStartingState = eCollectionState.CS_PROFILE, NKCUIUpsideMenu.eMode upsideMenuMode = NKCUIUpsideMenu.eMode.Normal, bool bWillCloseUnderPopupOnOpen = true)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(cOperatorData.id);
		if (unitTempletBase == null || unitTempletBase.IsUnitDescNullOrEmplty())
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_GAUNTLET_EVENT_UNIT_DETAIL_INFO_NOT_POSSIBLE);
		}
		else
		{
			Instance.Open(cOperatorData, openOption, eStartingState, upsideMenuMode, bWillCloseUnderPopupOnOpen);
		}
	}

	private void Open(NKMOperator cOperatorData, NKCUIOperatorInfo.OpenOption openOption, eCollectionState eStartingState = eCollectionState.CS_PROFILE, NKCUIUpsideMenu.eMode upsideMenuMode = NKCUIUpsideMenu.eMode.Normal, bool bWillCloseUnderPopupOnOpen = true)
	{
		m_eCurrentState = eCollectionState.CS_NONE;
		m_UpsideMenuMode = upsideMenuMode;
		m_bWillCloseUnderPopupOnOpen = bWillCloseUnderPopupOnOpen;
		m_OpenOption = openOption;
		if (m_OpenOption == null)
		{
			if (m_OpenOptionReserved != null)
			{
				m_OpenOption = m_OpenOptionReserved;
			}
			else
			{
				m_OpenOption = new NKCUIOperatorInfo.OpenOption(new List<long>());
				m_OpenOption.m_lstOperatorData.Add(cOperatorData);
			}
		}
		m_OpenOptionReserved = null;
		if (m_DragCharacterView != null)
		{
			if (m_OpenOption.m_lstOperatorData.Count == 0)
			{
				m_OpenOption.m_lstOperatorData.Add(cOperatorData);
			}
			m_DragCharacterView.TotalCount = m_OpenOption.m_lstOperatorData.Count;
			for (int i = 0; i < m_OpenOption.m_lstOperatorData.Count; i++)
			{
				if (m_OpenOption.m_lstOperatorData[i].id == cOperatorData.id)
				{
					m_DragCharacterView.SetIndex(i);
					break;
				}
			}
		}
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		SetData(cOperatorData);
		int num = 0;
		bool flag = true;
		bool flag2 = true;
		bool flag3 = true;
		bool flag4 = true;
		NKCUtil.SetGameobjectActive(m_UnitProfileTab, flag);
		NKCUtil.SetGameobjectActive(m_UnitInformationTab, flag2);
		NKCUtil.SetGameobjectActive(m_UnitVoiceTab, flag3);
		NKCUtil.SetGameobjectActive(m_UnitStatusTab, flag4);
		if (flag)
		{
			m_UnitProfileTab.SetTabNumber(++num);
		}
		if (flag2)
		{
			m_UnitInformationTab.SetTabNumber(++num);
		}
		if (flag3)
		{
			m_UnitVoiceTab.SetTabNumber(++num);
		}
		if (flag4)
		{
			m_UnitStatusTab.SetTabNumber(++num);
		}
		m_UnitProfileTab.Select(select: false, force: true);
		m_UnitInformationTab.Select(select: false, force: true);
		m_UnitVoiceTab.Select(select: false, force: true);
		m_UnitStatusTab.Select(select: false, force: true);
		switch (eStartingState)
		{
		case eCollectionState.CS_PROFILE:
			m_UnitProfileTab.Select(select: true);
			break;
		case eCollectionState.CS_INFOMATION:
			m_UnitInformationTab.Select(select: true);
			break;
		case eCollectionState.CS_VOICE:
			m_UnitVoiceTab.Select(select: true);
			break;
		case eCollectionState.CS_STATUS:
			m_UnitStatusTab.Select(select: true);
			break;
		}
		NKCUtil.SetGameobjectActive(m_NKCCharInfoSummary.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKCUIShipInfoSummary.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_NKCUIOperInfoSummary.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_objUnitStatsList, bValue: false);
		NKCUtil.SetGameobjectActive(m_objShipStatsList, bValue: false);
		NKCUtil.SetGameobjectActive(m_objOperatorStatsList, bValue: true);
		UIOpened();
	}

	private void ChangeState(eCollectionState newStat)
	{
		if (m_eCurrentState != newStat && !m_bViewMode)
		{
			m_eCurrentState = newStat;
			UpdateUI();
		}
	}

	private void UpdateUI()
	{
		switch (m_eCurrentState)
		{
		case eCollectionState.CS_PROFILE:
			NKCUtil.SetGameobjectActive(m_UnitProfile, bValue: true);
			NKCUtil.SetGameobjectActive(m_UnitInfomation, bValue: false);
			NKCUtil.SetGameobjectActive(m_UnitVoice, bValue: false);
			NKCUtil.SetGameobjectActive(m_objUnitStatus, bValue: false);
			m_UnitProfile?.ResetCRFGauge();
			m_UnitProfile?.SetData(m_Operator.id);
			break;
		case eCollectionState.CS_INFOMATION:
			NKCUtil.SetGameobjectActive(m_UnitProfile, bValue: false);
			NKCUtil.SetGameobjectActive(m_UnitInfomation, bValue: true);
			NKCUtil.SetGameobjectActive(m_UnitVoice, bValue: false);
			NKCUtil.SetGameobjectActive(m_objUnitStatus, bValue: false);
			m_UnitInfomation?.SetData(m_Operator);
			break;
		case eCollectionState.CS_VOICE:
			NKCUtil.SetGameobjectActive(m_UnitProfile, bValue: false);
			NKCUtil.SetGameobjectActive(m_UnitInfomation, bValue: false);
			NKCUtil.SetGameobjectActive(m_UnitVoice, bValue: true);
			NKCUtil.SetGameobjectActive(m_objUnitStatus, bValue: false);
			m_UnitVoice?.SetData(m_Operator);
			break;
		case eCollectionState.CS_STATUS:
			NKCUtil.SetGameobjectActive(m_UnitProfile, bValue: false);
			NKCUtil.SetGameobjectActive(m_UnitInfomation, bValue: false);
			NKCUtil.SetGameobjectActive(m_UnitVoice, bValue: false);
			NKCUtil.SetGameobjectActive(m_objUnitStatus, bValue: true);
			break;
		}
	}

	private void SetData(NKMOperator operData)
	{
		NKCCollectionUnitTemplet unitTemplet = NKCCollectionManager.GetUnitTemplet(operData.id);
		NKCUtil.SetGameobjectActive(m_objUnitIdRoot, unitTemplet != null && !unitTemplet.m_bExclude);
		string employeeNumber = NKCCollectionManager.GetEmployeeNumber(operData.id);
		NKCUtil.SetLabelText(m_lbUnitId, employeeNumber);
		m_OperatorSkill.SetData(operData.mainSkill.id, operData.mainSkill.level);
		m_OperatorTacticalSkillCombo.SetData(operData.id);
		if (m_Operator == null || m_Operator.id != operData.id)
		{
			if (m_Operator == null)
			{
				m_Operator = new NKMOperator();
			}
			m_Operator.DeepCopyFrom(operData);
			UpdateOperatorDiscription();
			SetDetailedStat(m_Operator);
			m_NKCUIOperInfoSummary.SetData(operData);
			CheckHasUnit(m_Operator.id);
			SetVoiceData();
			NKCUtil.SetGameobjectActive(m_objEquipSlotParent, bValue: false);
			UpdateVoiceActorName();
		}
	}

	private void UpdateVoiceActorName()
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_Operator.id);
		NKCUtil.SetLabelText(m_lbVoiceActorName, NKCVoiceActorNameTemplet.FindActorName(unitTempletBase));
	}

	private void CheckHasUnit(int iUnitID)
	{
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_NOTICE_NOTGET, armyData.IsFirstGetUnit(iUnitID));
	}

	private void UpdateOperatorDiscription()
	{
		if (m_eCurrentState == eCollectionState.CS_PROFILE)
		{
			m_UnitProfile?.SetData(m_Operator.id);
		}
		else if (m_eCurrentState == eCollectionState.CS_INFOMATION)
		{
			m_UnitInfomation?.SetData(m_Operator);
		}
	}

	private void SetVoiceData()
	{
		if (m_eCurrentState == eCollectionState.CS_VOICE)
		{
			m_UnitVoice?.SetData(m_Operator);
		}
	}

	private void SetDetailedStat(NKMOperator operData)
	{
		if (operData == null)
		{
			NKMOperator nKMOperator = new NKMOperator();
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(operData.id);
			if (unitTempletBase != null)
			{
				nKMOperator.id = operData.id;
				NKMOperatorSkillTemplet skillTemplet = NKCOperatorUtil.GetSkillTemplet(unitTempletBase.m_lstSkillStrID[0]);
				if (skillTemplet != null)
				{
					NKMOperatorSkill nKMOperatorSkill = new NKMOperatorSkill();
					nKMOperatorSkill.id = skillTemplet.m_OperSkillID;
					nKMOperatorSkill.level = (byte)skillTemplet.m_MaxSkillLevel;
					nKMOperator.mainSkill = nKMOperatorSkill;
				}
				NKMOperatorRandomPassiveGroupTemplet nKMOperatorRandomPassiveGroupTemplet = NKMOperatorRandomPassiveGroupTemplet.Find(NKCOperatorUtil.GetPassiveGroupID(operData.id));
				if (nKMOperatorRandomPassiveGroupTemplet != null && nKMOperatorRandomPassiveGroupTemplet.Groups.Count > 0)
				{
					NKMOperatorSkillTemplet skillTemplet2 = NKCOperatorUtil.GetSkillTemplet(nKMOperatorRandomPassiveGroupTemplet.Groups[0].operSkillId);
					if (skillTemplet2 != null)
					{
						NKMOperatorSkill nKMOperatorSkill2 = new NKMOperatorSkill();
						nKMOperatorSkill2.id = skillTemplet2.m_OperSkillID;
						nKMOperatorSkill2.level = (byte)skillTemplet2.m_MaxSkillLevel;
						nKMOperator.subSkill = nKMOperatorSkill2;
					}
				}
			}
			NKCUtil.SetLabelText(m_lbUnitPowerText, CalculateOperatorOperationPower(operData).ToString());
		}
		else
		{
			NKCUtil.SetLabelText(m_lbUnitPowerText, CalculateOperatorOperationPower(operData).ToString());
		}
		m_slotHP.SetStatString(NKCOperatorUtil.GetStatPercentageString(operData, NKM_STAT_TYPE.NST_HP) ?? "");
		m_slotAttack.SetStatString(NKCOperatorUtil.GetStatPercentageString(operData, NKM_STAT_TYPE.NST_ATK) ?? "");
		m_slotDefense.SetStatString(NKCOperatorUtil.GetStatPercentageString(operData, NKM_STAT_TYPE.NST_DEF) ?? "");
		m_slotCoolTime.SetStatString(NKCOperatorUtil.GetStatPercentageString(operData, NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE) ?? "");
	}

	private int CalculateOperatorOperationPower(NKMOperator operatorData)
	{
		int result = 0;
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(operatorData.id);
		if (unitTempletBase != null && operatorData != null)
		{
			NKMOperatorSkillTemplet skillTemplet = NKCOperatorUtil.GetSkillTemplet(operatorData.mainSkill.id);
			NKMOperatorSkillTemplet skillTemplet2 = NKCOperatorUtil.GetSkillTemplet(operatorData.subSkill.id);
			float num = ((skillTemplet != null) ? ((float)(int)operatorData.mainSkill.level / (float)skillTemplet.m_MaxSkillLevel * 3000f) : 0f);
			float num2 = ((skillTemplet2 != null) ? ((float)(int)operatorData.subSkill.level / (float)skillTemplet2.m_MaxSkillLevel * 3000f) : 0f);
			float num3 = (float)operatorData.level / (float)NKMCommonConst.OperatorConstTemplet.unitMaximumLevel * 3000f;
			float num4 = 3000f;
			switch (unitTempletBase.m_NKM_UNIT_GRADE)
			{
			case NKM_UNIT_GRADE.NUG_SR:
				num4 *= 0.6f;
				break;
			case NKM_UNIT_GRADE.NUG_R:
				num4 *= 0.3f;
				break;
			case NKM_UNIT_GRADE.NUG_N:
				num4 *= 0.1f;
				break;
			}
			result = (int)(num + num2 + num3 + num4 + 0.5f);
		}
		return result;
	}

	public override void OnBackButton()
	{
		if (m_bViewMode)
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
	}

	public override void CloseInternal()
	{
		BannerCleanUp();
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
		if (m_spineSD != null)
		{
			m_spineSD.Unload();
			m_spineSD = null;
		}
		NKCPopupUnitInfoDetail.CheckInstanceAndClose();
		NKCUIPopupIllustView.CheckInstanceAndClose();
		NKCUIPopupCollectionAchievement.CheckInstanceAndClose();
		m_UnitProfile?.ResetCRFGauge();
		m_Operator = null;
	}

	private void OnClickChangeIllust()
	{
		NKCUIPopupIllustView.Instance.Open(m_Operator);
	}

	private void OpenSDIllust(NKMUnitData unitData, int skinID)
	{
		if (unitData == null)
		{
			NKCUtil.SetGameobjectActive(m_rtSDRoot, bValue: false);
			return;
		}
		if (m_spineSD != null)
		{
			m_spineSD.Unload();
			m_spineSD = null;
		}
		m_spineSD = NKCResourceUtility.OpenSpineSD(unitData.m_UnitID, skinID);
		if (m_spineSD != null)
		{
			m_spineSD.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE);
			m_spineSD.SetAnimation(NKCASUIUnitIllust.eAnimation.SD_IDLE, loop: true);
			m_spineSD.SetParent(m_rtSDRoot, worldPositionStays: false);
			RectTransform rectTransform = m_spineSD.GetRectTransform();
			if (rectTransform != null)
			{
				rectTransform.localPosition = Vector3.zero;
				rectTransform.localScale = Vector3.one * m_fSDScale;
			}
			NKCUtil.SetGameobjectActive(m_rtSDRoot, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_rtSDRoot, bValue: false);
		}
	}

	private void SetLifetimeButtonUI(int unitID)
	{
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		bool bValue = false;
		if (armyData.IsCollectedUnit(unitID))
		{
			bValue = armyData.SearchUnitByID(NKM_UNIT_TYPE.NUT_NORMAL, unitID, NKMArmyData.UNIT_SEARCH_OPTION.Devotion, 0);
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_LOYALTY, bValue);
	}

	public void OpenUnitInfoDetailPopup(bool value)
	{
	}

	private bool IsSeizedUnit(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return true;
		}
		if (unitData.IsSeized)
		{
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_UNIT_IS_SEIZED);
			return true;
		}
		return false;
	}

	private void OnClickUnitAchievement()
	{
	}
}
