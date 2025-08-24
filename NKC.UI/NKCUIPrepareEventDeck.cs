using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClientPacket.Common;
using ClientPacket.User;
using Cs.Core.Util;
using Cs.Logging;
using NKC.UI.Component;
using NKM;
using NKM.Guild;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI;

public class NKCUIPrepareEventDeck : NKCUIBase
{
	public delegate void OnEventDeckConfirm(NKMStageTempletV2 stageTemplet, NKMDungeonTempletBase dungeonTempletBase, NKMEventDeckData eventDeckData, long supportUserUID = 0L);

	public delegate void OnBackButtonEvent();

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_operation";

	private const string UI_ASSET_NAME = "NKM_UI_OPERATION_EVENTDECK";

	private static NKCUIPrepareEventDeck m_Instance;

	[Header("일반 전투")]
	public GameObject m_objNormalInfo;

	public Text m_lbTitle;

	public Text m_lbSubTitle;

	[Header("길드 협력전 전용")]
	public GameObject m_objGuildCoopInfo;

	public Text m_lbTitleGuildCoop;

	public Text m_lbArenaNum;

	public Text m_lbClearPercent;

	public Image m_imgClearGauge;

	public Text m_lbNextArtifactCount;

	[Header("함선")]
	public Text m_lbShipLevel;

	public Text m_lbShipName;

	public GameObject m_objShipFaceCard;

	public Image m_imgShipFaceCard;

	public GameObject m_objShipUnitFaceCard;

	public Image m_imgShipUnitFaceCard;

	public NKCUIComButton m_cbtnChngeShip;

	public GameObject m_objShipBanRoot;

	public NKCUIComStateButton m_csbtnShipRandom;

	public Text m_lbShipBan;

	[Header("오퍼레이터")]
	public NKCUIOperatorDeckSlot m_OperatorSlot;

	public NKCUIComStateButton m_csbtnCanChangeOperator;

	public GameObject m_OperatorSkill;

	public NKCUIOperatorSkill m_OperatorMainSkill;

	public NKCUIOperatorSkill m_OperatorSubSkill;

	public NKCUIOperatorTacticalSkillCombo m_OperatorSkillCombo;

	[Header("지원 유닛")]
	public GameObject m_objUnitAssist;

	public NKCUIComStateButton m_csbtnUnitAssist;

	public GameObject m_objUnitAssistLock;

	public GameObject m_objUnitAssistEmpty;

	public NKCDeckViewUnitSelectListSlot m_UnitAssistSlot;

	[Header("작전능력")]
	public Text m_lbDeckPower;

	public Text m_lbAvgCost;

	[Header("우하단 버튼")]
	public NKCUIComButton m_cbtnAutoSetup;

	public NKCUIComButton m_cbtnClearAll;

	public NKCUIComButton m_cbtnBegin;

	public NKCUIComResourceButton m_ResourceBtn;

	public List<NKCUIUnitSelectListEventDeckSlot> m_lstSlot;

	[Header("전투환경")]
	public NKCUIComBattleEnvironmentList m_comBattleEnvironment;

	[Header("입장 제한")]
	public GameObject m_AB_UI_NKM_UI_OPERATION_EVENTDECK_EnterLimit;

	public Text m_EnterLimit_TEXT;

	public Image m_AB_UI_NKM_UI_OPERATION_EVENTDECK_BUTTON_ICON;

	public Text m_AB_UI_NKM_UI_OPERATION_EVENTDECK_BUTTON_TEXT;

	[Header("전투스킵")]
	public GameObject m_objSkip;

	public NKCUIOperationSkip m_NKCUIOperationSkip;

	public NKCUIComToggle m_tglSkip;

	[Header("등장 적 부대")]
	public NKCUIComStateButton m_btnEnemyList;

	public Text m_lbEnemyLevel;

	[Header("컨텐츠에 따라 변경되는 항목")]
	public Image m_imgTitleDeco;

	public Image m_imgInfoBG;

	public Color m_ShadowColor;

	public Color m_FierceColor;

	public Color m_NormalColor;

	public Image m_imgDeckBG;

	public GameObject m_objBlackBG;

	public GameObject m_objShadowDust;

	[Header("드래그")]
	public Image m_imgDragObject;

	[Header("격전지원")]
	public GameObject m_AB_UI_NKM_UI_OPERATION_EVENTDECK_SHIP_FIERCE_BATTLE;

	public Text m_NKM_UI_UNIT_SELECT_LIST_FIERCE_BATTLE_TEXT;

	public GameObject m_AB_UI_NKM_UI_OPERATION_EVENTDECK_BATTLE_CONDITION;

	public GameObject m_objNotPossibleOperator;

	public Text m_lbNotPossibleOpeatorDescription;

	[Space]
	public GameObject m_AB_UI_NKM_UI_OPERATION_EVENTDECK_ENEMY_FIERCE_BATTLE;

	public Image m_FIERCE_BATTLE_BOSS_IMAGE_Root;

	public GameObject m_FIERCE_BATTLE_BOSS_INFO;

	public Text m_BOSS_LEVEL_Text;

	public Image m_CLASS_Icon;

	public Text m_CLASS_Text;

	public Image m_imgWeakMain;

	public Image m_imgWeakSub;

	public NKCUIComStateButton m_csbtnFierceEnemy;

	public NKCUIComStateButton m_csbtnLeaderSelect;

	[Header("덱 복사")]
	public NKCUIComStateButton m_csbtnDeckCopy;

	public NKCUIComStateButton m_csbtnDeckPaste;

	private int m_CurrMultiplyRewardCount = 1;

	private bool m_bOperationSkip;

	private NKMStageTempletV2 m_StageTemplet;

	private NKMDungeonTempletBase m_currentDungeonTempletBase;

	private NKMEventPvpSeasonTemplet m_eventPvpTemplet;

	private NKMDungeonEventDeckTemplet m_currentDeckTemplet;

	private NKMDeckCondition m_DeckCondition;

	private int m_currentIndex;

	private int m_currentLeaderIndex;

	private long m_SelectedShipUid;

	private long m_SelectedOperatorUid;

	private bool m_equipEnabled = true;

	private bool m_enableShowUpBan;

	private Dictionary<int, long> m_dicSelectedUnits = new Dictionary<int, long>();

	private DeckContents m_eventDeckContents;

	private OnEventDeckConfirm dOnEventDeckConfirm;

	private OnBackButtonEvent dOnBackButtonEvent;

	private List<NKCDeckViewUnitSlot> m_lstEnemySlot = new List<NKCDeckViewUnitSlot>();

	private bool m_isSelectingLeader;

	private NKMDungeonEventDeckTemplet.SLOT_TYPE m_oldSlotType = NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE;

	private Dictionary<NKMDeckCondition.ALL_DECK_CONDITION, int> m_dicAllDeckValueCache = new Dictionary<NKMDeckCondition.ALL_DECK_CONDITION, int>();

	private int m_iCostItemID;

	private int m_iCostItemCount;

	private bool m_bDrag;

	private int m_dragBeginIndex = -1;

	private RectTransform m_rectTransform;

	private bool m_bReqSupportUnitList;

	private long m_lselectedSupportUserUID;

	public static NKCUIPrepareEventDeck Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIPrepareEventDeck>("ab_ui_nkm_ui_operation", "NKM_UI_OPERATION_EVENTDECK", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIPrepareEventDeck>();
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

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override string MenuName => NKCUtilString.GET_STRING_EVENT_DECK;

	private bool OperatorEnabled => NKMContentsVersionManager.HasTag("OPERATOR");

	private bool OperatorUnlocked
	{
		get
		{
			if (OperatorEnabled)
			{
				return NKCContentManager.IsContentsUnlocked(ContentsType.OPERATOR);
			}
			return false;
		}
	}

	public override List<int> UpsideMenuShowResourceList
	{
		get
		{
			if (m_eventDeckContents == DeckContents.SHADOW_PALACE)
			{
				return new List<int> { 1, 19, 20 };
			}
			if (m_StageTemplet != null)
			{
				List<int> list = new List<int>();
				NKMEpisodeTempletV2 episodeTemplet = m_StageTemplet.EpisodeTemplet;
				if (episodeTemplet != null && episodeTemplet.ResourceIdList != null && episodeTemplet.ResourceIdList.Count > 0)
				{
					list = episodeTemplet.ResourceIdList;
				}
				if (!list.Contains(m_iCostItemID))
				{
					list.Add(m_iCostItemID);
				}
				return list;
			}
			if (m_iCostItemCount == 0 || m_iCostItemID == 0 || base.UpsideMenuShowResourceList.Contains(m_iCostItemID))
			{
				return new List<int> { 1, 2, 3, 101 };
			}
			List<int> list2 = new List<int>();
			list2.Add(m_iCostItemID);
			list2.AddRange(base.UpsideMenuShowResourceList);
			return list2;
		}
	}

	private RectTransform rectTransform
	{
		get
		{
			if (m_rectTransform == null)
			{
				m_rectTransform = GetComponent<RectTransform>();
			}
			return m_rectTransform;
		}
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

	public int GetCurrMultiplyRewardCount()
	{
		return m_CurrMultiplyRewardCount;
	}

	public bool GetOperationSkipState()
	{
		return m_bOperationSkip;
	}

	public override void CloseInternal()
	{
		m_dicSelectedUnits.Clear();
		CloseShipIllust();
		m_SelectedShipUid = 0L;
		m_StageTemplet = null;
		m_currentDungeonTempletBase = null;
		m_eventPvpTemplet = null;
		m_currentDeckTemplet = null;
		m_DeckCondition = null;
		base.gameObject.SetActive(value: false);
		ResetDrag();
		m_oldSlotType = NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE;
		if (m_comBattleEnvironment != null)
		{
			m_comBattleEnvironment.Close();
			m_comBattleEnvironment.ClearData();
		}
		if (NKCUIUnitSelectList.IsInstanceLoaded)
		{
			NKCUIUnitSelectList.Instance.ClearCachOption();
		}
	}

	public override void Hide()
	{
		base.Hide();
		ResetDrag();
	}

	public override void OnBackButton()
	{
		if (dOnBackButtonEvent != null)
		{
			dOnBackButtonEvent();
		}
		else
		{
			base.OnBackButton();
		}
	}

	public void Init()
	{
		foreach (NKCUIUnitSelectListEventDeckSlot item in m_lstSlot)
		{
			if (item != null)
			{
				item.Init();
				item.SetDragHandler(OnDragBegin, OnDrag, OnDragEnd, OnDrop);
			}
		}
		m_cbtnBegin.PointerClick.RemoveAllListeners();
		m_cbtnBegin.PointerClick.AddListener(OnBtnBegin);
		NKCUtil.SetHotkey(m_cbtnBegin, HotkeyEventType.Confirm);
		m_cbtnChngeShip.PointerClick.RemoveAllListeners();
		m_cbtnChngeShip.PointerClick.AddListener(OpenShipSelectList);
		m_cbtnAutoSetup.PointerClick.RemoveAllListeners();
		m_cbtnAutoSetup.PointerClick.AddListener(AutoPrepare);
		m_cbtnClearAll.PointerClick.RemoveAllListeners();
		m_cbtnClearAll.PointerClick.AddListener(ClearAll);
		if (null != m_tglSkip)
		{
			m_tglSkip.OnValueChanged.RemoveAllListeners();
			m_tglSkip.OnValueChanged.AddListener(OnClickSkip);
			NKCUtil.SetHotkey(m_tglSkip, HotkeyEventType.RotateLeft, null, bUpDownEvent: true);
		}
		m_CurrMultiplyRewardCount = 1;
		if (m_NKCUIOperationSkip != null)
		{
			m_NKCUIOperationSkip.Init(OnOperationSkipUpdated, OnClickOperationSkipClose);
		}
		NKCUtil.SetButtonClickDelegate(m_btnEnemyList, OnBtnEnemyList);
		m_OperatorSlot.Init(OnClickOperatorSlot);
		NKCUtil.SetButtonClickDelegate(m_csbtnCanChangeOperator, OpenOperatorSelectList);
		NKCUtil.SetButtonClickDelegate(m_csbtnLeaderSelect, OnClickLeaderSelect);
		NKCUtil.SetButtonClickDelegate(m_csbtnShipRandom, OnClickShipRandom);
		NKCUtil.SetButtonClickDelegate(m_csbtnFierceEnemy, OnClickFierceEnemy);
		NKCUtil.SetButtonClickDelegate(m_csbtnDeckCopy, OnClickDeckCopy);
		NKCUtil.SetButtonClickDelegate(m_csbtnDeckPaste, OnClickDeckPaste);
		NKCUtil.SetHotkey(m_csbtnDeckCopy, HotkeyEventType.Copy);
		NKCUtil.SetHotkey(m_csbtnDeckPaste, HotkeyEventType.Paste);
		base.gameObject.SetActive(value: false);
	}

	private void SetSkipCountUIData()
	{
		int maxCount = 99;
		NKMStageTempletV2 stageTemplet = m_StageTemplet;
		if (stageTemplet != null && stageTemplet.EnterLimit > 0)
		{
			int statePlayCnt = NKCScenManager.CurrentUserData().GetStatePlayCnt(stageTemplet.Key);
			maxCount = stageTemplet.EnterLimit - statePlayCnt;
		}
		int num = 0;
		int eternium = 0;
		if (m_StageTemplet != null)
		{
			num = m_StageTemplet.m_StageReqItemID;
			eternium = m_StageTemplet.m_StageReqItemCount;
			if (num == 2)
			{
				NKCCompanyBuff.SetDiscountOfEterniumInEnteringDungeon(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref eternium);
			}
		}
		m_NKCUIOperationSkip.SetData(NKMCommonConst.SkipCostMiscItemId, NKMCommonConst.SkipCostMiscItemCount, num, eternium, m_CurrMultiplyRewardCount, 1, maxCount);
	}

	private void OnOperationSkipUpdated(int newCount)
	{
		m_CurrMultiplyRewardCount = newCount;
		UpdateAttackCost();
	}

	private void OnClickOperationSkipClose()
	{
		m_tglSkip.Select(bSelect: false);
	}

	public void Open(NKMStageTempletV2 stageTemplet, NKMDungeonTempletBase dungeonTempletBase, NKMEventPvpSeasonTemplet eventPvpTemplet, OnEventDeckConfirm onEventDeckConfirm, OnBackButtonEvent onBackButtonEvent = null, DeckContents eventDeckContents = DeckContents.NORMAL)
	{
		m_dicSelectedUnits.Clear();
		m_SelectedShipUid = 0L;
		m_SelectedOperatorUid = 0L;
		m_CurrMultiplyRewardCount = 1;
		dOnEventDeckConfirm = onEventDeckConfirm;
		dOnBackButtonEvent = onBackButtonEvent;
		m_eventDeckContents = eventDeckContents;
		m_currentLeaderIndex = 0;
		m_isSelectingLeader = false;
		m_csbtnLeaderSelect?.Select(bSelect: false);
		m_bReqSupportUnitList = false;
		NKCUtil.SetGameobjectActive(m_objUnitAssistLock, bValue: false);
		NKCUtil.SetGameobjectActive(m_UnitAssistSlot, bValue: false);
		NKCUtil.SetGameobjectActive(m_objUnitAssistEmpty, bValue: false);
		NKCUtil.SetGameobjectActive(m_csbtnDeckCopy, NKMOpenTagManager.IsOpened("COPY_SQUAD"));
		NKCUtil.SetGameobjectActive(m_csbtnDeckPaste, NKMOpenTagManager.IsOpened("COPY_SQUAD"));
		m_csbtnUnitAssist?.PointerClick.RemoveAllListeners();
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.USE_SUPPORT_UNIT) && eventDeckContents != DeckContents.FIERCE_BATTLE_SUPPORT && stageTemplet != null && stageTemplet.m_bSupportUnit)
		{
			m_UnitAssistSlot.SetEmpty(bEnableLayoutElement: false, null);
			NKCUtil.SetGameobjectActive(m_objUnitAssistEmpty, bValue: true);
			NKCUtil.SetBindFunction(m_csbtnUnitAssist, OnClickAssistUnit);
		}
		m_lselectedSupportUserUID = 0L;
		if (stageTemplet == null && dungeonTempletBase == null && eventPvpTemplet == null)
		{
			return;
		}
		m_StageTemplet = stageTemplet;
		m_currentDungeonTempletBase = dungeonTempletBase;
		m_eventPvpTemplet = eventPvpTemplet;
		if (eventPvpTemplet != null)
		{
			m_equipEnabled = !eventPvpTemplet.ForceBanEquip;
			m_enableShowUpBan = !eventPvpTemplet.ForcedBanIgnore;
		}
		else
		{
			m_equipEnabled = true;
			m_enableShowUpBan = false;
		}
		NKMDungeonEventDeckTemplet nKMDungeonEventDeckTemplet = null;
		if (eventDeckContents == DeckContents.EVENT_PVP)
		{
			nKMDungeonEventDeckTemplet = eventPvpTemplet?.EventDeckTemplet;
			m_DeckCondition = eventPvpTemplet?.DeckCondition;
		}
		else if (stageTemplet != null)
		{
			nKMDungeonEventDeckTemplet = stageTemplet.GetEventDeckTemplet();
			m_DeckCondition = stageTemplet.GetDeckCondition();
		}
		else if (dungeonTempletBase != null)
		{
			nKMDungeonEventDeckTemplet = dungeonTempletBase.EventDeckTemplet;
			m_DeckCondition = dungeonTempletBase.m_DeckCondition;
		}
		if (nKMDungeonEventDeckTemplet == null)
		{
			Debug.LogError("Dungeon does not using eventDeck");
			return;
		}
		base.gameObject.SetActive(value: true);
		OnClickOperationSkipClose();
		m_currentDeckTemplet = nKMDungeonEventDeckTemplet;
		InitEventDeckData(m_currentDeckTemplet);
		if (stageTemplet != null)
		{
			NKCUtil.SetLabelText(m_lbTitle, "");
			NKCUtil.SetLabelText(m_lbSubTitle, stageTemplet.GetDungeonName());
		}
		else if (m_currentDungeonTempletBase != null)
		{
			NKCUtil.SetLabelText(m_lbTitle, "");
			NKCUtil.SetLabelText(m_lbSubTitle, dungeonTempletBase.GetDungeonName());
		}
		else if (eventPvpTemplet != null)
		{
			NKCUtil.SetLabelText(m_lbTitle, "");
			NKCUtil.SetLabelText(m_lbSubTitle, NKCStringTable.GetString(eventPvpTemplet.SeasonName));
		}
		else
		{
			NKCUtil.SetLabelText(m_lbTitle, "");
			NKCUtil.SetLabelText(m_lbSubTitle, "");
		}
		LoadDungeonDeck();
		RecalculateDeckAllConditionCache();
		UpdateAttackCost();
		SetVisibleMultiplyRewardUI();
		SetUIByContents();
		SetEnemyList();
		ResetDrag();
		if (!SetAsLeader(m_currentLeaderIndex))
		{
			SetDefaultLeader();
		}
		InitBattleEnvironment();
		UIOpened();
		CheckTutorial();
	}

