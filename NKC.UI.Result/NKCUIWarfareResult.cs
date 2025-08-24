using System;
using System.Collections;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Community;
using ClientPacket.Mode;
using ClientPacket.Warfare;
using Cs.Core.Util;
using Cs.Logging;
using NKC.Templet;
using NKC.UI.Friend;
using NKM;
using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace NKC.UI.Result;

public class NKCUIWarfareResult : NKCUIBase
{
	public delegate void CallBackWhenClosed(NKM_SCEN_ID scenID);

	public enum NKC_WARFARE_RESULT_GRADE
	{
		NWRG_F,
		NWRG_C,
		NWRG_B,
		NWRG_A,
		NWRG_S,
		NWRG_COUNT
	}

	public enum USE_CONTENTS
	{
		WARFARE,
		DIVE,
		SHADOW,
		TRIM,
		DUNGEON,
		DEFENCE
	}

	private enum eRewardType
	{
		None,
		FirstClear,
		FirstAllClear,
		AllStarClear,
		OneTime
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_result";

	private const string UI_ASSET_NAME = "NKM_UI_WARFARE_RESULT";

	private static NKCUIWarfareResult m_Instance;

	public GameObject m_NKM_UI_WARFARE_RESULT_ROOT;

	public Animator m_Animtor;

	public List<GameObject> m_lst_NKM_UI_WARFARE_RESULT_BACKFX;

	public List<GameObject> m_lst_NKM_UI_WARFARE_RESULT_INFO_GRADE;

	public NKCComUITalkBox m_AB_UI_TALK_BOX;

	public List<GameObject> m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON;

	public List<GameObject> m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON_SUCCESS;

	public List<GameObject> m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON_FAIL;

	public List<Text> m_NKM_UI_WARFARE_RESULT_INFO_MISSION_TEXT;

	public Transform m_NKM_UI_WARFARE_RESULT_REWARDS_Content;

	public RectTransform m_UNITSPINEOBJECT;

	public RectTransform m_SHIPSPINEOBJECT;

	public GameObject m_PARTICLEOBJECT;

	[Header("우측")]
	public GameObject m_TITLE_REWARD;

	public GameObject m_Rewards_alert;

	public Image m_NKM_UI_WARFARE_RESULT_REWARD_BONUS_TYPE;

	public GameObject m_NKM_UI_WARFARE_RESULT_MULTIPLY_REWARD_TAG;

	public NKCComText m_MultiplyReward_COUNT_TEXT;

	[Header("패배시 게임 팁 관련")]
	public GameObject m_TITLE_GAMETIP;

	public GameObject m_NKM_UI_WARFARE_RESULT_GAMETIP;

	public Text m_NKM_UI_WARFARE_RESULT_GAMETIP_text;

	public GameObject m_objEntryFeeReturn;

	public NKCUIWRRewardSlot m_slotEntryFeeReturn;

	[Header("Button")]
	public NKCUIComStateButton m_csbtnCloseToHome;

	public NKCUIComStateButton m_csbtnCloseToList;

	public NKCUIComStateButton m_csbtnRestart;

	public NKCUIComStateButton m_csbtnConfirm;

	public NKCUIComStateButton m_csbtnNextMission;

	public NKCUIComStateButton m_csbtnBattleStat;

	public NKCUIComStateButton m_csbtnDeckEdit;

	[Header("반복작전")]
	public GameObject m_NKM_UI_WARFARE_RESULT_REPEAT;

	public NKCUIComStateButton m_NKM_UI_WARFARE_RESULT_BTN_REPEAT;

	public Text m_NKM_UI_WARFARE_RESULT_REPEAT_COUNT;

	public Image m_NKM_UI_WARFARE_RESULT_BTN_REPEAT_COUNT_DOWN_Gauge;

	public Text m_NKM_UI_WARFARE_RESULT_BTN_REPEAT_COUNT_DOWN;

	[Header("배경 Fallback")]
	public GameObject m_objBGFallBack;

	private CallBackWhenClosed m_CallBackWhenClosed;

	private NKM_SCEN_ID m_NextScenID;

	private NKMWarfareClearData m_NKMWarfareClearData;

	private NKMDeckIndex m_CurrentDeck;

	private List<NKCUIWRRewardSlot> m_lstNKCUIWRRewardSlot = new List<NKCUIWRRewardSlot>();

	private float m_fElapsedTime;

	private const int REWARD_SLOT_MAKE_START_FRAME = 131;

	private const float REWARD_SLOT_MAKE_START_TIME = 2.1833334f;

	private bool m_bDoneMakeRewardSlot;

	private bool m_bReservedShowGetUnit;

	private float m_fTimeToShowGetUnit;

	public const float REWARD_SLOT_ANI_INTERVAL_TIME = 2f / 15f;

	private const int ANI_FRAME = 208;

	private const int TUTORIAL_CHECK_ANIM_FRAME = 181;

	private const float TUTORIAL_CHECK_NORMALIZED_TIME = 0.8701923f;

	private const float SECOND_PER_ANI_FRAME = 1f / 60f;

	private bool m_bTriggeredAutoOK;

	private NKCASUIUnitIllust m_NKCASUISpineIllust;

	private NKCASUIUnitIllust m_NKCASUISpineIllustShip;

	private Coroutine m_coIntro;

	private bool m_bFinishedIntroMovie;

	private bool m_bfirstClear;

	private bool m_bFirstAllClear;

	private WarfareSupporterListData m_guestSptData;

	private USE_CONTENTS m_eContents;

	private bool m_bClearDive;

	private NKMRewardData m_DiveRewardData;

	private NKMRewardData m_DiveRewardDataArtifact;

	private NKMRewardData m_DiveRewardDataStorm;

	private List<int> m_lstArtifact = new List<int>();

	private bool m_bArtifactReturnEvent;

	private NKMShadowGameResult m_ShadowResult;

	private List<int> m_lstShadowBestTime;

	private bool m_bShadowRecordEvent;

	private NKMTrimClearData m_trimClearData;

	private TrimModeState m_trimModeState;

	private int m_trimBestScore;

	private bool m_bTrimFirstClear;

	private bool m_bTrimEvent;

	private bool m_bUserLevelUpPopupOpened;

	private bool m_bWaitContentUnlockPopup;

	private bool m_bWaitFriendRequestPopup;

	private bool m_bDefenceEvent;

	private int m_dummyUnitID;

	private int m_dummyUnitSkinID;

	private int m_dummyShipID;

	private long m_leaderUnitUID;

	private long m_leaderShipUID;

	private bool m_bRequiredTutorial;

	private float m_fFinalAutoProcessWaitTime = 5f;

	private const float AUTO_PROCESS_WAIT_TIME = 5f;

	private const float AUTO_PROCESS_WAIT_TIME_WITH_SPECIAL_EVENT = 10f;

	private bool m_bPause;

	private Vector3 m_vDefaultTalkBoxPos;

	private NKCUIResult.BattleResultData m_battleResultData;

	private NKCPopupArtifactExchange m_NKCPopupArtifactExchange;

	public float MoviePlaySpeed = 1f;

	private bool m_bWaitingMovie;

	public static NKCUIWarfareResult Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIWarfareResult>("ab_ui_nkm_ui_result", "NKM_UI_WARFARE_RESULT", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIWarfareResult>();
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

	public override string MenuName => NKCUtilString.GET_STRING_WARFARE_RESULT;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	private NKCPopupArtifactExchange NKCPopupArtifactExchange
	{
		get
		{
			if (m_NKCPopupArtifactExchange == null)
			{
				NKCUIManager.LoadedUIData loadedUIData = NKCUIManager.OpenNewInstance<NKCPopupArtifactExchange>("AB_UI_NKM_UI_WORLD_MAP_DIVE", "NKM_UI_DIVE_ARTIFACT_EXCHANGE_POPUP", NKCUIManager.GetUIBaseRect(NKCUIManager.eUIBaseRect.UIFrontPopup), null);
				m_NKCPopupArtifactExchange = loadedUIData.GetInstance<NKCPopupArtifactExchange>();
				m_NKCPopupArtifactExchange.InitUI();
			}
			return m_NKCPopupArtifactExchange;
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

	public void CheckNKCPopupArtifactExchangeAndClose()
	{
		if (m_NKCPopupArtifactExchange != null && m_NKCPopupArtifactExchange.IsOpen)
		{
			m_NKCPopupArtifactExchange.Close();
		}
	}

	private void InitUI()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		NKCUtil.SetButtonClickDelegate(m_csbtnCloseToHome, CloseToHome);
		NKCUtil.SetButtonClickDelegate(m_csbtnCloseToList, CloseToOK);
		NKCUtil.SetButtonClickDelegate(m_csbtnRestart, OnClickRetry);
		NKCUtil.SetButtonClickDelegate(m_csbtnConfirm, CloseToOK);
		NKCUtil.SetButtonClickDelegate(m_csbtnNextMission, PlayNextOperation);
		NKCUtil.SetButtonClickDelegate(m_NKM_UI_WARFARE_RESULT_BTN_REPEAT, OnClickRepeatOperation);
		NKCUtil.SetButtonClickDelegate(m_csbtnBattleStat, OnClickBattleStat);
		NKCUtil.SetButtonClickDelegate(m_csbtnDeckEdit, OnDeckEdit);
		m_vDefaultTalkBoxPos = m_AB_UI_TALK_BOX.transform.position;
	}

	public void OpenForDive(bool bClear, NKMRewardData cNKMRewardData, NKMRewardData cNKMRewardDataArtifact, NKMRewardData cNKMRewardDataStrom, List<int> lstArtifact, NKMDeckIndex currentDeck, CallBackWhenClosed callBackWhenClosed)
	{
		m_bPause = false;
		m_eContents = USE_CONTENTS.DIVE;
		SetAutoProcessWaitTime();
		m_bClearDive = bClear;
		m_DiveRewardData = cNKMRewardData;
		m_DiveRewardDataArtifact = cNKMRewardDataArtifact;
		m_DiveRewardDataStorm = cNKMRewardDataStrom;
		m_lstArtifact.Clear();
		if (lstArtifact != null)
		{
			m_lstArtifact.AddRange(lstArtifact);
		}
		m_bArtifactReturnEvent = false;
		m_bWaitingMovie = false;
		m_bDoneMakeRewardSlot = false;
		m_bReservedShowGetUnit = false;
		m_bFinishedIntroMovie = false;
		m_bWaitContentUnlockPopup = false;
		m_bTriggeredAutoOK = false;
		if (bClear)
		{
			if (m_DiveRewardData != null && m_DiveRewardData.GetUnitCount() > 0)
			{
				m_bReservedShowGetUnit = true;
				List<NKCUISlot.SlotData> list = NKCUISlot.MakeSlotDataListFromReward(m_DiveRewardData);
				m_fTimeToShowGetUnit = (float)(list.Count - 1) * (2f / 15f) + 2.1833334f + 5f / 6f;
			}
			if (m_DiveRewardDataStorm != null)
			{
				NKCUISlot.MakeSlotDataListFromReward(m_DiveRewardDataStorm);
			}
		}
		for (int i = 0; i < m_lstNKCUIWRRewardSlot.Count; i++)
		{
			NKCUIWRRewardSlot nKCUIWRRewardSlot = m_lstNKCUIWRRewardSlot[i];
			nKCUIWRRewardSlot.InvalidAni();
			NKCUtil.SetGameobjectActive(nKCUIWRRewardSlot.gameObject, bValue: false);
		}
		SetUIRewardAlert(bSet: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_REWARD_BONUS_TYPE.gameObject, bValue: false);
		m_CallBackWhenClosed = callBackWhenClosed;
		m_CurrentDeck = currentDeck;
		m_dummyUnitID = 0;
		m_dummyShipID = 0;
		m_NextScenID = NKM_SCEN_ID.NSI_INVALID;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (bClear)
		{
			UIOpened();
			NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_ROOT, bValue: false);
			if (m_coIntro != null)
			{
				StopCoroutine(m_coIntro);
			}
			m_coIntro = StartCoroutine(WarfareResultUIOpenProcess());
		}
		else
		{
			m_bFinishedIntroMovie = true;
			NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_ROOT, bValue: true);
			SetUI();
			UIOpened();
		}
	}

