using System;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Item;
using ClientPacket.Unit;
using ClientPacket.User;
using ClientPacket.WorldMap;
using Cs.Math;
using NKC.Templet;
using NKC.UI.Collection;
using NKC.UI.Component;
using NKC.UI.Shop;
using NKM;
using NKM.Templet;
using NKM.Templet.Recall;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIUnitInfo : NKCUIBase
{
	public delegate void OnRemoveFromDeck(NKMUnitData unitData);

	public class OpenOption
	{
		public readonly List<long> m_UnitUIDList = new List<long>();

		public readonly List<NKMUnitData> m_lstUnitData = new List<NKMUnitData>();

		public int m_SelectSlotIndex;

		public bool m_bShowFierceInfo;

		public OpenOption(List<long> UnitUIDList, int SlotIdx = 0)
		{
			if (UnitUIDList != null && UnitUIDList.Count != 0)
			{
				m_UnitUIDList = UnitUIDList;
				m_SelectSlotIndex = SlotIdx;
			}
		}

		public OpenOption(List<NKMUnitData> lstUnitData, int SlotIdx = 0)
		{
			if (lstUnitData != null && lstUnitData.Count != 0)
			{
				m_lstUnitData = lstUnitData;
				m_SelectSlotIndex = SlotIdx;
			}
		}

		public void FilterUnitlist()
		{
			if (m_lstUnitData != null && m_lstUnitData.Count != 0)
			{
				long oldSelectedUnitUID = 0L;
				if (m_SelectSlotIndex >= 0 && m_SelectSlotIndex < m_lstUnitData.Count)
				{
					oldSelectedUnitUID = ((m_lstUnitData[m_SelectSlotIndex] != null) ? m_lstUnitData[m_SelectSlotIndex].m_UnitUID : 0);
				}
				m_lstUnitData.RemoveAll(UnitRemoveRequired);
				int num = m_lstUnitData.FindIndex((NKMUnitData x) => x != null && x.m_UnitUID == oldSelectedUnitUID);
				if (num < 0)
				{
					num = 0;
				}
				m_SelectSlotIndex = num;
			}
		}

		private bool UnitRemoveRequired(NKMUnitData unitData)
		{
			return NKMUnitManager.GetUnitTempletBase(unitData)?.IsUnitDescNullOrEmplty() ?? true;
		}
	}

	public enum UNIT_INFO_TAB_STATE
	{
		NONE,
		BASE,
		NEGOTIATION,
		LIMIT_BREAK,
		SKILL_TRAIN
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_unit_info";

	private const string UI_ASSET_NAME = "NKM_UI_UNIT_INFO";

	private static NKCUIUnitInfo m_Instance;

	private NKMUnitData m_NKMUnitData;

	[Header("요약정보창")]
	public NKCUICharInfoSummary m_NKCUICharInfoSummary;

	[Header("지부장 표시")]
	public GameObject m_objCityLeader;

	[Header("애사심")]
	public NKCUIComStateButton m_btnLoyalty;

	public NKCUIComUnitLoyalty m_Loyalty;

	[Header("우측 상단 토글")]
	public NKCUIComToggle m_tglInfo;

	public NKCUIComToggle m_tglNegotiation;

	public NKCUIComToggle m_tglLimitBreak;

	public NKCUIComToggle m_tglSkillTrain;

	public GameObject m_objNegotiationLock;

	public GameObject m_objLimitBreakLock;

	public GameObject m_objSkillTrainLock;

	[Header("우측 서브UI")]
	public NKCUIUnitInfoInfo m_NKCUIUnitInfoInfo;

	public NKCUIUnitInfoLimitBreak m_NKCUIUnitInfoLimitBreak;

	public NKCUIUnitInfoNegotiation m_NKCUIUnitInfoNegotiation;

	public NKCUIUnitInfoSkillTrain m_NKCUIUnitInfoSkillTrain;

	public GameObject m_UnitInfoBlock;

	[Header("유닛 일러스트")]
	public NKCUIComDragSelectablePanel m_DragCharacterView;

	public EventTrigger m_evtPanel;

	[Header("기타 버튼")]
	public CanvasGroup m_cgPractice;

	public NKCUIComButton m_btnPractice;

	public NKCUIComButton m_cbtnChangeIllust;

	public NKCUIComToggle m_ctglLock;

	public NKCUIComToggle m_ctglFavorite;

	public NKCUIComButton m_cbtnReview;

	public CanvasGroup m_cgSkinMode;

	public NKCUIComButton m_cbtnSkinMode;

	public Text m_lbSkinCount;

	public GameObject m_SKIN_COUNT_ROOT;

	public GameObject m_SKIN_COMPLETE;

	public NKCUIComButton m_cbtnCollection;

	public GameObject m_objCollectionReward;

	public CanvasGroup m_cgVoice;

	public NKCUIComButton m_cbtnVoicePopup;

	[Space]
	public GameObject m_UnitInfoControlBtn;

	public GameObject m_UnitInfoBottomBtn;

	[Space]
	public GameObject m_objSeized;

	public NKCUIComStateButton m_ChangeBtn;

	[Header("리콜")]
	public GameObject m_objRecall;

	public NKCUIComStateButton m_btnRecall;

	public Text m_lbRecallTime;

	[Header("성우")]
	public Text m_lbVoiceActor;

	[Header("추천 장비")]
	public CanvasGroup m_cgRecommend;

	public NKCUIComStateButton m_btnRecommendEquip;

	public NKCUIUnitRecommendSetOption m_NKCUIUnitRecommendSetOption;

	public NKCUIUnitRecommendEquipList m_NKCUIUnitRecommendEquipList;

	[Header("유닛 배치상태")]
	public NKCUIComStateButton m_csbtnUnitPlacement;

	[Header("유닛 태그")]
	public NKCUIComUnitTagList m_NKCUIUnitTagList;

	[Header("기타")]
	public Image m_BG;

	private OnRemoveFromDeck dOnRemoveFromDeck;

	private long m_lReserveUID;

	private bool m_bShowFierceInfo;

	private UNIT_INFO_TAB_STATE m_curUIState;

	private NKCUnitSortSystem.UnitListOptions m_preUnitListOption;

	private Dictionary<long, int> dicUnitLimitBreak = new Dictionary<long, int>();

	private List<NKMUnitData> m_UnitSortList = new List<NKMUnitData>();

	private NKCUIUnitSelectList m_UIUnitSelectList;

	private int m_PlayVoiceSoundID;

	private float m_fDeltaTime;

	public static NKCUIUnitInfo Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIUnitInfo>("ab_ui_nkm_ui_unit_info", "NKM_UI_UNIT_INFO", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIUnitInfo>();
				m_Instance.InitUI();
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

	public override string MenuName => m_curUIState switch
	{
		UNIT_INFO_TAB_STATE.NEGOTIATION => NKCUtilString.GET_STRING_NEGOTIATE, 
		UNIT_INFO_TAB_STATE.LIMIT_BREAK => NKCStringTable.GetString("SI_DP_LAB_MENU_NAME_LDS_UNIT_LIMITBREAK"), 
		UNIT_INFO_TAB_STATE.SKILL_TRAIN => NKCStringTable.GetString("SI_DP_LAB_MENU_NAME_LDS_UNIT_SKILL_TRAIN"), 
		_ => NKCUtilString.GET_STRING_UNIT_INFO, 
	};

	public override List<int> UpsideMenuShowResourceList
	{
		get
		{
			if (m_curUIState == UNIT_INFO_TAB_STATE.SKILL_TRAIN)
			{
				return new List<int> { 3, 1, 2, 101 };
			}
			return base.UpsideMenuShowResourceList;
		}
	}

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string GuideTempletID => m_curUIState switch
	{
		UNIT_INFO_TAB_STATE.NEGOTIATION => "ARTICLE_UNIT_NEGOTIATION", 
		UNIT_INFO_TAB_STATE.LIMIT_BREAK => "ARTICLE_UNIT_LIMITBREAK", 
		UNIT_INFO_TAB_STATE.SKILL_TRAIN => "ARTICLE_UNIT_TRAINING", 
		_ => "ARTICLE_UNIT_INFO", 
	};

	private NKCUIUnitSelectList UnitSelectList
	{
		get
		{
			if (m_UIUnitSelectList == null)
			{
				m_UIUnitSelectList = NKCUIUnitSelectList.OpenNewInstance();
			}
			return m_UIUnitSelectList;
		}
	}

	public NKCUIEquipPreset EquipPreset => m_NKCUIUnitInfoInfo.EquipPreset;

	public static NKCUIUnitInfo OpenNewInstance()
	{
		NKCUIUnitInfo instance = NKCUIManager.OpenNewInstance<NKCUIUnitInfo>("ab_ui_nkm_ui_unit_info", "NKM_UI_UNIT_INFO", NKCUIManager.eUIBaseRect.UIFrontCommon, null).GetInstance<NKCUIUnitInfo>();
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

	public override void CloseInternal()
	{
		BannerCleanUp();
		m_lReserveUID = 0L;
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: false);
		}
		if (NKCUIInventory.IsInstanceLoaded)
		{
			NKCUIInventory.Instance.ClearCachingData();
		}
		NKCPopupUnitInfoDetail.CheckInstanceAndClose();
		NKCUIPopupIllustView.CheckInstanceAndClose();
		NKCUIUnitPlacement.CheckInstanceAndClose();
		if (m_UIUnitSelectList != null && NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_UNIT_LIST)
		{
			UnitSelectList.Close();
		}
		m_UIUnitSelectList = null;
		m_NKCUIUnitInfoInfo.Clear();
		m_NKCUIUnitInfoLimitBreak.Clear();
		m_NKCUIUnitInfoSkillTrain.Clear();
		m_NKCUIUnitInfoNegotiation.Clear();
		m_curUIState = UNIT_INFO_TAB_STATE.NONE;
	}

	public override void OnBackButton()
	{
		base.OnBackButton();
	}

	public override void UnHide()
	{
		base.UnHide();
		if (m_curUIState == UNIT_INFO_TAB_STATE.BASE)
		{
			m_NKCUIUnitInfoInfo.UnActiveEffect();
		}
		UpdateSkinButton();
		CheckTabLock();
		TutorialCheck();
	}

	public override void OnUnitUpdate(NKMUserData.eChangeNotifyType eEventType, NKM_UNIT_TYPE eUnitType, long uid, NKMUnitData unitData)
	{
		if (eEventType == NKMUserData.eChangeNotifyType.Update && uid == m_NKMUnitData.m_UnitUID)
		{
			m_NKMUnitData = unitData;
			UpdateUnitData();
			OnSkinEquip(uid, m_NKMUnitData.m_SkinID);
			m_NKCUICharInfoSummary.SetData(unitData);
			if (m_NKCUIUnitTagList != null)
			{
				m_NKCUIUnitTagList.SetData(unitData);
			}
			m_NKMUnitData = unitData;
			int num = m_UnitSortList.FindIndex((NKMUnitData v) => v.m_UnitUID == uid);
			if (num >= 0)
			{
				m_UnitSortList[num] = unitData;
			}
			if (m_DragCharacterView != null)
			{
				RectTransform currentItem = m_DragCharacterView.GetCurrentItem();
				if (currentItem != null)
				{
					NKCUICharacterView componentInChildren = currentItem.gameObject.GetComponentInChildren<NKCUICharacterView>();
					if (componentInChildren != null)
					{
						componentInChildren.SetCharacterIllust(unitData);
					}
				}
			}
			switch (m_curUIState)
			{
			case UNIT_INFO_TAB_STATE.LIMIT_BREAK:
				m_NKCUIUnitInfoLimitBreak.OnUnitUpdate(uid, unitData);
				break;
			case UNIT_INFO_TAB_STATE.SKILL_TRAIN:
				m_NKCUIUnitInfoSkillTrain.OnUnitUpdate(uid, unitData);
				break;
			case UNIT_INFO_TAB_STATE.NEGOTIATION:
				m_NKCUIUnitInfoNegotiation.OnUnitUpdate(uid, unitData);
				break;
			}
		}
		if (eEventType != NKMUserData.eChangeNotifyType.Remove)
		{
			return;
		}
		int num2 = m_UnitSortList.FindIndex((NKMUnitData v) => v.m_UnitUID == uid);
		if (num2 >= 0)
		{
			m_UnitSortList.RemoveAt(num2);
			int index = m_UnitSortList.FindIndex((NKMUnitData v) => v.m_UnitUID == m_NKMUnitData.m_UnitUID);
			m_DragCharacterView.TotalCount = m_UnitSortList.Count;
			m_DragCharacterView.SetIndex(index);
		}
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		switch (m_curUIState)
		{
		case UNIT_INFO_TAB_STATE.LIMIT_BREAK:
			m_NKCUIUnitInfoLimitBreak.OnInventoryChange(itemData);
			break;
		case UNIT_INFO_TAB_STATE.SKILL_TRAIN:
			m_NKCUIUnitInfoSkillTrain.OnInventoryChange(itemData);
			break;
		case UNIT_INFO_TAB_STATE.NEGOTIATION:
			m_NKCUIUnitInfoNegotiation.OnInventoryChange(itemData);
			break;
		}
	}

	public override void OnEquipChange(NKMUserData.eChangeNotifyType eType, long equipUID, NKMEquipItemData equipItem)
	{
		if (eType == NKMUserData.eChangeNotifyType.Update && equipItem.m_OwnerUnitUID == m_NKMUnitData.m_UnitUID && m_curUIState == UNIT_INFO_TAB_STATE.BASE)
		{
			UpdateUnitData();
		}
		if ((eType == NKMUserData.eChangeNotifyType.Update || eType == NKMUserData.eChangeNotifyType.Remove) && EquipPresetOpened())
		{
			m_NKCUIUnitInfoInfo.EquipPreset?.UpdatePresetData(null, setScrollPositon: false, 0, forceRefresh: true);
		}
	}

	public override void OnCompanyBuffUpdate(NKMUserData userData)
	{
		m_NKCUIUnitInfoNegotiation.OnCompanyBuffUpdate();
	}

	public void InitUI()
	{
		m_NKCUIUnitInfoInfo.Init();
		m_NKCUIUnitInfoLimitBreak.Init(GetUnitSortList, ChangeUnitByOtherUI);
		m_NKCUIUnitInfoNegotiation.Init();
		m_NKCUIUnitInfoSkillTrain.Init();
		m_NKCUIUnitRecommendSetOption.Init(OnCloseRecommend, AutoEquipBySetOption);
		m_NKCUIUnitRecommendEquipList.Init();
		if (m_cbtnChangeIllust != null)
		{
			m_cbtnChangeIllust.PointerClick.RemoveAllListeners();
			m_cbtnChangeIllust.PointerClick.AddListener(OnClickChangeIllust);
		}
		if (m_ctglLock != null)
		{
			m_ctglLock.OnValueChanged.RemoveAllListeners();
			m_ctglLock.OnValueChanged.AddListener(OnLockToggle);
		}
		NKCUtil.SetToggleValueChangedDelegate(m_ctglFavorite, OnFavoriteToggle);
		if (m_btnPractice != null)
		{
			m_btnPractice.PointerClick.RemoveAllListeners();
			m_btnPractice.PointerClick.AddListener(OnUnitTestButton);
		}
		if (m_cbtnReview != null)
		{
			m_cbtnReview.PointerClick.RemoveAllListeners();
			m_cbtnReview.PointerClick.AddListener(OnReviewButton);
		}
		NKCUtil.SetGameobjectActive(m_cbtnReview, NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.UNIT_REVIEW_SYSTEM));
		if (m_cbtnSkinMode != null)
		{
			m_cbtnSkinMode.PointerClick.RemoveAllListeners();
			m_cbtnSkinMode.PointerClick.AddListener(OnSkinButton);
		}
		if (m_cbtnCollection != null)
		{
			m_cbtnCollection.PointerClick.RemoveAllListeners();
			m_cbtnCollection.PointerClick.AddListener(OnCollectionButton);
		}
		if (m_cbtnVoicePopup != null)
		{
			m_cbtnVoicePopup.PointerClick.RemoveAllListeners();
			m_cbtnVoicePopup.PointerClick.AddListener(OnVoiceButton);
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
		NKCUtil.SetBindFunction(m_btnLoyalty, OnTouchLoyalty);
		NKCUtil.SetBindFunction(m_ChangeBtn, OnSelectUnit);
		if (null != m_tglInfo)
		{
			m_tglInfo.OnValueChanged.RemoveAllListeners();
			m_tglInfo.OnValueChanged.AddListener(delegate(bool bSet)
			{
				if (bSet)
				{
					ChangeState(UNIT_INFO_TAB_STATE.BASE);
				}
			});
		}
		if (null != m_tglNegotiation)
		{
			m_tglNegotiation.OnValueChanged.RemoveAllListeners();
			m_tglNegotiation.OnValueChanged.AddListener(delegate(bool bSet)
			{
				if (bSet)
				{
					ChangeState(UNIT_INFO_TAB_STATE.NEGOTIATION);
				}
			});
		}
		if (null != m_tglLimitBreak)
		{
			m_tglLimitBreak.OnValueChanged.RemoveAllListeners();
			m_tglLimitBreak.OnValueChanged.AddListener(delegate(bool bSet)
			{
				if (bSet)
				{
					ChangeState(UNIT_INFO_TAB_STATE.LIMIT_BREAK);
				}
			});
		}
		if (null != m_tglSkillTrain)
		{
			m_tglSkillTrain.OnValueChanged.RemoveAllListeners();
			m_tglSkillTrain.OnValueChanged.AddListener(delegate(bool bSet)
			{
				if (bSet)
				{
					ChangeState(UNIT_INFO_TAB_STATE.SKILL_TRAIN);
				}
			});
		}
		if (m_btnRecall != null)
		{
			m_btnRecall.PointerClick.RemoveAllListeners();
			m_btnRecall.PointerClick.AddListener(OnClickRecall);
		}
		if (m_btnRecommendEquip != null)
		{
			m_btnRecommendEquip.PointerClick.RemoveAllListeners();
			m_btnRecommendEquip.PointerClick.AddListener(OnClickRecommend);
		}
		NKCUtil.SetButtonClickDelegate(m_csbtnUnitPlacement, OnClickPlacement);
		m_NKCUICharInfoSummary.SetUnitClassRootActive(value: false);
		m_NKCUICharInfoSummary.Init();
		base.gameObject.SetActive(value: false);
		m_lReserveUID = 0L;
	}

	public NKMUnitData GetNKMUnitData()
	{
		return m_NKMUnitData;
	}

	public void Open(NKMUnitData cNKMUnitData, OnRemoveFromDeck onRemoveFromDeck, OpenOption openOption = null, NKC_SCEN_UNIT_LIST.eUIOpenReserve ReserveUI = NKC_SCEN_UNIT_LIST.eUIOpenReserve.Nothing)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCPopupUnitInfoDetail.CheckInstanceAndClose();
		dOnRemoveFromDeck = onRemoveFromDeck;
		m_NKMUnitData = cNKMUnitData;
		UpdateUnitList(openOption, cNKMUnitData.m_UnitUID);
		m_bShowFierceInfo = openOption.m_bShowFierceInfo;
		switch (ReserveUI)
		{
		case NKC_SCEN_UNIT_LIST.eUIOpenReserve.UnitLimitbreak:
			m_tglLimitBreak.Select(bSelect: true, bForce: true, bImmediate: true);
			ChangeState(UNIT_INFO_TAB_STATE.LIMIT_BREAK);
			break;
		case NKC_SCEN_UNIT_LIST.eUIOpenReserve.UnitNegotiate:
			m_tglNegotiation.Select(bSelect: true, bForce: true, bImmediate: true);
			ChangeState(UNIT_INFO_TAB_STATE.NEGOTIATION);
			break;
		case NKC_SCEN_UNIT_LIST.eUIOpenReserve.UnitSkillTraining:
			m_tglSkillTrain.Select(bSelect: true, bForce: true, bImmediate: true);
			ChangeState(UNIT_INFO_TAB_STATE.SKILL_TRAIN);
			break;
		default:
			m_tglInfo.Select(bSelect: true, bForce: true, bImmediate: true);
			ChangeState(UNIT_INFO_TAB_STATE.BASE);
			break;
		}
		m_fDeltaTime = 0f;
		NKCUtil.SetGameobjectActive(m_objRecall, NKCRecallManager.IsRecallTargetUnit(m_NKMUnitData, NKCSynchronizedTime.GetServerUTCTime()));
		if (m_objRecall.activeSelf)
		{
			SetRecallRemainTime();
		}
		m_NKCUICharInfoSummary.SetData(m_NKMUnitData);
		if (m_NKCUIUnitTagList != null)
		{
			m_NKCUIUnitTagList.SetData(m_NKMUnitData);
		}
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		m_lReserveUID = m_NKMUnitData.m_UnitUID;
		CheckTabLock();
		UIOpened();
	}

	private void CheckTabLock()
	{
		NKCUtil.SetGameobjectActive(m_objLimitBreakLock, !NKCContentManager.IsContentsUnlocked(ContentsType.LAB_LIMITBREAK));
		NKCUtil.SetGameobjectActive(m_objNegotiationLock, !NKCContentManager.IsContentsUnlocked(ContentsType.PERSONNAL_NEGO));
		NKCUtil.SetGameobjectActive(m_objSkillTrainLock, !NKCContentManager.IsContentsUnlocked(ContentsType.LAB_TRAINING));
	}

	private void UpdateUnitList(OpenOption openOption, long selectedUnitUID)
	{
		m_UnitSortList.Clear();
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData != null)
		{
			NKMArmyData armyData = nKMUserData.m_ArmyData;
			if (armyData != null)
			{
				foreach (long unitUID in openOption.m_UnitUIDList)
				{
					NKMUnitData unitOrTrophyFromUID = armyData.GetUnitOrTrophyFromUID(unitUID);
					if (unitOrTrophyFromUID != null)
					{
						m_UnitSortList.Add(unitOrTrophyFromUID);
					}
				}
				foreach (NKMUnitData unitData in openOption.m_lstUnitData)
				{
					if (m_UnitSortList.Find((NKMUnitData x) => x.m_UnitUID == unitData.m_UnitUID) == null && armyData.GetUnitOrTrophyFromUID(unitData.m_UnitUID) != null)
					{
						m_UnitSortList.Add(unitData);
					}
				}
				if (m_UnitSortList.Find((NKMUnitData x) => x.m_UnitUID == selectedUnitUID) == null)
				{
					m_UnitSortList.Add(armyData.GetUnitOrTrophyFromUID(selectedUnitUID));
				}
			}
		}
		int index = 0;
		for (int num = 0; num < m_UnitSortList.Count; num++)
		{
			if (m_UnitSortList[num].m_UnitUID == selectedUnitUID)
			{
				index = num;
				break;
			}
		}
		m_DragCharacterView.TotalCount = m_UnitSortList.Count;
		m_DragCharacterView.SetIndex(index);
	}

	private void UpdateUnitInfoUI()
	{
		if (m_NKMUnitData != null)
		{
			NKMWorldMapManager.WorldMapLeaderState unitWorldMapLeaderState = NKMWorldMapManager.GetUnitWorldMapLeaderState(NKCScenManager.CurrentUserData(), m_NKMUnitData.m_UnitUID);
			NKCUtil.SetGameobjectActive(m_objCityLeader, unitWorldMapLeaderState != NKMWorldMapManager.WorldMapLeaderState.None);
			if (m_ctglLock != null)
			{
				m_ctglLock.Select(m_NKMUnitData.m_bLock, bForce: true, bImmediate: true);
			}
			if (m_ctglFavorite != null)
			{
				m_ctglFavorite.Select(m_NKMUnitData.isFavorite, bForce: true, bImmediate: true);
			}
			NKCUtil.SetGameobjectActive(m_objSeized, m_NKMUnitData.IsSeized);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objCityLeader, bValue: false);
		}
		if (m_Loyalty != null)
		{
			m_Loyalty.SetLoyalty(m_NKMUnitData);
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_NKMUnitData.m_UnitID);
		if (unitTempletBase != null && unitTempletBase.IsTrophy)
		{
			m_btnPractice.enabled = false;
			m_cgPractice.alpha = 0.4f;
			NKCUtil.SetGameobjectActive(m_UnitInfoBlock, bValue: true);
			NKCUtil.SetGameobjectActive(m_Loyalty, bValue: false);
		}
		else if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM)
		{
			m_cbtnSkinMode.enabled = false;
			m_cgSkinMode.alpha = 0.4f;
			m_btnPractice.enabled = false;
			m_cgPractice.alpha = 0.4f;
			NKCUtil.SetGameobjectActive(m_UnitInfoBlock, bValue: false);
			NKCUtil.SetGameobjectActive(m_Loyalty, bValue: true);
		}
		else
		{
			m_btnPractice.enabled = true;
			m_cgPractice.alpha = 1f;
			NKCUtil.SetGameobjectActive(m_UnitInfoBlock, bValue: false);
			NKCUtil.SetGameobjectActive(m_Loyalty, bValue: true);
		}
		NKCUtil.SetGameobjectActive(m_csbtnUnitPlacement, NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_UNIT_LIST && !unitTempletBase.IsTrophy);
		if (m_NKCUIUnitRecommendSetOption.gameObject.activeSelf)
		{
			m_NKCUIUnitRecommendSetOption.Close();
		}
		if (m_NKCUIUnitRecommendEquipList.gameObject.activeSelf)
		{
			m_NKCUIUnitRecommendEquipList.Close();
		}
		NKCUtil.SetGameobjectActive(m_cgRecommend.gameObject, unitTempletBase.IsUnitStyleType());
		NKCUnitEquipRecommendTemplet nKCUnitEquipRecommendTemplet = NKCUnitEquipRecommendTemplet.Find(m_NKMUnitData.m_UnitID);
		if (nKCUnitEquipRecommendTemplet != null && nKCUnitEquipRecommendTemplet.m_lstSetOptionID.Count > 0)
		{
			m_cgRecommend.alpha = 1f;
			m_btnRecommendEquip.enabled = true;
		}
		else
		{
			m_cgRecommend.alpha = 0.4f;
			m_btnRecommendEquip.enabled = false;
		}
		if (NKCCollectionManager.IsCollectionV2Active)
		{
			NKCUtil.SetGameobjectActive(m_cgVoice.gameObject, bValue: false);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_cgVoice.gameObject, bValue: true);
		}
		if (NKCPopupUnitInfoDetail.IsInstanceOpen)
		{
			NKCPopupUnitInfoDetail.Instance.SetData(m_NKMUnitData);
		}
		NKCUtil.SetLabelText(m_lbVoiceActor, NKCVoiceActorNameTemplet.FindActorName(m_NKMUnitData));
		bool bValue = false;
		if (m_cbtnCollection != null && NKCCollectionManager.GetUnitTemplet(m_NKMUnitData.m_UnitID) != null)
		{
			bValue = true;
		}
		NKCUtil.SetGameobjectActive(m_cbtnCollection, bValue);
		bool bValue2 = false;
		if (m_cbtnCollection != null && m_cbtnCollection.gameObject.activeSelf && m_NKMUnitData != null)
		{
			bValue2 = NKCUnitMissionManager.HasRewardEnableMission(m_NKMUnitData.m_UnitID);
		}
		NKCUtil.SetGameobjectActive(m_objCollectionReward, bValue2);
		UpdateSkinButton();
	}

	private void UpdateSkinButton()
	{
		if (m_NKMUnitData != null && NKMSkinManager.IsCharacterHasSkin(m_NKMUnitData.m_UnitID))
		{
			List<NKMSkinTemplet> skinlistForCharacter = NKMSkinManager.GetSkinlistForCharacter(m_NKMUnitData.m_UnitID, NKCScenManager.CurrentUserData().m_InventoryData);
			if (skinlistForCharacter == null)
			{
				NKCUtil.SetGameobjectActive(m_SKIN_COMPLETE, bValue: false);
				NKCUtil.SetGameobjectActive(m_SKIN_COUNT_ROOT, bValue: false);
				return;
			}
			int num = 0;
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			foreach (NKMSkinTemplet item in skinlistForCharacter)
			{
				if (item != null && nKMUserData != null && nKMUserData.m_InventoryData.HasItemSkin(item.m_SkinID))
				{
					num++;
				}
			}
			bool flag = num >= skinlistForCharacter.Count;
			NKCUtil.SetGameobjectActive(m_SKIN_COMPLETE, flag);
			NKCUtil.SetGameobjectActive(m_SKIN_COUNT_ROOT, !flag);
			if (!flag)
			{
				NKCUtil.SetLabelText(m_lbSkinCount, $"{num + 1}/{skinlistForCharacter.Count + 1}");
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_SKIN_COMPLETE, bValue: false);
			NKCUtil.SetGameobjectActive(m_SKIN_COUNT_ROOT, bValue: false);
		}
	}

	private void OnUnitTestButton()
	{
		if (!IsSeizedUnit(m_NKMUnitData))
		{
			NKCScenManager.GetScenManager().Get_SCEN_GAME().OpenPracticeGameComfirmPopup(m_NKMUnitData);
		}
	}

	private void OnReviewButton()
	{
		NKCUIUnitReview.Instance.OpenUI(m_NKMUnitData.m_UnitID);
	}

	private void OnSkinButton()
	{
		if (!IsSeizedUnit(m_NKMUnitData))
		{
			NKCUIShopSkinPopup.Instance.OpenForUnitInfo(m_NKMUnitData);
		}
	}

	private void OnCollectionButton()
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_NKMUnitData);
		if (unitTempletBase != null)
		{
			NKCUICollectionUnitInfo.CheckInstanceAndOpen(NKCUtil.MakeDummyUnit(unitTempletBase.m_UnitID, 100, 3), null);
		}
	}

	private void OnVoiceButton()
	{
		NKCUIPopupVoice.Instance.Open(m_NKMUnitData);
	}

	private void OnClickChangeIllust()
	{
		NKCUIPopupIllustView.Instance.Open(m_NKMUnitData);
	}

	private bool IsEquipPresetOpend()
	{
		if (m_curUIState == UNIT_INFO_TAB_STATE.BASE)
		{
			return m_NKCUIUnitInfoInfo.IsPresetOpend();
		}
		return false;
	}

	private void OnLockToggle(bool bValue)
	{
		if (bValue != m_NKMUnitData.m_bLock)
		{
			NKCPacketSender.Send_NKMPacket_LOCK_UNIT_REQ(m_NKMUnitData.m_UnitUID, !m_NKMUnitData.m_bLock);
		}
	}

	private void OnFavoriteToggle(bool bValue)
	{
		if (bValue != m_NKMUnitData.isFavorite)
		{
			NKCPacketSender.Send_NKMPacket_FAVORITE_UNIT_REQ(m_NKMUnitData.m_UnitUID, !m_NKMUnitData.isFavorite);
		}
	}

	public void UpdateLock(long UnitUID, bool bLock)
	{
		if (m_NKMUnitData != null && m_NKMUnitData.m_UnitUID == UnitUID)
		{
			m_ctglLock.Select(bLock, bForce: true);
		}
	}

	public void UpdateFavorite(long UnitUID, bool bFavorite)
	{
		if (m_NKMUnitData != null && m_NKMUnitData.m_UnitUID == UnitUID)
		{
			m_ctglFavorite.Select(bFavorite, bForce: true);
		}
	}

	private void OnSkinEquip(long unitUID, int equippedSkinID)
	{
		foreach (NKMUnitData unitSort in m_UnitSortList)
		{
			if (unitSort.m_UnitUID != unitUID)
			{
				continue;
			}
			if (equippedSkinID != 0)
			{
				NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(equippedSkinID);
				if (!NKMSkinManager.IsSkinForCharacter(unitSort.m_UnitID, skinTemplet))
				{
					continue;
				}
			}
			unitSort.m_SkinID = equippedSkinID;
			if (!(m_DragCharacterView != null))
			{
				break;
			}
			NKCUICharacterView[] componentsInChildren = m_DragCharacterView.GetComponentsInChildren<NKCUICharacterView>();
			foreach (NKCUICharacterView nKCUICharacterView in componentsInChildren)
			{
				if (nKCUICharacterView.GetCurrentUnitData().m_UnitUID == unitUID)
				{
					nKCUICharacterView.SetCharacterIllust(unitSort, unitSort.m_SkinID);
					break;
				}
			}
			break;
		}
	}

	private void OnTouchLoyalty()
	{
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAUNTLET_PRIVATE_ROOM)
		{
			NKCUIManager.NKCPopupMessage.Open(new PopupMessage(NKCUtilString.GET_STRING_PRIVATE_PVP_UNUSABLE_FUNCTION, NKCPopupMessage.eMessagePosition.Top, 0f, bPreemptive: true, bShowFX: false, bWaitForGameEnd: false));
		}
		else if (m_NKMUnitData.IsPermanentContract)
		{
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_LIFETIME_REPLAY, delegate
			{
				NKCUILifetime.Instance.Open(m_NKMUnitData, replay: true);
			});
		}
		else if (m_NKMUnitData.loyalty >= 10000)
		{
			if (!IsSeizedUnit(m_NKMUnitData))
			{
				NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_LIFETIME_CONTRACT_POPUP, delegate
				{
					NKCContentManager.MoveToShortCut(NKM_SHORTCUT_TYPE.SHORTCUT_UNIT_LIFETIME, m_NKMUnitData.m_UnitUID.ToString());
				});
			}
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_INFORMATION, NKCUtilString.GET_STRING_LIFETIME_LOYALTY_INFO);
		}
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

	private void OnClickRecall()
	{
		if (m_NKMUnitData == null)
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		DateTime serverUTCTime = NKCSynchronizedTime.GetServerUTCTime();
		NKMRecallTemplet nKMRecallTemplet = NKMRecallTemplet.Find(m_NKMUnitData.m_UnitID, NKMTime.UTCtoLocal(serverUTCTime));
		if (nKMRecallTemplet == null)
		{
			return;
		}
		if (nKMUserData.m_RecallHistoryData.ContainsKey(m_NKMUnitData.m_UnitID))
		{
			RecallHistoryInfo recallHistoryInfo = nKMUserData.m_RecallHistoryData[m_NKMUnitData.m_UnitID];
			if (NKCRecallManager.IsValidTime(nKMRecallTemplet, recallHistoryInfo.lastUpdateDate))
			{
				NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_RECALL_ALREADY_USED);
				return;
			}
		}
		if (!NKCRecallManager.IsValidTime(nKMRecallTemplet, serverUTCTime))
		{
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_RECALL_PERIOD_EXPIRED);
			return;
		}
		if (!NKCRecallManager.IsValidRegTime(nKMRecallTemplet, m_NKMUnitData.m_regDate))
		{
			NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_RECALL_INVALID_ACCQUIRE_TIME);
			return;
		}
		if (m_NKMUnitData.m_bLock || nKMUserData.m_ArmyData.IsUnitInAnyDeck(m_NKMUnitData.m_UnitUID))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_RECALL_ERROR_ALT_USING_UNIT);
			return;
		}
		if (nKMUserData.backGroundInfo.unitInfoList.Find((NKMBackgroundUnitInfo e) => e.unitUid == m_NKMUnitData.m_UnitUID) != null)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_RECALL_ERROR_ALT_USING_UNIT);
			return;
		}
		foreach (NKMWorldMapCityData value in nKMUserData.m_WorldmapData.worldMapCityDataMap.Values)
		{
			if (value.leaderUnitUID == m_NKMUnitData.m_UnitUID)
			{
				NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_RECALL_ERROR_ALT_USING_UNIT);
				return;
			}
		}
		if (m_NKMUnitData.OfficeRoomId > 0)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_WARNING, NKCUtilString.GET_STRING_RECALL_ERROR_ALT_USING_UNIT);
		}
		else
		{
			NKCPopupRecall.Instance.Open(m_NKMUnitData);
		}
	}

	private void OnClickRecommend()
	{
		if (null != m_NKCUIUnitInfoInfo && null != m_NKCUIUnitInfoInfo.m_objEquipInfo && m_NKCUIUnitInfoInfo.m_objEquipInfo.activeSelf)
		{
			m_NKCUIUnitInfoInfo.SetEnableEquipInfo(bEnable: false);
		}
		else if (m_NKCUIUnitRecommendSetOption.gameObject.activeSelf)
		{
			m_NKCUIUnitRecommendSetOption.Close();
		}
		else
		{
			m_NKCUIUnitRecommendSetOption.Open(m_NKMUnitData.m_UnitID);
		}
	}

	private void OnCloseRecommend()
	{
		if (m_NKCUIUnitRecommendEquipList.gameObject.activeSelf)
		{
			m_NKCUIUnitRecommendEquipList.Close();
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
			NKCUIUnitPlacement.Instance.Open(NKCUIUnitPlacement.UnitType.Unit, m_NKMUnitData.m_UnitUID);
		}
	}

	public void OpenRecommendEquipList(int setOptionID)
	{
		NKCUnitEquipRecommendTemplet nKCUnitEquipRecommendTemplet = NKCUnitEquipRecommendTemplet.Find(m_NKMUnitData.m_UnitID);
		if (nKCUnitEquipRecommendTemplet != null && nKCUnitEquipRecommendTemplet.m_dicRecommendList.ContainsKey(setOptionID))
		{
			m_NKCUIUnitRecommendEquipList.Open(setOptionID, nKCUnitEquipRecommendTemplet.m_dicRecommendList[setOptionID]);
		}
	}

	public void AutoEquipBySetOption(int setOptionID)
	{
		m_NKCUIUnitInfoInfo.AddAllEquipBySetOption(setOptionID);
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
		if (m_UnitSortList == null || m_UnitSortList.Count <= idx)
		{
			return;
		}
		NKMUnitData nKMUnitData = m_UnitSortList[idx];
		if (nKMUnitData != null)
		{
			NKCUICharacterView component = tr.GetComponent<NKCUICharacterView>();
			if (component != null)
			{
				component.SetCharacterIllust(nKMUnitData);
				return;
			}
			NKCUICharacterView nKCUICharacterView = tr.gameObject.AddComponent<NKCUICharacterView>();
			nKCUICharacterView.m_rectIllustRoot = tr.GetComponent<RectTransform>();
			nKCUICharacterView.SetCharacterIllust(nKMUnitData);
		}
	}

	private void ReturnMainBannerListSlot(Transform go)
	{
		NKCUtil.SetGameobjectActive(go, bValue: false);
		UnityEngine.Object.Destroy(go.gameObject);
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

	public void SelectCharacter(int idx)
	{
		if (m_UnitSortList.Count < idx || idx < 0)
		{
			Debug.LogWarning($"Error - Count : {m_UnitSortList.Count}, Index : {idx}");
			return;
		}
		NKMUnitData nKMUnitData = m_UnitSortList[idx];
		if (nKMUnitData != null)
		{
			ChangeUnit(nKMUnitData);
		}
	}

	private void BannerCleanUp()
	{
		if (!(m_DragCharacterView != null))
		{
			return;
		}
		NKCUICharacterView[] componentsInChildren = m_DragCharacterView.gameObject.GetComponentsInChildren<NKCUICharacterView>(includeInactive: true);
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

	private void ChangeUnit(NKMUnitData cNKMUnitData)
	{
		if (m_NKMUnitData.m_UnitUID != cNKMUnitData.m_UnitUID)
		{
			m_NKCUICharInfoSummary.SetData(cNKMUnitData);
			if (m_NKCUIUnitTagList != null)
			{
				m_NKCUIUnitTagList.SetData(cNKMUnitData);
			}
			m_NKMUnitData = cNKMUnitData;
			UpdateUnitData();
			NKCUIManager.NKCUIOverlayCaption.CloseAllCaption();
			m_lReserveUID = cNKMUnitData.m_UnitUID;
			m_fDeltaTime = 0f;
			NKCUtil.SetGameobjectActive(m_objRecall, NKCRecallManager.IsRecallTargetUnit(m_NKMUnitData, NKCSynchronizedTime.GetServerUTCTime()));
			if (m_objRecall.activeSelf)
			{
				SetRecallRemainTime();
			}
		}
	}

	private void ChangeUnitByOtherUI(NKMUnitData cNKMUnitData)
	{
		for (int i = 0; i < m_UnitSortList.Count; i++)
		{
			if (m_UnitSortList[i].m_UnitUID == cNKMUnitData.m_UnitUID)
			{
				m_DragCharacterView.TotalCount = m_UnitSortList.Count;
				m_DragCharacterView.SetIndex(i);
			}
		}
	}

	private void UpdateUnitData()
	{
		switch (m_curUIState)
		{
		case UNIT_INFO_TAB_STATE.BASE:
			m_NKCUIUnitInfoInfo.SetData(m_NKMUnitData, m_bShowFierceInfo);
			break;
		case UNIT_INFO_TAB_STATE.NEGOTIATION:
			m_NKCUIUnitInfoNegotiation.SetData(m_NKMUnitData);
			break;
		case UNIT_INFO_TAB_STATE.LIMIT_BREAK:
			m_NKCUIUnitInfoLimitBreak.SetData(m_NKMUnitData);
			break;
		case UNIT_INFO_TAB_STATE.SKILL_TRAIN:
			m_NKCUIUnitInfoSkillTrain.SetData(m_NKMUnitData, m_NKCUIUnitInfoSkillTrain.SelectedSkillID);
			break;
		}
		bool flag = NKMSkinManager.IsCharacterHasSkin(m_NKMUnitData.m_UnitID);
		m_cgSkinMode.alpha = (flag ? 1f : 0.4f);
		m_cbtnSkinMode.enabled = flag;
		UpdateUnitInfoUI();
		if (NKCUIUnitPlacement.IsInstanceOpen)
		{
			NKCUIUnitPlacement.Instance.SetData(NKCUIUnitPlacement.UnitType.Unit, m_NKMUnitData.m_UnitUID);
		}
	}

	private void ChangeState(UNIT_INFO_TAB_STATE newState)
	{
		ContentsType contentsType = ContentsType.None;
		switch (newState)
		{
		case UNIT_INFO_TAB_STATE.NEGOTIATION:
			contentsType = ContentsType.PERSONNAL_NEGO;
			break;
		case UNIT_INFO_TAB_STATE.SKILL_TRAIN:
			contentsType = ContentsType.LAB_TRAINING;
			break;
		case UNIT_INFO_TAB_STATE.LIMIT_BREAK:
			contentsType = ContentsType.LAB_LIMITBREAK;
			break;
		}
		if (contentsType != ContentsType.None && !NKCContentManager.IsContentsUnlocked(contentsType))
		{
			switch (m_curUIState)
			{
			case UNIT_INFO_TAB_STATE.LIMIT_BREAK:
				m_tglLimitBreak.Select(bSelect: true, bForce: true, bImmediate: true);
				break;
			case UNIT_INFO_TAB_STATE.NEGOTIATION:
				m_tglNegotiation.Select(bSelect: true, bForce: true, bImmediate: true);
				break;
			case UNIT_INFO_TAB_STATE.SKILL_TRAIN:
				m_tglSkillTrain.Select(bSelect: true, bForce: true, bImmediate: true);
				break;
			default:
				m_tglInfo.Select(bSelect: true, bForce: true, bImmediate: true);
				break;
			}
			NKCContentManager.ShowLockedMessagePopup(contentsType);
		}
		else if (newState != m_curUIState)
		{
			if (NKCPopupUnitInfoDetail.IsInstanceOpen && newState != UNIT_INFO_TAB_STATE.BASE)
			{
				NKCPopupUnitInfoDetail.CheckInstanceAndClose();
			}
			m_curUIState = newState;
			TutorialCheck();
			UpdateStateUI();
			UpdateUnitData();
			NKCUIManager.UpdateUpsideMenu();
		}
	}

	private void UpdateStateUI()
	{
		NKCUtil.SetGameobjectActive(m_UnitInfoControlBtn, m_curUIState == UNIT_INFO_TAB_STATE.BASE);
		NKCUtil.SetGameobjectActive(m_UnitInfoBottomBtn, m_curUIState == UNIT_INFO_TAB_STATE.BASE);
		NKCUtil.SetGameobjectActive(m_NKCUIUnitInfoInfo, m_curUIState == UNIT_INFO_TAB_STATE.BASE);
		if (m_curUIState != UNIT_INFO_TAB_STATE.BASE)
		{
			m_NKCUIUnitInfoInfo.SetEnableEquipInfo(bEnable: false);
		}
		NKCUtil.SetGameobjectActive(m_NKCUIUnitInfoNegotiation, m_curUIState == UNIT_INFO_TAB_STATE.NEGOTIATION);
		NKCUtil.SetGameobjectActive(m_NKCUIUnitInfoLimitBreak, m_curUIState == UNIT_INFO_TAB_STATE.LIMIT_BREAK);
		NKCUtil.SetGameobjectActive(m_NKCUIUnitInfoSkillTrain, m_curUIState == UNIT_INFO_TAB_STATE.SKILL_TRAIN);
		NKCUtil.SetGameobjectActive(m_ChangeBtn.gameObject, m_curUIState == UNIT_INFO_TAB_STATE.NEGOTIATION || m_curUIState == UNIT_INFO_TAB_STATE.LIMIT_BREAK || m_curUIState == UNIT_INFO_TAB_STATE.SKILL_TRAIN);
	}

	public void OnUnitSortOption(NKCUnitSortSystem.UnitListOptions unitOption)
	{
		m_preUnitListOption = unitOption;
	}

	private void OnSelectUnit()
	{
		if (m_curUIState == UNIT_INFO_TAB_STATE.LIMIT_BREAK && m_curUIState == UNIT_INFO_TAB_STATE.SKILL_TRAIN)
		{
			UnitSelectList.Open(GetUnitSelectListOption(), OnUnitSelected, OnUnitSortList, null, OnUnitSortOption);
		}
		else
		{
			UnitSelectList.Open(GetUnitSelectListOption(), OnUnitSelected, OnUnitSortList);
		}
	}

	public NKCUIUnitSelectList.UnitSelectListOptions GetUnitSelectListOption(UNIT_INFO_TAB_STATE newTabState = UNIT_INFO_TAB_STATE.NONE)
	{
		if (newTabState == UNIT_INFO_TAB_STATE.NONE)
		{
			newTabState = m_curUIState;
		}
		NKCUIUnitSelectList.UnitSelectListOptions options = new NKCUIUnitSelectList.UnitSelectListOptions(NKM_UNIT_TYPE.NUT_NORMAL, _bMultipleSelect: false, NKM_DECK_TYPE.NDT_NORMAL);
		options.lstSortOption = NKCUnitSortSystem.GetDefaultSortOptions(NKM_UNIT_TYPE.NUT_NORMAL, bIsCollection: false);
		if (newTabState == UNIT_INFO_TAB_STATE.SKILL_TRAIN || newTabState == UNIT_INFO_TAB_STATE.LIMIT_BREAK)
		{
			options.bShowRemoveSlot = false;
			options.bCanSelectUnitInMission = true;
			options.eDeckType = NKM_DECK_TYPE.NDT_NORMAL;
			if (m_NKMUnitData != null)
			{
				options.m_IncludeUnitUID = m_NKMUnitData.m_UnitUID;
			}
		}
		options.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
		options.setExcludeUnitUID = new HashSet<long>();
		options.bDescending = true;
		options.bExcludeLockedUnit = false;
		options.bExcludeDeckedUnit = false;
		options.bHideDeckedUnit = false;
		string menuName = NKCUtilString.GetMenuName(newTabState);
		string emptyMessage = NKCUtilString.GetEmptyMessage(newTabState);
		options.strUpsideMenuName = menuName;
		options.strEmptyMessage = emptyMessage;
		options.bPushBackUnselectable = false;
		options.m_SortOptions.bIgnoreCityState = true;
		options.m_SortOptions.bIgnoreWorldMapLeader = true;
		options.bShowHideDeckedUnitMenu = false;
		options.setUnitFilterCategory = NKCUnitSortSystem.setDefaultUnitFilterCategory;
		options.setUnitSortCategory = NKCUnitSortSystem.setDefaultUnitSortCategory;
		options.m_bUseFavorite = true;
		if (newTabState == UNIT_INFO_TAB_STATE.NEGOTIATION)
		{
			options.bEnableLockUnitSystem = false;
			options.m_SortOptions.AdditionalExcludeFilterFunc = CheckUnitCanNegotiate;
		}
		if (newTabState == UNIT_INFO_TAB_STATE.SKILL_TRAIN || newTabState == UNIT_INFO_TAB_STATE.LIMIT_BREAK)
		{
			NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
			switch (newTabState)
			{
			case UNIT_INFO_TAB_STATE.LIMIT_BREAK:
			{
				dicUnitLimitBreak.Clear();
				NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
				if (nKMUserData != null)
				{
					foreach (KeyValuePair<long, NKMUnitData> item in armyData.m_dicMyUnit)
					{
						int num = NKMUnitLimitBreakManager.CanThisUnitLimitBreakNow(item.Value, nKMUserData);
						if (item.Value.m_UnitUID == options.m_IncludeUnitUID && num < 0)
						{
							num = 0;
						}
						dicUnitLimitBreak[item.Key] = num;
					}
				}
				options.m_SortOptions.AdditionalExcludeFilterFunc = CheckUnitCanLimitBreak;
				options.setUnitSortCategory = new HashSet<NKCUnitSortSystem.eSortCategory>();
				options.setUnitSortCategory.Add(NKCUnitSortSystem.eSortCategory.Custom1);
				foreach (NKCUnitSortSystem.eSortCategory item2 in NKCUnitSortSystem.setDefaultUnitSortCategory)
				{
					options.setUnitSortCategory.Add(item2);
				}
				string key = NKCStringTable.GetString("SI_GUIDE_MANUAL_TEMPLET_ARTICLE_UNIT_LIMITBREAK");
				options.m_SortOptions.lstCustomSortFunc.Add(NKCUnitSortSystem.eSortCategory.Custom1, new KeyValuePair<string, NKCUnitSortSystem.NKCDataComparerer<NKMUnitData>.CompareFunc>(key, SortByUnitCanLimitBreakNow));
				options.lstSortOption = new List<NKCUnitSortSystem.eSortOption>();
				options.lstSortOption.Add(NKCUnitSortSystem.eSortOption.CustomDescend1);
				NKCUnitSortSystem.GetDefaultSortOptions(NKM_UNIT_TYPE.NUT_NORMAL, bIsCollection: false).ForEach(delegate(NKCUnitSortSystem.eSortOption e)
				{
					options.lstSortOption.Add(e);
				});
				options.dOnSlotSetData = OnLimitBreakSlotSetData;
				break;
			}
			case UNIT_INFO_TAB_STATE.SKILL_TRAIN:
				foreach (KeyValuePair<long, NKMUnitData> item3 in armyData.m_dicMyUnit)
				{
					if (item3.Value.m_UnitUID == options.m_IncludeUnitUID)
					{
						options.setExcludeUnitUID.Add(item3.Key);
					}
					else if (!NKMUnitSkillManager.CheckHaveUpgradableSkill(item3.Value))
					{
						options.setExcludeUnitUID.Add(item3.Key);
					}
				}
				options.m_SortOptions.AdditionalExcludeFilterFunc = CheckUnitCanSkillTrain;
				break;
			}
			if (m_NKMUnitData != null && !options.setExcludeUnitUID.Contains(m_NKMUnitData.m_UnitUID))
			{
				options.setExcludeUnitUID.Add(m_NKMUnitData.m_UnitUID);
			}
		}
		return options;
	}

	private bool CheckUnitCanNegotiate(NKMUnitData unitData)
	{
		return NKCNegotiateManager.CanTargetNegotiate(NKCScenManager.CurrentUserData(), unitData) == NKM_ERROR_CODE.NEC_OK;
	}

	private bool CheckUnitCanSkillTrain(NKMUnitData unitData)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
		if (unitTempletBase == null || unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
		{
			return false;
		}
		return true;
	}

	private bool CheckUnitCanLimitBreak(NKMUnitData unitData)
	{
		return NKMUnitLimitBreakManager.CanThisUnitLimitBreak(unitData);
	}

	private int SortByUnitCanLimitBreakNow(NKMUnitData lhs, NKMUnitData rhs)
	{
		if (!dicUnitLimitBreak.TryGetValue(lhs.m_UnitUID, out var value))
		{
			value = NKMUnitLimitBreakManager.CanThisUnitLimitBreakNow(lhs, NKCScenManager.CurrentUserData());
			dicUnitLimitBreak[lhs.m_UnitUID] = value;
		}
		if (!dicUnitLimitBreak.TryGetValue(rhs.m_UnitUID, out var value2))
		{
			value2 = NKMUnitLimitBreakManager.CanThisUnitLimitBreakNow(rhs, NKCScenManager.CurrentUserData());
			dicUnitLimitBreak[rhs.m_UnitUID] = value2;
		}
		return value2.CompareTo(value);
	}

	private void OnLimitBreakSlotSetData(NKCUIUnitSelectListSlotBase cUnitSlot, NKMUnitData cNKMUnitData, NKMDeckIndex deckIndex)
	{
		if (cNKMUnitData != null && dicUnitLimitBreak.TryGetValue(cNKMUnitData.m_UnitUID, out var value))
		{
			NKCUIUnitSelectListSlot nKCUIUnitSelectListSlot = cUnitSlot as NKCUIUnitSelectListSlot;
			if (nKCUIUnitSelectListSlot != null)
			{
				nKCUIUnitSelectListSlot.SetLimitPossibleMark(value >= 0, NKMUnitLimitBreakManager.IsMaxLimitBreak(cNKMUnitData, 0));
			}
		}
	}

	private NKM_ERROR_CODE CanSelectUnit(NKMUnitData unitData, NKMArmyData armyData)
	{
		if (unitData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_NOT_EXIST;
		}
		return armyData.GetUnitDeckState(unitData) switch
		{
			NKM_DECK_STATE.DECK_STATE_WARFARE => NKM_ERROR_CODE.NEC_FAIL_WARFARE_DOING, 
			NKM_DECK_STATE.DECK_STATE_DIVE => NKM_ERROR_CODE.NEC_FAIL_DIVE_DOING, 
			_ => NKM_ERROR_CODE.NEC_OK, 
		};
	}

	private void OnUnitSelected(List<long> lstUnitUID)
	{
		if (lstUnitUID == null || lstUnitUID.Count != 1)
		{
			Debug.LogError("NKCUIUnitInfo.OpenUnitEnhance, Fatal Error : UnitSelectList returned wrong list");
			return;
		}
		long uID = lstUnitUID[0];
		NKMUnitData unitOrTrophyFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitOrTrophyFromUID(uID);
		if (unitOrTrophyFromUID == null)
		{
			Debug.Log("NKCUIUnitInfo.OpenUnitEnhance, Fatal Error : wrong uid, newUnitData is null");
			return;
		}
		NKM_ERROR_CODE nKM_ERROR_CODE = CanSelectUnit(unitOrTrophyFromUID);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupMessageManager.AddPopupMessage(nKM_ERROR_CODE);
			return;
		}
		if (m_curUIState == UNIT_INFO_TAB_STATE.LIMIT_BREAK && NKMUnitLimitBreakManager.IsMaxLimitBreak(unitOrTrophyFromUID))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_ALREADY_LIMITBREAK_MAX);
			return;
		}
		UnitSelectList.Close();
		if (m_curUIState == UNIT_INFO_TAB_STATE.NEGOTIATION)
		{
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_NEGOTITATE_READY, unitOrTrophyFromUID);
		}
		m_NKMUnitData = unitOrTrophyFromUID;
	}

	private NKM_ERROR_CODE CanSelectUnit(NKMUnitData unitData)
	{
		NKM_DECK_STATE unitDeckState = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitDeckState(unitData);
		NKM_ERROR_CODE result = NKM_ERROR_CODE.NEC_OK;
		switch (unitDeckState)
		{
		case NKM_DECK_STATE.DECK_STATE_WARFARE:
			result = NKM_ERROR_CODE.NEC_FAIL_WARFARE_DOING;
			break;
		case NKM_DECK_STATE.DECK_STATE_DIVE:
			result = NKM_ERROR_CODE.NEC_FAIL_DIVE_DOING;
			break;
		}
		return result;
	}

	private void OnUnitSortList(long UID, List<NKMUnitData> unitUIDList)
	{
		if (unitUIDList.Count > 0)
		{
			NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
			if (armyData == null || CanSelectUnit(armyData.GetUnitOrTrophyFromUID(UID), armyData) != NKM_ERROR_CODE.NEC_OK)
			{
				return;
			}
			for (int i = 0; i < unitUIDList.Count; i++)
			{
				if (CanSelectUnit(unitUIDList[i], armyData) != NKM_ERROR_CODE.NEC_OK)
				{
					unitUIDList.RemoveAt(i);
					i--;
				}
			}
		}
		BannerCleanUp();
		m_UnitSortList = unitUIDList;
		m_DragCharacterView.TotalCount = m_UnitSortList.Count;
		if (m_UnitSortList.Count <= 0)
		{
			return;
		}
		for (int j = 0; j < m_UnitSortList.Count; j++)
		{
			if (m_UnitSortList[j].m_UnitUID == UID)
			{
				m_DragCharacterView.SetIndex(j);
				break;
			}
		}
	}

	public bool IsBlockedUnit()
	{
		if (m_NKMUnitData != null)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_NKMUnitData.m_UnitID);
			if (unitTempletBase != null && unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
			{
				return true;
			}
		}
		return false;
	}

	public void OnRecv(NKMPacket_UNIT_SKILL_UPGRADE_ACK sPacket)
	{
		if (m_curUIState == UNIT_INFO_TAB_STATE.SKILL_TRAIN)
		{
			m_NKCUIUnitInfoSkillTrain.OnSkillLevelUp(sPacket.skillID);
			if (m_PlayVoiceSoundID == 0 || !NKCSoundManager.IsPlayingVoice(m_PlayVoiceSoundID))
			{
				m_PlayVoiceSoundID = NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_GROWTH_SKILL, NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(sPacket.unitUID));
			}
		}
	}

	public void OnRecv(NKMPacket_EQUIP_PRESET_LIST_ACK cPacket)
	{
		m_NKCUIUnitInfoInfo?.OpenEquipPreset(cPacket.presetDatas);
	}

	public void OnRecv(NKMPacket_LIMIT_BREAK_UNIT_ACK sPacket)
	{
		if (m_curUIState != UNIT_INFO_TAB_STATE.LIMIT_BREAK)
		{
			return;
		}
		NKCUIGameResultGetUnit.ShowUnitTranscendence(sPacket.unitData, delegate
		{
			if (NKCGameEventManager.IsEventPlaying())
			{
				NKCGameEventManager.WaitFinished();
			}
		});
	}

	public void ReserveLevelUpFx(NKCNegotiateManager.NegotiateResultUIData uiData)
	{
		m_NKCUIUnitInfoNegotiation.ReserveLevelUpFx(uiData);
	}

	private void TutorialCheck()
	{
		switch (m_curUIState)
		{
		case UNIT_INFO_TAB_STATE.BASE:
			if (NKCTutorialManager.TutorialRequired(TutorialPoint.UnitInfo) == TutorialStep.None)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(m_NKMUnitData.m_UnitID);
				if (unitTempletBase != null && unitTempletBase.m_NKM_UNIT_SOURCE_TYPE != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
				{
					NKCTutorialManager.TutorialRequired(TutorialPoint.SourceUnitInfo);
				}
			}
			break;
		case UNIT_INFO_TAB_STATE.LIMIT_BREAK:
			NKCTutorialManager.TutorialRequired(TutorialPoint.UnitLimitBreak);
			break;
		case UNIT_INFO_TAB_STATE.NEGOTIATION:
			NKCTutorialManager.TutorialRequired(TutorialPoint.UnitNegotiate);
			break;
		case UNIT_INFO_TAB_STATE.SKILL_TRAIN:
			NKCTutorialManager.TutorialRequired(TutorialPoint.UnitSkillTraining);
			break;
		}
	}

	public RectTransform GetSkillLevelSlotRect(int index)
	{
		if (m_NKCUIUnitInfoSkillTrain == null || m_NKCUIUnitInfoSkillTrain.m_lstSkillSlot == null)
		{
			return null;
		}
		if (index < m_NKCUIUnitInfoSkillTrain.m_lstSkillSlot.Count)
		{
			return m_NKCUIUnitInfoSkillTrain.m_lstSkillSlot[index].m_slot.m_cbtnSlot?.GetComponent<RectTransform>();
		}
		return null;
	}

	public void UpdateEquipSlots()
	{
		m_NKCUIUnitInfoInfo.UpdateEquipSlots();
	}

	public bool EquipPresetOpened()
	{
		if (!(m_NKCUIUnitInfoInfo == null))
		{
			return m_NKCUIUnitInfoInfo.EquipPresetOpened();
		}
		return false;
	}

	public void RefreshUIForReconnect()
	{
		if (m_curUIState == UNIT_INFO_TAB_STATE.NEGOTIATION)
		{
			m_NKCUIUnitInfoNegotiation.RefreshUIForReconnect();
		}
	}

	private Sprite GetBackgroundSprite(UNIT_INFO_TAB_STATE type)
	{
		string text = "";
		string text2 = ((type == UNIT_INFO_TAB_STATE.LIMIT_BREAK || type == UNIT_INFO_TAB_STATE.NEGOTIATION || type == UNIT_INFO_TAB_STATE.SKILL_TRAIN) ? "AB_UI_NUF_BASE_BG" : "AB_UI_BG_SPRITE");
		switch (type)
		{
		case UNIT_INFO_TAB_STATE.NEGOTIATION:
			text = "NKM_UI_BASE_PERSONNEL_BG";
			break;
		case UNIT_INFO_TAB_STATE.LIMIT_BREAK:
		case UNIT_INFO_TAB_STATE.SKILL_TRAIN:
			text = "NKM_UI_BASE_LAB_BG";
			break;
		default:
			text = "BG";
			break;
		}
		Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>(text2, text);
		if (orLoadAssetResource == null)
		{
			Debug.LogError("Error - NKCUIUnitInfo::GetBackgroundSprite - path:" + text2 + ", name:" + text);
		}
		return orLoadAssetResource;
	}

	private void SetRecallRemainTime()
	{
		NKMRecallTemplet nKMRecallTemplet = NKMRecallTemplet.Find(m_NKMUnitData.m_UnitID, NKMTime.UTCtoLocal(NKCSynchronizedTime.GetServerUTCTime()));
		if (nKMRecallTemplet != null)
		{
			NKCUtil.SetLabelText(m_lbRecallTime, string.Format(NKCUtilString.GET_STRING_RECALL_DESC_END_DATE, NKCUtilString.GetRemainTimeStringEx(nKMRecallTemplet.IntervalTemplet.GetEndDateUtc())));
		}
	}

	public void SetUnitAnimation(NKCASUIUnitIllust.eAnimation animation = NKCASUIUnitIllust.eAnimation.UNIT_IDLE)
	{
		if (m_DragCharacterView != null)
		{
			RectTransform currentItem = m_DragCharacterView.GetCurrentItem();
			if (currentItem != null)
			{
				NKCUICharacterView componentInChildren = currentItem.gameObject.GetComponentInChildren<NKCUICharacterView>();
				if (componentInChildren != null)
				{
					componentInChildren.SetAnimation(animation, loop: false);
				}
			}
		}
		switch (animation)
		{
		case NKCASUIUnitIllust.eAnimation.UNIT_IDLE:
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_NEGOTITATE_SUCCESS, m_NKMUnitData);
			break;
		case NKCASUIUnitIllust.eAnimation.UNIT_LAUGH:
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_NEGOTITATE_SUCCESS_GREAT, m_NKMUnitData);
			break;
		}
	}

	private void Update()
	{
		if (m_curUIState == UNIT_INFO_TAB_STATE.NEGOTIATION)
		{
			m_NKCUIUnitInfoNegotiation.OnUpdateButtonHold();
		}
		if (m_objRecall.activeSelf)
		{
			m_fDeltaTime += Time.deltaTime;
			if (m_fDeltaTime > 1f)
			{
				m_fDeltaTime -= 1f;
				SetRecallRemainTime();
			}
		}
	}

	private List<NKMUnitData> GetUnitSortList()
	{
		return m_UnitSortList;
	}
}
