using System;
using System.Collections.Generic;
using ClientPacket.Community;
using Cs.Protocol;
using NKC.Templet;
using NKC.UI.Component;
using NKC.UI.Guide;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Collection;

public class NKCUICollectionUnitInfo : NKCUIBase
{
	public enum eCollectionState
	{
		CS_NONE,
		CS_PROFILE,
		CS_STATUS,
		CS_TAG
	}

	private enum eTagSortOption
	{
		TS_NONE,
		TS_TAG_NORMAL,
		TS_TAG_VOTE
	}

	[Serializable]
	public struct tagDiscription
	{
		public GameObject m_tagObj;

		public Text tagTigle;

		public Text tagCount;
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_collection";

	private const string UI_ASSET_NAME = "NKM_UI_COLLECTION_UNIT_INFO";

	private const string UI_ASSET_NAME_OTHER = "NKM_UI_COLLECTION_UNIT_INFO_OTHER";

	private static bool m_isGauntlet;

	private static bool m_bWillCloseUnderPopupOnOpen = true;

	private static NKCUICollectionUnitInfo m_Instance;

	private static NKCUIManager.LoadedUIData m_loadedUIData;

	private NKCUIUpsideMenu.eMode m_UpsideMenuMode = NKCUIUpsideMenu.eMode.Normal;

	private int m_UnitID;

	private int m_SkinID;

	private NKMUnitData m_NKMUnitData;

	private List<NKMEquipItemData> m_listNKMEquipItemData;

	[Header("요약정보창")]
	public NKCUICharInfoSummary m_NKCUICharInfoSummary;

	[Header("장착정보창")]
	public GameObject m_objEquipSlotParent;

	public NKCUISlot m_slotEquipWeapon;

	public NKCUISlot m_slotEquipDefense;

	public NKCUISlot m_slotEquipAcc;

	public NKCUISlot m_slotEquipAcc_2;

	[Header("세트아이템 이펙트")]
	public GameObject m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_WEAPON;

	public Animator m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_WEAPON;

	public GameObject m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_DEFENCE;

	public Animator m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_DEFENCE;

	public GameObject m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC;

	public Animator m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC;

	public GameObject m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC_02;

	public Animator m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC_02;

	[Header("상세정보창")]
	public Text m_NKM_UI_COLLECTION_UNIT_STAT_SUMMARY_POWER_TEXT;

	public ScrollRect m_srUnitIntroduce;

	public Text m_NKM_UI_COLLECTION_UNIT_PROFILE_UNIT_INTRODUCE_TEXT;

	public NKCUIComStateButton m_GuideBtn;

	public string m_GuideStrID;

	public NKCUIUnitStatSlot m_slotHP;

	public NKCUIUnitStatSlot m_slotAttack;

	public NKCUIUnitStatSlot m_slotDefense;

	public NKCUIUnitStatSlot m_slotHitRate;

	public NKCUIUnitStatSlot m_slotCritHitRate;

	public NKCUIUnitStatSlot m_slotEvade;

	[Header("탭")]
	public NKCUIComStateButton m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_PROFILE;

	public GameObject m_NKM_UI_COLLECTION_UNIT_PROFILE;

	public NKCUIComStateButton m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_STAT;

	public GameObject m_NKM_UI_COLLECTION_UNIT_STAT;

	public NKCUIComStateButton m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_TAG;

	public GameObject m_NKM_UI_COLLECTION_UNIT_TAG;

	[Header("스킨")]
	public NKCUIUnitInfoSkinPanel m_UISkinPanel;

	public GameObject m_NKM_UI_COLLECTION_UNIT_INFO_UNIT_SD;

	[Header("유닛 일러스트")]
	public NKCUICharacterView m_CharacterView;

	[Header("일러스트 보기 모드에서 움직이는 Rect들. Base/ViewMode 두 이름으로 지정")]
	public Animator m_ani_NKM_UI_COLLECTION_UNIT_INFO_CONTENT;

	[Header("기타 버튼")]
	public NKCUIComStateButton m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_PRACTICE;

	public NKCUIComStateButton m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_APPRAISAL;

	public NKCUIComStateButton m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_ILLUST_CHANGE;

	public NKCUIComStateButton m_NKM_UI_COLLECTION_UNIT_PROFILE_TAG_VIEW_ALL_BUTTON;

	public NKCUIComStateButton m_NKM_UI_COLLECTION_UNIT_PROFILE_VOICE_BUTTON;

	public NKCUIComStateButton m_NKM_UI_COLLECTION_UNIT_INFO_LOYALTY;

	public NKCUIComStateButton m_NKM_UI_COLLECTION_UNIT_ACHIEVEMENT_BUTTON;

	public NKCUIComButton m_cbtnUnitInfoDetailPopup;

	[Space]
	public GameObject m_NKM_UI_COLLECTION_UNIT_INFO_NOTICE_NOTGET;

	public GameObject m_NKM_UI_COLLECTION_UNIT_ACHIEVEMENT_BUTTON_NEW;

	[Header("스킬 패널")]
	public NKCUIUnitInfoSkillPanel m_UISkillPanel;

	[Header("태그 Top6 노출")]
	public GameObject m_NKM_UI_COLLECTION_UNIT_PROFILE_USER_TAG;

	public GameObject m_NKM_UI_COLLECTION_UNIT_PROFILE_USER_TAG_CAUTION;

	[Header("성우")]
	public GameObject m_objVoiceActor;

	public Text m_lbVoiceActorName;

	private eCollectionState m_eCurrentState;

	[Header("캐릭터 판넬")]
	public NKCUIComDragSelectablePanel m_DragCharacterView;

	public EventTrigger m_evtPanel;

	private NKCUIUnitInfo.OpenOption m_OpenOption;

	private bool m_bAppraisal;

	private bool m_bViewMode;

	[Header("SD캐릭터 관련 설정")]
	public RectTransform m_rtSDRoot;

	private NKCASUIUnitIllust m_spineSD;

	public float m_fSDScale = 1.2f;

	private int m_CurUnitID = -1;

	[Space]
	[Header("태그")]
	public GameObject m_NKM_UI_COLLECTION_UNIT_INFO_CONTENT_USER_TAG;

	public NKCUIComStateButton m_cbtn_NKM_UI_COLLECTION_UNIT_INFO_USER_TAG_BUTTON_CLOSE;