	private void SetVisibleMultiplyRewardUI()
	{
		bool flag = NKCContentManager.IsContentsUnlocked(ContentsType.OPERATION_MULTIPLY);
		bool flag2 = false;
		bool flag3 = true;
		if (m_StageTemplet != null)
		{
			NKMEpisodeTempletV2 episodeTemplet = m_StageTemplet.EpisodeTemplet;
			if (episodeTemplet != null)
			{
				switch (episodeTemplet.m_EPCategory)
				{
				case EPISODE_CATEGORY.EC_MAINSTREAM:
				case EPISODE_CATEGORY.EC_DAILY:
				case EPISODE_CATEGORY.EC_SIDESTORY:
				case EPISODE_CATEGORY.EC_FIELD:
				case EPISODE_CATEGORY.EC_EVENT:
				case EPISODE_CATEGORY.EC_SUPPLY:
				case EPISODE_CATEGORY.EC_CHALLENGE:
				case EPISODE_CATEGORY.EC_SEASONAL:
					flag2 = true;
					break;
				default:
					flag2 = false;
					break;
				}
			}
			NKMDungeonTempletBase currentDungeonTempletBase = m_currentDungeonTempletBase;
			if (currentDungeonTempletBase != null)
			{
				flag3 = currentDungeonTempletBase.m_RewardMultiplyMax > 1;
			}
		}
		bool flag4 = true;
		if (m_StageTemplet != null)
		{
			if (m_StageTemplet.m_STAGE_TYPE == STAGE_TYPE.ST_PHASE)
			{
				flag4 = false;
			}
			if (m_StageTemplet.m_bNoAutoRepeat)
			{
				flag4 = false;
			}
		}
		bool flag5 = m_StageTemplet != null && m_StageTemplet.m_bActiveBattleSkip;
		NKCUtil.SetGameobjectActive(m_objSkip, flag && flag2 && flag3 && flag4 && flag5);
	}