	private void SetAutoProcessWaitTime()
	{
		if (CheckSpecialEventExist())
		{
			m_fFinalAutoProcessWaitTime = 10f;
		}
		else
		{
			m_fFinalAutoProcessWaitTime = 5f;
		}
	}

	public void Open(NKMWarfareClearData _NKMWarfareClearData, NKMDeckIndex currentDeck, CallBackWhenClosed callBackWhenClosed, bool bNoClearBefore, bool bNoAllClearBefore, WarfareSupporterListData guestSptData = null)
	{
		m_bPause = false;
		m_eContents = USE_CONTENTS.WARFARE;
		SetAutoProcessWaitTime();
		m_bWaitingMovie = false;
		m_bDoneMakeRewardSlot = false;
		m_bReservedShowGetUnit = false;
		m_bFinishedIntroMovie = false;
		m_guestSptData = guestSptData;
		m_bWaitContentUnlockPopup = false;
		m_bTriggeredAutoOK = false;
		m_bfirstClear = bNoClearBefore && _NKMWarfareClearData != null;
		m_bFirstAllClear = bNoAllClearBefore;
		m_NKMWarfareClearData = _NKMWarfareClearData;
		if (m_NKMWarfareClearData != null)
		{
			int num = 0;
			if (m_NKMWarfareClearData.m_RewardDataList != null && m_NKMWarfareClearData.m_RewardDataList.GetUnitCount() > 0)
			{
				m_bReservedShowGetUnit = true;
				List<NKCUISlot.SlotData> list = NKCUISlot.MakeSlotDataListFromReward(m_NKMWarfareClearData.m_RewardDataList);
				num += list.Count;
			}
			if (m_NKMWarfareClearData.m_OnetimeRewards != null && m_NKMWarfareClearData.m_OnetimeRewards.GetUnitCount() > 0)
			{
				m_bReservedShowGetUnit = true;
				List<NKCUISlot.SlotData> list2 = NKCUISlot.MakeSlotDataListFromReward(m_NKMWarfareClearData.m_OnetimeRewards);
				num += list2.Count;
			}
			m_fTimeToShowGetUnit = (float)(num - 1) * (2f / 15f) + 2.1833334f + 5f / 6f;
		}
		for (int i = 0; i < m_lstNKCUIWRRewardSlot.Count; i++)
		{
			NKCUIWRRewardSlot nKCUIWRRewardSlot = m_lstNKCUIWRRewardSlot[i];
			nKCUIWRRewardSlot.InvalidAni();
			NKCUtil.SetGameobjectActive(nKCUIWRRewardSlot.gameObject, bValue: false);
		}
		SetUIRewardAlert(bSet: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_REWARD_BONUS_TYPE.gameObject, bValue: false);
		m_CallBackWhenClosed = callBackWhenClosed;
		m_CurrentDeck = currentDeck;
		m_NextScenID = NKM_SCEN_ID.NSI_INVALID;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_csbtnCloseToList, bValue: true);
		NKCUtil.SetGameobjectActive(m_csbtnRestart, bValue: true);
		NKCUtil.SetGameobjectActive(m_csbtnDeckEdit, bValue: false);
		if (_NKMWarfareClearData != null)
		{
			UIOpened();
			NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_ROOT, bValue: false);
			if (m_coIntro != null)
			{
				StopCoroutine(m_coIntro);
			}
			m_coIntro = StartCoroutine(WarfareResultUIOpenProcess());
		}
		else
		{
			m_bFinishedIntroMovie = true;
			NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_ROOT, bValue: true);
			SetUI();
			UIOpened();
		}
		if (m_bfirstClear && _NKMWarfareClearData != null)
		{
			NKCMMPManager.OnWarfareResult(_NKMWarfareClearData.m_WarfareID);
		}
		CheckTutorialRequired();
	}

	public void OpenForShadow(NKMShadowGameResult shadowResult, List<int> lstBestTime, int dummyUnitID, int dummyShipID, CallBackWhenClosed callBackWhenClosed)
	{
		if (shadowResult == null)
		{
			Debug.LogError("shadowResult == null");
			NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
			return;
		}
		m_bPause = false;
		m_eContents = USE_CONTENTS.SHADOW;
		SetAutoProcessWaitTime();
		m_ShadowResult = shadowResult;
		m_lstShadowBestTime = lstBestTime;
		m_bShadowRecordEvent = shadowResult.life > 0;
		m_CurrentDeck = default(NKMDeckIndex);
		m_dummyUnitID = dummyUnitID;
		m_dummyShipID = dummyShipID;
		m_bWaitingMovie = false;
		m_bDoneMakeRewardSlot = false;
		m_bReservedShowGetUnit = false;
		m_bFinishedIntroMovie = false;
		m_bWaitContentUnlockPopup = false;
		m_bTriggeredAutoOK = false;
		if (shadowResult.rewardData != null)
		{
			m_bReservedShowGetUnit = shadowResult.rewardData.GetUnitCount() > 0;
			m_fTimeToShowGetUnit = (float)(shadowResult.rewardData.GetUnitCount() - 1) * (2f / 15f) + 2.1833334f + 5f / 6f;
		}
		for (int i = 0; i < m_lstNKCUIWRRewardSlot.Count; i++)
		{
			NKCUIWRRewardSlot nKCUIWRRewardSlot = m_lstNKCUIWRRewardSlot[i];
			nKCUIWRRewardSlot.InvalidAni();
			NKCUtil.SetGameobjectActive(nKCUIWRRewardSlot.gameObject, bValue: false);
		}
		SetUIRewardAlert(bSet: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_REWARD_BONUS_TYPE.gameObject, bValue: false);
		m_CallBackWhenClosed = callBackWhenClosed;
		m_NextScenID = NKM_SCEN_ID.NSI_INVALID;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (shadowResult.life > 0)
		{
			UIOpened();
			NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_ROOT, bValue: false);
			if (m_coIntro != null)
			{
				StopCoroutine(m_coIntro);
			}
			m_coIntro = StartCoroutine(WarfareResultUIOpenProcess());
		}
		else
		{
			m_bFinishedIntroMovie = true;
			NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_ROOT, bValue: true);
			SetUI();
			UIOpened();
		}
	}

	public void OpenForTrim(NKMTrimClearData trimClearData, TrimModeState trimModeState, long unitUId, int dummyUnitId, bool firstClear, int bestScore, CallBackWhenClosed callBackWhenClosed)
	{
		if (trimClearData == null || trimModeState == null)
		{
			return;
		}
		m_bPause = false;
		m_eContents = USE_CONTENTS.TRIM;
		m_bTrimEvent = true;
		SetAutoProcessWaitTime();
		m_trimClearData = trimClearData;
		m_trimModeState = trimModeState;
		m_leaderUnitUID = unitUId;
		m_dummyUnitID = dummyUnitId;
		m_dummyShipID = 0;
		m_trimBestScore = bestScore;
		m_bTrimFirstClear = firstClear;
		m_bWaitingMovie = false;
		m_bDoneMakeRewardSlot = false;
		m_bReservedShowGetUnit = false;
		m_bFinishedIntroMovie = false;
		m_bWaitContentUnlockPopup = false;
		m_bTriggeredAutoOK = false;
		if (trimClearData.rewardData != null && trimClearData.rewardData.GetUnitCount() > 0)
		{
			int unitCount = trimClearData.rewardData.GetUnitCount();
			m_bReservedShowGetUnit = unitCount > 0;
			m_fTimeToShowGetUnit = (float)(unitCount - 1) * (2f / 15f) + 2.1833334f + 5f / 6f;
		}
		for (int i = 0; i < m_lstNKCUIWRRewardSlot.Count; i++)
		{
			NKCUIWRRewardSlot nKCUIWRRewardSlot = m_lstNKCUIWRRewardSlot[i];
			nKCUIWRRewardSlot.InvalidAni();
			NKCUtil.SetGameobjectActive(nKCUIWRRewardSlot.gameObject, bValue: false);
		}
		SetUIRewardAlert(bSet: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_REWARD_BONUS_TYPE.gameObject, bValue: false);
		m_CallBackWhenClosed = callBackWhenClosed;
		m_CurrentDeck = new NKMDeckIndex(NKM_DECK_TYPE.NDT_NORMAL, 0);
		m_NextScenID = NKM_SCEN_ID.NSI_INVALID;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		if (trimClearData.isWin)
		{
			UIOpened();
			NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_ROOT, bValue: false);
			if (m_coIntro != null)
			{
				StopCoroutine(m_coIntro);
			}
			m_coIntro = StartCoroutine(WarfareResultUIOpenProcess());
		}
		else
		{
			m_bFinishedIntroMovie = true;
			NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_ROOT, bValue: true);
			SetUI();
			UIOpened();
		}
	}

	public void OpenForDungeon(NKCUIResult.BattleResultData battleResultData, long leaderUnitUID, long leaderShipUID, int dummyLeaderUnitID, int dummyLeaderSkinID, CallBackWhenClosed callBackWhenClosed)
	{
		m_bPause = false;
		if (battleResultData == null)
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			return;
		}
		if (battleResultData.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_PVE_DEFENCE)
		{
			m_bDefenceEvent = true;
			m_eContents = USE_CONTENTS.DEFENCE;
		}
		else
		{
			m_bDefenceEvent = false;
			m_eContents = USE_CONTENTS.DUNGEON;
		}
		SetAutoProcessWaitTime();
		m_bWaitingMovie = false;
		m_bDoneMakeRewardSlot = false;
		m_bReservedShowGetUnit = false;
		m_bFinishedIntroMovie = false;
		m_bWaitContentUnlockPopup = false;
		m_bTriggeredAutoOK = false;
		m_bfirstClear = battleResultData.m_firstRewardData != null;
		m_bFirstAllClear = battleResultData.m_firstAllClearData != null;
		m_battleResultData = battleResultData;
		if (m_battleResultData != null)
		{
			int num = 0;
			List<NKCUISlot.SlotData> allListRewardSlotData = m_battleResultData.GetAllListRewardSlotData();
			if (allListRewardSlotData.Exists((NKCUISlot.SlotData x) => x.eType == NKCUISlot.eSlotMode.Unit || x.eType == NKCUISlot.eSlotMode.Skin))
			{
				m_bReservedShowGetUnit = true;
				num = allListRewardSlotData.Count;
			}
			m_fTimeToShowGetUnit = (float)(num - 1) * (2f / 15f) + 2.1833334f + 5f / 6f;
		}
		for (int num2 = 0; num2 < m_lstNKCUIWRRewardSlot.Count; num2++)
		{
			NKCUIWRRewardSlot nKCUIWRRewardSlot = m_lstNKCUIWRRewardSlot[num2];
			nKCUIWRRewardSlot.InvalidAni();
			NKCUtil.SetGameobjectActive(nKCUIWRRewardSlot.gameObject, bValue: false);
		}
		SetUIRewardAlert(bSet: false);
		NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_REWARD_BONUS_TYPE.gameObject, bValue: false);
		m_CallBackWhenClosed = callBackWhenClosed;
		m_leaderUnitUID = leaderUnitUID;
		m_leaderShipUID = leaderShipUID;
		m_dummyUnitID = dummyLeaderUnitID;
		m_dummyUnitSkinID = dummyLeaderSkinID;
		m_NextScenID = NKM_SCEN_ID.NSI_INVALID;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_csbtnCloseToList, m_eContents != USE_CONTENTS.DEFENCE);
		NKCUtil.SetGameobjectActive(m_csbtnRestart, m_eContents != USE_CONTENTS.DEFENCE);
		NKCUtil.SetGameobjectActive(m_csbtnDeckEdit, m_eContents == USE_CONTENTS.DEFENCE);
		if (battleResultData.m_BATTLE_RESULT_TYPE == BATTLE_RESULT_TYPE.BRT_WIN)
		{
			UIOpened();
			NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_ROOT, bValue: false);
			if (m_coIntro != null)
			{
				StopCoroutine(m_coIntro);
			}
			m_coIntro = StartCoroutine(WarfareResultUIOpenProcess());
		}
		else
		{
			m_bFinishedIntroMovie = true;
			NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_ROOT, bValue: true);
			SetUI();
			UIOpened();
		}
		if (m_bfirstClear && battleResultData.m_BATTLE_RESULT_TYPE == BATTLE_RESULT_TYPE.BRT_WIN)
		{
			NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(battleResultData.m_stageID);
			if (nKMStageTempletV != null && nKMStageTempletV.DungeonTempletBase != null)
			{
				NKCMMPManager.OnClearDungeon(nKMStageTempletV.DungeonTempletBase.m_DungeonID);
			}
		}
		CheckTutorialRequired();
	}

	private IEnumerator WarfareResultUIOpenProcess()
	{
		NKCUIComVideoCamera videoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (videoPlayer != null)
		{
			videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
			videoPlayer.m_fMoviePlaySpeed = MoviePlaySpeed;
			m_bWaitingMovie = true;
			videoPlayer.Play("WarfareResultIntro.mp4", bLoop: false, bPlaySound: false, VideoPlayMessageCallback);
			yield return null;
			NKCSoundManager.PlaySound("FX_UI_WARFARE_RESULT_START", 1f, 0f, 0f);
			while (m_bWaitingMovie)
			{
				yield return null;
				if (Input.anyKeyDown && PlayerPrefs.GetInt("WARFARE_RESULT_INTRO_SKIP", 0) == 1)
				{
					break;
				}
			}
			videoPlayer.Stop();
			if (PlayerPrefs.GetInt("WARFARE_RESULT_INTRO_SKIP", 0) == 0)
			{
				PlayerPrefs.SetInt("WARFARE_RESULT_INTRO_SKIP", 1);
			}
		}
		m_bWaitingMovie = false;
		SetUI();
		m_bFinishedIntroMovie = true;
		m_coIntro = null;
	}

	private void VideoPlayMessageCallback(NKCUIComVideoPlayer.eVideoMessage message)
	{
		switch (message)
		{
		case NKCUIComVideoPlayer.eVideoMessage.PlayFailed:
		case NKCUIComVideoPlayer.eVideoMessage.PlayComplete:
			m_bWaitingMovie = false;
			break;
		case NKCUIComVideoPlayer.eVideoMessage.PlayBegin:
			break;
		}
	}

	private void SetUI()
	{
		NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_ROOT, bValue: true);
		NKCUtil.SetGameobjectActive(m_objEntryFeeReturn, bValue: false);
		if (GetIsWinGame())
		{
			USE_CONTENTS eContents = m_eContents;
			string stateName = ((eContents != USE_CONTENTS.WARFARE && (uint)(eContents - 4) > 1u) ? "NKM_UI_DIVE_RESULT_INTRO" : "NKM_UI_WARFARE_RESULT_INTRO");
			m_Animtor.Play(stateName);
			NKCSoundManager.PlayMusic("UI_WARFARE_RESULT_WIN", bLoop: true);
			NKCUtil.SetGameobjectActive(m_PARTICLEOBJECT, bValue: true);
		}
		else
		{
			USE_CONTENTS eContents = m_eContents;
			string stateName2 = ((eContents != USE_CONTENTS.WARFARE && (uint)(eContents - 4) > 1u) ? "NKM_UI_DIVE_RESULT_INTRO_FAILED" : "NKM_UI_WARFARE_RESULT_INTRO_DEFEAT");
			m_Animtor.Play(stateName2);
			NKCSoundManager.PlayMusic("UI_WARFARE_RESULT_LOSE", bLoop: true);
		}
		m_fElapsedTime = 0f;
		switch (m_eContents)
		{
		case USE_CONTENTS.WARFARE:
		case USE_CONTENTS.DUNGEON:
		case USE_CONTENTS.DEFENCE:
			SetUIByGrade(GetGrade());
			SetUIByLeader(m_CurrentDeck, m_dummyUnitID, m_dummyShipID);
			SetUIByMission();
			SetBtnUI();
			SetBG();
			SetUIGameTip();
			break;
		default:
			SetUIByGrade(GetGrade());
			SetUIByLeader(m_CurrentDeck, m_dummyUnitID, m_dummyShipID);
			NKCUtil.SetGameobjectActive(m_csbtnCloseToHome, bValue: true);
			NKCUtil.SetGameobjectActive(m_csbtnCloseToList, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnRestart, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnConfirm, bValue: true);
			NKCUtil.SetGameobjectActive(m_csbtnNextMission, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_REPEAT, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnBattleStat, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnDeckEdit, bValue: false);
			SetBG();
			SetUIGameTip();
			break;
		}
	}

	private void SetBG()
	{
		bool flag = NKCScenManager.GetScenManager().GetGameOptionData()?.UseVideoTexture ?? false;
		NKCUtil.SetGameobjectActive(m_objBGFallBack, !flag);
		if (flag)
		{
			NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
			if (subUICameraVideoPlayer != null)
			{
				subUICameraVideoPlayer.renderMode = VideoRenderMode.CameraFarPlane;
				subUICameraVideoPlayer.m_fMoviePlaySpeed = MoviePlaySpeed;
				subUICameraVideoPlayer.Play("WarfareResultBG.mp4", bLoop: true);
			}
		}
	}

	public void SetBtnUI()
	{
		NKCRepeatOperaion nKCRepeatOperaion = NKCScenManager.GetScenManager().GetNKCRepeatOperaion();
		if (nKCRepeatOperaion.GetIsOnGoing())
		{
			NKCUtil.SetGameobjectActive(m_csbtnCloseToHome, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnCloseToList, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnConfirm, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnNextMission, bValue: false);
			NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_REPEAT, bValue: true);
			NKCUtil.SetGameobjectActive(m_csbtnRestart, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnDeckEdit, bValue: false);
			NKCUtil.SetLabelText(m_NKM_UI_WARFARE_RESULT_REPEAT_COUNT, $"({nKCRepeatOperaion.GetCurrRepeatCount()}/{nKCRepeatOperaion.GetMaxRepeatCount()})");
			NKCUtil.SetLabelText(m_NKM_UI_WARFARE_RESULT_BTN_REPEAT_COUNT_DOWN, ((int)m_fFinalAutoProcessWaitTime).ToString());
			m_NKM_UI_WARFARE_RESULT_BTN_REPEAT_COUNT_DOWN_Gauge.fillAmount = 0f;
			return;
		}
		NKCUtil.SetGameobjectActive(m_csbtnCloseToHome, bValue: true);
		NKCUtil.SetGameobjectActive(m_csbtnCloseToList, m_eContents != USE_CONTENTS.DEFENCE);
		NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_REPEAT, bValue: false);
		NKCUtil.SetGameobjectActive(m_csbtnRestart, m_eContents != USE_CONTENTS.DEFENCE);
		NKCUtil.SetGameobjectActive(m_csbtnDeckEdit, m_eContents == USE_CONTENTS.DEFENCE);
		if (GetPossibleNextOperation() != null)
		{
			NKCUtil.SetGameobjectActive(m_csbtnConfirm, bValue: false);
			NKCUtil.SetGameobjectActive(m_csbtnNextMission, bValue: true);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_csbtnConfirm, bValue: true);
			NKCUtil.SetGameobjectActive(m_csbtnNextMission, bValue: false);
		}
		NKMStageTempletV2 currentStageTemplet = GetCurrentStageTemplet();
		if (currentStageTemplet != null)
		{
			m_csbtnRestart.PointerClick.RemoveAllListeners();
			NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
			if (currentStageTemplet.EnterLimit > 0 && nKMUserData.GetStatePlayCnt(currentStageTemplet.Key) >= currentStageTemplet.EnterLimit && nKMUserData.GetStageRestoreCnt(currentStageTemplet.Key) >= currentStageTemplet.RestoreLimit)
			{
				m_csbtnRestart.PointerClick.AddListener(delegate
				{
					NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCUtilString.GET_STRING_ENTER_LIMIT_OVER);
				});
			}
			else
			{
				m_csbtnRestart.PointerClick.AddListener(OnClickRetry);
			}
		}
		NKCUtil.SetGameobjectActive(m_csbtnBattleStat, m_battleResultData != null && m_battleResultData.m_battleData != null);
	}

	private bool IsAllStarClear()
	{
		NKMStageTempletV2 currentStageTemplet = GetCurrentStageTemplet();
		if (currentStageTemplet != null)
		{
			if (currentStageTemplet.MissionReward.ID != 0)
			{
				return currentStageTemplet.MissionReward.Count > 0;
			}
			return false;
		}
		return false;
	}

	private NKMStageTempletV2 GetCurrentStageTemplet()
	{
		switch (m_eContents)
		{
		case USE_CONTENTS.DUNGEON:
			if (m_battleResultData == null)
			{
				return null;
			}
			return NKMStageTempletV2.Find(m_battleResultData.m_stageID);
		case USE_CONTENTS.WARFARE:
			if (m_NKMWarfareClearData == null)
			{
				return null;
			}
			return NKMEpisodeMgr.FindStageTempletByBattleStrID(NKMWarfareTemplet.Find(m_NKMWarfareClearData.m_WarfareID)?.m_WarfareStrID ?? string.Empty);
		default:
			return null;
		}
	}

	private NKMStageTempletV2 GetPossibleNextOperation()
	{
		NKMStageTempletV2 currentStageTemplet = GetCurrentStageTemplet();
		if (currentStageTemplet != null)
		{
			NKMEpisodeTempletV2 episodeTemplet = currentStageTemplet.EpisodeTemplet;
			if (episodeTemplet != null && episodeTemplet.m_DicStage.ContainsKey(currentStageTemplet.ActId))
			{
				bool flag = false;
				for (int i = 0; i < episodeTemplet.m_DicStage[currentStageTemplet.ActId].Count; i++)
				{
					if (flag)
					{
						NKMStageTempletV2 nKMStageTempletV = episodeTemplet.m_DicStage[currentStageTemplet.ActId][i];
						if (NKMEpisodeMgr.CheckEpisodeMission(NKCScenManager.CurrentUserData(), nKMStageTempletV))
						{
							return nKMStageTempletV;
						}
						return null;
					}
					if (episodeTemplet.m_DicStage[currentStageTemplet.ActId][i].Key == currentStageTemplet.Key)
					{
						flag = true;
					}
				}
			}
		}
		return null;
	}

	public void PlayNextOperation()
	{
		NKMStageTempletV2 possibleNextOperation = GetPossibleNextOperation();
		if (possibleNextOperation != null)
		{
			NKC_SCEN_OPERATION_V2 sCEN_OPERATION = NKCScenManager.GetScenManager().Get_SCEN_OPERATION();
			if (sCEN_OPERATION != null)
			{
				NKCScenManager.GetScenManager().GetNKCRepeatOperaion().Init();
				sCEN_OPERATION.SetReservedStage(possibleNextOperation);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OPERATION);
			}
		}
	}

	private bool CheckSpecialEventExist()
	{
		if (NKCContentManager.CheckLevelChanged())
		{
			return true;
		}
		if (NKCContentManager.HasUnlockedContent(GetReqType()))
		{
			return true;
		}
		if (NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetAlarmRepeatOperationQuitByDefeat())
		{
			return true;
		}
		if (NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetAlarmRepeatOperationSuccess())
		{
			return true;
		}
		return false;
	}

	private STAGE_UNLOCK_REQ_TYPE GetReqType()
	{
		return m_eContents switch
		{
			USE_CONTENTS.DIVE => STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DIVE, 
			USE_CONTENTS.SHADOW => STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_PALACE, 
			USE_CONTENTS.DUNGEON => STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DUNGEON, 
			_ => STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_WARFARE, 
		};
	}

	private void Update()
	{
		if (!m_bFinishedIntroMovie)
		{
			return;
		}
		if (!m_bPause)
		{
			m_fElapsedTime += Time.deltaTime;
		}
		if (m_eContents == USE_CONTENTS.DIVE)
		{
			if (m_fElapsedTime > 2.1833334f && !m_bDoneMakeRewardSlot)
			{
				m_bDoneMakeRewardSlot = true;
				SetUIByReward();
				if (m_bReservedShowGetUnit)
				{
					m_Animtor.speed = 0f;
				}
			}
			if (m_bReservedShowGetUnit && m_fElapsedTime > m_fTimeToShowGetUnit)
			{
				m_bReservedShowGetUnit = false;
				NKCUIGameResultGetUnit.ShowNewUnitGetUI(m_DiveRewardData, GetUnitCallback, NKCScenManager.GetScenManager().GetMyUserData().m_UserOption.m_bAutoDive, bUseDefaultSort: true, bSkipDuplicateNormalUnit: true);
				NKCUtil.SetGameobjectActive(m_PARTICLEOBJECT, bValue: false);
			}
			if (m_bArtifactReturnEvent)
			{
				if (!NKCPopupArtifactExchange.IsOpen && m_Animtor.speed == 1f && m_fElapsedTime > 2.466667f)
				{
					m_bPause = true;
					NKCPopupArtifactExchange.Open(m_lstArtifact, NKMCommonConst.DiveArtifactReturnItemId, delegate
					{
						m_bArtifactReturnEvent = false;
						m_bPause = false;
					});
				}
			}
			else if (NKCContentManager.CheckLevelChanged())
			{
				if (!m_bUserLevelUpPopupOpened && m_Animtor.speed == 1f && m_fElapsedTime > 2.466667f)
				{
					m_bUserLevelUpPopupOpened = true;
					NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
					if (nKMUserData != null)
					{
						NKCPopupUserLevelUp.instance.Open(nKMUserData, OnCloseUserLevelUpPopup);
					}
				}
			}
			else if (NKCContentManager.HasUnlockedContent(STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DIVE) || m_bWaitContentUnlockPopup)
			{
				if (!m_bWaitContentUnlockPopup && m_Animtor.speed == 1f && m_fElapsedTime > 2.466667f)
				{
					m_bWaitContentUnlockPopup = true;
					NKCContentManager.ShowContentUnlockPopup(OnCloseContentUnlockPopup, STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_DIVE);
				}
			}
			else if (NKCScenManager.GetScenManager().GetMyUserData().m_UserOption.m_bAutoDive && !m_bTriggeredAutoOK && m_fElapsedTime > 3.466667f + m_fFinalAutoProcessWaitTime)
			{
				m_bTriggeredAutoOK = true;
				CloseToOK();
			}
		}
		else if (m_eContents == USE_CONTENTS.SHADOW)
		{
			if (m_fElapsedTime > 2.1833334f && !m_bDoneMakeRewardSlot)
			{
				m_bDoneMakeRewardSlot = true;
				SetUIByReward();
				if (m_bReservedShowGetUnit)
				{
					m_Animtor.speed = 0f;
				}
			}
			if (m_bReservedShowGetUnit && m_fElapsedTime > m_fTimeToShowGetUnit)
			{
				m_bReservedShowGetUnit = false;
				NKCUIGameResultGetUnit.ShowNewUnitGetUI(m_ShadowResult.rewardData, GetUnitCallback, bEnableAutoSkip: false, bUseDefaultSort: true, bSkipDuplicateNormalUnit: true);
				NKCUtil.SetGameobjectActive(m_PARTICLEOBJECT, bValue: false);
			}
			if (m_bShadowRecordEvent)
			{
				if (!NKCPopupShadowRecord.IsInstanceOpen && m_Animtor.speed == 1f && m_fElapsedTime > 2.466667f)
				{
					m_bPause = true;
					NKCPopupShadowRecord.Instance.Open(m_ShadowResult, m_lstShadowBestTime, delegate
					{
						m_bShadowRecordEvent = false;
						m_bPause = false;
					});
				}
			}
			else if (NKCContentManager.CheckLevelChanged())
			{
				if (!m_bUserLevelUpPopupOpened && m_Animtor.speed == 1f && m_fElapsedTime > 2.466667f)
				{
					m_bUserLevelUpPopupOpened = true;
					NKMUserData nKMUserData2 = NKCScenManager.CurrentUserData();
					if (nKMUserData2 != null)
					{
						NKCPopupUserLevelUp.instance.Open(nKMUserData2, OnCloseUserLevelUpPopup);
					}
				}
			}
			else if ((NKCContentManager.HasUnlockedContent(STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_PALACE) || m_bWaitContentUnlockPopup) && !m_bWaitContentUnlockPopup && m_Animtor.speed == 1f && m_fElapsedTime > 2.466667f)
			{
				m_bWaitContentUnlockPopup = true;
				NKCContentManager.ShowContentUnlockPopup(OnCloseContentUnlockPopup, STAGE_UNLOCK_REQ_TYPE.SURT_CLEAR_PALACE);
			}
		}
		else if (m_eContents == USE_CONTENTS.TRIM)
		{
			if (m_fElapsedTime > 2.1833334f && !m_bDoneMakeRewardSlot)
			{
				m_bDoneMakeRewardSlot = true;
				SetUIByReward();
				if (m_bReservedShowGetUnit)
				{
					m_Animtor.speed = 0f;
				}
			}
			if (m_bReservedShowGetUnit && m_fElapsedTime > m_fTimeToShowGetUnit)
			{
				m_bReservedShowGetUnit = false;
				NKCUIGameResultGetUnit.ShowNewUnitGetUI(m_ShadowResult.rewardData, GetUnitCallback, bEnableAutoSkip: false, bUseDefaultSort: true, bSkipDuplicateNormalUnit: true);
				NKCUtil.SetGameobjectActive(m_PARTICLEOBJECT, bValue: false);
			}
			if (m_bTrimEvent)
			{
				m_bTrimEvent = false;
				m_bPause = false;
			}
			else if (NKCContentManager.CheckLevelChanged() && !m_bUserLevelUpPopupOpened && m_Animtor.speed == 1f && m_fElapsedTime > 2.466667f)
			{
				m_bUserLevelUpPopupOpened = true;
				NKMUserData nKMUserData3 = NKCScenManager.CurrentUserData();
				if (nKMUserData3 != null)
				{
					NKCPopupUserLevelUp.instance.Open(nKMUserData3, OnCloseUserLevelUpPopup);
				}
			}
		}
		else if (m_eContents == USE_CONTENTS.DEFENCE)
		{
			if (m_fElapsedTime > 2.1833334f && !m_bDoneMakeRewardSlot)
			{
				m_bDoneMakeRewardSlot = true;
				SetUIByReward();
				if (m_bReservedShowGetUnit)
				{
					m_Animtor.speed = 0f;
				}
			}
			if (m_bReservedShowGetUnit && m_fElapsedTime > m_fTimeToShowGetUnit)
			{
				m_bReservedShowGetUnit = false;
				NKCUIGameResultGetUnit.ShowNewUnitGetUI(m_battleResultData.m_firstAllClearData, GetUnitCallback, bEnableAutoSkip: false, bUseDefaultSort: true, bSkipDuplicateNormalUnit: true);
				NKCUtil.SetGameobjectActive(m_PARTICLEOBJECT, bValue: false);
			}
			if (m_bDefenceEvent)
			{
				if (!NKCPopupDefenceRecord.IsInstanceOpen && m_Animtor.speed == 1f && m_fElapsedTime > 2.466667f)
				{
					m_bPause = true;
					NKCPopupDefenceRecord.Instance.Open(m_battleResultData.m_KillCountGain, m_battleResultData.m_KillCountStageRecord, delegate
					{
						m_bDefenceEvent = false;
						m_bPause = false;
					});
				}
			}
			else if (NKCContentManager.CheckLevelChanged() && !m_bUserLevelUpPopupOpened && m_Animtor.speed == 1f && m_fElapsedTime > 2.466667f)
			{
				m_bUserLevelUpPopupOpened = true;
				NKMUserData nKMUserData4 = NKCScenManager.CurrentUserData();
				if (nKMUserData4 != null)
				{
					NKCPopupUserLevelUp.instance.Open(nKMUserData4, OnCloseUserLevelUpPopup);
				}
			}
		}
		else
		{
			if (m_fElapsedTime > 2.1833334f && !m_bDoneMakeRewardSlot)
			{
				m_bDoneMakeRewardSlot = true;
				SetUIByReward();
				if (m_bReservedShowGetUnit)
				{
					m_Animtor.speed = 0f;
				}
			}
			if (m_bReservedShowGetUnit && m_fElapsedTime > m_fTimeToShowGetUnit)
			{
				List<NKMRewardData> list = new List<NKMRewardData>();
				switch (m_eContents)
				{
				case USE_CONTENTS.WARFARE:
					if (m_NKMWarfareClearData.m_RewardDataList != null)
					{
						list.Add(m_NKMWarfareClearData.m_RewardDataList);
					}
					if (m_NKMWarfareClearData.m_OnetimeRewards != null)
					{
						list.Add(m_NKMWarfareClearData.m_OnetimeRewards);
					}
					break;
				case USE_CONTENTS.DUNGEON:
					if (m_battleResultData != null)
					{
						if (m_battleResultData.m_firstRewardData != null)
						{
							list.Add(m_battleResultData.m_firstRewardData);
						}
						if (m_battleResultData.m_RewardData != null)
						{
							list.Add(m_battleResultData.m_RewardData);
						}
						if (m_battleResultData.m_OnetimeRewardData != null)
						{
							list.Add(m_battleResultData.m_OnetimeRewardData);
						}
						if (m_battleResultData.m_firstAllClearData != null)
						{
							list.Add(m_battleResultData.m_firstAllClearData);
						}
					}
					break;
				}
				m_bReservedShowGetUnit = false;
				NKCUIGameResultGetUnit.ShowNewUnitGetUI(list, GetUnitCallback, NKCScenManager.GetScenManager().GetMyUserData().m_UserOption.m_bAutoWarfare, bUseDefaultSort: true, bSkipDuplicateNormalUnit: true);
				NKCUtil.SetGameobjectActive(m_PARTICLEOBJECT, bValue: false);
			}
			if (NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing())
			{
				float num = m_fFinalAutoProcessWaitTime - (m_fElapsedTime - 3.466667f);
				if (num < 0f)
				{
					num = 0f;
				}
				int num2 = Mathf.CeilToInt(num);
				NKCUtil.SetLabelText(m_NKM_UI_WARFARE_RESULT_BTN_REPEAT_COUNT_DOWN, num2.ToString());
				m_NKM_UI_WARFARE_RESULT_BTN_REPEAT_COUNT_DOWN_Gauge.fillAmount = (float)num2 - num;
			}
			if (NKCContentManager.CheckLevelChanged())
			{
				if (!m_bUserLevelUpPopupOpened && IsPopupTiming())
				{
					m_bUserLevelUpPopupOpened = true;
					NKMUserData nKMUserData5 = NKCScenManager.CurrentUserData();
					if (nKMUserData5 != null)
					{
						NKCPopupUserLevelUp.instance.Open(nKMUserData5, OnCloseUserLevelUpPopup);
					}
				}
			}
			else if (NKCContentManager.HasUnlockedContent(default(STAGE_UNLOCK_REQ_TYPE)) || m_bWaitContentUnlockPopup)
			{
				if (!m_bWaitContentUnlockPopup && IsPopupTiming())
				{
					m_bWaitContentUnlockPopup = true;
					NKCContentManager.ShowContentUnlockPopup(OnCloseContentUnlockPopup, default(STAGE_UNLOCK_REQ_TYPE));
				}
			}
			else if (m_guestSptData != null && !NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing())
			{
				if (!m_bWaitFriendRequestPopup && IsPopupTiming())
				{
					m_bWaitFriendRequestPopup = true;
					NKCPopupFriendRequest.Instance.Open(m_guestSptData, OnCloseFriendRequestPopup);
				}
			}
			else if (NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetAlarmRepeatOperationQuitByDefeat() && IsPopupTiming())
			{
				if (!NKCPopupRepeatOperation.IsInstanceOpen)
				{
					m_bPause = true;
					NKCScenManager.GetScenManager().GetNKCRepeatOperaion().Init();
					NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetStopReason(NKCStringTable.GetString("SI_POPUP_REPEAT_FAIL_DEFEAT"));
					NKCPopupRepeatOperation.Instance.OpenForResult(delegate
					{
						m_bPause = false;
						NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetAlarmRepeatOperationQuitByDefeat(bSet: false);
					});
				}
			}
			else if (NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetAlarmRepeatOperationSuccess() && IsPopupTiming())
			{
				if (!NKCPopupRepeatOperation.IsInstanceOpen)
				{
					m_bPause = true;
					NKCScenManager.GetScenManager().GetNKCRepeatOperaion().Init();
					NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetStopReason(NKCUtilString.GET_STRING_REPEAT_OPERATION_IS_TERMINATED);
					NKCPopupRepeatOperation.Instance.OpenForResult(delegate
					{
						m_bPause = false;
						NKCScenManager.GetScenManager().GetNKCRepeatOperaion().SetAlarmRepeatOperationSuccess(bSet: false);
					});
				}
			}
			else if (NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing())
			{
				if (!m_bTriggeredAutoOK && m_fElapsedTime > 3.466667f + m_fFinalAutoProcessWaitTime)
				{
					m_bTriggeredAutoOK = true;
					Retry();
				}
			}
			else
			{
				if (!m_bRequiredTutorial && NKCScenManager.GetScenManager().GetMyUserData().m_UserOption.m_bAutoWarfare && !m_bTriggeredAutoOK && m_fElapsedTime > 3.466667f + m_fFinalAutoProcessWaitTime)
				{
					m_bTriggeredAutoOK = true;
					CloseToOK();
				}
				if (m_bRequiredTutorial && m_Animtor.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.8701923f)
				{
					NKCUtil.SetGameobjectActive(m_csbtnCloseToHome, bValue: true);
					NKCTutorialManager.TutorialRequired(TutorialPoint.WarfareResult);
					m_bRequiredTutorial = false;
					m_bTriggeredAutoOK = true;
				}
			}
		}
		int num3 = 0;
		for (num3 = 0; num3 < m_lstNKCUIWRRewardSlot.Count; num3++)
		{
			if (m_lstNKCUIWRRewardSlot[num3] != null)
			{
				m_lstNKCUIWRRewardSlot[num3].ManualUpdate();
			}
		}
	}

	private void GetUnitCallback()
	{
		m_Animtor.speed = 1f;
	}

	private void OnCloseUserLevelUpPopup()
	{
		m_bUserLevelUpPopupOpened = false;
		NKCContentManager.SetLevelChanged(bValue: false);
	}

	private void OnCloseContentUnlockPopup()
	{
		m_bWaitContentUnlockPopup = false;
	}

	private void OnCloseFriendRequestPopup()
	{
		m_guestSptData = null;
		m_bWaitFriendRequestPopup = false;
	}

	public override void UnHide()
	{
		base.UnHide();
		m_Animtor.Play("NKM_UI_WARFARE_RESULT_INTRO", -1, 85f / 104f);
	}

	private void UpdateRewardList(ref List<NKCUISlot.SlotData> lstReward, ref int globalIndex, eRewardType type)
	{
		List<NKCUISlot.SlotData> list = new List<NKCUISlot.SlotData>();
		if ((type == eRewardType.FirstClear && !m_bfirstClear) || (type == eRewardType.FirstAllClear && !m_bFirstAllClear))
		{
			return;
		}
		if (m_NKMWarfareClearData != null)
		{
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_NKMWarfareClearData.m_WarfareID);
			if (nKMWarfareTemplet != null)
			{
				NKCUISlot.SlotData slotData = null;
				int num = 0;
				switch (type)
				{
				case eRewardType.FirstClear:
				{
					FirstRewardData firstRewardData = nKMWarfareTemplet.GetFirstRewardData();
					if (firstRewardData.Type != NKM_REWARD_TYPE.RT_NONE)
					{
						slotData = NKCUISlot.SlotData.MakeRewardTypeData(firstRewardData.Type, firstRewardData.RewardId, firstRewardData.RewardQuantity);
						num = firstRewardData.RewardQuantity;
					}
					break;
				}
				case eRewardType.FirstAllClear:
				{
					NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(nKMWarfareTemplet.m_WarfareStrID);
					if (nKMStageTempletV == null || nKMStageTempletV.MissionReward == null || nKMStageTempletV.MissionReward.rewardType == NKM_REWARD_TYPE.RT_NONE || !NKMRewardTemplet.IsOpenedReward(nKMStageTempletV.MissionReward.rewardType, nKMStageTempletV.MissionReward.ID, useRandomContract: false))
					{
						return;
					}
					slotData = NKCUISlot.SlotData.MakeRewardTypeData(nKMStageTempletV.MissionReward.rewardType, nKMStageTempletV.MissionReward.ID, nKMStageTempletV.MissionReward.Count);
					num = nKMStageTempletV.MissionReward.Count;
					break;
				}
				}
				if (slotData != null)
				{
					List<NKCUISlot.SlotData> list2 = new List<NKCUISlot.SlotData>();
					for (int i = 0; i < lstReward.Count; i++)
					{
						NKCUISlot.SlotData slotData2 = lstReward[i];
						if (slotData2.eType == slotData.eType && slotData2.ID == slotData.ID)
						{
							list2.Add(slotData2);
						}
						else if (slotData2.eType == NKCUISlot.eSlotMode.Unit && slotData.eType == NKCUISlot.eSlotMode.UnitCount && slotData2.ID == slotData.ID)
						{
							list2.Add(slotData2);
						}
					}
					for (int j = 0; j < list2.Count; j++)
					{
						NKCUISlot.SlotData slotData3 = list2[j];
						if (slotData3.eType != NKCUISlot.eSlotMode.Unit && slotData3.eType != NKCUISlot.eSlotMode.UnitCount)
						{
							slotData3.Count -= num;
							num = 0;
							if (slotData3.Count <= 0)
							{
								lstReward.Remove(slotData3);
							}
							list.Add(slotData);
						}
						else
						{
							lstReward.Remove(slotData3);
							list.Add(slotData3);
							num--;
						}
						if (num == 0)
						{
							break;
						}
					}
				}
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		int count = m_lstNKCUIWRRewardSlot.Count;
		if (count + list.Count > m_lstNKCUIWRRewardSlot.Count)
		{
			int newCount = count + list.Count - m_lstNKCUIWRRewardSlot.Count;
			AddExtraRewardSlot(newCount);
		}
		for (int k = 0; k < list.Count; k++)
		{
			NKCUISlot.SlotData slotData4 = list[k];
			m_lstNKCUIWRRewardSlot[globalIndex].SetUI(slotData4, globalIndex);
			m_lstNKCUIWRRewardSlot[globalIndex].SetMultiplyMark(bSet: false);
			switch (type)
			{
			case eRewardType.FirstClear:
				m_lstNKCUIWRRewardSlot[globalIndex].SetFirstMark(bFirst: true);
				break;
			case eRewardType.FirstAllClear:
				m_lstNKCUIWRRewardSlot[globalIndex].SetFirstAllClearMark(bClear: true);
				break;
			case eRewardType.AllStarClear:
				m_lstNKCUIWRRewardSlot[globalIndex].SetStarAllClearMark(bAllStarClear: true);
				break;
			}
			globalIndex++;
		}
	}

	private void ApplyRewardData(List<NKCUISlot.SlotData> lstSlotData, ref int globalIndex, bool bMultiply, eRewardType extraType)
	{
		int count = m_lstNKCUIWRRewardSlot.Count;
		int num = globalIndex + lstSlotData.Count;
		if (num > count)
		{
			AddExtraRewardSlot(num - count);
		}
		foreach (NKCUISlot.SlotData lstSlotDatum in lstSlotData)
		{
			m_lstNKCUIWRRewardSlot[globalIndex].SetUI(lstSlotDatum, globalIndex);
			m_lstNKCUIWRRewardSlot[globalIndex].SetMultiplyMark(bMultiply && extraType == eRewardType.None);
			switch (extraType)
			{
			case eRewardType.FirstClear:
				m_lstNKCUIWRRewardSlot[globalIndex].SetFirstMark(bFirst: true);
				break;
			case eRewardType.FirstAllClear:
				m_lstNKCUIWRRewardSlot[globalIndex].SetFirstAllClearMark(bClear: true);
				break;
			case eRewardType.AllStarClear:
				m_lstNKCUIWRRewardSlot[globalIndex].SetStarAllClearMark(bAllStarClear: true);
				break;
			}
			globalIndex++;
		}
	}

	private void ApplyRewardData(NKMRewardData rewardData, ref int globalIndex, bool bMultiply, eRewardType extraType)
	{
		List<NKCUISlot.SlotData> lstSlotData = NKCUISlot.MakeSlotDataListFromReward(rewardData);
		ApplyRewardData(lstSlotData, ref globalIndex, bMultiply, extraType);
	}

	private void ApplyRewardData(NKMAdditionalReward rewardData, ref int globalIndex, bool bMultiply, eRewardType extraType)
	{
		List<NKCUISlot.SlotData> lstSlotData = NKCUISlot.MakeSlotDataListFromReward(rewardData);
		ApplyRewardData(lstSlotData, ref globalIndex, bMultiply, extraType);
	}

	private void SetUIByReward()
	{
		if (m_eContents == USE_CONTENTS.DIVE)
		{
			int i = 0;
			int num = 0;
			int num2 = 0;
			if (m_DiveRewardDataArtifact != null)
			{
				List<NKCUISlot.SlotData> list = NKCUISlot.MakeSlotDataListFromReward(m_DiveRewardDataArtifact);
				num = list.Count;
				if (list.Count > m_lstNKCUIWRRewardSlot.Count)
				{
					int num3 = list.Count - m_lstNKCUIWRRewardSlot.Count;
					for (int j = 0; j < num3; j++)
					{
						NKCUIWRRewardSlot newInstance = NKCUIWRRewardSlot.GetNewInstance(m_NKM_UI_WARFARE_RESULT_REWARDS_Content);
						m_lstNKCUIWRRewardSlot.Add(newInstance);
					}
				}
				for (i = 0; i < list.Count; i++)
				{
					NKCUISlot.SlotData slotData = list[i];
					m_lstNKCUIWRRewardSlot[i].SetUI(slotData, i);
					m_lstNKCUIWRRewardSlot[i].SetArtifactMark(bSet: true);
					m_lstNKCUIWRRewardSlot[i].SetDiveStormMark(bSet: false);
				}
			}
			if (m_DiveRewardData != null)
			{
				List<NKCUISlot.SlotData> list2 = NKCUISlot.MakeSlotDataListFromReward(m_DiveRewardData, bIncludeContractList: false, bStackEnchantType: true);
				num2 = list2.Count;
				if (list2.Count > m_lstNKCUIWRRewardSlot.Count - num)
				{
					int num4 = list2.Count - (m_lstNKCUIWRRewardSlot.Count - num);
					for (int k = 0; k < num4; k++)
					{
						NKCUIWRRewardSlot newInstance2 = NKCUIWRRewardSlot.GetNewInstance(m_NKM_UI_WARFARE_RESULT_REWARDS_Content);
						m_lstNKCUIWRRewardSlot.Add(newInstance2);
					}
				}
				for (int l = 0; l < list2.Count; l++)
				{
					NKCUISlot.SlotData slotData2 = list2[l];
					m_lstNKCUIWRRewardSlot[i].SetUI(slotData2, i);
					m_lstNKCUIWRRewardSlot[i].SetArtifactMark(bSet: false);
					m_lstNKCUIWRRewardSlot[i].SetDiveStormMark(bSet: false);
					i++;
				}
			}
			if (m_DiveRewardDataStorm == null)
			{
				return;
			}
			List<NKCUISlot.SlotData> list3 = NKCUISlot.MakeSlotDataListFromReward(m_DiveRewardDataStorm);
			if (list3.Count > m_lstNKCUIWRRewardSlot.Count - num - num2)
			{
				int num5 = list3.Count - (m_lstNKCUIWRRewardSlot.Count - num - num2);
				for (int m = 0; m < num5; m++)
				{
					NKCUIWRRewardSlot newInstance3 = NKCUIWRRewardSlot.GetNewInstance(m_NKM_UI_WARFARE_RESULT_REWARDS_Content);
					m_lstNKCUIWRRewardSlot.Add(newInstance3);
				}
			}
			for (int n = 0; n < list3.Count; n++)
			{
				NKCUISlot.SlotData slotData3 = list3[n];
				m_lstNKCUIWRRewardSlot[i].SetUI(slotData3, i);
				m_lstNKCUIWRRewardSlot[i].SetArtifactMark(bSet: false);
				m_lstNKCUIWRRewardSlot[i].SetDiveStormMark(bSet: true);
				i++;
			}
		}
		else if (m_eContents == USE_CONTENTS.SHADOW)
		{
			if (m_ShadowResult != null && m_ShadowResult.rewardData != null)
			{
				List<NKCUISlot.SlotData> list4 = NKCUISlot.MakeSlotDataListFromReward(m_ShadowResult.rewardData);
				int count = list4.Count;
				int count2 = m_lstNKCUIWRRewardSlot.Count;
				AddExtraRewardSlot(count - count2);
				NKMShadowPalace shadowPalace = NKCScenManager.GetScenManager().GetMyUserData().m_ShadowPalace;
				bool flag = shadowPalace.rewardMultiply > 1;
				if (!NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.SHADOW_PALACE_MULTIPLY))
				{
					flag = false;
				}
				NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_MULTIPLY_REWARD_TAG, flag);
				NKCUtil.SetLabelText(m_MultiplyReward_COUNT_TEXT, string.Format(NKCUtilString.GET_MULTIPLY_REWARD_COUNT_PARAM_02, shadowPalace.rewardMultiply));
				for (int num6 = 0; num6 < count; num6++)
				{
					m_lstNKCUIWRRewardSlot[num6].SetUI(list4[num6], num6);
					m_lstNKCUIWRRewardSlot[num6].SetMultiplyMark(flag);
				}
			}
		}
		else if (m_eContents == USE_CONTENTS.TRIM)
		{
			if (m_trimClearData.rewardData == null)
			{
				return;
			}
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_MULTIPLY_REWARD_TAG, bValue: false);
			List<NKCUISlot.SlotData> list5 = NKCUISlot.MakeSlotDataListFromReward(m_trimClearData.rewardData);
			NKCUISlot.SlotData firstReward = null;
			NKCTrimRewardTemplet nKCTrimRewardTemplet = NKCTrimRewardTemplet.Find(m_trimClearData.trimId, m_trimClearData.trimLevel);
			if (m_bTrimFirstClear && nKCTrimRewardTemplet != null)
			{
				firstReward = NKCUISlot.SlotData.MakeRewardTypeData(nKCTrimRewardTemplet.FirstClearRewardType, nKCTrimRewardTemplet.FirstClearRewardID, nKCTrimRewardTemplet.FirstClearValue);
			}
			if (firstReward != null)
			{
				if (list5.Find((NKCUISlot.SlotData e) => e.ID == firstReward.ID && e.eType == firstReward.eType) != null)
				{
					int count3 = m_lstNKCUIWRRewardSlot.Count;
					num8 = 1;
					AddExtraRewardSlot(num8 - count3);
					if (m_lstNKCUIWRRewardSlot.Count > num7)
					{
						m_lstNKCUIWRRewardSlot[num7].SetUI(firstReward, 0);
						m_lstNKCUIWRRewardSlot[num7].SetFirstMark(bFirst: true);
						num7++;
					}
				}
				else
				{
					firstReward = null;
				}
			}
			if (list5 == null)
			{
				return;
			}
			num9 = list5.Count;
			int count4 = m_lstNKCUIWRRewardSlot.Count;
			AddExtraRewardSlot(num9 + num8 - count4);
			for (int num10 = 0; num10 < num9; num10++)
			{
				if (m_lstNKCUIWRRewardSlot.Count <= num7)
				{
					Log.Error("Trim reward slot out of index", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUIWarfareResult.cs", 1891);
					break;
				}
				if (firstReward != null && firstReward.ID == list5[num10].ID && firstReward.eType == list5[num10].eType)
				{
					list5[num10].Count = Math.Max(1L, list5[num10].Count - firstReward.Count);
				}
				m_lstNKCUIWRRewardSlot[num7].SetUI(list5[num10], num7);
				num7++;
			}
		}
		else if (m_eContents == USE_CONTENTS.WARFARE)
		{
			if (m_NKMWarfareClearData == null)
			{
				return;
			}
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(m_NKMWarfareClearData.m_WarfareID);
			WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
			bool flag2 = warfareGameData.rewardMultiply > 1;
			NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_MULTIPLY_REWARD_TAG, flag2);
			NKCUtil.SetLabelText(m_MultiplyReward_COUNT_TEXT, string.Format(NKCUtilString.GET_MULTIPLY_REWARD_COUNT_PARAM_02, warfareGameData.rewardMultiply));
			List<NKCUISlot.SlotData> lstReward = new List<NKCUISlot.SlotData>();
			if (m_NKMWarfareClearData.m_RewardDataList != null)
			{
				lstReward = NKCUISlot.MakeSlotDataListFromReward(m_NKMWarfareClearData.m_RewardDataList);
			}
			if (m_NKMWarfareClearData.m_MissionReward != null && m_bFirstAllClear)
			{
				lstReward.AddRange(NKCUISlot.MakeSlotDataListFromReward(m_NKMWarfareClearData.m_MissionReward));
			}
			int globalIndex = 0;
			UpdateRewardList(ref lstReward, ref globalIndex, (!IsAllStarClear()) ? eRewardType.FirstClear : eRewardType.AllStarClear);
			UpdateRewardList(ref lstReward, ref globalIndex, eRewardType.FirstAllClear);
			if (m_NKMWarfareClearData.m_OnetimeRewards != null)
			{
				List<NKCUISlot.SlotData> list6 = NKCUISlot.MakeSlotDataListFromReward(m_NKMWarfareClearData.m_OnetimeRewards);
				int count5 = m_lstNKCUIWRRewardSlot.Count;
				if (count5 + list6.Count > m_lstNKCUIWRRewardSlot.Count)
				{
					int newCount = count5 + list6.Count - m_lstNKCUIWRRewardSlot.Count;
					AddExtraRewardSlot(newCount);
				}
				for (int num11 = 0; num11 < list6.Count; num11++)
				{
					NKCUISlot.SlotData slotData4 = list6[num11];
					m_lstNKCUIWRRewardSlot[globalIndex].SetUI(slotData4, globalIndex);
					m_lstNKCUIWRRewardSlot[globalIndex].SetChanceUpMark(bSet: true);
					m_lstNKCUIWRRewardSlot[globalIndex].SetMultiplyMark(flag2);
					globalIndex++;
				}
			}
			if (m_NKMWarfareClearData.m_ContainerRewards != null)
			{
				List<NKCUISlot.SlotData> list7 = NKCUISlot.MakeSlotDataListFromReward(m_NKMWarfareClearData.m_ContainerRewards);
				int count6 = m_lstNKCUIWRRewardSlot.Count;
				if (count6 + list7.Count > m_lstNKCUIWRRewardSlot.Count)
				{
					int newCount2 = count6 + list7.Count - m_lstNKCUIWRRewardSlot.Count;
					AddExtraRewardSlot(newCount2);
				}
				for (int num12 = 0; num12 < list7.Count; num12++)
				{
					NKCUISlot.SlotData slotData5 = list7[num12];
					m_lstNKCUIWRRewardSlot[globalIndex].SetUI(slotData5, globalIndex);
					m_lstNKCUIWRRewardSlot[globalIndex].SetContainerMark(bContainer: true);
					m_lstNKCUIWRRewardSlot[globalIndex].SetMultiplyMark(flag2);
					globalIndex++;
				}
			}
			int count7 = m_lstNKCUIWRRewardSlot.Count;
			if (count7 + lstReward.Count > m_lstNKCUIWRRewardSlot.Count)
			{
				int newCount3 = count7 + lstReward.Count - m_lstNKCUIWRRewardSlot.Count;
				AddExtraRewardSlot(newCount3);
			}
			for (int num13 = 0; num13 < lstReward.Count; num13++)
			{
				NKCUISlot.SlotData slotData6 = lstReward[num13];
				m_lstNKCUIWRRewardSlot[globalIndex].SetUI(slotData6, globalIndex);
				m_lstNKCUIWRRewardSlot[globalIndex].SetMultiplyMark(flag2);
				globalIndex++;
			}
			SetUIRewardAlert(m_NKMWarfareClearData.m_enemiesKillCount == 0);
			if (m_NKMWarfareClearData.m_enemiesKillCount > 0)
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_REWARD_BONUS_TYPE.gameObject, !nKMWarfareTemplet.StageTemplet.m_BuffType.Equals(RewardTuningType.None));
				NKCUtil.SetImageSprite(m_NKM_UI_WARFARE_RESULT_REWARD_BONUS_TYPE, NKCUtil.GetBounsTypeIcon(nKMWarfareTemplet.StageTemplet.m_BuffType, big: false));
			}
		}
		else
		{
			if ((m_eContents != USE_CONTENTS.DUNGEON && m_eContents != USE_CONTENTS.DEFENCE) || m_battleResultData == null)
			{
				return;
			}
			if (GetIsWinGame())
			{
				bool flag3 = m_battleResultData.m_multiply > 1;
				NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_MULTIPLY_REWARD_TAG, flag3);
				NKCUtil.SetLabelText(m_MultiplyReward_COUNT_TEXT, string.Format(NKCUtilString.GET_MULTIPLY_REWARD_COUNT_PARAM_02, m_battleResultData.m_multiply));
				int globalIndex2 = 0;
				ApplyRewardData(m_battleResultData.m_firstRewardData, ref globalIndex2, flag3, eRewardType.FirstClear);
				ApplyRewardData(m_battleResultData.m_firstAllClearData, ref globalIndex2, flag3, eRewardType.FirstAllClear);
				ApplyRewardData(m_battleResultData.m_OnetimeRewardData, ref globalIndex2, flag3, eRewardType.OneTime);
				if (m_battleResultData.m_iUnitExp > 0)
				{
					NKCUISlot.SlotData item = NKCUISlot.SlotData.MakeMiscItemData(502, m_battleResultData.m_iUnitExp, m_battleResultData.m_iUnitExpBonusRate);
					List<NKCUISlot.SlotData> list8 = new List<NKCUISlot.SlotData>();
					list8.Add(item);
					ApplyRewardData(list8, ref globalIndex2, flag3, eRewardType.None);
				}
				ApplyRewardData(m_battleResultData.m_RewardData, ref globalIndex2, flag3, eRewardType.None);
				ApplyRewardData(m_battleResultData.m_additionalReward, ref globalIndex2, flag3, eRewardType.None);
				SetUIRewardAlert(bSet: false);
				NKMStageTempletV2 currentStageTemplet = GetCurrentStageTemplet();
				if (currentStageTemplet != null)
				{
					NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_REWARD_BONUS_TYPE.gameObject, !currentStageTemplet.m_BuffType.Equals(RewardTuningType.None));
					NKCUtil.SetImageSprite(m_NKM_UI_WARFARE_RESULT_REWARD_BONUS_TYPE, NKCUtil.GetBounsTypeIcon(currentStageTemplet.m_BuffType, big: false));
				}
				return;
			}
			NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_MULTIPLY_REWARD_TAG, bValue: false);
			List<NKCUISlot.SlotData> allListRewardSlotData = m_battleResultData.GetAllListRewardSlotData();
			if (allListRewardSlotData.Count > 0)
			{
				if (m_slotEntryFeeReturn != null)
				{
					NKCUISlot.SlotData slotData7 = allListRewardSlotData[0];
					m_slotEntryFeeReturn.SetUI(slotData7, 0);
					m_slotEntryFeeReturn.SetMultiplyMark(bSet: false);
					m_slotEntryFeeReturn.SetFirstAllClearMark(bClear: false);
					m_slotEntryFeeReturn.SetFirstMark(bFirst: false);
					m_slotEntryFeeReturn.SetStarAllClearMark(bAllStarClear: false);
				}
				NKCUtil.SetGameobjectActive(m_objEntryFeeReturn, bValue: true);
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_objEntryFeeReturn, bValue: false);
			}
		}
	}

	private void AddExtraRewardSlot(int newCount)
	{
		for (int i = 0; i < newCount; i++)
		{
			NKCUIWRRewardSlot newInstance = NKCUIWRRewardSlot.GetNewInstance(m_NKM_UI_WARFARE_RESULT_REWARDS_Content);
			m_lstNKCUIWRRewardSlot.Add(newInstance);
		}
	}

	private void SetUIByMission()
	{
		if (m_eContents == USE_CONTENTS.WARFARE)
		{
			WarfareGameData warfareGameData = NKCScenManager.GetScenManager().WarfareGameData;
			if (warfareGameData == null)
			{
				return;
			}
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(warfareGameData.warfareTempletID);
			if (nKMWarfareTemplet == null)
			{
				return;
			}
			m_NKM_UI_WARFARE_RESULT_INFO_MISSION_TEXT[2].text = NKCUtilString.GetWFMissionText(nKMWarfareTemplet.m_WFMissionType_2, nKMWarfareTemplet.m_WFMissionValue_2);
			m_NKM_UI_WARFARE_RESULT_INFO_MISSION_TEXT[1].text = NKCUtilString.GetWFMissionText(nKMWarfareTemplet.m_WFMissionType_1, nKMWarfareTemplet.m_WFMissionValue_1);
			m_NKM_UI_WARFARE_RESULT_INFO_MISSION_TEXT[0].text = NKCUtilString.GetWFMissionText(WARFARE_GAME_MISSION_TYPE.WFMT_CLEAR, 0);
			foreach (GameObject item in m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON)
			{
				NKCUtil.SetGameobjectActive(item, bValue: true);
			}
			if (GetIsWinGame())
			{
				NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON_SUCCESS[2], m_NKMWarfareClearData.m_mission_result_2);
				NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON_FAIL[2], !m_NKMWarfareClearData.m_mission_result_2);
				NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON_SUCCESS[1], m_NKMWarfareClearData.m_mission_result_1);
				NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON_FAIL[1], !m_NKMWarfareClearData.m_mission_result_1);
				NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON_SUCCESS[0], bValue: true);
				NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON_FAIL[0], bValue: false);
			}
			else
			{
				for (int i = 0; i < m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON_SUCCESS.Count; i++)
				{
					NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON_SUCCESS[i], bValue: false);
				}
				for (int j = 0; j < m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON_FAIL.Count; j++)
				{
					NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON_FAIL[j], bValue: true);
				}
			}
		}
		else
		{
			if ((m_eContents != USE_CONTENTS.DUNGEON && m_eContents != USE_CONTENTS.DEFENCE) || m_battleResultData == null)
			{
				return;
			}
			for (int k = 0; k < m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON.Count; k++)
			{
				if (m_battleResultData.m_lstMissionData != null && m_battleResultData.m_lstMissionData.Count > k)
				{
					NKCUIResultSubUIDungeon.MissionData missionData = m_battleResultData.m_lstMissionData[k];
					if (missionData.eMissionType == DUNGEON_GAME_MISSION_TYPE.DGMT_NONE)
					{
						NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON[k], bValue: false);
						NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON_SUCCESS[k], bValue: false);
						NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON_FAIL[k], bValue: false);
						NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_INFO_MISSION_TEXT[k], bValue: false);
					}
					else
					{
						bool flag = m_battleResultData.IsWin && missionData.bSuccess;
						NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON[k], bValue: true);
						NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_INFO_MISSION_TEXT[k], bValue: true);
						m_NKM_UI_WARFARE_RESULT_INFO_MISSION_TEXT[k].text = NKCUtilString.GetDGMissionText(missionData.eMissionType, missionData.iMissionValue);
						NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON_SUCCESS[k], flag);
						NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON_FAIL[k], !flag);
					}
				}
				else
				{
					NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON[k], bValue: false);
					NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON_SUCCESS[k], bValue: false);
					NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_INFO_MISSION_ICON_FAIL[k], bValue: false);
					NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_INFO_MISSION_TEXT[k], bValue: false);
				}
			}
		}
	}

	private void SetUIByGrade(NKC_WARFARE_RESULT_GRADE grade)
	{
		for (int i = 0; i < 5; i++)
		{
			NKCUtil.SetGameobjectActive(m_lst_NKM_UI_WARFARE_RESULT_BACKFX[i], grade == (NKC_WARFARE_RESULT_GRADE)i);
			NKCUtil.SetGameobjectActive(m_lst_NKM_UI_WARFARE_RESULT_INFO_GRADE[i], grade == (NKC_WARFARE_RESULT_GRADE)i);
		}
	}

	private bool GetIsWinGame()
	{
		switch (m_eContents)
		{
		case USE_CONTENTS.DUNGEON:
			if (m_battleResultData != null)
			{
				return m_battleResultData.IsWin;
			}
			return false;
		case USE_CONTENTS.DIVE:
			return m_bClearDive;
		case USE_CONTENTS.SHADOW:
			return m_ShadowResult.life > 0;
		case USE_CONTENTS.TRIM:
			return m_trimClearData.isWin;
		case USE_CONTENTS.WARFARE:
			return m_NKMWarfareClearData != null;
		case USE_CONTENTS.DEFENCE:
		{
			NKMDefenceTemplet currentDefenceDungeonTemplet = NKMDefenceTemplet.GetCurrentDefenceDungeonTemplet(ServiceTime.Recent);
			if (currentDefenceDungeonTemplet != null && currentDefenceDungeonTemplet.CheckGameEnable(ServiceTime.Recent, out var _))
			{
				return m_battleResultData.m_KillCountGain > NKMDefenceTemplet.GetCurrentDefenceDungeonTemplet(ServiceTime.Recent).m_ClearScore;
			}
			return false;
		}
		default:
			return false;
		}
	}

	private void SetUIByLeader(NKMDeckIndex deckIndex, int dummyUnitID = 0, int dummyShipID = 0)
	{
		if (m_NKCASUISpineIllust != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_NKCASUISpineIllust);
		}
		m_NKCASUISpineIllust = null;
		if (m_NKCASUISpineIllustShip != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_NKCASUISpineIllustShip);
		}
		m_NKCASUISpineIllustShip = null;
		m_AB_UI_TALK_BOX.SetText("");
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		NKMUnitData nKMUnitData = null;
		int unitID;
		int skinID;
		if (dummyUnitID == 0)
		{
			if (m_leaderUnitUID != 0L)
			{
				nKMUnitData = armyData.GetUnitFromUID(m_leaderUnitUID);
				if (nKMUnitData == null)
				{
					return;
				}
			}
			else if (deckIndex.m_eDeckType != NKM_DECK_TYPE.NDT_NONE)
			{
				nKMUnitData = armyData.GetDeckLeaderUnitData(deckIndex);
				if (nKMUnitData == null)
				{
					return;
				}
			}
			else if (m_NKMWarfareClearData != null && m_NKMWarfareClearData.m_RewardDataList != null && m_NKMWarfareClearData.m_RewardDataList.UnitExpDataList.Count > 0)
			{
				nKMUnitData = armyData.GetUnitFromUID(m_NKMWarfareClearData.m_RewardDataList.UnitExpDataList[0].m_UnitUid);
			}
			else
			{
				if (m_battleResultData == null || m_battleResultData.m_RewardData == null || m_battleResultData.m_RewardData.UnitExpDataList.Count <= 0)
				{
					return;
				}
				nKMUnitData = armyData.GetUnitFromUID(m_battleResultData.m_RewardData.UnitExpDataList[0].m_UnitUid);
			}
			m_NKCASUISpineIllust = NKCResourceUtility.OpenSpineIllust(nKMUnitData);
			unitID = nKMUnitData.m_UnitID;
			skinID = nKMUnitData.m_SkinID;
		}
		else
		{
			NKMSkinTemplet skinTemplet = NKMSkinManager.GetSkinTemplet(m_dummyUnitSkinID);
			if (skinTemplet != null && NKMSkinManager.IsSkinForCharacter(dummyUnitID, skinTemplet))
			{
				m_NKCASUISpineIllust = NKCResourceUtility.OpenSpineIllust(skinTemplet);
				unitID = dummyUnitID;
				skinID = m_dummyUnitSkinID;
			}
			else
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(dummyUnitID);
				m_NKCASUISpineIllust = NKCResourceUtility.OpenSpineIllust(unitTempletBase);
				unitID = dummyUnitID;
				skinID = 0;
			}
		}
		if (m_NKCASUISpineIllust != null)
		{
			m_NKCASUISpineIllust.SetParent(m_UNITSPINEOBJECT, worldPositionStays: false);
			m_NKCASUISpineIllust.SetAnchoredPosition(Vector2.zero);
			m_NKCASUISpineIllust.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.UNIT_IDLE);
			if (m_NKCASUISpineIllust.GetResultTalkTransform() != null)
			{
				m_AB_UI_TALK_BOX.transform.position = m_NKCASUISpineIllust.GetResultTalkTransform().position;
			}
			else
			{
				m_AB_UI_TALK_BOX.transform.position = m_vDefaultTalkBoxPos;
			}
		}
		if (m_leaderShipUID != 0L)
		{
			NKMUnitData shipFromUID = armyData.GetShipFromUID(m_leaderShipUID);
			if (shipFromUID != null)
			{
				m_NKCASUISpineIllustShip = NKCResourceUtility.OpenSpineIllust(shipFromUID);
				if (m_NKCASUISpineIllustShip != null)
				{
					m_NKCASUISpineIllustShip.SetParent(m_SHIPSPINEOBJECT, worldPositionStays: false);
					m_NKCASUISpineIllustShip.SetAnchoredPosition(Vector2.zero);
					m_NKCASUISpineIllustShip.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SHIP_IDLE);
				}
			}
		}
		else if (deckIndex.m_eDeckType != NKM_DECK_TYPE.NDT_NONE)
		{
			NKMDeckData deckData = armyData.GetDeckData(deckIndex);
			if (deckData != null)
			{
				NKMUnitData shipFromUID2 = armyData.GetShipFromUID(deckData.m_ShipUID);
				if (shipFromUID2 != null)
				{
					m_NKCASUISpineIllustShip = NKCResourceUtility.OpenSpineIllust(shipFromUID2);
					if (m_NKCASUISpineIllustShip != null)
					{
						m_NKCASUISpineIllustShip.SetParent(m_SHIPSPINEOBJECT, worldPositionStays: false);
						m_NKCASUISpineIllustShip.SetAnchoredPosition(Vector2.zero);
						m_NKCASUISpineIllustShip.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SHIP_IDLE);
					}
				}
			}
		}
		else if (dummyShipID != 0)
		{
			NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(dummyShipID);
			m_NKCASUISpineIllustShip = NKCResourceUtility.OpenSpineIllust(unitTempletBase2);
			if (m_NKCASUISpineIllustShip != null)
			{
				m_NKCASUISpineIllustShip.SetParent(m_SHIPSPINEOBJECT, worldPositionStays: false);
				m_NKCASUISpineIllustShip.SetAnchoredPosition(Vector2.zero);
				m_NKCASUISpineIllustShip.SetDefaultAnimation(NKCASUIUnitIllust.eAnimation.SHIP_IDLE);
			}
		}
		NKCDescTemplet descTemplet = NKCDescMgr.GetDescTemplet(unitID, skinID);
		if (descTemplet != null)
		{
			NKCDescTemplet.NKCDescData nKCDescData = (GetIsWinGame() ? ((nKMUnitData == null || !nKMUnitData.IsPermanentContract) ? descTemplet.m_arrDescData[0] : descTemplet.m_arrDescData[4]) : ((nKMUnitData == null || !nKMUnitData.IsPermanentContract) ? descTemplet.m_arrDescData[1] : descTemplet.m_arrDescData[5]));
			m_AB_UI_TALK_BOX.SetText(nKCDescData.GetDesc());
			if (m_NKCASUISpineIllust != null)
			{
				m_NKCASUISpineIllust.SetAnimation(nKCDescData.m_Ani, loop: false);
			}
		}
		else
		{
			m_AB_UI_TALK_BOX.SetText("");
			if (m_NKCASUISpineIllust != null)
			{
				m_NKCASUISpineIllust.SetAnimation(NKCASUIUnitIllust.eAnimation.UNIT_TOUCH, loop: false);
			}
		}
		if (nKMUnitData == null)
		{
			if (GetIsWinGame())
			{
				NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_BATTLE_VICTORY, unitID, skinID, bIgnoreShowNormalAfterLifeTimeOption: true);
			}
			else
			{
				NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_BATTLE_FAIL, unitID, skinID, bIgnoreShowNormalAfterLifeTimeOption: true);
			}
		}
		else if (GetIsWinGame())
		{
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_BATTLE_VICTORY, nKMUnitData, bIgnoreShowNormalAfterLifeTimeOption: true);
		}
		else
		{
			NKCUIVoiceManager.PlayVoice(VOICE_TYPE.VT_BATTLE_FAIL, nKMUnitData, bIgnoreShowNormalAfterLifeTimeOption: true);
		}
	}

	private void SetUIGameTip()
	{
		bool isWinGame = GetIsWinGame();
		NKCUtil.SetGameobjectActive(m_TITLE_REWARD, isWinGame);
		NKCUtil.SetGameobjectActive(m_TITLE_GAMETIP, !isWinGame && m_eContents != USE_CONTENTS.TRIM);
		NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_GAMETIP, !isWinGame);
		switch (m_eContents)
		{
		case USE_CONTENTS.WARFARE:
			NKCUtil.SetLabelText(m_NKM_UI_WARFARE_RESULT_GAMETIP_text, NKCUtilString.GET_STRING_WARFARE_RESULT_GAME_TIP);
			break;
		case USE_CONTENTS.DIVE:
			NKCUtil.SetLabelText(m_NKM_UI_WARFARE_RESULT_GAMETIP_text, NKCUtilString.GET_STRING_DIVE_RESULT_GAME_TIP);
			break;
		case USE_CONTENTS.SHADOW:
			NKCUtil.SetLabelText(m_NKM_UI_WARFARE_RESULT_GAMETIP_text, NKCUtilString.GET_SHADOW_PALACE_RESULT_GAME_TIP);
			break;
		case USE_CONTENTS.DUNGEON:
			NKCUtil.SetLabelText(m_NKM_UI_WARFARE_RESULT_GAMETIP_text, NKCUtilString.GET_STRING_DUNGEON_RESULT_GAME_TIP);
			break;
		case USE_CONTENTS.DEFENCE:
			NKCUtil.SetGameobjectActive(m_TITLE_REWARD, HasAnyReward());
			NKCUtil.SetGameobjectActive(m_TITLE_GAMETIP, !HasAnyReward());
			NKCUtil.SetGameobjectActive(m_NKM_UI_WARFARE_RESULT_GAMETIP, !HasAnyReward());
			NKCUtil.SetLabelText(m_NKM_UI_WARFARE_RESULT_GAMETIP_text, NKCUtilString.GET_STRING_DUNGEON_RESULT_GAME_TIP);
			break;
		default:
			NKCUtil.SetLabelText(m_NKM_UI_WARFARE_RESULT_GAMETIP_text, string.Empty);
			break;
		}
	}

	private bool HasAnyReward()
	{
		if (m_battleResultData.m_RewardData != null && m_battleResultData.m_RewardData.HasAnyReward())
		{
			return true;
		}
		if (m_battleResultData.m_OnetimeRewardData != null && m_battleResultData.m_OnetimeRewardData.HasAnyReward())
		{
			return true;
		}
		if (m_battleResultData.m_firstRewardData != null && m_battleResultData.m_firstRewardData.HasAnyReward())
		{
			return true;
		}
		if (m_battleResultData.m_firstAllClearData != null && m_battleResultData.m_firstAllClearData.HasAnyReward())
		{
			return true;
		}
		return false;
	}

	private void SetUIRewardAlert(bool bSet)
	{
		NKCUtil.SetGameobjectActive(m_Rewards_alert, bSet);
		if (bSet)
		{
			m_Rewards_alert.transform.SetAsLastSibling();
		}
	}

	private NKC_WARFARE_RESULT_GRADE GetGrade()
	{
		if (m_battleResultData != null)
		{
			if (!m_battleResultData.IsWin)
			{
				return NKC_WARFARE_RESULT_GRADE.NWRG_C;
			}
			if (NKMStageTempletV2.Find(m_battleResultData.m_stageID) == null)
			{
				return NKC_WARFARE_RESULT_GRADE.NWRG_S;
			}
			int num = 0;
			int num2 = 0;
			foreach (NKCUIResultSubUIDungeon.MissionData lstMissionDatum in m_battleResultData.m_lstMissionData)
			{
				if (lstMissionDatum.eMissionType != DUNGEON_GAME_MISSION_TYPE.DGMT_NONE)
				{
					num2++;
					if (lstMissionDatum.bSuccess)
					{
						num++;
					}
				}
			}
			return (num2 - num) switch
			{
				0 => NKC_WARFARE_RESULT_GRADE.NWRG_S, 
				1 => NKC_WARFARE_RESULT_GRADE.NWRG_A, 
				2 => NKC_WARFARE_RESULT_GRADE.NWRG_B, 
				_ => NKC_WARFARE_RESULT_GRADE.NWRG_C, 
			};
		}
		if (m_NKMWarfareClearData != null)
		{
			int num3 = 1;
			if (m_NKMWarfareClearData.m_mission_result_1)
			{
				num3++;
			}
			if (m_NKMWarfareClearData.m_mission_result_2)
			{
				num3++;
			}
			if (num3 >= 3)
			{
				return NKC_WARFARE_RESULT_GRADE.NWRG_S;
			}
			return num3 switch
			{
				2 => NKC_WARFARE_RESULT_GRADE.NWRG_A, 
				1 => NKC_WARFARE_RESULT_GRADE.NWRG_B, 
				_ => NKC_WARFARE_RESULT_GRADE.NWRG_C, 
			};
		}
		if (m_eContents == USE_CONTENTS.DIVE)
		{
			if (m_bClearDive)
			{
				return NKC_WARFARE_RESULT_GRADE.NWRG_S;
			}
			return NKC_WARFARE_RESULT_GRADE.NWRG_C;
		}
		if (m_eContents == USE_CONTENTS.SHADOW)
		{
			if (m_ShadowResult.life == 0)
			{
				return NKC_WARFARE_RESULT_GRADE.NWRG_C;
			}
			return NKC_WARFARE_RESULT_GRADE.NWRG_S;
		}
		if (m_eContents == USE_CONTENTS.TRIM)
		{
			if (m_trimClearData.isWin)
			{
				return NKC_WARFARE_RESULT_GRADE.NWRG_S;
			}
			return NKC_WARFARE_RESULT_GRADE.NWRG_C;
		}
		return NKC_WARFARE_RESULT_GRADE.NWRG_C;
	}

	public override void CloseInternal()
	{
		m_bFinishedIntroMovie = false;
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		if (m_CallBackWhenClosed != null && m_NextScenID != NKM_SCEN_ID.NSI_INVALID)
		{
			m_CallBackWhenClosed(m_NextScenID);
			m_NextScenID = NKM_SCEN_ID.NSI_INVALID;
		}
		if (m_NKCASUISpineIllust != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_NKCASUISpineIllust);
		}
		m_NKCASUISpineIllust = null;
		if (m_NKCASUISpineIllustShip != null)
		{
			NKCScenManager.GetScenManager().GetObjectPool().CloseObj(m_NKCASUISpineIllustShip);
		}
		m_NKCASUISpineIllustShip = null;
		if (m_coIntro != null)
		{
			StopCoroutine(m_coIntro);
			m_coIntro = null;
		}
		NKCUIComVideoCamera subUICameraVideoPlayer = NKCCamera.GetSubUICameraVideoPlayer();
		if (subUICameraVideoPlayer != null)
		{
			subUICameraVideoPlayer.CleanUp();
		}
		for (int i = 0; i < m_lstNKCUIWRRewardSlot.Count; i++)
		{
			m_lstNKCUIWRRewardSlot[i].Close();
		}
		m_lstNKCUIWRRewardSlot.Clear();
		NKCUIOperationIntro.CheckInstanceAndClose();
		m_trimClearData = null;
		m_trimModeState = null;
		m_leaderUnitUID = 0L;
		m_leaderShipUID = 0L;
		m_dummyShipID = 0;
		m_dummyUnitID = 0;
		m_dummyUnitSkinID = 0;
		m_bTrimEvent = false;
		m_bDefenceEvent = false;
		m_battleResultData = null;
		CheckNKCPopupArtifactExchangeAndClose();
	}

	public override void OnBackButton()
	{
		CloseToOK();
	}

	public void CloseToHome()
	{
		m_NextScenID = NKM_SCEN_ID.NSI_HOME;
		m_bPause = true;
		NKCScenManager.GetScenManager().GetNKCRepeatOperaion().Init();
		Close();
	}

	private void OnClickRepeatOperation()
	{
		m_bPause = true;
		NKCPopupRepeatOperation.Instance.Open(delegate
		{
			m_bPause = false;
			if (!NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing())
			{
				SetBtnUI();
			}
		});
	}

	private void OnClickRetry()
	{
		m_bPause = true;
		NKCScenManager.GetScenManager().GetNKCRepeatOperaion().Init();
		Retry();
	}

	private void OnDeckEdit()
	{
		if (NKCScenManager.GetScenManager().Get_SCEN_DUNGEON_ATK_READY().SavedDeckContents == DeckContents.DEFENCE)
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_DUNGEON_ATK_READY);
		}
	}

	public void Retry()
	{
		switch (m_eContents)
		{
		case USE_CONTENTS.WARFARE:
		{
			NKC_SCEN_WARFARE_GAME nKC_SCEN_WARFARE_GAME = NKCScenManager.GetScenManager().Get_NKC_SCEN_WARFARE_GAME();
			if (nKC_SCEN_WARFARE_GAME != null && NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_WARFARE_GAME)
			{
				nKC_SCEN_WARFARE_GAME.SetRetry(bSet: true);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_WARFARE_GAME);
			}
			break;
		}
		case USE_CONTENTS.DUNGEON:
		{
			NKMStageTempletV2 currentStageTemplet = GetCurrentStageTemplet();
			if (currentStageTemplet != null)
			{
				NKC_SCEN_OPERATION_V2 sCEN_OPERATION = NKCScenManager.GetScenManager().Get_SCEN_OPERATION();
				if (sCEN_OPERATION != null)
				{
					NKCScenManager.GetScenManager().GetNKCRepeatOperaion().Init();
					sCEN_OPERATION.SetReservedStage(currentStageTemplet);
					NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_OPERATION);
				}
			}
			break;
		}
		}
	}

	public void CloseToOK()
	{
		m_bPause = true;
		switch (m_eContents)
		{
		case USE_CONTENTS.DIVE:
			m_NextScenID = NKM_SCEN_ID.NSI_DIVE_READY;
			break;
		case USE_CONTENTS.SHADOW:
			m_NextScenID = NKM_SCEN_ID.NSI_SHADOW_PALACE;
			break;
		case USE_CONTENTS.TRIM:
			m_NextScenID = NKM_SCEN_ID.NSI_TRIM;
			break;
		case USE_CONTENTS.WARFARE:
		case USE_CONTENTS.DUNGEON:
		{
			NKCScenManager.GetScenManager().GetNKCRepeatOperaion().Init();
			NKC_SCEN_OPERATION_V2 sCEN_OPERATION = NKCScenManager.GetScenManager().Get_SCEN_OPERATION();
			NKMStageTempletV2 possibleNextOperation = GetPossibleNextOperation();
			if (possibleNextOperation != null)
			{
				sCEN_OPERATION.SetReservedStage(possibleNextOperation);
				m_NextScenID = NKM_SCEN_ID.NSI_OPERATION;
				break;
			}
			NKMStageTempletV2 currentStageTemplet = GetCurrentStageTemplet();
			if (currentStageTemplet != null)
			{
				sCEN_OPERATION.SetReservedStage(currentStageTemplet);
				m_NextScenID = NKM_SCEN_ID.NSI_OPERATION;
			}
			else
			{
				m_NextScenID = NKM_SCEN_ID.NSI_HOME;
			}
			break;
		}
		case USE_CONTENTS.DEFENCE:
			m_NextScenID = NKM_SCEN_ID.NSI_HOME;
			break;
		}
		Close();
	}

	private void OnClickBattleStat()
	{
		if (m_battleResultData != null)
		{
			NKCUIBattleStatistics.Instance.Open(m_battleResultData.m_battleData, null);
		}
	}

	private bool IsPopupTiming()
	{
		if (m_Animtor.speed == 1f)
		{
			return m_fElapsedTime > 2.466667f;
		}
		return false;
	}

	private void CheckTutorialRequired()
	{
		m_bRequiredTutorial = NKCTutorialManager.TutorialRequired(TutorialPoint.WarfareResult, play: false) != TutorialStep.None;
	}
}
