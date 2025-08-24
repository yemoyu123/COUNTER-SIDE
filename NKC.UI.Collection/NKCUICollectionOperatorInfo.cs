using System.Collections.Generic;
using NKC.UI.Component;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionOperatorInfo : NKCUIBase
{
	public enum eCollectionState
	{
		CS_NONE,
		CS_PROFILE,
		CS_STATUS
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_collection";

	private const string UI_ASSET_NAME = "NKM_UI_COLLECTION_OPERATOR_INFO";

	private static NKCUICollectionOperatorInfo m_Instance;

	private static NKCUIManager.LoadedUIData m_loadedUIData;

	private NKCUIUpsideMenu.eMode m_UpsideMenuMode = NKCUIUpsideMenu.eMode.Normal;

	private NKMOperator m_NKMOperator;

	private NKMUnitTempletBase m_NKMUnitTempletBase;

	public NKCUIOperatorSummary m_NKCUIOperatorSummary;

	[Header("상세정보창")]
	public Text m_lbPower;

	public Text m_STAT_NUMBER_HP;

	public Text m_STAT_NUMBER_ATK;

	public Text m_STAT_NUMBER_DEF;

	public Text m_STAT_NUMBER_SKILL_COOL;

	public ScrollRect m_srUnitIntroduce;

	public Text m_NKM_UI_COLLECTION_UNIT_PROFILE_UNIT_INTRODUCE_TEXT;

	[Header("탭")]
	public NKCUIComStateButton m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_PROFILE;

	public GameObject m_NKM_UI_COLLECTION_UNIT_PROFILE;

	public NKCUIComStateButton m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_STAT;

	public GameObject m_NKM_UI_COLLECTION_UNIT_STAT;

	[Header("유닛 일러스트")]
	public NKCUICharacterView m_CharacterView;

	[Header("일러스트 보기 모드에서 움직이는 Rect들. Base/ViewMode 두 이름으로 지정")]
	public Animator m_ani_NKM_UI_COLLECTION_UNIT_INFO_CONTENT;

	[Header("기타 버튼")]
	public NKCUIComStateButton m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_APPRAISAL;

	public NKCUIComStateButton m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_ILLUST_CHANGE;

	public NKCUIComStateButton m_NKM_UI_COLLECTION_UNIT_PROFILE_VOICE_BUTTON;

	[Space]
	public GameObject m_NKM_UI_COLLECTION_UNIT_INFO_NOTICE_NOTGET;

	public GameObject m_NKM_UI_COLLECTION_UNIT_PROFILE_VOICE_BUTTON_DISABLE;

	[Header("스킬 패널")]
	public NKCUIOperatorSkill m_MainSkill;

	public NKCUIOperatorSkill m_SubSkill;

	public NKCUIOperatorTacticalSkillCombo m_MainSkillCombo;

	public NKCUIComStateButton m_SkillBtn;

	public Text m_lbSkillCoolTime;

	private eCollectionState m_eCurrentState;

	[Header("캐릭터 판넬")]
	public NKCUIComDragSelectablePanel m_DragCharacterView;

	public EventTrigger m_evtPanel;

	[Header("성우")]
	public Text m_lbVoiceActorName;

	private bool m_isGauntlet;

	private NKCUIOperatorInfo.OpenOption m_OpenOption;

	private bool m_bAppraisal;

	private bool m_bViewMode;

	public static NKCUICollectionOperatorInfo Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_loadedUIData = NKCUIManager.OpenNewInstance<NKCUICollectionOperatorInfo>("ab_ui_nkm_ui_collection", "NKM_UI_COLLECTION_OPERATOR_INFO", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
				m_Instance = m_loadedUIData.GetInstance<NKCUICollectionOperatorInfo>();
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

	public override bool WillCloseUnderPopupOnOpen => false;

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
		}
	}

	private void OnDestroy()
	{
	}

	public void Init()
	{
		InitUI();
	}

	public void InitUI()
	{
		NKCUtil.SetBindFunction(m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_ILLUST_CHANGE, OnClickChangeIllust);
		NKCUtil.SetBindFunction(m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_APPRAISAL, OnUnitAppraisal);
		NKCUtil.SetBindFunction(m_NKM_UI_COLLECTION_UNIT_PROFILE_VOICE_BUTTON, OnUnitVoice);
		NKCUtil.SetBindFunction(m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_PROFILE, delegate
		{
			ChangeState(eCollectionState.CS_PROFILE);
		});
		NKCUtil.SetBindFunction(m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_STAT, delegate
		{
			ChangeState(eCollectionState.CS_STATUS);
		});
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
		NKCUtil.SetBindFunction(m_SkillBtn, OnClickSkillInfo);
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

	private void ChangeUnit(NKMOperator operatorData)
	{
		m_NKCUIOperatorSummary.SetData(operatorData);
		SetData(operatorData);
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
		if (m_OpenOption.m_lstOperatorData.Count >= idx && idx >= 0)
		{
			NKMOperator nKMOperator = m_OpenOption.m_lstOperatorData[idx];
			if (nKMOperator != null)
			{
				ChangeUnit(nKMOperator);
			}
		}
	}

	private void BannerCleanUp()
	{
		if (m_DragCharacterView != null)
		{
			NKCUICharacterView[] componentsInChildren = m_DragCharacterView.gameObject.GetComponentsInChildren<NKCUICharacterView>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].CloseImmediatelyIllust();
			}
		}
	}

	public void Open(NKMOperator operatorData, NKCUIOperatorInfo.OpenOption openOption = null, eCollectionState startingState = eCollectionState.CS_PROFILE, NKCUIUpsideMenu.eMode upsideMenuMode = NKCUIUpsideMenu.eMode.Normal, bool isGauntlet = false, bool bRecord = false)
	{
		bool bForceUpdate = false;
		m_isGauntlet = isGauntlet;
		if (m_isGauntlet)
		{
			bForceUpdate = true;
		}
		m_eCurrentState = eCollectionState.CS_NONE;
		m_UpsideMenuMode = upsideMenuMode;
		SetData(operatorData, bForceUpdate);
		SetData(operatorData, bRecord);
		ChangeState(startingState);
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		if (openOption == null)
		{
			openOption = new NKCUIOperatorInfo.OpenOption(new List<long>());
			openOption.m_lstOperatorData.Add(operatorData);
		}
		m_OpenOption = openOption;
		if (m_DragCharacterView != null)
		{
			if (m_OpenOption.m_lstOperatorData.Count == 0)
			{
				m_OpenOption.m_lstOperatorData.Add(operatorData);
			}
			m_DragCharacterView.TotalCount = m_OpenOption.m_lstOperatorData.Count;
			for (int i = 0; i < m_OpenOption.m_lstOperatorData.Count; i++)
			{
				if (m_OpenOption.m_lstOperatorData[i].uid == operatorData.uid)
				{
					m_DragCharacterView.SetIndex(i);
					break;
				}
			}
		}
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
			NKCUtil.SetGameobjectActive(m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_PROFILE.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_STAT.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_PROFILE, bValue: true);
			m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_PROFILE.Select(bSelect: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_STAT, bValue: false);
			m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_STAT.Select(bSelect: false);
			break;
		case eCollectionState.CS_STATUS:
			NKCUtil.SetGameobjectActive(m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_PROFILE.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_STAT.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_PROFILE, bValue: false);
			m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_PROFILE.Select(bSelect: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_STAT, bValue: true);
			m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_STAT.Select(bSelect: true);
			break;
		}
	}

	private void SetData(NKMOperator cNKMOperator, bool bForceUpdate = false, bool bRecord = false)
	{
		if (cNKMOperator == null || m_NKMOperator == null || cNKMOperator.uid != m_NKMOperator.uid || bForceUpdate)
		{
			m_NKMOperator = cNKMOperator;
			UpdateSkillInfo();
			SetUnitDiscription(m_NKMOperator.id);
			SetDetailedStat(m_NKMOperator);
			m_NKCUIOperatorSummary.SetData(m_NKMOperator);
			if (m_isGauntlet)
			{
				m_NKCUIOperatorSummary.ShowLevelExpGauge(bShow: false);
			}
			CheckHasUnit(m_NKMOperator.id);
			SetVoiceButtonUI();
			if (bRecord)
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_PROFILE_VOICE_BUTTON, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_PROFILE_VOICE_BUTTON_DISABLE, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_APPRAISAL, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_NOTICE_NOTGET, bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_APPRAISAL, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.UNIT_REVIEW_SYSTEM));
			}
			NKCUtil.SetLabelText(m_lbVoiceActorName, NKCCollectionManager.GetVoiceActorName(m_NKMOperator.id));
		}
	}

	private void CheckHasUnit(int iUnitID)
	{
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_NOTICE_NOTGET, armyData.IsFirstGetUnit(iUnitID));
	}

	private void SetUnitDiscription(int unitID)
	{
		NKCCollectionUnitTemplet unitTemplet = NKCCollectionManager.GetUnitTemplet(unitID);
		if (unitTemplet != null)
		{
			if (m_srUnitIntroduce != null)
			{
				m_srUnitIntroduce.verticalNormalizedPosition = 1f;
			}
			NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_UNIT_PROFILE_UNIT_INTRODUCE_TEXT, unitTemplet.GetUnitIntro());
		}
	}

	private void SetDetailedStat(NKMOperator operatorData)
	{
		if (operatorData == null)
		{
			return;
		}
		if (m_NKMOperator == null)
		{
			NKMOperator nKMOperator = new NKMOperator();
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(operatorData.id);
			if (unitTempletBase != null)
			{
				nKMOperator.id = operatorData.id;
				NKMOperatorSkillTemplet skillTemplet = NKCOperatorUtil.GetSkillTemplet(unitTempletBase.m_lstSkillStrID[0]);
				if (skillTemplet != null)
				{
					NKMOperatorSkill nKMOperatorSkill = new NKMOperatorSkill();
					nKMOperatorSkill.id = skillTemplet.m_OperSkillID;
					nKMOperatorSkill.level = (byte)skillTemplet.m_MaxSkillLevel;
					nKMOperator.mainSkill = nKMOperatorSkill;
				}
				NKMOperatorRandomPassiveGroupTemplet nKMOperatorRandomPassiveGroupTemplet = NKMOperatorRandomPassiveGroupTemplet.Find(NKCOperatorUtil.GetPassiveGroupID(operatorData.id));
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
			NKCUtil.SetLabelText(m_lbPower, CalculateOperatorOperationPower(operatorData).ToString());
		}
		else
		{
			NKCUtil.SetLabelText(m_lbPower, CalculateOperatorOperationPower(operatorData).ToString());
		}
		NKCUtil.SetLabelText(m_STAT_NUMBER_ATK, NKCOperatorUtil.GetStatPercentageString(operatorData, NKM_STAT_TYPE.NST_ATK) ?? "");
		NKCUtil.SetLabelText(m_STAT_NUMBER_DEF, NKCOperatorUtil.GetStatPercentageString(operatorData, NKM_STAT_TYPE.NST_DEF) ?? "");
		NKCUtil.SetLabelText(m_STAT_NUMBER_HP, NKCOperatorUtil.GetStatPercentageString(operatorData, NKM_STAT_TYPE.NST_HP) ?? "");
		NKCUtil.SetLabelText(m_STAT_NUMBER_SKILL_COOL, NKCOperatorUtil.GetStatPercentageString(operatorData, NKM_STAT_TYPE.NST_SKILL_COOL_TIME_REDUCE_RATE) ?? "");
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
		m_bAppraisal = false;
		base.UnHide();
	}

	public override void CloseInternal()
	{
		BannerCleanUp();
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
		NKCPopupUnitInfoDetail.CheckInstanceAndClose();
		NKCUIPopupIllustView.CheckInstanceAndClose();
		m_NKMOperator = null;
	}

	private void OnUnitAppraisal()
	{
		if (!m_bViewMode)
		{
			NKCUIUnitReview.Instance.OpenUI(m_NKMOperator.id);
			m_bAppraisal = true;
		}
	}

	private void OnClickChangeIllust()
	{
		if (!m_bAppraisal)
		{
			NKCUIPopupIllustView.Instance.Open(m_NKMOperator);
		}
	}

	private void SetVoiceButtonUI()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_PROFILE_VOICE_BUTTON, bValue: true);
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_PROFILE_VOICE_BUTTON_DISABLE, bValue: false);
	}

	private void OnUnitVoice()
	{
		if (m_NKMOperator != null)
		{
			NKCUIPopupVoice.Instance.Open(m_NKMOperator.id, 0, bLifetime: false);
		}
	}

	private void UpdateSkillInfo()
	{
		m_MainSkillCombo.SetData(m_NKMOperator.id);
		NKCUtil.SetGameobjectActive(m_SubSkill, m_NKMOperator.subSkill.id != 0);
		if (m_NKMOperator != null)
		{
			m_MainSkill.SetData(m_NKMOperator.mainSkill.id, m_NKMOperator.mainSkill.level);
			m_SubSkill.SetData(m_NKMOperator.subSkill.id, m_NKMOperator.subSkill.level);
		}
		else
		{
			m_MainSkill.SetDataForCollection(m_NKMOperator.id);
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_NKMOperator.id);
		if (unitTempletBase == null)
		{
			return;
		}
		NKMOperatorSkillTemplet skillTemplet = NKCOperatorUtil.GetSkillTemplet(unitTempletBase.m_lstSkillStrID[0]);
		if (skillTemplet != null)
		{
			NKMTacticalCommandTemplet tacticalCommandTempletByStrID = NKMTacticalCommandManager.GetTacticalCommandTempletByStrID(skillTemplet.m_OperSkillTarget);
			if (tacticalCommandTempletByStrID != null)
			{
				NKCUtil.SetLabelText(m_lbSkillCoolTime, string.Format(NKCStringTable.GetString("SI_DP_REMAIN_TIME_STRING_SECONDS"), (int)tacticalCommandTempletByStrID.m_fCoolTime));
			}
		}
	}

	private void OnClickSkillInfo()
	{
		if (m_NKMOperator != null)
		{
			NKCUIOperatorPopUpSkill.Instance.OpenForCollection(m_NKMOperator.id);
		}
	}
}