	public Text m_USER_TAG_MY_OPINION_COUNT_TEXT;

	[Header("태그 슬롯 프리팹 & 사이즈 설정")]
	public NKCUICollectionTagSlot m_pfbTagSlot;

	public Vector2 m_vTagSlotSize;

	public Vector2 m_vTagSlotSpacing;

	[Header("태그 UI Component")]
	public RectTransform m_rectContentRect;

	public RectTransform m_rectSlotPoolRect;

	public LoopScrollRect m_LoopScrollRect;

	public GridLayoutGroup m_GridLayoutGroup;

	private List<NKCUICollectionTagSlot> m_lstVisibleSlot = new List<NKCUICollectionTagSlot>();

	private Stack<NKCUICollectionTagSlot> m_stkTagSlotPool = new Stack<NKCUICollectionTagSlot>();

	[Header("정렬 방식 선택")]
	public NKCUIComToggle m_cbtn_NKM_UI_COLLECTION_UNIT_INFO_USER_TAG_BG;

	public GameObject m_NKM_UI_COLLECTION_UNIT_INFO_USER_TAG_OPEN;

	public NKCUIComButton m_cbtnSortTypeNormal;

	public NKCUIComButton m_cbtnSortTypeVote;

	public Text m_NKM_UI_COLLECTION_UNIT_INFO_USER_TAGS_SORT_TYPE_TEXT;

	private int minColumnTag = 3;

	private eTagSortOption m_eCurrentTagSortType;

	private eTagSortOption m_eOldTagSortType;

	private bool m_bCellPrepared;

	[Header("유저 태그 순위 노출")]
	public List<tagDiscription> tagRank;

	private List<NKCUnitTagData> m_lst_CurTagData = new List<NKCUnitTagData>();

	private const int m_iMaxTagVotdeCount = 7;

	private int m_iCurVotedCount;

	private int m_iClickSlotIdx;

	public override bool WillCloseUnderPopupOnOpen => m_bWillCloseUnderPopupOnOpen;

	public static NKCUICollectionUnitInfo Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_loadedUIData = NKCUIManager.OpenNewInstance<NKCUICollectionUnitInfo>("ab_ui_nkm_ui_collection", m_isGauntlet ? "NKM_UI_COLLECTION_UNIT_INFO_OTHER" : "NKM_UI_COLLECTION_UNIT_INFO", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance);
				m_Instance = m_loadedUIData.GetInstance<NKCUICollectionUnitInfo>();
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
		m_isGauntlet = false;
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
		InitTag();
	}