	private bool CheckSkip(bool bMsg)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null || m_currentDungeonTempletBase == null)
		{
			return false;
		}
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.OPERATION_MULTIPLY))
		{
			return false;
		}
		if (!NKCUtil.IsCanStartEterniumStage(m_StageTemplet, bCallLackPopup: true))
		{
			return false;
		}
		if (!nKMUserData.IsSuperUser())
		{
			if (!nKMUserData.CheckDungeonClear(m_currentDungeonTempletBase.m_DungeonStrID))
			{
				if (bMsg)
				{
					NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_MULTIPLY_OPERATION_MEDAL_COND);
				}
				return false;
			}
			NKMDungeonClearData dungeonClearData = nKMUserData.GetDungeonClearData(m_currentDungeonTempletBase.m_DungeonStrID);
			if (dungeonClearData == null || !dungeonClearData.missionResult1 || !dungeonClearData.missionResult2)
			{
				if (bMsg)
				{
					NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_MULTIPLY_OPERATION_MEDAL_COND);
				}
				return false;
			}
		}
		if (m_currentDungeonTempletBase.m_RewardMultiplyMax <= 1)
		{
			return false;
		}
		return true;
	}

	private void OnClickSkip(bool bSet)
	{
		if (bSet)
		{
			if (!CheckSkip(bMsg: true))
			{
				m_tglSkip.Select(bSelect: false);
				return;
			}
			m_bOperationSkip = true;
			UpdateAttackCost();
			SetSkipCountUIData();
		}
		NKCUtil.SetGameobjectActive(m_NKCUIOperationSkip, bSet);
		if (!bSet)
		{
			m_CurrMultiplyRewardCount = 1;
			m_bOperationSkip = false;
			UpdateAttackCost();
			SetSkipCountUIData();
		}
	}

	public void UpdateEnterLimitUI()
	{
		bool bValue = false;
		if (m_StageTemplet != null && m_StageTemplet.EnterLimit > 0)
		{
			bValue = true;
			NKCUtil.SetGameobjectActive(m_AB_UI_NKM_UI_OPERATION_EVENTDECK_EnterLimit, bValue: true);
			int statePlayCnt = NKCScenManager.CurrentUserData().GetStatePlayCnt(m_StageTemplet.Key);
			string text = "";
			NKCUtil.SetLabelText(msg: m_StageTemplet.EnterLimitCond switch
			{
				NKMStageTempletV2.RESET_TYPE.DAY => string.Format(NKCUtilString.GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_DAY_02, m_StageTemplet.EnterLimit - statePlayCnt, m_StageTemplet.EnterLimit), 
				NKMStageTempletV2.RESET_TYPE.MONTH => string.Format(NKCUtilString.GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_MONTH_02, m_StageTemplet.EnterLimit - statePlayCnt, m_StageTemplet.EnterLimit), 
				NKMStageTempletV2.RESET_TYPE.WEEK => string.Format(NKCUtilString.GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_WEEK_02, m_StageTemplet.EnterLimit - statePlayCnt, m_StageTemplet.EnterLimit), 
				_ => string.Format(NKCUtilString.GET_STRING_POPUP_DUNGEON_ENTER_LIMIT_DESC_DAY_02, m_StageTemplet.EnterLimit - statePlayCnt, m_StageTemplet.EnterLimit), 
			}, label: m_EnterLimit_TEXT);
			if (m_StageTemplet.EnterLimit - statePlayCnt <= 0)
			{
				NKCUtil.SetLabelTextColor(m_EnterLimit_TEXT, Color.red);
				m_cbtnBegin.PointerClick.RemoveAllListeners();
				m_cbtnBegin.PointerClick.AddListener(ConfirmResetStagePlayCnt);
				NKCUtil.SetLabelText(m_AB_UI_NKM_UI_OPERATION_EVENTDECK_BUTTON_TEXT, NKCUtilString.GET_STRING_WARFARE_GAME_HUD_OPERATION_RESTORE);
				NKCUtil.SetImageSprite(m_AB_UI_NKM_UI_OPERATION_EVENTDECK_BUTTON_ICON, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_ICON_ENTERLIMIT_RECOVER_SMALL"));
			}
			else
			{
				NKCUtil.SetLabelTextColor(m_EnterLimit_TEXT, Color.white);
				m_cbtnBegin.PointerClick.RemoveAllListeners();
				m_cbtnBegin.PointerClick.AddListener(OnBtnBegin);
				NKCUtil.SetLabelText(m_AB_UI_NKM_UI_OPERATION_EVENTDECK_BUTTON_TEXT, NKCUtilString.GET_STRING_WARFARE_GAME_HUD_OPERATION_START);
				NKCUtil.SetImageSprite(m_AB_UI_NKM_UI_OPERATION_EVENTDECK_BUTTON_ICON, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_ICON_GAUNTLET"));
			}
		}
		else
		{
			NKCUtil.SetLabelTextColor(m_EnterLimit_TEXT, Color.white);
			m_cbtnBegin.PointerClick.RemoveAllListeners();
			m_cbtnBegin.PointerClick.AddListener(OnBtnBegin);
			NKCUtil.SetLabelText(m_AB_UI_NKM_UI_OPERATION_EVENTDECK_BUTTON_TEXT, NKCUtilString.GET_STRING_WARFARE_GAME_HUD_OPERATION_START);
			NKCUtil.SetImageSprite(m_AB_UI_NKM_UI_OPERATION_EVENTDECK_BUTTON_ICON, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_ICON_GAUNTLET"));
		}
		NKCUtil.SetGameobjectActive(m_AB_UI_NKM_UI_OPERATION_EVENTDECK_EnterLimit, bValue);
	}

	private void SetUIByContents()
	{
		switch (m_eventDeckContents)
		{
		case DeckContents.SHADOW_PALACE:
		{
			NKCUtil.SetGameobjectActive(m_objNormalInfo, bValue: true);
			NKCUtil.SetGameobjectActive(m_objGuildCoopInfo, bValue: false);
			NKCUtil.SetImageColor(m_imgInfoBG, m_ShadowColor);
			Sprite orLoadAssetResource2 = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_OPERATION_SPRITE", "NKM_UI_OPERATION_EVENTDECK_TITLE_DECO_SHADOW");
			NKCUtil.SetImageSprite(m_imgTitleDeco, orLoadAssetResource2);
			orLoadAssetResource2 = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_WORLD_MAP_SHADOW_BG", "AB_UI_SHADOW_BG");
			NKCUtil.SetImageSprite(m_imgDeckBG, orLoadAssetResource2);
			NKCUtil.SetGameobjectActive(m_imgDeckBG, bValue: true);
			NKCUtil.SetGameobjectActive(m_objBlackBG, bValue: true);
			NKCUtil.SetGameobjectActive(m_objShadowDust, bValue: true);
			break;
		}
		case DeckContents.FIERCE_BATTLE_SUPPORT:
		{
			NKCUtil.SetGameobjectActive(m_objNormalInfo, bValue: true);
			NKCUtil.SetGameobjectActive(m_objGuildCoopInfo, bValue: false);
			NKCUtil.SetImageColor(m_imgInfoBG, m_FierceColor);
			Sprite orLoadAssetResource3 = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_OPERATION_SPRITE", "NKM_UI_OPERATION_EVENTDECK_TITLE_DECO_FIERCE");
			NKCUtil.SetImageSprite(m_imgTitleDeco, orLoadAssetResource3);
			bool bNightMareMode = false;
			NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
			if (nKCFierceBattleSupportDataMgr != null)
			{
				bNightMareMode = nKCFierceBattleSupportDataMgr.GetSelfPenalty().Count > 0;
			}
			NKCUtil.SetImageSprite(m_imgDeckBG, NKCUtil.GetSpriteFierceBattleBackgroud(bNightMareMode));
			NKCUtil.SetGameobjectActive(m_imgDeckBG, bValue: true);
			NKCUtil.SetGameobjectActive(m_objBlackBG, bValue: false);
			NKCUtil.SetGameobjectActive(m_objShadowDust, bValue: false);
			break;
		}
		case DeckContents.GUILD_COOP:
		{
			NKCUtil.SetGameobjectActive(m_objNormalInfo, bValue: false);
			NKCUtil.SetGameobjectActive(m_objGuildCoopInfo, bValue: true);
			NKCUtil.SetGameobjectActive(m_imgDeckBG, bValue: true);
			GuildSeasonTemplet guildSeasonTemplet = GuildDungeonTempletManager.GetGuildSeasonTemplet(NKCGuildCoopManager.m_SeasonId);
			if (guildSeasonTemplet != null && m_currentDungeonTempletBase != null)
			{
				GuildDungeonInfoTemplet guildDungeonInfoTemplet = GuildDungeonTempletManager.GetDungeonInfoList(guildSeasonTemplet.GetSeasonDungeonGroup()).Find((GuildDungeonInfoTemplet x) => x.GetSeasonDungeonId() == m_currentDungeonTempletBase.m_DungeonID);
				if (guildDungeonInfoTemplet != null)
				{
					NKCUtil.SetLabelText(m_lbTitleGuildCoop, m_currentDungeonTempletBase.GetDungeonName());
					NKCUtil.SetLabelText(m_lbArenaNum, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_DUNGEON_DUNGEON_UI_ARENA_INFO, guildDungeonInfoTemplet.GetArenaIndex()));
					NKCUtil.SetLabelText(m_lbNextArtifactCount, string.Format(NKCUtilString.GET_STRING_CONSORTIUM_DUNGEON_ARTIFACT_DUNGEON_CHALLENGE_INFO, NKCGuildCoopManager.GetCurrentArtifactCountByArena(guildDungeonInfoTemplet.GetArenaIndex()) + 1));
					float clearPointPercentage = NKCGuildCoopManager.GetClearPointPercentage(guildDungeonInfoTemplet.GetArenaIndex());
					NKCUtil.SetLabelText(m_lbClearPercent, string.Format("{0}%", (clearPointPercentage * 100f).ToString("N0")));
					m_imgClearGauge.fillAmount = clearPointPercentage;
				}
			}
			break;
		}
		case DeckContents.EVENT_PVP:
		{
			NKCUtil.SetGameobjectActive(m_objNormalInfo, bValue: true);
			NKCUtil.SetGameobjectActive(m_objGuildCoopInfo, bValue: false);
			NKCUtil.SetImageColor(m_imgInfoBG, m_NormalColor);
			Sprite orLoadAssetResource4 = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_OPERATION_SPRITE", "NKM_UI_OPERATION_EVENTDECK_TITLE_DECO");
			NKCUtil.SetImageSprite(m_imgTitleDeco, orLoadAssetResource4);
			NKCUtil.SetGameobjectActive(m_imgDeckBG, bValue: false);
			NKCUtil.SetGameobjectActive(m_objBlackBG, bValue: false);
			NKCUtil.SetGameobjectActive(m_objShadowDust, bValue: false);
			break;
		}
		case DeckContents.DEFENCE:
		{
			NKCUtil.SetGameobjectActive(m_objNormalInfo, bValue: true);
			NKCUtil.SetGameobjectActive(m_objGuildCoopInfo, bValue: false);
			NKCUtil.SetImageColor(m_imgInfoBG, m_NormalColor);
			Sprite sprite = null;
			NKMDefenceTemplet currentDefenceDungeonTemplet = NKMDefenceTemplet.GetCurrentDefenceDungeonTemplet(ServiceTime.Now);
			if (currentDefenceDungeonTemplet != null)
			{
				sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_BG_TEXTURE", currentDefenceDungeonTemplet.EventDeckBG, tryParseAssetName: true);
				NKCUtil.SetImageSprite(m_imgDeckBG, sprite, bDisableIfSpriteNull: true);
			}
			else
			{
				sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_BG_TEXTURE", "AB_UI_BG_2");
				NKCUtil.SetImageSprite(m_imgDeckBG, sprite);
			}
			sprite = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_OPERATION_SPRITE", "NKM_UI_OPERATION_EVENTDECK_TITLE_DECO");
			NKCUtil.SetImageSprite(m_imgTitleDeco, sprite);
			NKCUtil.SetGameobjectActive(m_imgDeckBG, bValue: true);
			NKCUtil.SetGameobjectActive(m_objBlackBG, bValue: false);
			NKCUtil.SetGameobjectActive(m_objShadowDust, bValue: false);
			break;
		}
		default:
		{
			NKCUtil.SetGameobjectActive(m_objNormalInfo, bValue: true);
			NKCUtil.SetGameobjectActive(m_objGuildCoopInfo, bValue: false);
			NKCUtil.SetImageColor(m_imgInfoBG, m_NormalColor);
			Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_OPERATION_SPRITE", "NKM_UI_OPERATION_EVENTDECK_TITLE_DECO");
			NKCUtil.SetImageSprite(m_imgTitleDeco, orLoadAssetResource);
			orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_BG_TEXTURE", "AB_UI_BG_2");
			NKCUtil.SetImageSprite(m_imgDeckBG, orLoadAssetResource);
			NKCUtil.SetGameobjectActive(m_imgDeckBG, bValue: true);
			NKCUtil.SetGameobjectActive(m_objBlackBG, bValue: false);
			NKCUtil.SetGameobjectActive(m_objShadowDust, bValue: false);
			break;
		}
		}
	}

	private void SetEnemyList()
	{
		NKCUtil.SetGameobjectActive(m_btnEnemyList, m_eventDeckContents != DeckContents.FIERCE_BATTLE_SUPPORT && m_eventDeckContents != DeckContents.EVENT_PVP);
		NKCUtil.SetGameobjectActive(m_AB_UI_NKM_UI_OPERATION_EVENTDECK_ENEMY_FIERCE_BATTLE, m_eventDeckContents == DeckContents.FIERCE_BATTLE_SUPPORT || m_eventDeckContents == DeckContents.EVENT_PVP);
		NKCUtil.SetGameobjectActive(m_csbtnFierceEnemy, m_eventDeckContents == DeckContents.FIERCE_BATTLE_SUPPORT);
		if (m_eventDeckContents == DeckContents.FIERCE_BATTLE_SUPPORT)
		{
			foreach (NKMFierceBossGroupTemplet value in NKMFierceBossGroupTemplet.Values)
			{
				if (value.DungeonID == m_currentDungeonTempletBase.m_DungeonID)
				{
					Sprite orLoadAssetResource = NKCResourceUtility.GetOrLoadAssetResource<Sprite>("ab_ui_nkm_ui_fierce_battle_boss_thumbnail", value.UI_BossFaceSlot);
					NKCUtil.SetImageSprite(m_FIERCE_BATTLE_BOSS_IMAGE_Root, orLoadAssetResource);
					NKMDungeonTemplet dungeonTemplet = NKMDungeonManager.GetDungeonTemplet(m_currentDungeonTempletBase.m_DungeonStrID);
					if (dungeonTemplet != null)
					{
						NKCUtil.SetLabelText(m_BOSS_LEVEL_Text, string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, m_currentDungeonTempletBase.m_DungeonLevel));
						NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(dungeonTemplet.m_BossUnitStrID);
						if (unitTempletBase != null)
						{
							NKCUtil.SetImageSprite(m_CLASS_Icon, NKCResourceUtility.GetOrLoadUnitRoleIcon(unitTempletBase));
							NKCUtil.SetLabelText(m_CLASS_Text, NKCUtilString.GetRoleText(unitTempletBase));
							NKCUtil.SetImageSprite(m_imgWeakMain, NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(m_currentDungeonTempletBase.m_StageSourceTypeMain), bDisableIfSpriteNull: true);
							NKCUtil.SetImageSprite(m_imgWeakSub, NKCResourceUtility.GetOrLoadUnitSourceTypeIcon(m_currentDungeonTempletBase.m_StageSourceTypeSub), bDisableIfSpriteNull: true);
						}
						NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_BOSS_INFO, unitTempletBase != null);
					}
					break;
				}
			}
			return;
		}
		if (m_eventDeckContents == DeckContents.EVENT_PVP)
		{
			Sprite eventDeckSeasonArt = NKCEventPvpMgr.GetEventDeckSeasonArt();
			NKCUtil.SetImageSprite(m_FIERCE_BATTLE_BOSS_IMAGE_Root, eventDeckSeasonArt);
			NKCUtil.SetGameobjectActive(m_FIERCE_BATTLE_BOSS_INFO, bValue: false);
		}
		else if (m_StageTemplet != null)
		{
			int stageLevel = m_StageTemplet.GetStageLevel();
			NKCUtil.SetGameobjectActive(m_lbEnemyLevel, stageLevel > 0);
			NKCUtil.SetLabelText(m_lbEnemyLevel, NKCUtilString.GET_STRING_DUNGEON_LEVEL_ONE_PARAM, stageLevel);
		}
		else if (m_currentDungeonTempletBase != null)
		{
			int num = ((m_currentDungeonTempletBase.StageTemplet == null) ? m_currentDungeonTempletBase.m_DungeonLevel : m_currentDungeonTempletBase.StageTemplet.GetStageLevel());
			NKCUtil.SetGameobjectActive(m_lbEnemyLevel, num > 0);
			NKCUtil.SetLabelText(m_lbEnemyLevel, NKCUtilString.GET_STRING_DUNGEON_LEVEL_ONE_PARAM, num);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_lbEnemyLevel, bValue: false);
		}
	}

	private void OnBtnEnemyList()
	{
		if (m_StageTemplet != null)
		{
			NKCPopupEnemyList.Instance.Open(m_StageTemplet);
		}
		else
		{
			NKCPopupEnemyList.Instance.Open(m_currentDungeonTempletBase);
		}
	}

	private void InitEventDeckData(NKMDungeonEventDeckTemplet eventDeckTemplet)
	{
		for (int i = 0; i < m_lstSlot.Count; i++)
		{
			NKCUIUnitSelectListEventDeckSlot nKCUIUnitSelectListEventDeckSlot = m_lstSlot[i];
			NKMDungeonEventDeckTemplet.EventDeckSlot unitSlot = eventDeckTemplet.GetUnitSlot(i);
			nKCUIUnitSelectListEventDeckSlot.SetEnableShowBan(m_enableShowUpBan);
			nKCUIUnitSelectListEventDeckSlot.SetEnableShowUpUnit(m_enableShowUpBan);
			nKCUIUnitSelectListEventDeckSlot.InitEventSlot(unitSlot, i, bEnableLayoutElement: true, m_equipEnabled, OnEventDeckSlotSelect, OpenUnitData);
			nKCUIUnitSelectListEventDeckSlot.ClearTouchHoldEvent();
		}
		m_cbtnChngeShip.dOnPointerHolding = null;
		NKCUtil.SetGameobjectActive(m_csbtnShipRandom, eventDeckTemplet.ShipSlot.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM);
		switch (eventDeckTemplet.ShipSlot.m_eType)
		{
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC:
		{
			NKMUnitTempletBase unitTempletBase3 = NKMUnitManager.GetUnitTempletBase(eventDeckTemplet.ShipSlot.m_ID);
			SetShipData(unitTempletBase3, eventDeckTemplet.ShipSlot.m_Level);
			NKCUtil.SetGameobjectActive(m_cbtnChngeShip, bValue: false);
			break;
		}
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM:
			SetShipData(null, 0);
			CloseShipIllust();
			NKCUtil.SetLabelText(m_lbShipName, NKCUtilString.GET_STRING_RANDOM);
			NKCUtil.SetLabelText(m_lbShipLevel, "");
			NKCUtil.SetGameobjectActive(m_cbtnChngeShip, bValue: false);
			SetShipBanLevel(null);
			break;
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST:
		{
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(eventDeckTemplet.ShipSlot.m_ID);
			SetShipData(unitTempletBase2, eventDeckTemplet.ShipSlot.m_Level);
			NKCUtil.SetGameobjectActive(m_cbtnChngeShip, bValue: true);
			break;
		}
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE:
			SetShipData(null, 0);
			NKCUtil.SetGameobjectActive(m_cbtnChngeShip, bValue: true);
			break;
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FIXED:
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(eventDeckTemplet.ShipSlot.m_ID);
			CloseShipIllust();
			m_lbShipLevel.text = "";
			m_lbShipName.text = string.Format(NKCUtilString.GET_STRING_EVENT_DECK_FIXED_TWO_PARAM, unitTempletBase.m_StarGradeMax, unitTempletBase.GetUnitName());
			NKCUtil.SetGameobjectActive(m_cbtnChngeShip, bValue: true);
			SetShipBanLevel(unitTempletBase);
			break;
		}
		default:
			Debug.LogError("invalid ship slot setup!");
			break;
		}
		if (OperatorEnabled)
		{
			if (m_OperatorSlot != null && m_OperatorSlot.m_NKM_UI_OPERATOR_DECK_SLOT != null)
			{
				m_OperatorSlot.m_NKM_UI_OPERATOR_DECK_SLOT.dOnPointerHolding = null;
			}
			NKCUtil.SetGameobjectActive(m_OperatorSlot, bValue: true);
			NKCUtil.SetGameobjectActive(m_OperatorSkill, bValue: true);
			NKCUtil.SetGameobjectActive(m_OperatorSkillCombo, bValue: true);
			switch (eventDeckTemplet.OperatorSlot.m_eType)
			{
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED:
				SetOperatorLock();
				NKCUtil.SetGameobjectActive(m_csbtnCanChangeOperator, bValue: false);
				break;
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC:
			{
				NKMUnitTempletBase unitTempletBase6 = NKMUnitManager.GetUnitTempletBase(eventDeckTemplet.OperatorSlot.m_ID);
				SetOperatorData(unitTempletBase6, eventDeckTemplet.OperatorSlot.m_Level, eventDeckTemplet.OperatorSubSkillID);
				NKCUtil.SetGameobjectActive(m_csbtnCanChangeOperator, bValue: false);
				break;
			}
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM:
				SetOperatorRandom();
				NKCUtil.SetGameobjectActive(m_csbtnCanChangeOperator, bValue: false);
				break;
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST:
			{
				NKMUnitTempletBase unitTempletBase5 = NKMUnitManager.GetUnitTempletBase(eventDeckTemplet.OperatorSlot.m_ID);
				SetOperatorData(unitTempletBase5, eventDeckTemplet.OperatorSlot.m_Level, eventDeckTemplet.OperatorSubSkillID);
				NKCUtil.SetGameobjectActive(m_csbtnCanChangeOperator, OperatorUnlocked);
				break;
			}
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE:
				if (OperatorUnlocked)
				{
					SetOperatorData(null, 0);
					NKCUtil.SetGameobjectActive(m_csbtnCanChangeOperator, bValue: true);
				}
				else
				{
					SetOperatorLock();
					NKCUtil.SetGameobjectActive(m_csbtnCanChangeOperator, bValue: false);
				}
				break;
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FIXED:
				if (OperatorUnlocked)
				{
					NKMUnitTempletBase unitTempletBase4 = NKMUnitManager.GetUnitTempletBase(eventDeckTemplet.OperatorSlot.m_ID);
					SetOperatorData(unitTempletBase4, 0);
					NKCUtil.SetGameobjectActive(m_csbtnCanChangeOperator, bValue: true);
				}
				else
				{
					SetOperatorLock();
					NKCUtil.SetGameobjectActive(m_csbtnCanChangeOperator, bValue: false);
				}
				break;
			default:
				Debug.LogError("invalid operator slot setup!");
				SetOperatorLock();
				NKCUtil.SetGameobjectActive(m_csbtnCanChangeOperator, bValue: false);
				break;
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_OperatorSlot, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnCanChangeOperator, bValue: false);
			NKCUtil.SetGameobjectActive(m_OperatorSkill, bValue: false);
			NKCUtil.SetGameobjectActive(m_OperatorSkillCombo, bValue: false);
		}
	}

	private void OpenUnitSelectList(int targetIndex)
	{
		m_currentIndex = targetIndex;
		NKMDungeonEventDeckTemplet.EventDeckSlot unitSlot = m_currentDeckTemplet.GetUnitSlot(targetIndex);
		switch (unitSlot.m_eType)
		{
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM:
			OpenRandomSlotList(unitSlot);
			return;
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC:
			return;
		}
		if (unitSlot.m_eType != m_oldSlotType)
		{
			m_oldSlotType = unitSlot.m_eType;
			NKCUIUnitSelectList.Instance.m_SortUI?.ClearFilterSet();
		}
		NKCUIUnitSelectList.UnitSelectListOptions options = MakeUnitSelectOptions(targetIndex, bIsAutoSelect: false);
		options.m_strCachingUIName = MenuName;
		NKCUIUnitSelectList.Instance.Open(options, OnUnitSelected);
	}

	private NKCUIUnitSelectList.UnitSelectListOptions MakeUnitSelectOptions(int targetIndex, bool bIsAutoSelect)
	{
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		NKMDungeonEventDeckTemplet.EventDeckSlot slotData = m_currentDeckTemplet.GetUnitSlot(targetIndex);
		NKCUIUnitSelectList.UnitSelectListOptions result = new NKCUIUnitSelectList.UnitSelectListOptions(NKM_UNIT_TYPE.NUT_NORMAL, _bMultipleSelect: false, NKM_DECK_TYPE.NDT_NORMAL);
		result.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
		result.lstSortOption = NKCUnitSortSystem.GetDefaultSortOptions(NKM_UNIT_TYPE.NUT_NORMAL, bIsCollection: false);
		result.bDescending = true;
		result.bShowRemoveSlot = m_dicSelectedUnits.ContainsKey(targetIndex) && m_dicSelectedUnits[targetIndex] != 0;
		result.bExcludeLockedUnit = false;
		result.bExcludeDeckedUnit = false;
		result.bCanSelectUnitInMission = true;
		result.bShowHideDeckedUnitMenu = false;
		result.bHideDeckedUnit = false;
		result.setExcludeUnitUID = new HashSet<long>();
		result.setExcludeUnitBaseID = new HashSet<int>();
		result.setDuplicateUnitID = new HashSet<int>();
		result.bIncludeUndeckableUnit = false;
		result.m_SortOptions.bIgnoreCityState = true;
		result.m_SortOptions.bIgnoreWorldMapLeader = true;
		result.m_SortOptions.bIgnoreMissionState = true;
		result.strEmptyMessage = NKCUtilString.GET_STRING_NO_EXIST_SELECT_UNIT;
		result.setUnitFilterCategory = NKCUnitSortSystem.setDefaultUnitFilterCategory;
		result.setUnitSortCategory = NKCUnitSortSystem.setDefaultUnitSortCategory;
		result.m_bUseFavorite = true;
		result.m_SortOptions.bUseUpData = m_eventPvpTemplet != null && !m_eventPvpTemplet.ForcedBanIgnore;
		result.m_SortOptions.bUseBanData = m_eventPvpTemplet != null && !m_eventPvpTemplet.ForcedBanIgnore;
		switch (slotData.m_eType)
		{
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FIXED:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST:
			result.setOnlyIncludeUnitBaseID = new HashSet<int>();
			result.setOnlyIncludeUnitBaseID.Add(slotData.m_ID);
			break;
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_COUNTER:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_SOLDIER:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FREE_MECHANIC:
			result.m_SortOptions.setOnlyIncludeFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
			result.m_SortOptions.setOnlyIncludeFilterOption.Add(NKCUnitSortSystem.GetFilterOption(NKMDungeonManager.GetUnitStyleTypeFromEventDeckType(slotData.m_eType)));
			break;
		}
		for (int i = 0; i < m_currentDeckTemplet.m_lstUnitSlot.Count; i++)
		{
			if (i == targetIndex)
			{
				continue;
			}
			NKMDungeonEventDeckTemplet.EventDeckSlot unitSlot = m_currentDeckTemplet.GetUnitSlot(i);
			switch (unitSlot.m_eType)
			{
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FIXED:
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST:
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC:
				result.setExcludeUnitBaseID.Add(unitSlot.m_ID);
				break;
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM:
			{
				List<int> connectedUnitList = unitSlot.GetConnectedUnitList(NKM_TEAM_TYPE.NTT_A1);
				if (connectedUnitList != null)
				{
					result.setExcludeUnitBaseID.UnionWith(connectedUnitList);
				}
				List<int> connectedUnitList2 = unitSlot.GetConnectedUnitList(NKM_TEAM_TYPE.NTT_B1);
				if (connectedUnitList2 != null)
				{
					result.setExcludeUnitBaseID.UnionWith(connectedUnitList2);
				}
				break;
			}
			default:
			{
				if (m_dicSelectedUnits.TryGetValue(i, out var value))
				{
					NKMUnitData unitFromUID = armyData.GetUnitFromUID(value);
					result.setDuplicateUnitID.Add(unitFromUID.m_UnitID);
				}
				break;
			}
			}
		}
		foreach (KeyValuePair<int, long> dicSelectedUnit in m_dicSelectedUnits)
		{
			result.setExcludeUnitUID.Add(dicSelectedUnit.Value);
		}
		if (m_dicSelectedUnits.ContainsKey(targetIndex))
		{
			result.beforeUnit = armyData.GetUnitFromUID(m_dicSelectedUnits[targetIndex]);
			result.beforeUnitDeckIndex = new NKMDeckIndex(NKM_DECK_TYPE.NDT_NONE);
		}
		if (bIsAutoSelect && slotData.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST)
		{
			result.m_SortOptions.AdditionalExcludeFilterFunc = (NKMUnitData unitData) => unitData.m_UnitLevel >= slotData.m_Level;
		}
		else
		{
			result.m_SortOptions.AdditionalExcludeFilterFunc = (NKMUnitData unitData) => m_currentDeckTemplet.IsUnitFitInSlot(slotData, unitData);
		}
		if (m_eventPvpTemplet != null && m_eventPvpTemplet.EnableTournamentBan && NKCTournamentManager.m_TournamentInfo != null)
		{
			result.setExcludeUnitID = new HashSet<int>();
			result.setExcludeUnitID.UnionWith(NKCTournamentManager.GetTournamentFinalBanIds(NKM_UNIT_TYPE.NUT_NORMAL));
			result.setExcludeUnitBaseID = new HashSet<int>();
			result.setExcludeUnitBaseID.UnionWith(NKCTournamentManager.GetTournamentFinalBanIds(NKM_UNIT_TYPE.NUT_SHIP));
		}
		result.m_SortOptions.AdditionalUnitStateFunc = (NKMUnitData unitData) => CheckUnitCondition(unitData, targetIndex, bIsAutoSelect);
		return result;
	}

	private NKCUnitSortSystem.eUnitState CheckUnitCondition(NKMUnitData unitData, int slotIndex, bool bCheckAllDeckCondition)
	{
		if (m_DeckCondition == null)
		{
			return NKCUnitSortSystem.eUnitState.NONE;
		}
		NKMDungeonEventDeckTemplet.EventDeckSlot unitSlot = m_currentDeckTemplet.GetUnitSlot(slotIndex);
		if (bCheckAllDeckCondition)
		{
			NKCUnitSortSystem.eUnitState eUnitState = CanAddThisUnitToDeck(unitData);
			if (eUnitState != NKCUnitSortSystem.eUnitState.NONE)
			{
				return eUnitState;
			}
		}
		if (m_DeckCondition.CheckEventUnitCondition(unitData, unitSlot) != NKM_ERROR_CODE.NEC_OK)
		{
			return NKCUnitSortSystem.eUnitState.DUNGEON_RESTRICTED;
		}
		return NKCUnitSortSystem.eUnitState.NONE;
	}

	private void OpenShipSelectList()
	{
		NKMDungeonEventDeckTemplet.EventDeckSlot shipSlot = m_currentDeckTemplet.ShipSlot;
		switch (shipSlot.m_eType)
		{
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM:
			OpenRandomSlotList(shipSlot);
			return;
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC:
			return;
		}
		NKCUIUnitSelectList.UnitSelectListOptions options = MakeShipSelectOptions();
		options.m_strCachingUIName = MenuName;
		options.m_SortOptions.bUseUpData = m_eventPvpTemplet != null && !m_eventPvpTemplet.ForcedBanIgnore;
		options.m_SortOptions.bUseBanData = m_eventPvpTemplet != null && !m_eventPvpTemplet.ForcedBanIgnore;
		NKCUIUnitSelectList.Instance.Open(options, OnShipSelected);
	}

	private NKCUIUnitSelectList.UnitSelectListOptions MakeOperatorSelectOptions()
	{
		NKMDungeonEventDeckTemplet.EventDeckSlot operatorSlot = m_currentDeckTemplet.OperatorSlot;
		NKCUIUnitSelectList.UnitSelectListOptions result = new NKCUIUnitSelectList.UnitSelectListOptions(NKM_UNIT_TYPE.NUT_OPERATOR, _bMultipleSelect: false, NKM_DECK_TYPE.NDT_NORMAL);
		result.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
		result.lstSortOption = NKCOperatorSortSystem.ConvertSortOption(NKCOperatorSortSystem.GetDefaultSortOptions(bIsCollection: false));
		result.bDescending = true;
		result.bShowRemoveSlot = m_SelectedOperatorUid != 0;
		result.bExcludeLockedUnit = false;
		result.bExcludeDeckedUnit = false;
		result.bCanSelectUnitInMission = true;
		result.bShowHideDeckedUnitMenu = false;
		result.bHideDeckedUnit = false;
		result.strEmptyMessage = NKCUtilString.GET_STRING_NO_EXIST_TARGET_TO_SELECT;
		result.m_OperatorSortOptions.AdditionalExcludeFilterFunc = CheckOperatorCondition;
		result.m_OperatorSortOptions.bIgnoreMissionState = true;
		result.m_bUseFavorite = true;
		result.setOperatorFilterCategory = NKCPopupFilterOperator.MakeDefaultFilterCategory(NKCPopupFilterOperator.FILTER_OPEN_TYPE.NORMAL);
		result.setOperatorSortCategory = NKCOperatorSortSystem.setDefaultOperatorSortCategory;
		if (operatorSlot.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FIXED || operatorSlot.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST)
		{
			result.setOnlyIncludeOperatorID = new HashSet<int>();
			result.setOnlyIncludeOperatorID.Add(operatorSlot.m_ID);
		}
		if (m_SelectedOperatorUid != 0L)
		{
			result.setExcludeOperatorUID = new HashSet<long>();
			result.setExcludeOperatorUID.Add(m_SelectedOperatorUid);
		}
		return result;
	}

	private bool CheckOperatorCondition(NKMOperator operatorData)
	{
		if (m_DeckCondition == null)
		{
			return true;
		}
		return m_DeckCondition.CheckEventOperatorCondition(operatorData, m_currentDeckTemplet.OperatorSlot) == NKM_ERROR_CODE.NEC_OK;
	}

	private bool CheckShipCondition(NKMUnitData unitData)
	{
		if (m_DeckCondition == null)
		{
			return true;
		}
		return m_DeckCondition.CheckEventUnitCondition(unitData, m_currentDeckTemplet.ShipSlot) == NKM_ERROR_CODE.NEC_OK;
	}

	private void CloseShipIllust()
	{
		NKCUtil.SetGameobjectActive(m_objShipFaceCard, bValue: false);
		NKCUtil.SetGameobjectActive(m_objShipUnitFaceCard, bValue: false);
		NKCUtil.SetImageSprite(m_imgShipFaceCard, null, bDisableIfSpriteNull: true);
		NKCUtil.SetImageSprite(m_imgShipUnitFaceCard, null, bDisableIfSpriteNull: true);
	}

	private void SetShipIllust(NKMUnitTempletBase shipTempletBase)
	{
		if (shipTempletBase == null)
		{
			CloseShipIllust();
			return;
		}
		NKCUtil.SetGameobjectActive(m_objShipFaceCard, shipTempletBase.IsShip());
		NKCUtil.SetGameobjectActive(m_objShipUnitFaceCard, !shipTempletBase.IsShip());
		if (shipTempletBase.IsShip())
		{
			NKCUtil.SetImageSprite(m_imgShipFaceCard, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, shipTempletBase), bDisableIfSpriteNull: true);
		}
		else
		{
			NKCUtil.SetImageSprite(m_imgShipUnitFaceCard, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, shipTempletBase), bDisableIfSpriteNull: true);
		}
	}

	private void SetShipIllust(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			CloseShipIllust();
			return;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
		if (unitTempletBase == null)
		{
			CloseShipIllust();
			return;
		}
		NKCUtil.SetGameobjectActive(m_objShipFaceCard, unitTempletBase.IsShip());
		NKCUtil.SetGameobjectActive(m_objShipUnitFaceCard, !unitTempletBase.IsShip());
		if (unitTempletBase.IsShip())
		{
			NKCUtil.SetImageSprite(m_imgShipFaceCard, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitData), bDisableIfSpriteNull: true);
		}
		else
		{
			NKCUtil.SetImageSprite(m_imgShipUnitFaceCard, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitData), bDisableIfSpriteNull: true);
		}
	}

	private void SetShipData(NKMUnitData unitData)
	{
		CloseShipIllust();
		bool bValue = false;
		if (unitData != null)
		{
			SetShipIllust(unitData);
			m_lbShipLevel.text = string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, unitData.m_UnitLevel);
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
			m_lbShipName.text = unitTempletBase.GetUnitName();
			SetShipBanLevel(unitTempletBase);
		}
		else
		{
			m_lbShipLevel.text = "";
			m_lbShipName.text = NKCUtilString.GET_STRING_DECK_SELECT_SHIP;
			SetShipBanLevel(null);
		}
		NKCUtil.SetGameobjectActive(m_AB_UI_NKM_UI_OPERATION_EVENTDECK_SHIP_FIERCE_BATTLE, bValue);
		UpdateGameEnvironmentData();
	}

	private void SetShipData(NKMUnitTempletBase shipTemplet, int level)
	{
		CloseShipIllust();
		if (shipTemplet != null)
		{
			SetShipIllust(shipTemplet);
			m_lbShipLevel.text = string.Format(NKCUtilString.GET_STRING_LEVEL_ONE_PARAM, level);
			m_lbShipName.text = shipTemplet.GetUnitName();
			SetShipBanLevel(shipTemplet);
		}
		else
		{
			m_lbShipLevel.text = "";
			m_lbShipName.text = NKCUtilString.GET_STRING_DECK_SELECT_SHIP;
			NKCUtil.SetGameobjectActive(m_AB_UI_NKM_UI_OPERATION_EVENTDECK_SHIP_FIERCE_BATTLE, bValue: false);
			NKCUtil.SetGameobjectActive(m_objNotPossibleOperator, bValue: false);
			SetShipBanLevel(null);
		}
		NKCUtil.SetGameobjectActive(m_AB_UI_NKM_UI_OPERATION_EVENTDECK_SHIP_FIERCE_BATTLE, bValue: false);
		UpdateGameEnvironmentData();
	}

	private void SetShipBanLevel(NKMUnitTempletBase shipTemplet)
	{
		if (shipTemplet != null && m_eventPvpTemplet != null && !m_eventPvpTemplet.ForcedBanIgnore && NKCBanManager.IsBanShip(shipTemplet.m_ShipGroupID))
		{
			NKCUtil.SetGameobjectActive(m_objShipBanRoot, bValue: true);
			int shipBanLevel = NKCBanManager.GetShipBanLevel(shipTemplet.m_ShipGroupID);
			NKCUtil.SetLabelText(m_lbShipBan, string.Format(NKCUtilString.GET_STRING_GAUNTLET_BAN_LEVEL_ONE_PARAM, shipBanLevel));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objShipBanRoot, bValue: false);
		}
	}

	private void OnEventDeckSlotSelect(int index)
	{
		if (!m_isSelectingLeader)
		{
			OpenUnitSelectList(index);
		}
		else if (m_lstSlot != null && m_lstSlot.Count > index && index >= 0)
		{
			if (m_lstSlot[index].CanBecomeLeader())
			{
				SetAsLeader(index);
			}
			UpdateGameEnvironmentData();
		}
	}

	private void OnShipSelected(List<long> lstUID)
	{
		if (lstUID.Count > 0)
		{
			NKCUIUnitSelectList.CheckInstanceAndClose();
			long targetUID = lstUID[0];
			OnShipSelected(targetUID);
		}
	}

	public void OnShipSelected(long targetUID)
	{
		if (m_cbtnChngeShip.gameObject.activeSelf)
		{
			NKMUnitData shipFromUID = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetShipFromUID(targetUID);
			m_SelectedShipUid = targetUID;
			SetShipData(shipFromUID);
			if (targetUID == 0L)
			{
				m_cbtnChngeShip.dOnPointerHolding = null;
			}
			else
			{
				m_cbtnChngeShip.dOnPointerHolding = OpenShipData;
			}
		}
	}

	private void OnUnitSelected(List<long> lstUID)
	{
		if (lstUID.Count > 0)
		{
			NKCUIUnitSelectList.CheckInstanceAndClose();
			long unitUID = lstUID[0];
			OnUnitSelected(m_currentIndex, unitUID);
		}
	}

	public void OnUnitSelected(int index, long unitUID)
	{
		NKMDungeonEventDeckTemplet.EventDeckSlot unitSlot = m_currentDeckTemplet.GetUnitSlot(index);
		NKMDungeonEventDeckTemplet.SLOT_TYPE eType = unitSlot.m_eType;
		if (eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED || eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC || eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM)
		{
			return;
		}
		if (unitUID == 0L)
		{
			if (index < m_lstSlot.Count)
			{
				if (unitSlot.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST)
				{
					NKMDungeonEventDeckTemplet.EventDeckSlot unitSlot2 = m_currentDeckTemplet.GetUnitSlot(index);
					m_lstSlot[index].InitEventSlot(unitSlot2, index, bEnableLayoutElement: true, m_equipEnabled, OnEventDeckSlotSelect, OpenUnitData);
				}
				else
				{
					m_lstSlot[index].SetEmpty(bEnableLayoutElement: false, null);
				}
				m_lstSlot[index].ClearTouchHoldEvent();
			}
			m_lstSlot[index].ClearTouchHoldEvent();
			m_dicSelectedUnits.Remove(index);
			m_lstSlot[index].SetActiveUpBan(value: false);
		}
		else
		{
			NKMUnitData unitFromUID = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetUnitFromUID(unitUID);
			if (((unitSlot.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FIXED || unitSlot.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST) && !unitFromUID.IsSameBaseUnit(unitSlot.m_ID)) || CheckUnitCondition(unitFromUID, index, bCheckAllDeckCondition: false) != NKCUnitSortSystem.eUnitState.NONE)
			{
				return;
			}
			int num = -1;
			foreach (KeyValuePair<int, long> dicSelectedUnit in m_dicSelectedUnits)
			{
				if (dicSelectedUnit.Value == unitUID)
				{
					num = dicSelectedUnit.Key;
				}
			}
			if (num != -1)
			{
				m_dicSelectedUnits.Remove(num);
				if (index < m_lstSlot.Count)
				{
					m_lstSlot[num].SetEmpty(bEnableLayoutElement: false, null);
				}
			}
			m_dicSelectedUnits[index] = unitUID;
			m_lstSlot[index].SetData(unitFromUID, index, bEnableLayoutElement: false, OnEventDeckSlotSelect, m_eventDeckContents == DeckContents.FIERCE_BATTLE_SUPPORT, m_equipEnabled);
			m_lstSlot[index].SetTouchHoldEvent(OpenUnitData);
			m_lstSlot[index].ConfirmLeader(m_currentLeaderIndex);
		}
		RecalculateDeckAllConditionCache();
		UpdateGameEnvironmentData();
	}

	private void OpenUnitData(NKMUnitData unitData)
	{
		_ = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		if (unitData != null)
		{
			NKCUIUnitInfo.OpenOption openOption = new NKCUIUnitInfo.OpenOption(new List<long>(m_dicSelectedUnits.Values));
			openOption.m_bShowFierceInfo = m_eventDeckContents == DeckContents.FIERCE_BATTLE_SUPPORT;
			NKCUIUnitInfo.Instance.Open(unitData, null, openOption);
		}
	}

	private void OnOperatorSelected(List<long> lstUID)
	{
		if (lstUID.Count > 0)
		{
			NKCUIUnitSelectList.CheckInstanceAndClose();
			long targetUID = lstUID[0];
			OnOperatorSelected(targetUID);
		}
	}

	public void OnOperatorSelected(long targetUID)
	{
		if (!OperatorUnlocked)
		{
			return;
		}
		m_SelectedOperatorUid = targetUID;
		NKMOperator operatorData = NKCOperatorUtil.GetOperatorData(m_SelectedOperatorUid);
		m_OperatorSlot.SetData(operatorData);
		if (operatorData != null)
		{
			m_OperatorSkillCombo.SetData(operatorData.id);
			m_OperatorMainSkill.SetData(operatorData.mainSkill.id, operatorData.mainSkill.level);
			m_OperatorSubSkill.SetData(operatorData.subSkill.id, operatorData.subSkill.level);
		}
		NKCUtil.SetGameobjectActive(m_OperatorSkillCombo, operatorData != null);
		NKCUtil.SetGameobjectActive(m_OperatorSkill, operatorData != null);
		NKCUtil.SetGameobjectActive(m_objNotPossibleOperator, bValue: false);
		if (m_OperatorSlot != null && m_OperatorSlot.m_NKM_UI_OPERATOR_DECK_SLOT != null)
		{
			if (targetUID == 0L)
			{
				m_OperatorSlot.m_NKM_UI_OPERATOR_DECK_SLOT.dOnPointerHolding = null;
			}
			else
			{
				m_OperatorSlot.m_NKM_UI_OPERATOR_DECK_SLOT.dOnPointerHolding = OpenOperatorData;
			}
		}
		UpdateGameEnvironmentData();
	}

	private void SetOperatorData(NKMUnitTempletBase unitTempletBase, int level, int subSkillID = 0)
	{
		m_OperatorSlot.SetData(unitTempletBase, level);
		NKCUtil.SetGameobjectActive(m_OperatorSkillCombo, unitTempletBase != null);
		NKCUtil.SetGameobjectActive(m_OperatorSkill, unitTempletBase != null);
		if (unitTempletBase != null)
		{
			m_OperatorSkillCombo.SetData(unitTempletBase.m_UnitID);
			NKMOperatorSkillTemplet skillTemplet = NKCOperatorUtil.GetSkillTemplet(unitTempletBase.m_lstSkillStrID[0]);
			if (skillTemplet != null)
			{
				m_OperatorMainSkill.SetData(skillTemplet.m_OperSkillID, skillTemplet.m_MaxSkillLevel);
			}
			NKCUtil.SetGameobjectActive(m_OperatorSubSkill, skillTemplet != null);
			NKMOperatorSkillTemplet skillTemplet2 = NKCOperatorUtil.GetSkillTemplet(subSkillID);
			if (skillTemplet2 != null)
			{
				m_OperatorSubSkill.SetData(skillTemplet2.m_OperSkillID, skillTemplet2.m_MaxSkillLevel);
			}
			NKCUtil.SetGameobjectActive(m_OperatorSubSkill, skillTemplet2 != null);
		}
		NKCUtil.SetGameobjectActive(m_objNotPossibleOperator, bValue: false);
		UpdateGameEnvironmentData();
	}

	private void SetOperatorLock()
	{
		if (m_OperatorSlot != null)
		{
			m_OperatorSlot.SetLock();
		}
		NKCUtil.SetGameobjectActive(m_objNotPossibleOperator, bValue: false);
		NKCUtil.SetGameobjectActive(m_OperatorSkill, bValue: false);
		NKCUtil.SetGameobjectActive(m_OperatorSkillCombo, bValue: false);
	}

	private void SetOperatorRandom()
	{
		if (m_OperatorSlot != null)
		{
			m_OperatorSlot.SetRandom();
		}
		NKCUtil.SetGameobjectActive(m_objNotPossibleOperator, bValue: false);
		NKCUtil.SetGameobjectActive(m_OperatorSkill, bValue: false);
		NKCUtil.SetGameobjectActive(m_OperatorSkillCombo, bValue: false);
	}

	private void OpenShipData()
	{
		if (m_SelectedShipUid != 0L)
		{
			NKMUnitData shipFromUID = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData.GetShipFromUID(m_SelectedShipUid);
			NKCUIShipInfo.Instance.Open(shipFromUID, NKMDeckIndex.None);
		}
	}

	private void OpenOperatorData()
	{
		if (m_SelectedOperatorUid != 0L)
		{
			NKCUIOperatorInfo.Instance.Open(NKCOperatorUtil.GetOperatorData(m_SelectedOperatorUid), new NKCUIOperatorInfo.OpenOption(new List<long> { m_SelectedOperatorUid }));
		}
	}

	public void StartGame()
	{
		if (m_StageTemplet != null && m_StageTemplet.EpisodeTemplet != null && !m_StageTemplet.EpisodeTemplet.IsOpen)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_EXCEPTION_EVENT_EXPIRED_POPUP, delegate
			{
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			});
		}
		else if (dOnEventDeckConfirm != null)
		{
			NKMEventDeckData eventDeckData = new NKMEventDeckData(m_dicSelectedUnits, m_SelectedShipUid, m_SelectedOperatorUid, m_currentLeaderIndex);
			dOnEventDeckConfirm(m_StageTemplet, m_currentDungeonTempletBase, eventDeckData, m_lselectedSupportUserUID);
			SaveDungeonDeck();
		}
	}

	public NKMEventDeckData GetEventDeckData()
	{
		return new NKMEventDeckData(m_dicSelectedUnits, m_SelectedShipUid, m_SelectedOperatorUid, m_currentLeaderIndex);
	}

	public bool CheckStartPossible()
	{
		NKM_ERROR_CODE nKM_ERROR_CODE = NKMDungeonManager.IsValidEventDeck(NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData, eventDeckData: new NKMEventDeckData(m_dicSelectedUnits, m_SelectedShipUid, m_SelectedOperatorUid, m_currentLeaderIndex), eventDeckTemplet: m_currentDeckTemplet, deckCondition: m_DeckCondition);
		if (nKM_ERROR_CODE != NKM_ERROR_CODE.NEC_OK)
		{
			NKCPopupOKCancel.OpenOKBox(nKM_ERROR_CODE);
			return false;
		}
		if (!NKCUtil.IsCanStartEterniumStage(m_StageTemplet, bCallLackPopup: true))
		{
			return false;
		}
		UpdateAttackCost();
		if (!NKCScenManager.GetScenManager().GetMyUserData().CheckPrice(m_iCostItemCount, m_iCostItemID))
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_ATTACK_COST_IS_NOT_ENOUGH);
			return false;
		}
		if (m_eventPvpTemplet != null && m_eventPvpTemplet.EnableTournamentBan && NKCTournamentManager.m_TournamentInfo != null)
		{
			foreach (int tournamentFinalBanId in NKCTournamentManager.GetTournamentFinalBanIds(NKM_UNIT_TYPE.NUT_NORMAL))
			{
				if (m_dicSelectedUnits.ContainsKey(tournamentFinalBanId))
				{
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_ATTACK_COST_IS_NOT_ENOUGH);
					return false;
				}
			}
			NKMUnitData shipFromUID = NKCScenManager.CurrentArmyData().GetShipFromUID(m_SelectedShipUid);
			if (NKCTournamentManager.GetTournamentFinalBanIds(NKM_UNIT_TYPE.NUT_SHIP).Contains(shipFromUID.GetShipGroupId()))
			{
				NKCPopupOKCancel.OpenOKBox(NKM_ERROR_CODE.NEC_FAIL_TOURNAMENT_DECK_CONTAIN_BAN_UNIT);
				return false;
			}
		}
		return true;
	}

	private void OnBtnBegin()
	{
		if (CheckStartPossible())
		{
			StartGame();
		}
	}

	private void AutoPrepare()
	{
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		HashSet<int> setExcludeIndex = new HashSet<int>(m_dicSelectedUnits.Keys);
		NKCUnitSortSystem.UnitListOptions options = new NKCUnitSortSystem.UnitListOptions
		{
			eDeckType = NKM_DECK_TYPE.NDT_DAILY,
			setExcludeUnitID = null,
			setOnlyIncludeUnitID = null,
			setDuplicateUnitID = new HashSet<int>(),
			setExcludeUnitUID = new HashSet<long>(),
			bExcludeLockedUnit = false,
			bExcludeDeckedUnit = false,
			setFilterOption = null,
			lstSortOption = new List<NKCUnitSortSystem.eSortOption>
			{
				NKCUnitSortSystem.eSortOption.Power_High,
				NKCUnitSortSystem.eSortOption.UID_First
			},
			bDescending = true,
			bHideDeckedUnit = !NKMArmyData.IsAllowedSameUnitInMultipleDeck(NKM_DECK_TYPE.NDT_DAILY),
			bPushBackUnselectable = true,
			bIncludeUndeckableUnit = false,
			bIgnoreCityState = true,
			bIgnoreWorldMapLeader = true
		};
		NKCUnitSort powerSort = null;
		options.PreemptiveSortFunc = FavoriteFirst;
		powerSort = new NKCUnitSort(myUserData, options);
		for (int i = 0; i < 8; i++)
		{
			NKMDungeonEventDeckTemplet.EventDeckSlot slotData = m_currentDeckTemplet.GetUnitSlot(i);
			NKMDungeonEventDeckTemplet.SLOT_TYPE eType = slotData.m_eType;
			if (eType != NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED && eType != NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC && eType != NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM && !m_dicSelectedUnits.ContainsKey(i))
			{
				NKMUnitData nKMUnitData = powerSort.AutoSelect(null, FilterRule);
				if (nKMUnitData != null)
				{
					OnUnitSelected(i, nKMUnitData.m_UnitUID);
				}
			}
			bool FilterRule(NKMUnitData unitData)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
				if (unitTempletBase == null)
				{
					return false;
				}
				NKMArmyData nKMArmyData = NKCScenManager.CurrentArmyData();
				if (NKMDungeonManager.CheckEventSlot(nKMArmyData, m_currentDeckTemplet, slotData, unitData.m_UnitUID, NKM_UNIT_TYPE.NUT_NORMAL) != NKM_ERROR_CODE.NEC_OK)
				{
					return false;
				}
				if (slotData.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST && slotData.m_Level > unitData.m_UnitLevel)
				{
					return false;
				}
				for (int j = 0; j < m_currentDeckTemplet.m_lstUnitSlot.Count; j++)
				{
					if (j != i)
					{
						NKMDungeonEventDeckTemplet.EventDeckSlot unitSlot = m_currentDeckTemplet.GetUnitSlot(j);
						switch (unitSlot.m_eType)
						{
						case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FIXED:
						case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST:
						case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC:
							if (unitTempletBase.IsSameBaseUnit(unitSlot.m_ID))
							{
								return false;
							}
							break;
						case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM:
							if (NKMUnitManager.CheckContainsBaseUnit(unitSlot.GetConnectedUnitList(NKM_TEAM_TYPE.NTT_A1), unitTempletBase))
							{
								return false;
							}
							if (NKMUnitManager.CheckContainsBaseUnit(unitSlot.GetConnectedUnitList(NKM_TEAM_TYPE.NTT_B1), unitTempletBase))
							{
								return false;
							}
							break;
						}
						if (m_dicSelectedUnits.TryGetValue(j, out var value))
						{
							NKMUnitData unitFromUID2 = nKMArmyData.GetUnitFromUID(value);
							if (unitTempletBase.IsSameBaseUnit(unitFromUID2.m_UnitID))
							{
								return false;
							}
						}
					}
				}
				if (CheckUnitCondition(unitData, i, bCheckAllDeckCondition: true) != NKCUnitSortSystem.eUnitState.NONE)
				{
					return false;
				}
				if (m_eventPvpTemplet != null && m_eventPvpTemplet.EnableTournamentBan && NKCTournamentManager.m_TournamentInfo != null && NKCTournamentManager.m_TournamentInfo != null && NKCTournamentManager.m_TournamentInfo.tournamentBanResult != null)
				{
					if (unitTempletBase.IsShip())
					{
						if (NKCTournamentManager.GetTournamentFinalBanIds(NKM_UNIT_TYPE.NUT_SHIP).Contains(unitTempletBase.m_ShipGroupID))
						{
							return false;
						}
					}
					else if (NKCTournamentManager.GetTournamentFinalBanIds(NKM_UNIT_TYPE.NUT_NORMAL).Contains(unitTempletBase.m_UnitID))
					{
						return false;
					}
				}
				return true;
			}
		}
		NKMDeckCondition.AllDeckCondition allDeckCondition = m_DeckCondition?.GetAllDeckCondition(NKMDeckCondition.ALL_DECK_CONDITION.UNIT_COST_TOTAL);
		HashSet<int> hsDuplicateUnit;
		if (allDeckCondition != null && !allDeckCondition.IsValueOk(GetDeckAllValueCache(NKMDeckCondition.ALL_DECK_CONDITION.UNIT_COST_TOTAL)))
		{
			hsDuplicateUnit = new HashSet<int>();
			NKCUnitSortSystem.UnitListOptions options2 = new NKCUnitSortSystem.UnitListOptions
			{
				eDeckType = NKM_DECK_TYPE.NDT_NORMAL,
				setExcludeUnitBaseID = new HashSet<int>()
			};
			foreach (NKMDungeonEventDeckTemplet.EventDeckSlot item in m_currentDeckTemplet.m_lstUnitSlot)
			{
				switch (item.m_eType)
				{
				case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FIXED:
				case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST:
				case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC:
					options2.setExcludeUnitBaseID.Add(item.m_ID);
					hsDuplicateUnit.Add(item.m_ID);
					break;
				case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM:
				{
					List<int> connectedUnitList = item.GetConnectedUnitList(NKM_TEAM_TYPE.NTT_A1);
					if (connectedUnitList != null)
					{
						options2.setExcludeUnitBaseID.UnionWith(connectedUnitList);
						hsDuplicateUnit.UnionWith(connectedUnitList);
					}
					List<int> connectedUnitList2 = item.GetConnectedUnitList(NKM_TEAM_TYPE.NTT_B1);
					if (connectedUnitList2 != null)
					{
						options2.setExcludeUnitBaseID.UnionWith(connectedUnitList2);
						hsDuplicateUnit.UnionWith(connectedUnitList2);
					}
					break;
				}
				}
			}
			options2.lstSortOption = new List<NKCUnitSortSystem.eSortOption> { NKCUnitSortSystem.eSortOption.Power_High };
			options2.bExcludeLockedUnit = false;
			options2.bExcludeDeckedUnit = false;
			options2.bHideDeckedUnit = false;
			options2.setExcludeUnitUID = new HashSet<long>();
			options2.bIncludeUndeckableUnit = false;
			options2.bIgnoreCityState = true;
			options2.bIgnoreWorldMapLeader = true;
			options2.bIgnoreMissionState = true;
			options2.AdditionalUnitStateFunc = CanAddThisUnitToDeck;
			NKCUnitSort nKCUnitSort = new NKCUnitSort(myUserData, options2);
			HashSet<long> hashSet = new HashSet<long>(m_dicSelectedUnits.Values);
			Dictionary<int, long> dictionary = new Dictionary<int, long>(m_dicSelectedUnits);
			Dictionary<int, long> dictionary2 = new Dictionary<int, long>();
			foreach (KeyValuePair<int, long> dicSelectedUnit in m_dicSelectedUnits)
			{
				NKMUnitData unitFromUID = myUserData.m_ArmyData.GetUnitFromUID(dicSelectedUnit.Value);
				if (unitFromUID != null)
				{
					hsDuplicateUnit.Add(unitFromUID.m_UnitID);
				}
			}
			while (!allDeckCondition.IsValueOk(GetCurrentUnitTotalCost(dictionary)))
			{
				NKMUnitData nKMUnitData2 = nKCUnitSort.AutoSelect(hashSet);
				if (nKMUnitData2 == null)
				{
					break;
				}
				hashSet.Add(nKMUnitData2.m_UnitUID);
				if (!CheckDuplicateUnit(nKMUnitData2.m_UnitID))
				{
					int num = FindTotalCostAutoSelectChangeableIndex(nKMUnitData2, GetCurrentUnitTotalCost(dictionary), dictionary, setExcludeIndex);
					if (num != -1)
					{
						dictionary[num] = nKMUnitData2.m_UnitUID;
						dictionary2[num] = nKMUnitData2.m_UnitUID;
						hsDuplicateUnit.Add(nKMUnitData2.m_UnitID);
					}
				}
			}
			foreach (KeyValuePair<int, long> item2 in dictionary2)
			{
				OnUnitSelected(item2.Key, item2.Value);
			}
		}
		if (m_SelectedShipUid == 0L)
		{
			NKMDungeonEventDeckTemplet.SLOT_TYPE eType = m_currentDeckTemplet.ShipSlot.m_eType;
			if (eType != NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED && eType != NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC && eType != NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM)
			{
				NKCUIUnitSelectList.UnitSelectListOptions unitSelectListOptions = MakeShipSelectOptions();
				unitSelectListOptions.lstSortOption = new List<NKCUnitSortSystem.eSortOption> { NKCUnitSortSystem.eSortOption.Power_High };
				NKMUnitData nKMUnitData3 = new NKCShipSort(myUserData, unitSelectListOptions.m_SortOptions).AutoSelect(null);
				if (nKMUnitData3 != null)
				{
					OnShipSelected(nKMUnitData3.m_UnitUID);
				}
			}
		}
		if (m_SelectedOperatorUid == 0L)
		{
			NKMDungeonEventDeckTemplet.SLOT_TYPE eType = m_currentDeckTemplet.OperatorSlot.m_eType;
			if (eType != NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED && eType != NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC && eType != NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM)
			{
				NKCUIUnitSelectList.UnitSelectListOptions unitSelectListOptions2 = MakeOperatorSelectOptions();
				unitSelectListOptions2.lstOperatorSortOption = new List<NKCOperatorSortSystem.eSortOption> { NKCOperatorSortSystem.eSortOption.Power_High };
				NKMOperator nKMOperator = new NKCOperatorSort(myUserData, unitSelectListOptions2.m_OperatorSortOptions).AutoSelect(null);
				if (nKMOperator != null)
				{
					OnOperatorSelected(nKMOperator.uid);
				}
			}
		}
		UpdateGameEnvironmentData();
		SetAsLeader(m_currentLeaderIndex);
		bool CheckDuplicateUnit(int unitID)
		{
			NKMUnitTempletBase nKMUnitTempletBase = NKMUnitTempletBase.Find(unitID);
			foreach (int item3 in hsDuplicateUnit)
			{
				if (nKMUnitTempletBase.IsSameBaseUnit(item3))
				{
					return true;
				}
			}
			return false;
		}
		int FavoriteFirst(NKMUnitData lhs, NKMUnitData rhs)
		{
			bool value = lhs.isFavorite && powerSort.GetUnitPowerCache(lhs.m_UnitUID) > 9000;
			return (rhs.isFavorite && powerSort.GetUnitPowerCache(rhs.m_UnitUID) > 9000).CompareTo(value);
		}
	}

	private int GetDeckAllValueCache(NKMDeckCondition.ALL_DECK_CONDITION type)
	{
		if (m_dicAllDeckValueCache.TryGetValue(type, out var value))
		{
			return value;
		}
		return 0;
	}

	private void RecalculateDeckAllConditionCache()
	{
		Debug.Log("RecalculateDeckAllConditionCache");
		m_dicAllDeckValueCache.Clear();
		if (m_DeckCondition?.m_dicAllDeckCondition == null)
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		foreach (NKMDeckCondition.AllDeckCondition value2 in m_DeckCondition.m_dicAllDeckCondition.Values)
		{
			int num = 0;
			for (int i = 0; i < 8; i++)
			{
				NKMDungeonEventDeckTemplet.EventDeckSlot unitSlot = m_currentDeckTemplet.GetUnitSlot(i);
				switch (unitSlot.m_eType)
				{
				case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST:
				case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC:
				{
					NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitSlot.m_ID);
					num += value2.GetAllDeckConditionValue(unitTempletBase);
					continue;
				}
				case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED:
				case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM:
					continue;
				}
				if (m_dicSelectedUnits.TryGetValue(i, out var value))
				{
					NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(nKMUserData.m_ArmyData.GetUnitFromUID(value));
					num += value2.GetAllDeckConditionValue(unitTempletBase2);
				}
			}
			m_dicAllDeckValueCache[value2.eCondition] = num;
		}
	}

	private NKCUnitSortSystem.eUnitState CanAddThisUnitToDeck(NKMUnitData unitData)
	{
		if (m_DeckCondition?.m_dicAllDeckCondition != null)
		{
			foreach (NKMDeckCondition.AllDeckCondition value in m_DeckCondition.m_dicAllDeckCondition.Values)
			{
				if (value.eCondition != NKMDeckCondition.ALL_DECK_CONDITION.UNIT_COST_TOTAL)
				{
					int currentValue = m_dicAllDeckValueCache[value.eCondition];
					if (!value.CanAddThisUnit(unitData, currentValue))
					{
						return NKCUnitSortSystem.eUnitState.DUNGEON_RESTRICTED;
					}
				}
			}
		}
		if (m_DeckCondition.CheckUnitCondition(unitData) != NKM_ERROR_CODE.NEC_OK)
		{
			return NKCUnitSortSystem.eUnitState.DUNGEON_RESTRICTED;
		}
		return NKCUnitSortSystem.eUnitState.NONE;
	}

	private int FindTotalCostAutoSelectChangeableIndex(NKMUnitData changeCandidate, int currentTotalCost, Dictionary<int, long> selectedUnits, HashSet<int> setExcludeIndex)
	{
		int respawnCost = NKMUnitManager.GetUnitStatTemplet(changeCandidate.m_UnitID).GetRespawnCost(bLeader: false, null, null);
		int num = -1;
		int num2 = -1;
		int num3 = -1;
		NKMDeckCondition.AllDeckCondition allDeckCondition = m_DeckCondition?.GetAllDeckCondition(NKMDeckCondition.ALL_DECK_CONDITION.UNIT_COST_TOTAL);
		foreach (KeyValuePair<int, long> selectedUnit in selectedUnits)
		{
			if (setExcludeIndex != null && setExcludeIndex.Contains(selectedUnit.Key))
			{
				continue;
			}
			NKMDungeonEventDeckTemplet.EventDeckSlot unitSlot = m_currentDeckTemplet.GetUnitSlot(selectedUnit.Key);
			switch (unitSlot.m_eType)
			{
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED:
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FIXED:
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST:
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC:
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM:
				continue;
			}
			if (!m_currentDeckTemplet.IsUnitFitInSlot(unitSlot, changeCandidate))
			{
				continue;
			}
			NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(selectedUnit.Value);
			int respawnCost2 = NKMUnitManager.GetUnitStatTemplet(unitFromUID.m_UnitID).GetRespawnCost(bLeader: false, null, null);
			int num4 = unitFromUID.CalculateOperationPower(NKCScenManager.CurrentUserData().m_InventoryData);
			bool flag = false;
			switch (allDeckCondition.eMoreLess)
			{
			case NKMDeckCondition.MORE_LESS.MORE:
				if (respawnCost2 < respawnCost)
				{
					flag = num < 0 || num2 < respawnCost2 || (num2 == respawnCost2 && num3 > num4);
				}
				break;
			case NKMDeckCondition.MORE_LESS.LESS:
				if (respawnCost2 > respawnCost)
				{
					flag = num < 0 || num2 > respawnCost2 || (num2 == respawnCost2 && num3 > num4);
				}
				break;
			case NKMDeckCondition.MORE_LESS.NOT:
				if (respawnCost2 != respawnCost)
				{
					flag = num3 > num4;
				}
				break;
			case NKMDeckCondition.MORE_LESS.EQUAL:
			{
				int num5 = Math.Abs(currentTotalCost - allDeckCondition.Value);
				if (currentTotalCost > allDeckCondition.Value)
				{
					if (respawnCost2 < respawnCost)
					{
						int num6 = respawnCost - respawnCost2;
						if (num6 == num5)
						{
							return selectedUnit.Key;
						}
						if (num6 < num5)
						{
							flag = num < 0 || num2 > respawnCost2 || (num2 == respawnCost2 && num3 > num4);
						}
					}
				}
				else if (currentTotalCost < allDeckCondition.Value && respawnCost2 > respawnCost)
				{
					int num7 = respawnCost2 - respawnCost;
					if (num7 == num5)
					{
						return selectedUnit.Key;
					}
					if (num7 < num5)
					{
						flag = num < 0 || num2 < respawnCost2 || (num2 == respawnCost2 && num3 > num4);
					}
				}
				break;
			}
			}
			if (flag)
			{
				num = selectedUnit.Key;
				num2 = respawnCost2;
				num3 = num4;
			}
		}
		return num;
	}

	private void ClearAll()
	{
		m_dicSelectedUnits.Clear();
		m_SelectedShipUid = 0L;
		m_SelectedOperatorUid = 0L;
		InitEventDeckData(m_currentDeckTemplet);
		SetAsLeader(m_currentLeaderIndex);
		RecalculateDeckAllConditionCache();
		UpdateGameEnvironmentData();
	}

	private void UpdateGameEnvironmentData()
	{
		UpdateDeckPower();
		UpdateEnterLimitUI();
		UpdateBattleEnvironment();
	}

	private void UpdateAttackCost()
	{
		bool flag = false;
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		bool flag2 = m_eventDeckContents == DeckContents.SHADOW_PALACE;
		if (m_eventDeckContents == DeckContents.FIERCE_BATTLE_SUPPORT)
		{
			NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
			if (nKCFierceBattleSupportDataMgr != null)
			{
				NKMFierceBossGroupTemplet bossGroupTemplet = nKCFierceBattleSupportDataMgr.GetBossGroupTemplet();
				if (bossGroupTemplet != null)
				{
					flag = true;
					m_iCostItemID = bossGroupTemplet.StageReqItemID;
					m_iCostItemCount = bossGroupTemplet.StageReqItemCount;
				}
			}
		}
		else if (m_StageTemplet == null || flag2)
		{
			m_iCostItemID = 0;
			m_iCostItemCount = 0;
		}
		else if (nKMUserData.GetStatePlayCnt(m_StageTemplet.Key) == m_StageTemplet.EnterLimit && m_StageTemplet.RestoreReqItem != null)
		{
			flag = true;
			m_iCostItemID = m_StageTemplet.RestoreReqItem.ItemId;
			m_iCostItemCount = m_StageTemplet.RestoreReqItem.Count32;
		}
		else
		{
			if (m_StageTemplet.m_StageReqItemID <= 0)
			{
				if (m_StageTemplet.m_STAGE_TYPE == STAGE_TYPE.ST_WARFARE)
				{
					m_iCostItemID = 2;
					m_iCostItemCount = m_StageTemplet.m_StageReqItemCount;
				}
				else
				{
					m_iCostItemID = 0;
					m_iCostItemCount = 0;
				}
			}
			else
			{
				flag = true;
				m_iCostItemID = m_StageTemplet.m_StageReqItemID;
				m_iCostItemCount = m_StageTemplet.m_StageReqItemCount;
			}
			if (m_iCostItemID == 2)
			{
				NKCCompanyBuff.SetDiscountOfEterniumInEnteringDungeon(NKCScenManager.CurrentUserData().m_companyBuffDataList, ref m_iCostItemCount);
			}
			m_iCostItemCount *= m_CurrMultiplyRewardCount;
		}
		if (NKMItemManager.GetItemMiscTempletByID(m_iCostItemID) == null)
		{
			m_ResourceBtn.OnShow(bShow: false);
		}
		else if (m_iCostItemID == 0)
		{
			m_ResourceBtn.OnShow(bShow: false);
		}
		else if (flag)
		{
			m_ResourceBtn.SetData(m_iCostItemID, m_iCostItemCount);
			m_ResourceBtn.OnShow(bShow: true);
		}
		else
		{
			m_ResourceBtn.OnShow(bShow: false);
		}
	}

	private int GetCurrentUnitTotalCost(Dictionary<int, long> dicSelectedUnit)
	{
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		int num = 0;
		for (int i = 0; i < m_currentDeckTemplet.m_lstUnitSlot.Count; i++)
		{
			NKMDungeonEventDeckTemplet.EventDeckSlot unitSlot = m_currentDeckTemplet.GetUnitSlot(i);
			switch (unitSlot.m_eType)
			{
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST:
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC:
			{
				NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitSlot.m_ID);
				num += unitStatTemplet.GetRespawnCost(bLeader: false, null, null);
				continue;
			}
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED:
			case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM:
				continue;
			}
			if (dicSelectedUnit.TryGetValue(i, out var value))
			{
				NKMUnitData unitFromUID = armyData.GetUnitFromUID(value);
				if (unitFromUID != null)
				{
					NKMUnitStatTemplet unitStatTemplet2 = NKMUnitManager.GetUnitStatTemplet(unitFromUID.m_UnitID);
					num += unitStatTemplet2.GetRespawnCost(bLeader: false, null, null);
				}
			}
		}
		return num;
	}

	private void CheckTutorial()
	{
		NKCTutorialManager.TutorialRequired(TutorialPoint.EventDeck);
	}

	private void ConfirmResetStagePlayCnt()
	{
		if (m_StageTemplet == null)
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		int num = 0;
		if (nKMUserData != null)
		{
			num = nKMUserData.GetStageRestoreCnt(m_StageTemplet.Key);
		}
		if (!m_StageTemplet.Restorable)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_ENTER_LIMIT_OVER);
			return;
		}
		if (num >= m_StageTemplet.RestoreLimit)
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_WARFARE_GAEM_HUD_RESTORE_LIMIT_OVER_DESC);
			return;
		}
		NKCPopupResourceWithdraw.Instance.OpenForRestoreEnterLimit(m_StageTemplet, delegate
		{
			NKCPacketSender.Send_NKMPacket_RESET_STAGE_PLAY_COUNT_REQ(m_StageTemplet.Key);
		}, num);
	}

	private void InitBattleEnvironment()
	{
		if (!(m_comBattleEnvironment == null))
		{
			if (m_comBattleEnvironment.InitData(m_DeckCondition, GetBattleConditionList(), GetPreConditionList()))
			{
				UpdateBattleEnvironment();
				m_comBattleEnvironment.Open();
			}
			else
			{
				m_comBattleEnvironment.Close();
			}
		}
	}

	private void UpdateBattleEnvironment()
	{
		if (!(m_comBattleEnvironment == null))
		{
			NKMEventDeckData eventDeckData = new NKMEventDeckData(m_dicSelectedUnits, m_SelectedShipUid, m_SelectedOperatorUid, m_currentLeaderIndex);
			m_comBattleEnvironment.UpdateData(eventDeckData, m_currentDeckTemplet);
		}
	}

	private void UpdateDeckPower()
	{
		NKMEventDeckData nKMEventDeckData = new NKMEventDeckData(m_dicSelectedUnits, m_SelectedShipUid, m_SelectedOperatorUid, m_currentLeaderIndex);
		List<GameUnitData> list = NKMDungeonManager.MakeEventDeckUnitDataList(NKCScenManager.CurrentArmyData(), m_currentDeckTemplet, m_DeckCondition, nKMEventDeckData, NKCScenManager.CurrentUserData().m_InventoryData, NKM_TEAM_TYPE.NTT_A1, bSkipRandomSlot: true);
		NKMUnitData ship = NKMDungeonManager.MakeEventDeckShipData(NKCScenManager.CurrentArmyData(), m_currentDeckTemplet, m_DeckCondition, nKMEventDeckData, NKM_TEAM_TYPE.NTT_A1, bSkipRandomSlot: true);
		NKMOperator nKMOperator = NKMDungeonManager.MakeEventDeckOperatorData(NKCScenManager.CurrentArmyData(), m_currentDeckTemplet, m_DeckCondition, nKMEventDeckData, NKM_TEAM_TYPE.NTT_A1, bSkipRandomSlot: true);
		long leaderUID = nKMEventDeckData.GetLeaderUID(list, m_currentDeckTemplet);
		IEnumerable<NKMUnitData> enumerable = list.Select((GameUnitData x) => x.unit);
		int num = NKMOperationPower.Calculate(operatorPower: nKMOperator?.CalculateOperatorOperationPower() ?? 0, ship: ship, units: enumerable, leaderUnitUID: leaderUID, invenData: NKCScenManager.CurrentUserData().m_InventoryData);
		NKCUtil.SetLabelText(m_lbDeckPower, num.ToString("N0"));
		int num2 = 0;
		int num3 = 0;
		foreach (NKMUnitData item in enumerable)
		{
			if (item != null)
			{
				num2++;
				NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(item.m_UnitID);
				if (unitStatTemplet == null)
				{
					Log.Error($"Cannot found UnitStatTemplet. UnitId:{item.m_UnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUIPrepareEventDeck.cs", 2746);
				}
				else
				{
					num3 += unitStatTemplet.GetRespawnCost(item.m_UnitUID == leaderUID, null, null);
				}
			}
		}
		float num4 = ((num2 != 0) ? ((float)num3 / (float)num2) : 0f);
		NKCUtil.SetLabelText(m_lbAvgCost, $"{num4:0.00}");
	}

	private List<NKMBattleConditionTemplet> GetBattleConditionList()
	{
		if (m_eventDeckContents == DeckContents.FIERCE_BATTLE_SUPPORT)
		{
			return NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr().GetCurBattleCondition();
		}
		if (m_eventDeckContents == DeckContents.EVENT_PVP)
		{
			return m_eventPvpTemplet?.BattleConditionTemplets;
		}
		if (m_StageTemplet != null)
		{
			return m_StageTemplet.GetBattleConditions();
		}
		return m_currentDungeonTempletBase?.BattleConditions;
	}

	private List<int> GetPreConditionList()
	{
		switch (m_eventDeckContents)
		{
		case DeckContents.FIERCE_BATTLE_SUPPORT:
			return NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr().GetCurPreConditionGroup();
		case DeckContents.EVENT_PVP:
			return m_eventPvpTemplet?.BCPreconditionGroups;
		default:
			if (m_StageTemplet != null)
			{
				return m_StageTemplet.GetPreConditionList();
			}
			return m_currentDungeonTempletBase?.m_BCPreconditionGroups;
		}
	}

	public override void OnInventoryChange(NKMItemMiscData itemData)
	{
		UpdateAttackCost();
		SetSkipCountUIData();
	}

	public override void OnUnitUpdate(NKMUserData.eChangeNotifyType eEventType, NKM_UNIT_TYPE eUnitType, long uid, NKMUnitData unitData)
	{
		foreach (KeyValuePair<int, long> dicSelectedUnit in m_dicSelectedUnits)
		{
			if (dicSelectedUnit.Value == uid)
			{
				m_lstSlot[dicSelectedUnit.Key].SetData(unitData, dicSelectedUnit.Key, bEnableLayoutElement: false, OnEventDeckSlotSelect, m_eventDeckContents == DeckContents.FIERCE_BATTLE_SUPPORT, m_equipEnabled);
				m_lstSlot[dicSelectedUnit.Key].ConfirmLeader(m_currentLeaderIndex);
				UpdateGameEnvironmentData();
				break;
			}
		}
	}

	public override void OnOperatorUpdate(NKMUserData.eChangeNotifyType eEventType, long uid, NKMOperator operatorData)
	{
		if (eEventType == NKMUserData.eChangeNotifyType.Update && m_SelectedOperatorUid == uid)
		{
			m_OperatorSlot.SetData(operatorData);
			if (operatorData != null)
			{
				m_OperatorMainSkill.SetData(operatorData.mainSkill.id, operatorData.mainSkill.level);
				m_OperatorSubSkill.SetData(operatorData.subSkill.id, operatorData.subSkill.level);
			}
			UpdateGameEnvironmentData();
		}
	}

	public void RefreshUIByContents()
	{
		SetUIByContents();
	}

	private NKCUIUnitSelectList.UnitSelectListOptions MakeShipSelectOptions()
	{
		NKMDungeonEventDeckTemplet.EventDeckSlot shipSlot = m_currentDeckTemplet.ShipSlot;
		NKCUIUnitSelectList.UnitSelectListOptions result = new NKCUIUnitSelectList.UnitSelectListOptions(NKM_UNIT_TYPE.NUT_SHIP, _bMultipleSelect: false, NKM_DECK_TYPE.NDT_NORMAL);
		result.setFilterOption = new HashSet<NKCUnitSortSystem.eFilterOption>();
		result.lstSortOption = NKCUnitSortSystem.GetDefaultSortOptions(NKM_UNIT_TYPE.NUT_SHIP, bIsCollection: false);
		result.bDescending = true;
		result.bShowRemoveSlot = m_SelectedShipUid != 0;
		result.bExcludeLockedUnit = false;
		result.bExcludeDeckedUnit = false;
		result.bCanSelectUnitInMission = true;
		result.bShowHideDeckedUnitMenu = false;
		result.bHideDeckedUnit = false;
		result.strEmptyMessage = NKCUtilString.GET_STRING_NO_EXIST_TARGET_TO_SELECT;
		result.m_SortOptions.AdditionalExcludeFilterFunc = CheckShipCondition;
		result.m_SortOptions.bIgnoreMissionState = true;
		result.setShipFilterCategory = NKCUnitSortSystem.setDefaultShipFilterCategory;
		result.setShipSortCategory = NKCUnitSortSystem.setDefaultShipSortCategory;
		if (shipSlot.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FIXED || shipSlot.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST)
		{
			result.setOnlyIncludeUnitBaseID = new HashSet<int>();
			result.setOnlyIncludeUnitBaseID.Add(shipSlot.m_ID);
		}
		if (m_SelectedShipUid != 0L)
		{
			result.setExcludeUnitUID = new HashSet<long>();
			result.setExcludeUnitUID.Add(m_SelectedShipUid);
		}
		if (m_eventPvpTemplet != null && m_eventPvpTemplet.EnableTournamentBan && NKCTournamentManager.m_TournamentInfo != null)
		{
			result.setExcludeUnitBaseID = NKCTournamentManager.GetTournamentFinalBanIds(NKM_UNIT_TYPE.NUT_SHIP);
		}
		return result;
	}

	private void OpenOperatorSelectList()
	{
		if (!OperatorUnlocked)
		{
			return;
		}
		switch (m_currentDeckTemplet.OperatorSlot.m_eType)
		{
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM:
			OpenRandomSlotList(m_currentDeckTemplet.OperatorSlot);
			return;
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC:
			return;
		}
		NKCUIUnitSelectList.UnitSelectListOptions options = MakeOperatorSelectOptions();
		if (m_SelectedOperatorUid != 0L)
		{
			options.beforeOperator = NKCOperatorUtil.GetOperatorData(m_SelectedOperatorUid);
		}
		options.m_strCachingUIName = MenuName;
		NKCUIUnitSelectList.Instance.Open(options, OnOperatorSelected);
	}

	private void OnClickOperatorSlot(long operatorUID)
	{
		OpenOperatorSelectList();
	}

	public bool SetAsLeader(int leaderIndex)
	{
		if (m_lstSlot == null)
		{
			return false;
		}
		bool result = false;
		int count = m_lstSlot.Count;
		for (int i = 0; i < count; i++)
		{
			if (!(m_lstSlot[i] == null) && m_lstSlot[i].ConfirmLeader(leaderIndex))
			{
				result = true;
				m_currentLeaderIndex = leaderIndex;
			}
		}
		m_isSelectingLeader = false;
		m_csbtnLeaderSelect?.Select(bSelect: false);
		return result;
	}

	private void SetDefaultLeader()
	{
		m_currentLeaderIndex = -1;
		if (m_lstSlot == null)
		{
			return;
		}
		int count = m_lstSlot.Count;
		for (int i = 0; i < count; i++)
		{
			if (m_lstSlot[i] == null)
			{
				continue;
			}
			if (m_currentLeaderIndex < 0)
			{
				if (m_lstSlot[i].ConfirmLeader(i))
				{
					m_currentLeaderIndex = i;
				}
			}
			else
			{
				m_lstSlot[i].ConfirmLeader(m_currentLeaderIndex);
			}
		}
	}

	private void OnClickLeaderSelect()
	{
		if (m_lstSlot != null)
		{
			m_isSelectingLeader = !m_isSelectingLeader;
			m_csbtnLeaderSelect.Select(m_isSelectingLeader);
			int count = m_lstSlot.Count;
			for (int i = 0; i < count; i++)
			{
				m_lstSlot[i]?.LeaderSelectState(m_isSelectingLeader);
			}
		}
	}

	private void OnClickFierceEnemy()
	{
		NKCPopupEnemyList.Instance.Open(m_currentDungeonTempletBase);
	}

	private void OnClickDeckCopy()
	{
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		int shipID = 0;
		NKMUnitData shipFromUID = armyData.GetShipFromUID(m_SelectedShipUid);
		if (shipFromUID != null)
		{
			shipID = shipFromUID.m_UnitID;
		}
		int operID = armyData.GetOperatorFromUId(m_SelectedOperatorUid)?.id ?? 0;
		List<int> list = new List<int>();
		for (int i = 0; i < 8; i++)
		{
			if (m_dicSelectedUnits.ContainsKey(i))
			{
				NKMUnitData unitFromUID = armyData.GetUnitFromUID(m_dicSelectedUnits[i]);
				if (unitFromUID != null)
				{
					list.Add(unitFromUID.m_UnitID);
					continue;
				}
			}
			list.Add(0);
		}
		NKCPopupDeckCopy.MakeDeckCopyCode(shipID, operID, list, m_currentLeaderIndex);
	}

	private void OnClickDeckPaste()
	{
		List<long> list = new List<long>();
		for (int i = 0; i < 8; i++)
		{
			if (m_dicSelectedUnits.ContainsKey(i))
			{
				list.Add(m_dicSelectedUnits[i]);
			}
			else
			{
				list.Add(0L);
			}
		}
		NKCPopupDeckCopy.Instance.Open(m_SelectedShipUid, m_SelectedOperatorUid, list, m_currentLeaderIndex);
	}

	private void LoadDungeonDeck()
	{
		string curEventDeckKey = GetCurEventDeckKey();
		if (string.IsNullOrEmpty(curEventDeckKey) || !PlayerPrefs.HasKey(curEventDeckKey))
		{
			return;
		}
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		string text = PlayerPrefs.GetString(curEventDeckKey);
		string[] array = text.Split('&');
		foreach (string text2 in array)
		{
			int num = text2.IndexOf('/');
			if (num < 0)
			{
				break;
			}
			int.TryParse(text2.Substring(0, num), out var result);
			long.TryParse(text2.Substring(num + 1, text2.Length - (num + 1)), out var result2);
			NKMUnitData unitFromUID = armyData.GetUnitFromUID(result2);
			if (!m_currentDeckTemplet.IsUnitFitInSlot(m_currentDeckTemplet.GetUnitSlot(result), unitFromUID))
			{
				continue;
			}
			if (m_eventPvpTemplet != null && m_eventPvpTemplet.EnableTournamentBan && NKCTournamentManager.m_TournamentInfo != null && NKCTournamentManager.m_TournamentInfo.tournamentBanResult != null)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitFromUID.m_UnitID);
				if (unitTempletBase.IsShip())
				{
					if (NKCTournamentManager.GetTournamentFinalBanIds(NKM_UNIT_TYPE.NUT_SHIP).Contains(unitTempletBase.m_ShipGroupID))
					{
						continue;
					}
				}
				else if (NKCTournamentManager.GetTournamentFinalBanIds(NKM_UNIT_TYPE.NUT_NORMAL).Contains(unitTempletBase.m_UnitID))
				{
					continue;
				}
			}
			OnUnitSelected(result, result2);
		}
		int num2 = text.IndexOf('_') + 1;
		if (num2 > 0)
		{
			int num3 = text.IndexOf('o');
			int num4 = text.IndexOf('l');
			int num5 = text.Length - num2;
			if (num3 > 0)
			{
				num5 -= text.Length - num3;
			}
			else if (num4 > 0)
			{
				num5 -= text.Length - num4;
			}
			long.TryParse(text.Substring(num2, num5), out var result3);
			NKMUnitData shipFromUID = armyData.GetShipFromUID(result3);
			if (m_currentDeckTemplet.IsUnitFitInSlot(m_currentDeckTemplet.ShipSlot, shipFromUID))
			{
				OnShipSelected(result3);
				if (m_eventPvpTemplet != null && m_eventPvpTemplet.EnableTournamentBan && NKCTournamentManager.m_TournamentInfo != null && NKCTournamentManager.GetTournamentFinalBanIds(NKM_UNIT_TYPE.NUT_SHIP).Contains(shipFromUID.GetShipGroupId()))
				{
					OnShipSelected(0L);
				}
			}
		}
		int num6 = text.IndexOf('|') + 1;
		if (num6 > 0)
		{
			int num7 = text.IndexOf('l');
			int num8 = text.Length - num6;
			if (num7 > 0)
			{
				num8 -= text.Length - num7;
			}
			long.TryParse(text.Substring(num6, num8), out var result4);
			NKMOperator operatorFromUId = armyData.GetOperatorFromUId(result4);
			if (m_currentDeckTemplet.IsOperatorFitInSlot(operatorFromUId))
			{
				OnOperatorSelected(result4);
			}
		}
		int num9 = text.IndexOf('^') + 1;
		if (num9 > 0)
		{
			int length = text.Length - num9;
			string text3 = text.Substring(num9, length);
			long num10 = NKCScenManager.CurrentUserData()?.m_UserUID ?? 0;
			long result5 = 0L;
			if (text3.Length > 1)
			{
				long.TryParse(text3.Substring(1), out result5);
			}
			if (result5 == num10 && int.TryParse(text3.Substring(0, 1), out var result6))
			{
				m_currentLeaderIndex = result6;
			}
		}
	}

	private string GetCurEventDeckKey()
	{
		if (m_currentDungeonTempletBase != null)
		{
			if (m_eventDeckContents == DeckContents.FIERCE_BATTLE_SUPPORT)
			{
				NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
				if (nKCFierceBattleSupportDataMgr != null && nKCFierceBattleSupportDataMgr.GetStatus() == NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_ACTIVATE)
				{
					return NKMDungeonManager.GetFierceEventDeckKey(nKCFierceBattleSupportDataMgr.GetBossGroupTemplet());
				}
			}
			return string.Format($"NKM_PREPARE_EVENT_DECK_{m_currentDungeonTempletBase.m_DungeonID}");
		}
		if (m_StageTemplet != null && m_StageTemplet.PhaseTemplet != null)
		{
			return string.Format($"NKM_PREPARE_EVENT_DECK_{m_StageTemplet.PhaseTemplet.Id}");
		}
		if (m_eventPvpTemplet != null)
		{
			return string.Format($"NKM_PREPARE_EVENT_DECK_EVENTPVP_{m_eventPvpTemplet.SeasonId}");
		}
		return "";
	}

	private void SaveDungeonDeck()
	{
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		StringBuilder stringBuilder = new StringBuilder();
		string curEventDeckKey = GetCurEventDeckKey();
		if (string.IsNullOrEmpty(curEventDeckKey))
		{
			return;
		}
		for (int i = 0; i < 8; i++)
		{
			if (m_dicSelectedUnits.ContainsKey(i))
			{
				stringBuilder.Append($"{i}/{m_dicSelectedUnits[i]}&");
			}
		}
		if (m_SelectedShipUid != 0L && armyData.IsHaveShipFromUID(m_SelectedShipUid))
		{
			stringBuilder.Append($"s_{m_SelectedShipUid}");
		}
		if (m_SelectedOperatorUid != 0L && armyData.IsHaveOperatorFromUID(m_SelectedOperatorUid))
		{
			stringBuilder.Append($"o|{m_SelectedOperatorUid}");
		}
		if (m_currentLeaderIndex != 0)
		{
			long num = NKCScenManager.CurrentUserData()?.m_UserUID ?? 0;
			stringBuilder.Append($"l^{m_currentLeaderIndex}{num}");
		}
		PlayerPrefs.SetString(curEventDeckKey, stringBuilder.ToString());
	}

	private void OnDragBegin(PointerEventData eventData, int beginIndex)
	{
		if (m_bDrag)
		{
			return;
		}
		switch (m_currentDeckTemplet.GetUnitSlot(beginIndex).m_eType)
		{
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_CLOSED:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_FIXED:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_GUEST:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_NPC:
		case NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM:
			return;
		}
		if (m_dicSelectedUnits.TryGetValue(beginIndex, out var value))
		{
			m_dragBeginIndex = beginIndex;
			NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(value);
			NKCUtil.SetImageSprite(m_imgDragObject, NKCResourceUtility.GetorLoadUnitSprite(NKCResourceUtility.eUnitResourceType.FACE_CARD, unitFromUID));
			NKCUtil.SetGameobjectActive(m_imgDragObject, bValue: true);
			MoveDragObject(eventData.position);
			m_bDrag = true;
		}
	}

	private void OnDrag(PointerEventData eventData, int endIndex)
	{
		if (m_bDrag)
		{
			MoveDragObject(eventData.position);
		}
	}

	private void OnDragEnd(PointerEventData eventData, int endIndex)
	{
		if (m_bDrag)
		{
			ResetDrag();
		}
	}

	private void OnDrop(PointerEventData eventData, int endIndex)
	{
		if (m_bDrag)
		{
			SwapSlot(m_dragBeginIndex, endIndex);
			ResetDrag();
		}
	}

	private void SwapSlot(int indexA, int indexB)
	{
		if (indexA == indexB)
		{
			return;
		}
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		bool flag;
		if (m_dicSelectedUnits.TryGetValue(indexA, out var value))
		{
			flag = NKMDungeonManager.CheckEventSlot(nKMUserData.m_ArmyData, m_currentDeckTemplet, m_currentDeckTemplet.GetUnitSlot(indexB), value, NKM_UNIT_TYPE.NUT_NORMAL) == NKM_ERROR_CODE.NEC_OK;
		}
		else
		{
			value = 0L;
			flag = true;
		}
		bool flag2;
		if (m_dicSelectedUnits.TryGetValue(indexB, out var value2))
		{
			flag2 = NKMDungeonManager.CheckEventSlot(nKMUserData.m_ArmyData, m_currentDeckTemplet, m_currentDeckTemplet.GetUnitSlot(indexA), value2, NKM_UNIT_TYPE.NUT_NORMAL) == NKM_ERROR_CODE.NEC_OK;
		}
		else
		{
			value2 = 0L;
			flag2 = true;
		}
		if (flag && flag2)
		{
			m_dicSelectedUnits.Remove(indexA);
			m_dicSelectedUnits.Remove(indexB);
			OnUnitSelected(indexA, value2);
			OnUnitSelected(indexB, value);
			if (m_currentLeaderIndex == indexA)
			{
				SetAsLeader(indexB);
			}
			else if (m_currentLeaderIndex == indexB)
			{
				SetAsLeader(indexA);
			}
			else
			{
				SetAsLeader(m_currentLeaderIndex);
			}
		}
	}

	private void ResetDrag()
	{
		m_bDrag = false;
		m_dragBeginIndex = -1;
		NKCUtil.SetGameobjectActive(m_imgDragObject, bValue: false);
	}

	private void MoveDragObject(Vector2 touchPos)
	{
		Vector3 worldPoint = Vector3.zero;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, touchPos, NKCCamera.GetSubUICamera(), out worldPoint);
		worldPoint.x /= rectTransform.lossyScale.x;
		worldPoint.y /= rectTransform.lossyScale.y;
		worldPoint.z = 0f;
		m_imgDragObject.transform.localPosition = worldPoint;
	}

	private void OnClickShipRandom()
	{
		if (m_currentDeckTemplet != null)
		{
			OpenRandomSlotList(m_currentDeckTemplet.ShipSlot);
		}
	}

	private void OpenRandomSlotList(NKMDungeonEventDeckTemplet.EventDeckSlot slotData)
	{
		if (slotData.m_eType == NKMDungeonEventDeckTemplet.SLOT_TYPE.ST_RANDOM)
		{
			HashSet<int> randomSlotUnitList = m_currentDeckTemplet.GetRandomSlotUnitList(slotData);
			NKCUISlotListViewer.Instance.OpenGenericUnitList(NKCStringTable.GetString("SI_DP_SLOT_RANDOM_UNIT_VIEWR"), NKCStringTable.GetString("SI_DP_SLOT_RANDOM_UNIT_VIEWR_DESC"), randomSlotUnitList, slotData.m_Level);
		}
	}

	private void OnClickAssistUnit()
	{
		if (!m_bReqSupportUnitList)
		{
			NKCPacketSender.Send_NKMPacket_SUPPORT_UNIT_LIST_REQ();
			m_bReqSupportUnitList = true;
		}
	}

	public void OnRecv(NKMPacket_SUPPORT_UNIT_LIST_ACK sPacket)
	{
		m_bReqSupportUnitList = false;
		NKCUIPopupAssistSelect.Instance.Open(sPacket.supportUnitData, NKMDeckIndex.None, m_lselectedSupportUserUID);
	}

	public void OnRecv(NKMPacket_SET_DUNGEON_SUPPORT_UNIT_ACK sPacket)
	{
		if (sPacket.selectUnitData != null)
		{
			NKMAsyncUnitData asyncUnit = sPacket.selectUnitData.asyncUnitEquip.asyncUnit;
			NKMUnitData nKMUnitData = NKCUtil.MakeDummyUnit(asyncUnit.unitId, asyncUnit.unitLevel, (short)asyncUnit.limitBreakLevel, asyncUnit.tacticLevel, asyncUnit.reactorLevel);
			nKMUnitData.m_SkinID = asyncUnit.skinId;
			m_lselectedSupportUserUID = sPacket.selectUnitData.userUid;
			m_UnitAssistSlot.SetData(nKMUnitData, NKMDeckIndex.None, bEnableLayoutElement: false, null);
			m_UnitAssistSlot.SetEquipData(NKCUIPopupAssistSelect.GetEquipSetData(sPacket.selectUnitData.asyncUnitEquip));
			NKCUtil.SetGameobjectActive(m_objUnitAssistEmpty, bValue: false);
			NKCUtil.SetGameobjectActive(m_UnitAssistSlot, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objUnitAssistEmpty, bValue: true);
			m_lselectedSupportUserUID = 0L;
			m_UnitAssistSlot.SetEmpty(bEnableLayoutElement: false, null);
			NKCUtil.SetGameobjectActive(m_UnitAssistSlot, bValue: false);
		}
	}
}