	public void InitUI()
	{
		if (m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_ILLUST_CHANGE != null)
		{
			m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_ILLUST_CHANGE.PointerClick.RemoveAllListeners();
			m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_ILLUST_CHANGE.PointerClick.AddListener(OnClickChangeIllust);
		}
		if (m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_PRACTICE != null)
		{
			m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_PRACTICE.PointerClick.RemoveAllListeners();
			m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_PRACTICE.PointerClick.AddListener(OnUnitTestButton);
		}
		if (m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_APPRAISAL != null)
		{
			m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_APPRAISAL.PointerClick.RemoveAllListeners();
			m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_APPRAISAL.PointerClick.AddListener(OnUnitAppraisal);
		}
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_APPRAISAL, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.UNIT_REVIEW_SYSTEM));
		if (m_NKM_UI_COLLECTION_UNIT_PROFILE_TAG_VIEW_ALL_BUTTON != null)
		{
			m_NKM_UI_COLLECTION_UNIT_PROFILE_TAG_VIEW_ALL_BUTTON.PointerClick.RemoveAllListeners();
			m_NKM_UI_COLLECTION_UNIT_PROFILE_TAG_VIEW_ALL_BUTTON.PointerClick.AddListener(delegate
			{
				ShowTag();
			});
		}
		if (m_NKM_UI_COLLECTION_UNIT_PROFILE_VOICE_BUTTON != null)
		{
			m_NKM_UI_COLLECTION_UNIT_PROFILE_VOICE_BUTTON.PointerClick.RemoveAllListeners();
			m_NKM_UI_COLLECTION_UNIT_PROFILE_VOICE_BUTTON.PointerClick.AddListener(OnUnitVoice);
		}
		if (m_NKM_UI_COLLECTION_UNIT_INFO_LOYALTY != null)
		{
			m_NKM_UI_COLLECTION_UNIT_INFO_LOYALTY.PointerClick.RemoveAllListeners();
			m_NKM_UI_COLLECTION_UNIT_INFO_LOYALTY.PointerClick.AddListener(OnReplayLifetime);
		}
		if (m_cbtnUnitInfoDetailPopup != null)
		{
			NKCUtil.SetGameobjectActive(m_cbtnUnitInfoDetailPopup, bValue: false);
			m_cbtnUnitInfoDetailPopup.PointerClick.RemoveAllListeners();
			m_cbtnUnitInfoDetailPopup.PointerClick.AddListener(OpenUnitInfoDetailPopup);
		}
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
		if (m_UISkillPanel != null)
		{
			m_UISkillPanel.Init();
			m_UISkillPanel.SetOpenPopupWhenSelected();
		}
		if (m_UISkinPanel != null)
		{
			m_UISkinPanel.Init(ChangeSkin);
		}
		m_slotEquipWeapon.Init();
		m_slotEquipWeapon.Set_EQUIP_BOX_BOTTOM_MENU_TYPE(NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
		m_slotEquipDefense.Init();
		m_slotEquipDefense.Set_EQUIP_BOX_BOTTOM_MENU_TYPE(NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
		m_slotEquipAcc.Init();
		m_slotEquipAcc.Set_EQUIP_BOX_BOTTOM_MENU_TYPE(NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
		m_slotEquipAcc_2.Init();
		m_slotEquipAcc_2.Set_EQUIP_BOX_BOTTOM_MENU_TYPE(NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
		if (null != m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_PROFILE)
		{
			m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_PROFILE.PointerClick.RemoveAllListeners();
			m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_PROFILE.PointerClick.AddListener(delegate
			{
				ChangeState(eCollectionState.CS_PROFILE);
			});
		}
		if (null != m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_STAT)
		{
			m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_STAT.PointerClick.RemoveAllListeners();
			m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_STAT.PointerClick.AddListener(delegate
			{
				ChangeState(eCollectionState.CS_STATUS);
			});
		}
		if (null != m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_TAG)
		{
			m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_TAG.PointerClick.RemoveAllListeners();
			m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_TAG.PointerClick.AddListener(delegate
			{
				ChangeState(eCollectionState.CS_TAG);
			});
		}
		if (null != m_cbtn_NKM_UI_COLLECTION_UNIT_INFO_USER_TAG_BUTTON_CLOSE)
		{
			m_cbtn_NKM_UI_COLLECTION_UNIT_INFO_USER_TAG_BUTTON_CLOSE.PointerClick.RemoveAllListeners();
			m_cbtn_NKM_UI_COLLECTION_UNIT_INFO_USER_TAG_BUTTON_CLOSE.PointerClick.AddListener(delegate
			{
				OnCloseTagList();
			});
		}
		if (null != m_GuideBtn)
		{
			m_GuideBtn.PointerClick.RemoveAllListeners();
			m_GuideBtn.PointerClick.AddListener(delegate
			{
				NKCUIPopUpGuide.Instance.Open(m_GuideStrID);
			});
		}
		bool openTagCollectionMission = NKCUnitMissionManager.GetOpenTagCollectionMission();
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_ACHIEVEMENT_BUTTON, openTagCollectionMission);
		if (openTagCollectionMission)
		{
			NKCUtil.SetButtonClickDelegate(m_NKM_UI_COLLECTION_UNIT_ACHIEVEMENT_BUTTON, OnClickUnitAchievement);
		}
		m_NKCUICharInfoSummary.SetUnitClassRootActive(value: false);
		m_NKCUICharInfoSummary.Init(bShowLevel: false);
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

	private void ChangeUnit(NKMUnitData cNKMUnitData)
	{
		m_eOldTagSortType = m_eCurrentTagSortType;
		m_eCurrentTagSortType = eTagSortOption.TS_TAG_NORMAL;
		UpdateTagInfo(cNKMUnitData.m_UnitID);
		m_NKCUICharInfoSummary.SetData(cNKMUnitData);
		SetData(cNKMUnitData);
		UpdateMyVoteCount(cNKMUnitData.m_UnitID);
		if (NKCPopupUnitInfoDetail.IsInstanceOpen)
		{
			NKCPopupUnitInfoDetail.InstanceOpen(m_NKMUnitData, NKCPopupUnitInfoDetail.UnitInfoDetailType.gauntlet, m_listNKMEquipItemData);
		}
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
		if (m_OpenOption == null || m_OpenOption.m_lstUnitData == null)
		{
			return;
		}
		NKMUnitData nKMUnitData = m_OpenOption.m_lstUnitData[idx];
		Debug.Log($"<color=yellow>target : {tr.name}, idx : {idx}, </color>");
		if (nKMUnitData != null)
		{
			m_SkinID = 0;
			NKCUICharacterView component = tr.GetComponent<NKCUICharacterView>();
			if (component != null)
			{
				component.SetCharacterIllust(nKMUnitData.m_UnitID);
				return;
			}
			NKCUICharacterView nKCUICharacterView = tr.gameObject.AddComponent<NKCUICharacterView>();
			nKCUICharacterView.m_rectIllustRoot = tr.GetComponent<RectTransform>();
			nKCUICharacterView.SetCharacterIllust(nKMUnitData.m_UnitID);
		}
	}

	private void ReturnMainBannerListSlot(Transform go)
	{
		NKCUtil.SetGameobjectActive(go, bValue: false);
		UnityEngine.Object.Destroy(go.gameObject);
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
		if (m_OpenOption.m_lstUnitData.Count >= idx && idx >= 0)
		{
			NKMUnitData nKMUnitData = m_OpenOption.m_lstUnitData[idx];
			if (nKMUnitData != null)
			{
				ChangeUnit(nKMUnitData);
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

	public static void CheckInstanceAndOpen(NKMUnitData cNKMUnitData, NKCUIUnitInfo.OpenOption openOption, List<NKMEquipItemData> listNKMEquipItemData = null, eCollectionState eStartingState = NKCUICollectionUnitInfo.eCollectionState.CS_PROFILE, bool isGauntlet = false, NKCUIUpsideMenu.eMode upsideMenuMode = NKCUIUpsideMenu.eMode.Normal, bool bWillCloseUnderPopupOnOpen = true)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(cNKMUnitData);
		if (unitTempletBase == null || unitTempletBase.IsUnitDescNullOrEmplty())
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_GAUNTLET_EVENT_UNIT_DETAIL_INFO_NOT_POSSIBLE);
			return;
		}
		openOption?.FilterUnitlist();
		bool flag = false;
		if (NKCDefineManager.DEFINE_UNITY_EDITOR() && Input.GetKey(KeyCode.LeftControl))
		{
			flag = true;
		}
		if (NKCCollectionManager.IsProfileCollectionV2Opened && !flag)
		{
			NKCUICollectionUnitInfoV2.eCollectionState eCollectionState = NKCUICollectionUnitInfoV2.eCollectionState.CS_NONE;
			switch (eStartingState)
			{
			case NKCUICollectionUnitInfo.eCollectionState.CS_NONE:
			case NKCUICollectionUnitInfo.eCollectionState.CS_TAG:
				eCollectionState = NKCUICollectionUnitInfoV2.eCollectionState.CS_NONE;
				break;
			case NKCUICollectionUnitInfo.eCollectionState.CS_PROFILE:
				eCollectionState = NKCUICollectionUnitInfoV2.eCollectionState.CS_PROFILE;
				break;
			case NKCUICollectionUnitInfo.eCollectionState.CS_STATUS:
				eCollectionState = NKCUICollectionUnitInfoV2.eCollectionState.CS_STATUS;
				break;
			default:
				eCollectionState = NKCUICollectionUnitInfoV2.eCollectionState.CS_NONE;
				break;
			}
			NKCUICollectionUnitInfoV2.CheckInstanceAndOpen(cNKMUnitData, openOption, listNKMEquipItemData, eCollectionState, isGauntlet, NKCUIUpsideMenu.eMode.LeftsideWithHamburger, bWillCloseUnderPopupOnOpen);
		}
		else
		{
			if (m_Instance != null && m_isGauntlet != isGauntlet)
			{
				m_loadedUIData.CloseInstance();
			}
			m_isGauntlet = isGauntlet;
			Instance.Open(cNKMUnitData, openOption, listNKMEquipItemData, eStartingState, upsideMenuMode, bWillCloseUnderPopupOnOpen);
		}
	}

	public static void CheckInstanceAndOpen(NKMOperator cOperatorData, NKCUIOperatorInfo.OpenOption openOption, eCollectionState eStartingState = NKCUICollectionUnitInfo.eCollectionState.CS_PROFILE, NKCUIUpsideMenu.eMode upsideMenuMode = NKCUIUpsideMenu.eMode.Normal, bool bWillCloseUnderPopupOnOpen = true)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(cOperatorData.id);
		if (unitTempletBase == null || unitTempletBase.IsUnitDescNullOrEmplty())
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_GAUNTLET_EVENT_UNIT_DETAIL_INFO_NOT_POSSIBLE);
			return;
		}
		bool flag = false;
		if (NKCDefineManager.DEFINE_UNITY_EDITOR() && Input.GetKey(KeyCode.LeftControl))
		{
			flag = true;
		}
		if (NKCCollectionManager.IsProfileCollectionV2Opened && !flag)
		{
			NKCUICollectionOperatorInfoV2.eCollectionState eCollectionState = NKCUICollectionOperatorInfoV2.eCollectionState.CS_NONE;
			switch (eStartingState)
			{
			case NKCUICollectionUnitInfo.eCollectionState.CS_NONE:
			case NKCUICollectionUnitInfo.eCollectionState.CS_PROFILE:
			case NKCUICollectionUnitInfo.eCollectionState.CS_TAG:
				eCollectionState = NKCUICollectionOperatorInfoV2.eCollectionState.CS_PROFILE;
				break;
			case NKCUICollectionUnitInfo.eCollectionState.CS_STATUS:
				eCollectionState = NKCUICollectionOperatorInfoV2.eCollectionState.CS_STATUS;
				break;
			default:
				eCollectionState = NKCUICollectionOperatorInfoV2.eCollectionState.CS_NONE;
				break;
			}
			NKCUICollectionOperatorInfoV2.CheckInstanceAndOpen(cOperatorData, openOption, eCollectionState, NKCUIUpsideMenu.eMode.LeftsideWithHamburger, bWillCloseUnderPopupOnOpen);
		}
		else
		{
			if (m_Instance != null)
			{
				m_loadedUIData.CloseInstance();
			}
			m_isGauntlet = false;
			NKCUICollectionOperatorInfo.Instance.Open(cOperatorData, openOption);
		}
	}

	private void Open(NKMUnitData cNKMUnitData, NKCUIUnitInfo.OpenOption openOption, List<NKMEquipItemData> listNKMEquipItemData = null, eCollectionState eStartingState = eCollectionState.CS_PROFILE, NKCUIUpsideMenu.eMode upsideMenuMode = NKCUIUpsideMenu.eMode.Normal, bool bWillCloseUnderPopupOnOpen = true)
	{
		m_eCurrentState = eCollectionState.CS_NONE;
		m_eCurrentTagSortType = eTagSortOption.TS_TAG_NORMAL;
		m_UpsideMenuMode = upsideMenuMode;
		m_bWillCloseUnderPopupOnOpen = bWillCloseUnderPopupOnOpen;
		m_listNKMEquipItemData = listNKMEquipItemData;
		m_OpenOption = openOption;
		if (m_OpenOption == null)
		{
			m_OpenOption = new NKCUIUnitInfo.OpenOption(new List<long>());
			m_OpenOption.m_lstUnitData.Add(cNKMUnitData);
		}
		if (m_DragCharacterView != null)
		{
			if (m_OpenOption.m_lstUnitData.Count == 0)
			{
				m_OpenOption.m_lstUnitData.Add(cNKMUnitData);
			}
			m_DragCharacterView.TotalCount = m_OpenOption.m_lstUnitData.Count;
			for (int i = 0; i < m_OpenOption.m_lstUnitData.Count; i++)
			{
				if (m_OpenOption.m_lstUnitData[i].m_UnitUID == cNKMUnitData.m_UnitUID)
				{
					m_DragCharacterView.SetIndex(i);
					break;
				}
			}
		}
		SetData(cNKMUnitData);
		ChangeState(eStartingState);
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		UIOpened();
	}

	private void ChangeState(eCollectionState newStat)
	{
		if (m_eCurrentState != newStat && !m_bViewMode)
		{
			m_eCurrentState = newStat;
			UpdateUI();
			if (eCollectionState.CS_TAG == m_eCurrentState)
			{
				UpdateTagInfo(m_UnitID);
			}
		}
	}

	private void ShowTag()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_CONTENT_USER_TAG, bValue: true);
		UpdateTagInfo(m_UnitID);
	}

	private void UpdateUI()
	{
		switch (m_eCurrentState)
		{
		case eCollectionState.CS_PROFILE:
			NKCUtil.SetGameobjectActive(m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_PROFILE.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_STAT.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_TAG.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_PROFILE, bValue: true);
			m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_PROFILE.Select(bSelect: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_STAT, bValue: false);
			m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_STAT.Select(bSelect: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_TAG, bValue: false);
			m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_TAG.Select(bSelect: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_CONTENT_USER_TAG, bValue: false);
			break;
		case eCollectionState.CS_STATUS:
			NKCUtil.SetGameobjectActive(m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_PROFILE.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_STAT.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_TAG.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_PROFILE, bValue: false);
			m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_PROFILE.Select(bSelect: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_STAT, bValue: true);
			m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_STAT.Select(bSelect: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_TAG, bValue: false);
			m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_TAG.Select(bSelect: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_CONTENT_USER_TAG, bValue: false);
			break;
		case eCollectionState.CS_TAG:
			NKCUtil.SetGameobjectActive(m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_PROFILE.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_STAT.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_TAG.gameObject, bValue: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_PROFILE, bValue: false);
			m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_PROFILE.Select(bSelect: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_STAT, bValue: false);
			m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_STAT.Select(bSelect: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_TAG, bValue: true);
			m_csbtn_NKM_UI_COLLECTION_UNIT_INFO_TAG.Select(bSelect: true);
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_CONTENT_USER_TAG, bValue: false);
			break;
		}
	}

	private void SetData(NKMUnitData unitData)
	{
		OpenSDIllust(unitData, 0);
		m_UISkillPanel.SetData(unitData, bDisplayEmptySlot: true);
		if (m_NKMUnitData == null || m_UnitID != unitData.m_UnitID)
		{
			if (m_NKMUnitData == null)
			{
				m_NKMUnitData = new NKMUnitData();
			}
			m_NKMUnitData.DeepCopyFrom(unitData);
			m_UnitID = unitData.m_UnitID;
			m_SkinID = 0;
			NKCUtil.SetGameobjectActive(m_UISkinPanel, bValue: true);
			m_UISkinPanel.SetData(m_NKMUnitData, resetSkin: true);
			SetUnitDiscription(m_UnitID);
			SetDetailedStat(m_NKMUnitData);
			m_NKCUICharInfoSummary.SetData(m_NKMUnitData);
			CheckHasUnit(m_UnitID);
			SetLifetimeButtonUI(m_UnitID);
			if (m_listNKMEquipItemData != null || m_isGauntlet)
			{
				NKCUtil.SetGameobjectActive(m_objEquipSlotParent, bValue: true);
				UpdateEquipSlots();
				NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_UNIT_SD, bValue: false);
				NKCUtil.SetGameobjectActive(m_UISkinPanel, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_PRACTICE, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_BOTTOM_BUTTON_APPRAISAL, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_PROFILE_TAG_VIEW_ALL_BUTTON, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_PROFILE_VOICE_BUTTON, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_LOYALTY, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_NOTICE_NOTGET, bValue: false);
				NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_ACHIEVEMENT_BUTTON, bValue: false);
				NKCUtil.SetGameobjectActive(m_objVoiceActor, bValue: false);
				NKCUtil.SetGameobjectActive(m_cbtnUnitInfoDetailPopup, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objEquipSlotParent, bValue: false);
			}
			UpdateUnitMissionRedDot();
			if (NKCUIPopupCollectionAchievement.IsInstanceOpen)
			{
				NKCUIPopupCollectionAchievement.Instance.Open(m_UnitID);
			}
			NKCUtil.SetLabelText(m_lbVoiceActorName, NKCVoiceActorNameTemplet.FindActorName(unitData));
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

	private void SetDetailedStat(NKMUnitData unitData)
	{
		NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitData.m_UnitID);
		bool bPvP = false;
		NKMStatData nKMStatData = new NKMStatData();
		nKMStatData.Init();
		nKMStatData.MakeBaseStat(null, bPvP, unitData, unitStatTemplet.m_StatData);
		if (m_listNKMEquipItemData != null)
		{
			NKMInventoryData nKMInventoryData = new NKMInventoryData();
			nKMInventoryData.AddItemEquip(m_listNKMEquipItemData);
			nKMStatData.MakeBaseBonusFactor(unitData, nKMInventoryData.EquipItems, null, null);
			NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_UNIT_STAT_SUMMARY_POWER_TEXT, unitData.CalculateOperationPower(nKMInventoryData).ToString("N0"));
		}
		else
		{
			nKMStatData.MakeBaseBonusFactor(unitData, null, null, null);
			NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_UNIT_STAT_SUMMARY_POWER_TEXT, unitData.CalculateOperationPower(NKCScenManager.CurrentUserData().m_InventoryData).ToString("N0"));
		}
		m_slotHP.SetStat(NKM_STAT_TYPE.NST_HP, nKMStatData, unitData);
		m_slotAttack.SetStat(NKM_STAT_TYPE.NST_ATK, nKMStatData, unitData);
		m_slotDefense.SetStat(NKM_STAT_TYPE.NST_DEF, nKMStatData, unitData);
		m_slotHitRate.SetStat(NKM_STAT_TYPE.NST_HIT, nKMStatData, unitData);
		m_slotCritHitRate.SetStat(NKM_STAT_TYPE.NST_CRITICAL, nKMStatData, unitData);
		m_slotEvade.SetStat(NKM_STAT_TYPE.NST_EVADE, nKMStatData, unitData);
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
		m_UISkinPanel.CleanUp();
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
		m_eCurrentTagSortType = eTagSortOption.TS_NONE;
		if (m_spineSD != null)
		{
			m_spineSD.Unload();
			m_spineSD = null;
		}
		NKCPopupUnitInfoDetail.CheckInstanceAndClose();
		NKCUIPopupIllustView.CheckInstanceAndClose();
		NKCUIPopupCollectionAchievement.CheckInstanceAndClose();
		m_NKMUnitData = null;
		m_UnitID = 0;
		m_SkinID = 0;
	}

	private void OnUnitTestButton()
	{
		if (!m_bViewMode)
		{
			NKM_SHORTCUT_TYPE returnUIShortcut = ((NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_COLLECTION) ? NKCScenManager.GetScenManager().Get_NKC_SCEN_COLLECTION().GetCurrentShortcutType() : NKM_SHORTCUT_TYPE.SHORTCUT_NONE);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_UnitID);
			string returnUIShortcutParam = ((unitTempletBase != null) ? unitTempletBase.m_UnitStrID : "");
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OpenPracticeGameComfirmPopup(m_NKMUnitData, returnUIShortcut, returnUIShortcutParam);
		}
	}

	private void OnUnitAppraisal()
	{
		if (!m_bViewMode)
		{
			NKCUIUnitReview.Instance.OpenUI(m_UnitID);
			m_bAppraisal = true;
		}
	}

	private void OnClickChangeIllust()
	{
		if (!m_bAppraisal)
		{
			NKCUIPopupIllustView.Instance.Open(m_NKMUnitData);
		}
	}

	public void ChangeSkin(int skinID)
	{
		if (m_OpenOption == null || !(m_DragCharacterView != null) || m_DragCharacterView.CurrentIndex < 0 || m_OpenOption.m_lstUnitData.Count <= m_DragCharacterView.CurrentIndex)
		{
			return;
		}
		NKMUnitData nKMUnitData = m_OpenOption.m_lstUnitData[m_DragCharacterView.CurrentIndex];
		if (nKMUnitData == null)
		{
			return;
		}
		if (skinID != 0)
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(skinID);
			if (!NKMSkinManager.IsSkinForCharacter(nKMUnitData.m_UnitID, skinTemplet))
			{
				return;
			}
		}
		if (m_DragCharacterView.GetCurrentItem() != null)
		{
			NKCUICharacterView componentInChildren = m_DragCharacterView.GetCurrentItem().GetComponentInChildren<NKCUICharacterView>();
			if (componentInChildren != null)
			{
				componentInChildren.CloseImmediatelyIllust();
				m_SkinID = skinID;
				m_NKMUnitData.m_SkinID = skinID;
				componentInChildren.SetCharacterIllust(nKMUnitData.m_UnitID, skinID);
				OpenSDIllust(nKMUnitData, skinID);
			}
		}
		NKCUtil.SetLabelText(m_lbVoiceActorName, NKCVoiceActorNameTemplet.FindActorName(m_NKMUnitData));
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

	private void InitTag()
	{
		if (null != m_cbtn_NKM_UI_COLLECTION_UNIT_INFO_USER_TAG_BG)
		{
			m_cbtn_NKM_UI_COLLECTION_UNIT_INFO_USER_TAG_BG.OnValueChanged.RemoveAllListeners();
			m_cbtn_NKM_UI_COLLECTION_UNIT_INFO_USER_TAG_BG.OnValueChanged.AddListener(OnSortMenuOpen);
		}
		if (null != m_cbtnSortTypeNormal)
		{
			m_cbtnSortTypeNormal.PointerClick.RemoveAllListeners();
			m_cbtnSortTypeNormal.PointerClick.AddListener(delegate
			{
				SetSort(eTagSortOption.TS_TAG_NORMAL);
			});
		}
		if (null != m_cbtnSortTypeVote)
		{
			m_cbtnSortTypeVote.PointerClick.RemoveAllListeners();
			m_cbtnSortTypeVote.PointerClick.AddListener(delegate
			{
				SetSort(eTagSortOption.TS_TAG_VOTE);
			});
		}
		if (null != m_LoopScrollRect)
		{
			m_LoopScrollRect.dOnGetObject += GetSlot;
			m_LoopScrollRect.dOnReturnObject += ReturnSlot;
			m_LoopScrollRect.dOnProvideData += ProvideSlotData;
			m_LoopScrollRect.dOnRepopulate += CalculateContentRectSize;
		}
	}

	private void CalculateContentRectSize()
	{
		NKCUtil.CalculateContentRectSize(m_LoopScrollRect, m_GridLayoutGroup, minColumnTag, m_vTagSlotSize, m_vTagSlotSpacing);
	}

	private RectTransform GetSlot(int index)
	{
		Stack<NKCUICollectionTagSlot> stkTagSlotPool = m_stkTagSlotPool;
		NKCUICollectionTagSlot pfbTagSlot = m_pfbTagSlot;
		if (stkTagSlotPool.Count > 0)
		{
			NKCUICollectionTagSlot nKCUICollectionTagSlot = stkTagSlotPool.Pop();
			NKCUtil.SetGameobjectActive(nKCUICollectionTagSlot, bValue: true);
			nKCUICollectionTagSlot.transform.localScale = Vector3.one;
			m_lstVisibleSlot.Add(nKCUICollectionTagSlot);
			return nKCUICollectionTagSlot.GetComponent<RectTransform>();
		}
		NKCUICollectionTagSlot nKCUICollectionTagSlot2 = UnityEngine.Object.Instantiate(pfbTagSlot);
		NKCUtil.SetGameobjectActive(nKCUICollectionTagSlot2, bValue: true);
		nKCUICollectionTagSlot2.transform.localScale = Vector3.one;
		m_lstVisibleSlot.Add(nKCUICollectionTagSlot2);
		return nKCUICollectionTagSlot2.GetComponent<RectTransform>();
	}

	private void ReturnSlot(Transform go)
	{
		NKCUICollectionTagSlot component = go.GetComponent<NKCUICollectionTagSlot>();
		if (!(component == null))
		{
			m_lstVisibleSlot.Remove(component);
			go.SetParent(m_rectSlotPoolRect);
			m_stkTagSlotPool.Push(component);
		}
	}

	private void ProvideSlotData(Transform tr, int idx)
	{
		NKCUICollectionTagSlot component = tr.GetComponent<NKCUICollectionTagSlot>();
		if (!(component == null))
		{
			NKCUnitTagData nKCUnitTagData = m_lst_CurTagData[idx];
			if (nKCUnitTagData != null)
			{
				component.SetData(tagClick, nKCUnitTagData.TagType, idx, nKCUnitTagData.Voted, NKCCollectionManager.GetTagTitle(nKCUnitTagData.TagType), nKCUnitTagData.VoteCount, nKCUnitTagData.IsTop);
			}
		}
	}

	private string GetSortText(eTagSortOption sortType)
	{
		return sortType switch
		{
			eTagSortOption.TS_TAG_NORMAL => NKCUtilString.GET_STRING_NORMAL, 
			eTagSortOption.TS_TAG_VOTE => NKCUtilString.GET_STRING_VOTE, 
			_ => "", 
		};
	}

	private void SetSort(eTagSortOption newSort)
	{
		OnSortMenuOpen(bOpen: false);
		if (SortTagContent(newSort))
		{
			UpdateTagInfo(m_UnitID);
		}
	}

	private bool SortTagContent(eTagSortOption newSort)
	{
		if (m_eCurrentTagSortType == newSort && m_CurUnitID == m_NKMUnitData.m_UnitID)
		{
			return false;
		}
		List<NKCUnitTagData> unitTagData = NKCCollectionManager.GetUnitTagData(m_CurUnitID);
		if (unitTagData == null)
		{
			return false;
		}
		switch (newSort)
		{
		case eTagSortOption.TS_TAG_NORMAL:
			unitTagData.Sort(TagCompareAscending);
			break;
		case eTagSortOption.TS_TAG_VOTE:
			unitTagData.Sort(VoteCompareDescending);
			break;
		}
		m_eCurrentTagSortType = newSort;
		NKCUtil.SetLabelText(m_NKM_UI_COLLECTION_UNIT_INFO_USER_TAGS_SORT_TYPE_TEXT, GetSortText(m_eCurrentTagSortType));
		return true;
	}

	private int TagCompareAscending(NKCUnitTagData a, NKCUnitTagData b)
	{
		return a.TagType.CompareTo(b.TagType);
	}

	private int VoteCompareDescending(NKCUnitTagData a, NKCUnitTagData b)
	{
		return b.VoteCount.CompareTo(a.VoteCount);
	}

	private void OnSortMenuOpen(bool bOpen)
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_USER_TAG_OPEN, bOpen);
	}

	private void PrepareTagList()
	{
		if (!m_bCellPrepared)
		{
			m_bCellPrepared = true;
			CalculateContentRectSize();
			m_LoopScrollRect.PrepareCells();
		}
	}

	private void UpdateTagInfo(int unitID)
	{
		m_CurUnitID = unitID;
		if (NKCCollectionManager.GetUnitTagData(m_CurUnitID) == null)
		{
			NKCPacketSender.Send_NKMPacket_UNIT_REVIEW_TAG_LIST_REQ(m_CurUnitID);
		}
		else
		{
			UpdateTagData(m_CurUnitID);
		}
	}

	private void UpdateTagData(int unitID, bool bResetScroll = true)
	{
		m_lst_CurTagData = NKCCollectionManager.GetUnitTagData(unitID);
		if (m_lst_CurTagData == null || m_eCurrentState != eCollectionState.CS_TAG)
		{
			return;
		}
		m_iCurVotedCount = 0;
		m_eOldTagSortType = m_eCurrentTagSortType;
		SortTagContent(eTagSortOption.TS_TAG_VOTE);
		int num = 6;
		for (int i = 0; i < m_lst_CurTagData.Count; i++)
		{
			if (m_lst_CurTagData[i].VoteCount > 0 && num > i)
			{
				m_lst_CurTagData[i].IsTop = true;
			}
			else
			{
				m_lst_CurTagData[i].IsTop = false;
			}
		}
		for (int j = 0; j < m_lst_CurTagData.Count; j++)
		{
			if (j < tagRank.Count)
			{
				if (m_lst_CurTagData[j].VoteCount > 0)
				{
					NKCUtil.SetLabelText(tagRank[j].tagTigle, NKCCollectionManager.GetTagTitle(m_lst_CurTagData[j].TagType));
					NKCUtil.SetLabelText(tagRank[j].tagCount, m_lst_CurTagData[j].VoteCount.ToString());
					NKCUtil.SetGameobjectActive(tagRank[j].m_tagObj, bValue: true);
				}
				else
				{
					NKCUtil.SetGameobjectActive(tagRank[j].m_tagObj, bValue: false);
				}
			}
			if (m_lst_CurTagData[j].Voted)
			{
				m_iCurVotedCount++;
			}
		}
		if (m_lst_CurTagData[0] != null)
		{
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_PROFILE_USER_TAG, m_lst_CurTagData[0].VoteCount > 0);
			NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_PROFILE_USER_TAG_CAUTION, m_lst_CurTagData[0].VoteCount <= 0);
		}
		NKCUtil.SetLabelText(m_USER_TAG_MY_OPINION_COUNT_TEXT, $"{m_iCurVotedCount}/{7}");
		SortTagContent(m_eOldTagSortType);
		PrepareTagList();
		m_LoopScrollRect.TotalCount = m_lst_CurTagData.Count;
		if (bResetScroll)
		{
			m_LoopScrollRect.SetIndexPosition(0);
		}
		else
		{
			m_LoopScrollRect.RefreshCells();
		}
	}

	private void OnCloseTagList()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_INFO_CONTENT_USER_TAG, bValue: false);
		ChangeState(eCollectionState.CS_TAG);
	}

	public void tagClick(short tagType, int slotIdx)
	{
		if (NKCCollectionManager.IsVoted(m_CurUnitID, tagType))
		{
			NKCPacketSender.Send_NKMPacket_UNIT_REVIEW_TAG_VOTE_CANCEL_REQ(m_CurUnitID, tagType);
		}
		else
		{
			if (m_iCurVotedCount >= 7)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_COLLECTION_ALLOWED_RANGE_VOTE_COMPLETE);
			}
			NKCPacketSender.Send_NKMPacket_UNIT_REVIEW_TAG_VOTE_REQ(m_CurUnitID, tagType);
		}
		m_iClickSlotIdx = slotIdx;
	}

	public void OnRecvReviewTagVoteCancelAck(NKMPacket_UNIT_REVIEW_TAG_VOTE_CANCEL_ACK sPacket)
	{
		UpdateMyVoteCount(sPacket.unitID);
		UpdateTagData(sPacket.unitID);
	}

	public void OnRecvReviewTagVoteAck(NKMPacket_UNIT_REVIEW_TAG_VOTE_ACK sPacket)
	{
		UpdateMyVoteCount(sPacket.unitID);
		UpdateTagData(sPacket.unitID, bResetScroll: false);
	}

	public void OnRecvReviewTagListAck(NKMPacket_UNIT_REVIEW_TAG_LIST_ACK sPacket)
	{
		List<NKMUnitReviewTagData> unitReviewTagDataList = sPacket.unitReviewTagDataList;
		List<NKCUnitTagData> list = new List<NKCUnitTagData>();
		for (int i = 0; i < unitReviewTagDataList.Count; i++)
		{
			NKCUnitTagData item = new NKCUnitTagData(unitReviewTagDataList[i].tagType, unitReviewTagDataList[i].isVoted, unitReviewTagDataList[i].votedCount, top: false);
			if (unitReviewTagDataList[i].isVoted)
			{
				m_iCurVotedCount++;
			}
			list.Add(item);
		}
		NKCUtil.SetLabelText(m_USER_TAG_MY_OPINION_COUNT_TEXT, $"{m_iCurVotedCount}/{7}");
		NKCCollectionManager.SetUnitTagData(m_CurUnitID, list);
		SortTagContent(m_eOldTagSortType);
		UpdateTagInfo(m_CurUnitID);
	}

	private void UpdateMyVoteCount(int unitID)
	{
		m_iCurVotedCount = 0;
		List<NKCUnitTagData> unitTagData = NKCCollectionManager.GetUnitTagData(unitID);
		if (unitTagData != null)
		{
			for (int i = 0; i < unitTagData.Count; i++)
			{
				if (unitTagData[i].Voted)
				{
					m_iCurVotedCount++;
				}
			}
		}
		NKCUtil.SetLabelText(m_USER_TAG_MY_OPINION_COUNT_TEXT, $"{m_iCurVotedCount}/{7}");
	}

	private void OnUnitVoice()
	{
		if (m_NKMUnitData != null)
		{
			bool bLifetime = false;
			NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
			if (armyData.IsCollectedUnit(m_UnitID))
			{
				bLifetime = armyData.SearchUnitByID(NKM_UNIT_TYPE.NUT_NORMAL, m_CurUnitID, NKMArmyData.UNIT_SEARCH_OPTION.Devotion, 0);
			}
			if (m_SkinID > 0)
			{
				NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(m_SkinID);
				NKCUIPopupVoice.Instance.Open(skinTemplet, bLifetime);
			}
			else
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_UnitID);
				NKCUIPopupVoice.Instance.Open(unitTempletBase, bLifetime);
			}
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

	private void OnReplayLifetime()
	{
		NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_LIFETIME_REPLAY, delegate
		{
			NKCUILifetime.Instance.Open(m_NKMUnitData, replay: true);
		});
	}

	public void OpenUnitInfoDetailPopup()
	{
		if (NKCPopupUnitInfoDetail.IsInstanceOpen)
		{
			NKCPopupUnitInfoDetail.CheckInstanceAndClose();
		}
		else
		{
			NKCPopupUnitInfoDetail.InstanceOpen(m_NKMUnitData, NKCPopupUnitInfoDetail.UnitInfoDetailType.gauntlet, m_listNKMEquipItemData);
		}
	}

	public void UpdateEquipSlots()
	{
		if (m_NKMUnitData != null)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_UnitID);
			if (unitTempletBase != null && unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
			{
				NKCUtil.SetGameobjectActive(m_objEquipSlotParent, bValue: false);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objEquipSlotParent, bValue: true);
				UpdateEquipItemSlot(m_NKMUnitData.GetEquipItemWeaponUid(), ref m_slotEquipWeapon, null, m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_WEAPON, m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_WEAPON);
				UpdateEquipItemSlot(m_NKMUnitData.GetEquipItemDefenceUid(), ref m_slotEquipDefense, null, m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_DEFENCE, m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_DEFENCE);
				UpdateEquipItemSlot(m_NKMUnitData.GetEquipItemAccessoryUid(), ref m_slotEquipAcc, null, m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC, m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC);
				UpdateEquipItemSlot(m_NKMUnitData.GetEquipItemAccessory2Uid(), ref m_slotEquipAcc_2, null, m_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC_02, m_ani_NKM_UI_UNIT_INFO_EQUIP_SET_FX_ACC_02);
			}
			if (!m_NKMUnitData.IsUnlockAccessory2())
			{
				m_slotEquipAcc_2.SetLock(OnSetLockMessage);
				m_slotEquipAcc_2.Set_EQUIP_BOX_BOTTOM_MENU_TYPE(NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
			}
			SetDetailedStat(m_NKMUnitData);
		}
		else
		{
			m_slotEquipAcc_2.SetLock(OnSetLockMessage);
			m_slotEquipAcc_2.Set_EQUIP_BOX_BOTTOM_MENU_TYPE(NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
		}
	}

	private void OnSetLockMessage(NKCUISlot.SlotData slotData, bool bLocked)
	{
		NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_EQUIP_ACC_2_LOCKED_DESC);
	}

	private NKMEquipItemData GetItemEquip(long itemUID)
	{
		if (m_listNKMEquipItemData != null)
		{
			return m_listNKMEquipItemData.Find((NKMEquipItemData x) => x.m_ItemUid == itemUID);
		}
		return null;
	}

	private void UpdateEquipItemSlot(long equipItemUID, ref NKCUISlot slot, NKCUISlot.OnClick func, GameObject effObj, Animator effAni)
	{
		bool flag = false;
		NKCUtil.SetGameobjectActive(effObj, bValue: true);
		if (equipItemUID > 0)
		{
			NKMEquipItemData itemEquip = GetItemEquip(equipItemUID);
			if (itemEquip != null)
			{
				slot.SetData(NKCUISlot.SlotData.MakeEquipData(itemEquip), bShowName: false, bShowNumber: true, bEnableLayoutElement: false, OpenEquipBoxForInspection);
				flag = true;
				if (NKMItemManager.IsActiveSetOptionItem(itemEquip) && effAni != null)
				{
					NKMItemEquipSetOptionTemplet equipSetOptionTemplet = NKMItemManager.GetEquipSetOptionTemplet(itemEquip.m_SetOptionId);
					if (equipSetOptionTemplet != null)
					{
						effAni.SetTrigger(equipSetOptionTemplet.m_EquipSetIconEffect);
					}
				}
			}
		}
		NKCUtil.SetGameobjectActive(effObj, bValue: false);
		if (!flag)
		{
			slot.SetCustomizedEmptySP(GetCustomizedEquipEmptySP());
			slot.SetEmpty(func);
		}
		slot.SetUsedMark(bVal: false);
	}

	private Sprite GetCustomizedEquipEmptySP()
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_NKMUnitData.m_UnitID);
		if (unitTempletBase == null)
		{
			return null;
		}
		NKM_UNIT_STYLE_TYPE nKM_UNIT_STYLE_TYPE = unitTempletBase.m_NKM_UNIT_STYLE_TYPE;
		if ((uint)(nKM_UNIT_STYLE_TYPE - 1) <= 2u)
		{
			return NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_inven_icon_common", "AB_INVEN_ICON_FRAME_EMPTY");
		}
		return null;
	}

	private void OpenEquipBoxForInspection(NKCUISlot.SlotData slotData, bool bLocked)
	{
		NKMEquipItemData itemEquip = GetItemEquip(slotData.UID);
		if (itemEquip != null)
		{
			NKCPopupItemEquipBox.Open(itemEquip, NKCPopupItemEquipBox.EQUIP_BOX_BOTTOM_MENU_TYPE.EBBMT_NONE);
		}
	}

	public void UpdateUnitMissionRedDot()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_COLLECTION_UNIT_ACHIEVEMENT_BUTTON_NEW, NKCUnitMissionManager.HasRewardEnableMission(m_UnitID));
	}

	private void OnClickUnitAchievement()
	{
		NKCUIPopupCollectionAchievement.Instance.Open(m_UnitID);
	}
}
