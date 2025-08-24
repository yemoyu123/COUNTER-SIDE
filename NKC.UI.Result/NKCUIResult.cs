using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClientPacket.Common;
using ClientPacket.Defence;
using ClientPacket.Game;
using ClientPacket.Mode;
using ClientPacket.Office;
using ClientPacket.Pvp;
using ClientPacket.WorldMap;
using Cs.Logging;
using Cs.Protocol;
using NKC.Publisher;
using NKC.Util;
using NKM;
using NKM.Guild;
using NKM.Templet;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NKC.UI.Result;

public class NKCUIResult : NKCUIBase
{
	public delegate void OnClose();

	public delegate void OnTouchGameRecord();

	private enum eTitleType
	{
		None,
		Win,
		Lose,
		Get,
		Replay,
		WinPrivate,
		LosePrivate,
		DrawPrivate
	}

	public class BattleResultData
	{
		public int m_stageID;

		public BATTLE_RESULT_TYPE m_BATTLE_RESULT_TYPE = BATTLE_RESULT_TYPE.BRT_LOSE;

		public NKM_GAME_TYPE m_NKM_GAME_TYPE = NKM_GAME_TYPE.NGT_PVP_RANK;

		public List<NKCUIResultSubUIDungeon.MissionData> m_lstMissionData;

		public bool m_bShowMedal;

		public bool m_bShowBonus;

		public List<NKCUIResultSubUIUnitExp.UnitLevelupUIData> m_lstUnitLevelupData;

		public int m_iUnitExp;

		public int m_iUnitExpBonusRate;

		public NKMRewardData m_firstRewardData;

		public NKMRewardData m_RewardData;

		public NKMRewardData m_OnetimeRewardData;

		public NKMRewardData m_firstAllClearData;

		public NKMAdditionalReward m_additionalReward;

		public int m_OrgPVPScore;

		public int m_OrgPVPTier;

		public long m_OrgDoubleToken;

		public NKMRaidBossResultData m_RaidBossResultData;

		public bool m_bShadowAllClear;

		public int m_ShadowPrevLife;

		public int m_ShadowCurrLife;

		public int m_ShadowBestClearTime;

		public int m_ShadowCurrClearTime;

		public int m_iFierceScore;

		public int m_iFierceBestScore;

		public float m_fFierceLastBossHPPercent;

		public float m_fFierceRestTime;

		public bool m_bShowClearPoint;

		public float m_fArenaClearPoint;

		public long m_KillCountGain;

		public long m_KillCountTotal;

		public long m_KillCountStageRecord;

		public NKCUIBattleStatistics.BattleData m_battleData;

		public int m_multiply = 1;

		public bool IsWin => m_BATTLE_RESULT_TYPE == BATTLE_RESULT_TYPE.BRT_WIN;

		public List<NKCUISlot.SlotData> GetAllListRewardSlotData()
		{
			List<NKCUISlot.SlotData> list = new List<NKCUISlot.SlotData>();
			list.AddRange(NKCUISlot.MakeSlotDataListFromReward(m_firstRewardData));
			list.AddRange(NKCUISlot.MakeSlotDataListFromReward(m_RewardData));
			list.AddRange(NKCUISlot.MakeSlotDataListFromReward(m_OnetimeRewardData));
			list.AddRange(NKCUISlot.MakeSlotDataListFromReward(m_firstAllClearData));
			list.AddRange(NKCUISlot.MakeSlotDataListFromReward(m_additionalReward));
			return list;
		}
	}

	public class CityMissionResultData
	{
		public int m_CityID;

		public int m_MissionID;

		public NKMUnitData m_LeaderUnitData;

		public int m_CityLevelOld;

		public int m_CityLevelNew;

		public int m_CityExpOld;

		public int m_CityExpNew;

		public bool m_bGreatSuccess;

		public List<NKCUIResultSubUIUnitExp.UnitLevelupUIData> m_lstUnitLevelupData;

		public NKCUIResultSubUIUnitExp.UnitLevelupUIData LeaderUnitLevelupData;

		public NKMRewardData m_RewardData;

		public NKMRewardData m_SuccessRewardData;

		public string m_SuccessSlotText;
	}

	private const string ASSET_BUNDLE_NAME = "ab_ui_nkm_ui_result";

	private const string UI_ASSET_NAME = "NKM_UI_RESULT_COMMON";

	private static NKCUIResult m_Instance;

	private RectTransform m_rtRoot;

	[Header("Basic Information")]
	private Animator m_aniTitleWin;

	private Animator m_aniTitleLose;

	private Animator m_aniTitleGet;

	private Animator m_aniTitleReplay;

	private Animator m_aniTitleWinPrivate;

	private Animator m_aniTitleLosePrivate;

	private Animator m_aniTitleDrawPrivate;

	public GameObject m_objTitle;

	private Text m_lbGetTitle;

	private GameObject m_objBottomButton;

	[Header("SubPages")]
	public NKCUIResultSubUIMiddle m_uiSubUIMiddle;

	public NKCUIResultSubUIReward m_uiReward;

	public NKCUIResultSubUITip m_uiTip;

	public NKCUIResultSubUIWorldmap m_uiWorldmap;

	public NKCUIResultSubUIShadowTime m_uiShadowTime;

	public NKCUIResultSubUIShadowLife m_uiShadowLife;

	public NKCUIResultSubUIFierceBattle m_ui_FierceBattle;

	public NKCUIResultSubUIRearmament m_ui_RearmExtract;

	public NKCUIResultSubUIKillCount m_uiKillCount;

	public NKCUIResultMiscContract m_uiMiscContract;

	public NKCUIResultSubUITrim m_uiTrim;

	[Header("더블토큰")]
	public Animator m_amtorDoubleToken;

	public GameObject m_objDoubleToken;

	public Text m_lbDoubleTokenCount;

	[Header("반복작전")]
	public GameObject m_objRepeatOperation;

	public Animator m_amtorRepeatOperation;

	public Text m_lbRepeatOperation;

	public NKCUIComStateButton m_csbtnRepeatOperation;

	public GameObject m_objRepeatOperationCountDown;

	public Text m_lbRepeatOperationCountDown;

	public Image m_imgRepeatOperationCountDown;

	[Header("Etc")]
	public GameObject m_objUserBuff;

	public GameObject m_objOperationMultiply;

	public Text m_txtOperationMultiply;

	public GameObject m_objContractMiscReward;

	public Image m_ImgContractMiscRewardIcon;

	public Text m_lbContractMiscReward;

	public GameObject m_WIN;

	public GameObject m_LOSE;

	public GameObject m_WIN_BATTLE_REPORT;

	public GameObject m_LOSE_BATTLE_REPORT;

	public GameObject m_ASSIST;

	[Header("Share")]
	public GameObject m_objShareBtn;

	public NKCUIComStateButton m_csbtnShare;

	public GameObject m_objShare;

	public Text m_lbLevel;

	public Text m_lbUserName;

	public Text m_lbUserUID;

	private Coroutine m_crtShare;

	private RectTransform m_rtTitleRoot;

	private RectTransform m_rtBackgroundOpen;

	private NKCUIComStateButton m_btnContinue;

	private NKCUIComStateButton m_btnBattleStatistics;

	private NKCUIComStateButton m_btnReplayBattleStatistics;

	[Header("Trim Retry")]
	public NKCUIComStateButton m_csbtnTrimRetry;

	private EventTrigger m_eventTrigger;

	private OnClose dOnClose;

	private OnTouchGameRecord dOnTouchGameRecord;

	public float UI_TITLE_ANI_END_DELAY = 0.5f;

	public float NoSkipSecond = 0.1f;

	protected bool m_bHadUserInput;

	private Animator m_uiCurrentPlayingTitleAni;

	private eTitleType m_eCurrentTitleType;

	private Coroutine m_LastCoroutine;

	private bool m_bPause;

	private bool m_bUserLevelUpPopupOpened;

	private List<NKMRewardData> m_lstUnitGainRewardData;

	public GameObject m_RESULT_WIN_Bonus_Type;

	public Image m_RESULT_WIN_Bonus_Type_icon;

	private const string CAPTURE_FILE_NAME = "ScreenCapture.png";

	private const string THUMBNAIL_FILE_NAME = "Thumbnail.png";

	private bool m_bWaitForUnitGain;

	private bool m_bSkipDuplicateUnitGain = true;

	private bool m_bDefaultSort = true;

	private bool m_bProcessingCountDown;

	private const float HOLD_SKIP_TIME = 0.3f;

	private float m_fCurrentHoldTime;

	public static NKCUIResult Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = NKCUIManager.OpenNewInstance<NKCUIResult>("ab_ui_nkm_ui_result", "NKM_UI_RESULT_COMMON", NKCUIManager.eUIBaseRect.UIFrontCommon, CleanupInstance).GetInstance<NKCUIResult>();
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

	public override NKCUIManager.eUIUnloadFlag UnloadFlag => NKCUIManager.eUIUnloadFlag.ON_PLAY_GAME;

	public override string MenuName => NKCUtilString.GET_STRING_RESULT;

	public override eMenutype eUIType => eMenutype.FullScreen;

	public override NKCUIUpsideMenu.eMode eUpsideMenuMode => NKCUIUpsideMenu.eMode.Disable;

	public override bool WillCloseUnderPopupOnOpen => false;

	private string CapturePath => Path.Combine(Application.persistentDataPath, "ScreenCapture.png");

	private string ThumbnailPath => Path.Combine(Application.persistentDataPath, "Thumbnail.png");

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

	private IEnumerable<NKCUIResultSubUIBase> GetSubUIEnumerator()
	{
		yield return m_uiSubUIMiddle;
		yield return m_uiReward;
		yield return m_uiTip;
		yield return m_uiWorldmap;
		yield return m_uiShadowTime;
		yield return m_uiShadowLife;
		yield return m_ui_FierceBattle;
		yield return m_ui_RearmExtract;
		yield return m_uiKillCount;
		yield return m_uiMiscContract;
		yield return m_uiTrim;
	}

	private void SelectTitle(eTitleType type)
	{
		if (NKCReplayMgr.IsPlayingReplay())
		{
			type = eTitleType.Replay;
		}
		m_eCurrentTitleType = type;
		switch (m_eCurrentTitleType)
		{
		case eTitleType.Win:
			m_uiCurrentPlayingTitleAni = m_aniTitleWin;
			break;
		case eTitleType.Lose:
			m_uiCurrentPlayingTitleAni = m_aniTitleLose;
			break;
		case eTitleType.Get:
			m_uiCurrentPlayingTitleAni = m_aniTitleGet;
			break;
		case eTitleType.Replay:
			m_uiCurrentPlayingTitleAni = m_aniTitleReplay;
			break;
		case eTitleType.WinPrivate:
			m_uiCurrentPlayingTitleAni = m_aniTitleWinPrivate;
			break;
		case eTitleType.LosePrivate:
			m_uiCurrentPlayingTitleAni = m_aniTitleLosePrivate;
			break;
		case eTitleType.DrawPrivate:
			m_uiCurrentPlayingTitleAni = m_aniTitleDrawPrivate;
			break;
		}
	}

	public void Init()
	{
		m_rtRoot = GetComponent<RectTransform>();
		m_aniTitleWin = m_rtRoot.Find("Result_WIN").GetComponent<Animator>();
		m_aniTitleLose = m_rtRoot.Find("Result_LOSE").GetComponent<Animator>();
		m_aniTitleGet = m_rtRoot.Find("Result_GET").GetComponent<Animator>();
		if (m_rtRoot.Find("Result_REPLAY") != null)
		{
			m_aniTitleReplay = m_rtRoot.Find("Result_REPLAY").GetComponent<Animator>();
		}
		if (m_rtRoot.Find("Result_PRIVATE_WIN") != null)
		{
			m_aniTitleWinPrivate = m_rtRoot.Find("Result_PRIVATE_WIN").GetComponent<Animator>();
		}
		if (m_rtRoot.Find("Result_PRIVATE_LOSE") != null)
		{
			m_aniTitleLosePrivate = m_rtRoot.Find("Result_PRIVATE_LOSE").GetComponent<Animator>();
		}
		if (m_rtRoot.Find("Result_PRIVATE_DRAW") != null)
		{
			m_aniTitleDrawPrivate = m_rtRoot.Find("Result_PRIVATE_DRAW").GetComponent<Animator>();
		}
		m_lbGetTitle = m_rtRoot.Find("Result_GET/SUB_TITLE/TEXT").GetComponent<Text>();
		m_objBottomButton = m_rtRoot.Find("Continue_button").gameObject;
		m_rtTitleRoot = m_rtRoot.Find("Result_GET/SUB_TITLE").GetComponent<RectTransform>();
		m_rtBackgroundOpen = m_rtRoot.Find("NKM_UI_WARFARE_RESULT_BG").GetComponent<RectTransform>();
		m_btnContinue = m_rtRoot.Find("Continue_button").GetComponent<NKCUIComStateButton>();
		m_btnContinue.PointerDown.RemoveAllListeners();
		m_btnContinue.PointerDown.AddListener(delegate
		{
			OnClickContinue();
		});
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerDown;
		entry.callback = new EventTrigger.TriggerEvent();
		entry.callback.AddListener(OnUserInputEvent);
		m_eventTrigger = m_rtRoot.gameObject.GetComponent<EventTrigger>();
		m_eventTrigger.triggers.Add(entry);
		m_btnBattleStatistics = m_rtRoot.Find("Battle_Statistics_button").GetComponent<NKCUIComStateButton>();
		NKCUtil.SetBindFunction(m_btnBattleStatistics, delegate
		{
			dOnTouchGameRecord?.Invoke();
		});
		m_btnReplayBattleStatistics = m_rtRoot.Find("REPLAY_Battle_Statistics_button").GetComponent<NKCUIComStateButton>();
		NKCUtil.SetBindFunction(m_btnReplayBattleStatistics, delegate
		{
			dOnTouchGameRecord?.Invoke();
		});
		NKCUtil.SetBindFunction(m_csbtnRepeatOperation, OnClickRepeatOperation);
		NKCUtil.SetButtonClickDelegate(m_csbtnTrimRetry, OnTrimRetry);
		if (m_lstUnitGainRewardData != null && m_lstUnitGainRewardData.Count > 0)
		{
			Log.Error($"NKCUIResult - m_lstUnitGainRewardData is not null! Count[{m_lstUnitGainRewardData.Count}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUIResult.cs", 307);
		}
		m_lstUnitGainRewardData = null;
		m_uiSubUIMiddle.Init();
		m_uiReward.Init(m_lbDoubleTokenCount, m_amtorDoubleToken);
		m_uiMiscContract?.Init();
		NKCUtil.SetBindFunction(m_csbtnShare, OnClickShareBtn);
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
	}

	private void OnClickShareBtn()
	{
		NKCPopupSnsShareMenu.Instance.Open(OnClickSnsShareIcon);
	}

	private void OnClickSnsShareIcon(NKCPublisherModule.SNS_SHARE_TYPE eSNS_SHARE_TYPE)
	{
		m_crtShare = StartCoroutine(ProcessShare(eSNS_SHARE_TYPE));
	}

	private IEnumerator ProcessShare(NKCPublisherModule.SNS_SHARE_TYPE eSNS_SHARE_TYPE)
	{
		yield return null;
		NKCUtil.SetGameobjectActive(m_objShareBtn, bValue: false);
		NKCUtil.SetGameobjectActive(m_objShare, bValue: true);
		yield return new WaitForEndOfFrame();
		if (!NKCScreenCaptureUtility.CaptureScreenWithThumbnail(CapturePath, ThumbnailPath))
		{
			OnShareFinished(NKC_PUBLISHER_RESULT_CODE.NPRC_MARKETING_SNS_SHARE_FAIL, null);
			yield break;
		}
		yield return null;
		NKCPublisherModule.Marketing.TrySnsShare(eSNS_SHARE_TYPE, CapturePath, ThumbnailPath, OnShareFinished);
	}

	private void _OnShareFinished()
	{
		NKCUtil.SetGameobjectActive(m_objShareBtn, bValue: true);
		NKCUtil.SetGameobjectActive(m_objShare, bValue: false);
	}

	private void OnShareFinished(NKC_PUBLISHER_RESULT_CODE result, string additionalError)
	{
		if (NKMContentsVersionManager.HasCountryTag(CountryTagType.CHN))
		{
			_OnShareFinished();
		}
		else if (NKCPublisherModule.CheckError(result, additionalError, bCloseWaitBox: true, _OnShareFinished))
		{
			_OnShareFinished();
		}
	}

	private void OnClickContinue()
	{
		m_bHadUserInput = true;
		foreach (NKCUIResultSubUIBase item in GetSubUIEnumerator())
		{
			if (item.gameObject.activeInHierarchy)
			{
				item.OnUserInput();
			}
		}
	}

	private void OnUserInputEvent(BaseEventData eventData)
	{
		OnClickContinue();
	}

	private void CloseAllSubUI()
	{
		foreach (NKCUIResultSubUIBase item in GetSubUIEnumerator())
		{
			item.Close();
			item.ProcessRequired = false;
		}
	}

	public override void Hide()
	{
		if (m_uiReward.gameObject.activeInHierarchy)
		{
			m_uiReward.SetActiveLoopScrollList(bActivate: false);
		}
		if (m_uiMiscContract.gameObject.activeInHierarchy)
		{
			m_uiMiscContract.SetActiveLoopScrollList(bActivate: false);
		}
		m_bHide = true;
		m_rtRoot.localScale = Vector3.zero;
	}

	public override void UnHide()
	{
		m_bHide = false;
		m_rtRoot.localScale = Vector3.one;
		if (m_uiReward.gameObject.activeInHierarchy)
		{
			m_uiReward.SetActiveLoopScrollList(bActivate: true);
		}
		if (m_uiMiscContract.gameObject.activeInHierarchy)
		{
			m_uiMiscContract.SetActiveLoopScrollList(bActivate: true);
		}
	}

	private void OnClickBattleStatistics(NKCUIBattleStatistics.BattleData battleData)
	{
		if (battleData != null)
		{
			NKCUIBattleStatistics.Instance.Open(battleData, delegate
			{
				SetPause(bSet: false);
			});
			SetPause(bSet: true);
		}
	}

	private void OnClickRepeatOperation()
	{
		NKCPopupRepeatOperation.Instance.Open(delegate
		{
			SetPause(bSet: false);
			if (!NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing())
			{
				NKCUIBase.SetGameObjectActive(m_objRepeatOperation, bValue: false);
			}
		});
		SetPause(bSet: true);
	}

	private void HideCommonObjects()
	{
		NKCUtil.SetGameobjectActive(m_objBottomButton, bValue: false);
		NKCUtil.SetGameobjectActive(m_btnBattleStatistics, bValue: false);
		NKCUtil.SetGameobjectActive(m_csbtnTrimRetry, bValue: false);
		NKCUtil.SetGameobjectActive(m_btnReplayBattleStatistics, bValue: false);
		NKCUtil.SetGameobjectActive(m_objDoubleToken, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRepeatOperation, bValue: false);
	}

	public static BattleResultData MakePvEBattleResultData(NKM_GAME_TYPE gameType, NKCGameClient nkmGame, NKMPacket_GAME_END_NOT cPacket_GAME_END_NOT, int dungeonID, int stageID)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		BattleResultData battleResultData;
		switch (gameType)
		{
		case NKM_GAME_TYPE.NGT_RAID:
		case NKM_GAME_TYPE.NGT_RAID_SOLO:
			battleResultData = MakeRaidResultData(nKMUserData.m_ArmyData, dungeonID, cPacket_GAME_END_NOT.raidBossResultData, NKCUIBattleStatistics.MakeBattleData(nkmGame, cPacket_GAME_END_NOT));
			break;
		case NKM_GAME_TYPE.NGT_SHADOW_PALACE:
			battleResultData = MakeShadowResultData(nKMUserData.m_ArmyData, cPacket_GAME_END_NOT.deckIndex, dungeonID, cPacket_GAME_END_NOT.win, cPacket_GAME_END_NOT.dungeonClearData, cPacket_GAME_END_NOT.shadowGameResult, nKMUserData.m_ShadowPalace, NKCUIBattleStatistics.MakeBattleData(nkmGame, cPacket_GAME_END_NOT), cPacket_GAME_END_NOT.updatedUnits);
			break;
		case NKM_GAME_TYPE.NGT_FIERCE:
			battleResultData = MakeFierceResultData(dungeonID, cPacket_GAME_END_NOT.win, cPacket_GAME_END_NOT.dungeonClearData, cPacket_GAME_END_NOT.fierceResultData, NKCUIBattleStatistics.MakeBattleData(nkmGame, cPacket_GAME_END_NOT));
			break;
		case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_ARENA:
			NKCGuildCoopManager.SetWaitForArenaResult(bValue: true);
			battleResultData = MakeGuildCoopArenaResultData(nKMUserData.m_ArmyData, dungeonID, cPacket_GAME_END_NOT.win, cPacket_GAME_END_NOT.dungeonClearData, cPacket_GAME_END_NOT.deckIndex, NKCUIBattleStatistics.MakeBattleData(nkmGame, cPacket_GAME_END_NOT));
			break;
		case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS:
		case NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS_PRACTICE:
			battleResultData = MakeRaidResultData(nKMUserData.m_ArmyData, dungeonID, cPacket_GAME_END_NOT.raidBossResultData, NKCUIBattleStatistics.MakeBattleData(nkmGame, cPacket_GAME_END_NOT));
			break;
		case NKM_GAME_TYPE.NGT_PHASE:
			battleResultData = MakePhaseResultData(stageID, nKMUserData.m_ArmyData, cPacket_GAME_END_NOT, NKCUIBattleStatistics.MakeBattleData(nkmGame, cPacket_GAME_END_NOT));
			break;
		case NKM_GAME_TYPE.NGT_TRIM:
			battleResultData = MakeMissionResultData(nKMUserData.m_ArmyData, dungeonID, stageID, cPacket_GAME_END_NOT.win, cPacket_GAME_END_NOT.dungeonClearData, cPacket_GAME_END_NOT.deckIndex, NKCUIBattleStatistics.MakeBattleData(nkmGame, cPacket_GAME_END_NOT), cPacket_GAME_END_NOT.updatedUnits, NKCScenManager.GetScenManager().GetGameClient().MultiplyReward);
			battleResultData.m_NKM_GAME_TYPE = NKM_GAME_TYPE.NGT_TRIM;
			break;
		case NKM_GAME_TYPE.NGT_DIVE:
			battleResultData = MakeMissionResultData(nKMUserData.m_ArmyData, dungeonID, stageID, cPacket_GAME_END_NOT.win, cPacket_GAME_END_NOT.dungeonClearData, cPacket_GAME_END_NOT.deckIndex, NKCUIBattleStatistics.MakeBattleData(nkmGame, cPacket_GAME_END_NOT), cPacket_GAME_END_NOT.updatedUnits, NKCScenManager.GetScenManager().GetGameClient().MultiplyReward);
			battleResultData.m_NKM_GAME_TYPE = NKM_GAME_TYPE.NGT_DIVE;
			break;
		case NKM_GAME_TYPE.NGT_PVE_DEFENCE:
			battleResultData = MakeDefenceResultData(nKMUserData.m_ArmyData, cPacket_GAME_END_NOT.deckIndex, dungeonID, cPacket_GAME_END_NOT.win, cPacket_GAME_END_NOT.dungeonClearData, null, null);
			battleResultData.m_NKM_GAME_TYPE = NKM_GAME_TYPE.NGT_PVE_DEFENCE;
			break;
		default:
			battleResultData = MakeMissionResultData(nKMUserData.m_ArmyData, dungeonID, stageID, cPacket_GAME_END_NOT.win, cPacket_GAME_END_NOT.dungeonClearData, cPacket_GAME_END_NOT.deckIndex, NKCUIBattleStatistics.MakeBattleData(nkmGame, cPacket_GAME_END_NOT), cPacket_GAME_END_NOT.updatedUnits, NKCScenManager.GetScenManager().GetGameClient().MultiplyReward);
			break;
		}
		if (cPacket_GAME_END_NOT.dungeonClearData != null)
		{
			battleResultData.m_iUnitExp = cPacket_GAME_END_NOT.dungeonClearData.unitExp;
		}
		NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(stageID);
		if (nKMStageTempletV != null)
		{
			if (nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_KILLCOUNT && cPacket_GAME_END_NOT.killCountData != null && cPacket_GAME_END_NOT.win)
			{
				battleResultData.m_KillCountGain = cPacket_GAME_END_NOT.killCountDelta;
				battleResultData.m_KillCountTotal = cPacket_GAME_END_NOT.killCountData.killCount;
				battleResultData.m_KillCountStageRecord = NKCScenManager.CurrentUserData().GetStageKillCountBest(stageID);
			}
			if (nKMStageTempletV.m_STAGE_SUB_TYPE == STAGE_SUB_TYPE.SST_TIMEATTACK)
			{
				battleResultData.m_ShadowCurrClearTime = (int)cPacket_GAME_END_NOT.totalPlayTime;
				battleResultData.m_ShadowBestClearTime = NKCScenManager.CurrentUserData().GetStageBestClearSec(stageID);
			}
		}
		return battleResultData;
	}

	public static BattleResultData MakePvPResultData(BATTLE_RESULT_TYPE battleResultType, NKMItemMiscData cNKMItemMiscData, NKCUIBattleStatistics.BattleData battleData, NKM_GAME_TYPE gameType)
	{
		BattleResultData battleResultData = new BattleResultData();
		battleResultData.m_stageID = 0;
		battleResultData.m_bShowMedal = true;
		battleResultData.m_bShowBonus = false;
		battleResultData.m_BATTLE_RESULT_TYPE = battleResultType;
		battleResultData.m_lstMissionData = new List<NKCUIResultSubUIDungeon.MissionData>();
		battleResultData.m_NKM_GAME_TYPE = gameType;
		NKCUIResultSubUIDungeon.MissionData missionData = new NKCUIResultSubUIDungeon.MissionData();
		missionData.bSuccess = battleResultType == BATTLE_RESULT_TYPE.BRT_WIN;
		missionData.eMissionType = DUNGEON_GAME_MISSION_TYPE.DGMT_CLEAR;
		battleResultData.m_lstMissionData.Add(missionData);
		NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
		PvpState pvpState = null;
		switch (gameType)
		{
		case NKM_GAME_TYPE.NGT_PVP_RANK:
			pvpState = myUserData.m_PvpData;
			break;
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
			pvpState = myUserData.m_AsyncData;
			break;
		case NKM_GAME_TYPE.NGT_PVP_PRIVATE:
			pvpState = myUserData.m_PvpData;
			break;
		case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
		case NKM_GAME_TYPE.NGT_PVP_UNLIMITED:
			pvpState = myUserData.m_LeagueData;
			break;
		case NKM_GAME_TYPE.NGT_PVP_EVENT:
			pvpState = myUserData.m_eventPvpData;
			break;
		default:
			Debug.LogError("[NKCUIResult.MakePvPResultData] 모르는 게임타입이 들어옴 - " + gameType);
			return null;
		case NKM_GAME_TYPE.NGT_PVE_SIMULATED:
			break;
		}
		if (pvpState != null)
		{
			int num = NKCPVPManager.FindPvPSeasonID(gameType, NKCSynchronizedTime.GetServerUTCTime());
			if (num == pvpState.SeasonID)
			{
				battleResultData.m_OrgPVPScore = pvpState.Score;
				battleResultData.m_OrgPVPTier = pvpState.LeagueTierID;
			}
			else
			{
				battleResultData.m_OrgPVPScore = NKCPVPManager.GetResetScore(pvpState.SeasonID, pvpState.Score, gameType);
				NKMPvpRankTemplet rankTempletByScore = NKCPVPManager.GetRankTempletByScore(gameType, num, battleResultData.m_OrgPVPScore);
				if (rankTempletByScore != null)
				{
					battleResultData.m_OrgPVPTier = rankTempletByScore.LeagueTier;
				}
			}
		}
		battleResultData.m_OrgDoubleToken = NKCScenManager.GetScenManager().GetMyUserData().m_InventoryData.GetCountMiscItem(301);
		if (cNKMItemMiscData != null)
		{
			battleResultData.m_RewardData = new NKMRewardData();
			battleResultData.m_RewardData.MiscItemDataList.Add(cNKMItemMiscData);
		}
		battleResultData.m_battleData = battleData;
		return battleResultData;
	}

	public static BattleResultData MakeMissionResultData(NKMArmyData armyData, int dungeonID, int stageID, bool bWin, NKMDungeonClearData dungeonClearData, NKMDeckIndex deckIndex, NKCUIBattleStatistics.BattleData battleData, List<UnitLoyaltyUpdateData> lstUnitUpdateData = null, int multiplyReward = 1)
	{
		BattleResultData battleResultData = new BattleResultData();
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(dungeonID);
		if (dungeonTempletBase == null)
		{
			Debug.LogError("DungeonTemplet Not Found!!");
			return battleResultData;
		}
		battleResultData.m_stageID = stageID;
		battleResultData.m_lstMissionData = new List<NKCUIResultSubUIDungeon.MissionData>();
		bool flag = true;
		bool flag2 = deckIndex.m_eDeckType == NKM_DECK_TYPE.NDT_FRIEND;
		if (flag2)
		{
			flag = false;
		}
		if (deckIndex.m_eDeckType == NKM_DECK_TYPE.NDT_DIVE)
		{
			flag = false;
		}
		if (dungeonTempletBase.IsUsingEventDeck())
		{
			deckIndex = NKMDeckIndex.None;
		}
		NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(dungeonTempletBase.m_DungeonStrID);
		battleResultData.m_firstRewardData = null;
		battleResultData.m_firstAllClearData = null;
		if (bWin)
		{
			battleResultData.m_BATTLE_RESULT_TYPE = BATTLE_RESULT_TYPE.BRT_WIN;
		}
		else
		{
			battleResultData.m_BATTLE_RESULT_TYPE = BATTLE_RESULT_TYPE.BRT_LOSE;
		}
		battleResultData.m_bShowMedal = nKMStageTempletV != null;
		battleResultData.m_bShowBonus = dungeonTempletBase.BonusResult && bWin;
		bool flag3 = false;
		FirstRewardData firstRewardData = nKMStageTempletV?.GetFirstRewardData();
		NKMRewardData rewardData = null;
		if (nKMStageTempletV != null && firstRewardData != null && bWin && firstRewardData.RewardId > 0 && !NKCScenManager.CurrentUserData().CheckDungeonClear(nKMStageTempletV.DungeonTempletBase.m_DungeonID))
		{
			flag3 = true;
		}
		bool flag4;
		if (!(flag4 = !NKCScenManager.GetScenManager().GetMyUserData().CheckDungeonClear(dungeonID)) && dungeonClearData != null && (!dungeonClearData.missionResult1 || !dungeonClearData.missionResult2 || !dungeonClearData.missionRewardResult || (dungeonClearData != null && dungeonClearData.missionReward != null)))
		{
			flag4 = true;
		}
		if (flag)
		{
			NKCUIResultSubUIDungeon.MissionData missionData = new NKCUIResultSubUIDungeon.MissionData();
			missionData.bSuccess = bWin;
			missionData.eMissionType = DUNGEON_GAME_MISSION_TYPE.DGMT_CLEAR;
			battleResultData.m_lstMissionData.Add(missionData);
			NKCUIResultSubUIDungeon.MissionData missionData2 = new NKCUIResultSubUIDungeon.MissionData();
			missionData2.bSuccess = dungeonClearData?.missionResult1 ?? false;
			missionData2.eMissionType = dungeonTempletBase.m_DGMissionType_1;
			missionData2.iMissionValue = dungeonTempletBase.m_DGMissionValue_1;
			battleResultData.m_lstMissionData.Add(missionData2);
			NKCUIResultSubUIDungeon.MissionData missionData3 = new NKCUIResultSubUIDungeon.MissionData();
			missionData3.bSuccess = dungeonClearData?.missionResult2 ?? false;
			missionData3.eMissionType = dungeonTempletBase.m_DGMissionType_2;
			missionData3.iMissionValue = dungeonTempletBase.m_DGMissionValue_2;
			battleResultData.m_lstMissionData.Add(missionData3);
		}
		if (dungeonClearData != null)
		{
			NKMRewardData nKMRewardData;
			if (dungeonClearData.rewardData != null)
			{
				nKMRewardData = dungeonClearData.rewardData.DeepCopy();
			}
			else
			{
				nKMRewardData = new NKMRewardData();
				Debug.LogWarning("Dungeon Reward Data is null !!");
			}
			battleResultData.m_iUnitExp = dungeonClearData.unitExp;
			battleResultData.m_lstUnitLevelupData = MakeUnitLevelupExpData(armyData, nKMRewardData.UnitExpDataList, deckIndex, lstUnitUpdateData);
			battleResultData.m_iUnitExpBonusRate = 0;
			if (nKMRewardData.UnitExpDataList != null && nKMRewardData.UnitExpDataList.Count > 0)
			{
				battleResultData.m_iUnitExpBonusRate = nKMRewardData.UnitExpDataList[0].m_BonusRatio;
			}
			battleResultData.m_RewardData = nKMRewardData;
			if (dungeonClearData.missionReward != null)
			{
				rewardData = dungeonClearData.missionReward.DeepCopy();
			}
			if (dungeonClearData.oneTimeRewards != null)
			{
				battleResultData.m_OnetimeRewardData = dungeonClearData.oneTimeRewards.DeepCopy();
			}
		}
		else if (!flag2)
		{
			battleResultData.m_lstUnitLevelupData = MakeUnitLevelupExpData(armyData, null, deckIndex, lstUnitUpdateData);
			battleResultData.m_iUnitExpBonusRate = 0;
		}
		if (flag3 && firstRewardData != null)
		{
			battleResultData.m_firstRewardData = GetRewardItemAfterFilter(ref battleResultData.m_RewardData, firstRewardData.Type, firstRewardData.RewardId, firstRewardData.RewardQuantity);
		}
		if (flag4 && rewardData != null)
		{
			battleResultData.m_firstAllClearData = GetRewardItemAfterFilter(ref rewardData, nKMStageTempletV.MissionReward.rewardType, nKMStageTempletV.MissionReward.ID, nKMStageTempletV.MissionReward.Count);
		}
		battleResultData.m_battleData = battleData;
		battleResultData.m_multiply = multiplyReward;
		return battleResultData;
	}

	public static NKMRewardData GetRewardItemAfterFilter(ref NKMRewardData rewardData, NKM_REWARD_TYPE rewardType, int rewardID, int rewardCount)
	{
		NKMRewardData nKMRewardData = new NKMRewardData();
		if (rewardData == null)
		{
			return nKMRewardData;
		}
		if (!NKMRewardTemplet.IsValidReward(rewardType, rewardID))
		{
			return nKMRewardData;
		}
		if (!NKMRewardTemplet.IsOpenedReward(rewardType, rewardID, useRandomContract: false))
		{
			return nKMRewardData;
		}
		switch (rewardType)
		{
		case NKM_REWARD_TYPE.RT_EQUIP:
		{
			NKMEquipItemData nKMEquipItemData = rewardData.EquipItemDataList.Find((NKMEquipItemData x) => x.m_ItemEquipID == rewardID);
			if (nKMEquipItemData != null)
			{
				rewardData.EquipItemDataList.Remove(nKMEquipItemData);
				nKMRewardData.EquipItemDataList.Add(nKMEquipItemData);
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_MISC:
		{
			NKMItemMiscData nKMItemMiscData = rewardData.MiscItemDataList.Find((NKMItemMiscData x) => x.ItemID == rewardID && x.TotalCount == rewardCount);
			if (nKMItemMiscData != null)
			{
				rewardData.MiscItemDataList.Remove(nKMItemMiscData);
				nKMRewardData.MiscItemDataList.Add(nKMItemMiscData);
				break;
			}
			NKMInteriorData nKMInteriorData = rewardData.Interiors.Find((NKMInteriorData x) => x.itemId == rewardID && x.count == rewardCount);
			if (nKMInteriorData != null)
			{
				rewardData.Interiors.Remove(nKMInteriorData);
				nKMRewardData.Interiors.Add(nKMInteriorData);
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_MOLD:
		{
			NKMMoldItemData nKMMoldItemData = rewardData.MoldItemDataList.Find((NKMMoldItemData x) => x.m_MoldID == rewardID && x.m_Count == rewardCount);
			if (nKMMoldItemData != null)
			{
				rewardData.MoldItemDataList.Remove(nKMMoldItemData);
				nKMRewardData.MoldItemDataList.Add(nKMMoldItemData);
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_OPERATOR:
		{
			NKMOperator nKMOperator = rewardData.OperatorList.Find((NKMOperator x) => x.id == rewardID);
			if (nKMOperator != null)
			{
				rewardData.OperatorList.Remove(nKMOperator);
				nKMRewardData.OperatorList.Add(nKMOperator);
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_UNIT:
		case NKM_REWARD_TYPE.RT_SHIP:
		{
			NKMUnitData nKMUnitData = rewardData.UnitDataList.Find((NKMUnitData x) => x.m_UnitID == rewardID);
			if (nKMUnitData != null)
			{
				rewardData.UnitDataList.Remove(nKMUnitData);
				nKMRewardData.UnitDataList.Add(nKMUnitData);
			}
			break;
		}
		case NKM_REWARD_TYPE.RT_SKIN:
			if (rewardData.SkinIdList.Contains(rewardID))
			{
				rewardData.SkinIdList.Remove(rewardID);
				nKMRewardData.SkinIdList.Add(rewardID);
			}
			break;
		default:
			Debug.LogError($"해당 타입 GetRewardItemAfterFilter 추가 필요 - {rewardType}");
			break;
		case NKM_REWARD_TYPE.RT_NONE:
			break;
		}
		return nKMRewardData;
	}

	public static BattleResultData MakeRaidResultData(NKMArmyData armyData, int dungeonID, NKMRaidBossResultData cNKMRaidBossResultData, NKCUIBattleStatistics.BattleData battleData)
	{
		BattleResultData battleResultData = new BattleResultData();
		if (cNKMRaidBossResultData == null)
		{
			Debug.LogError("BossResultData is null!! When make Raid Result Data");
			return battleResultData;
		}
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(dungeonID);
		if (dungeonTempletBase == null)
		{
			Debug.LogError("DungeonTemplet Not Found!!");
			return battleResultData;
		}
		BATTLE_RESULT_TYPE bATTLE_RESULT_TYPE = BATTLE_RESULT_TYPE.BRT_LOSE;
		battleResultData.m_RaidBossResultData = cNKMRaidBossResultData;
		if (cNKMRaidBossResultData.curHP <= 0f)
		{
			bATTLE_RESULT_TYPE = BATTLE_RESULT_TYPE.BRT_WIN;
		}
		battleResultData.m_bShowMedal = true;
		battleResultData.m_bShowBonus = dungeonTempletBase.BonusResult;
		battleResultData.m_BATTLE_RESULT_TYPE = bATTLE_RESULT_TYPE;
		battleResultData.m_battleData = battleData;
		return battleResultData;
	}

	public static BattleResultData MakePhaseResultData(int stageID, NKMArmyData armyData, NKMPacket_GAME_END_NOT sPacket, NKCUIBattleStatistics.BattleData battleData, int multiplyReward = 1)
	{
		BattleResultData battleResultData = new BattleResultData();
		NKMDeckIndex deckIndex = sPacket.deckIndex;
		bool win = sPacket.win;
		battleResultData.m_stageID = stageID;
		NKMStageTempletV2 nKMStageTempletV = NKMStageTempletV2.Find(stageID);
		if (nKMStageTempletV == null)
		{
			Debug.LogError("phase clear : StageTemplet Not Found!!");
			return battleResultData;
		}
		NKMPhaseTemplet phaseTemplet = nKMStageTempletV.PhaseTemplet;
		if (phaseTemplet == null)
		{
			Debug.LogError("phase clear : PhaseTemplet  Not Found!!");
			return battleResultData;
		}
		battleResultData.m_NKM_GAME_TYPE = NKM_GAME_TYPE.NGT_PHASE;
		battleResultData.m_lstMissionData = new List<NKCUIResultSubUIDungeon.MissionData>();
		bool flag = true;
		bool flag2 = deckIndex.m_eDeckType == NKM_DECK_TYPE.NDT_FRIEND;
		if (flag2)
		{
			flag = false;
		}
		if (phaseTemplet.IsUsingEventDeck())
		{
			deckIndex = NKMDeckIndex.None;
		}
		battleResultData.m_firstRewardData = null;
		battleResultData.m_firstAllClearData = null;
		if (win)
		{
			battleResultData.m_BATTLE_RESULT_TYPE = BATTLE_RESULT_TYPE.BRT_WIN;
		}
		else
		{
			battleResultData.m_BATTLE_RESULT_TYPE = BATTLE_RESULT_TYPE.BRT_LOSE;
		}
		battleResultData.m_bShowMedal = nKMStageTempletV != null;
		battleResultData.m_bShowBonus = phaseTemplet.BonusResult && win;
		bool flag3 = !NKCScenManager.GetScenManager().GetMyUserData().CheckStageCleared(nKMStageTempletV);
		bool flag4 = flag3;
		bool flag5 = false;
		FirstRewardData firstRewardData = nKMStageTempletV.GetFirstRewardData();
		NKMRewardData rewardData = null;
		if (nKMStageTempletV != null)
		{
			NKMEpisodeTempletV2 episodeTemplet = nKMStageTempletV.EpisodeTemplet;
			if (episodeTemplet != null && episodeTemplet.m_EPCategory == EPISODE_CATEGORY.EC_DAILY)
			{
				flag = false;
			}
			if (firstRewardData != null && win && firstRewardData.RewardId > 0 && flag3)
			{
				flag5 = true;
			}
		}
		NKMPhaseClearData phaseClearData = sPacket.phaseClearData;
		if (!flag3 && phaseClearData != null && (!phaseClearData.missionResult1 || !phaseClearData.missionResult2 || !phaseClearData.missionRewardResult || (phaseClearData != null && phaseClearData.missionReward != null)))
		{
			flag4 = true;
		}
		if (flag)
		{
			NKCUIResultSubUIDungeon.MissionData missionData = new NKCUIResultSubUIDungeon.MissionData();
			missionData.bSuccess = win;
			missionData.eMissionType = DUNGEON_GAME_MISSION_TYPE.DGMT_CLEAR;
			battleResultData.m_lstMissionData.Add(missionData);
			NKCUIResultSubUIDungeon.MissionData missionData2 = new NKCUIResultSubUIDungeon.MissionData();
			missionData2.bSuccess = phaseClearData?.missionResult1 ?? false;
			missionData2.eMissionType = phaseTemplet.m_DGMissionType_1;
			missionData2.iMissionValue = phaseTemplet.m_DGMissionValue_1;
			battleResultData.m_lstMissionData.Add(missionData2);
			NKCUIResultSubUIDungeon.MissionData missionData3 = new NKCUIResultSubUIDungeon.MissionData();
			missionData3.bSuccess = phaseClearData?.missionResult2 ?? false;
			missionData3.eMissionType = phaseTemplet.m_DGMissionType_2;
			missionData3.iMissionValue = phaseTemplet.m_DGMissionValue_2;
			battleResultData.m_lstMissionData.Add(missionData3);
		}
		if (phaseClearData != null)
		{
			NKMRewardData nKMRewardData;
			if (phaseClearData.rewardData != null)
			{
				nKMRewardData = phaseClearData.rewardData.DeepCopy();
			}
			else
			{
				nKMRewardData = new NKMRewardData();
				Debug.LogWarning("Dungeon Reward Data is null !!");
			}
			battleResultData.m_lstUnitLevelupData = MakeUnitLevelupExpData(armyData, nKMRewardData.UnitExpDataList, deckIndex, sPacket.updatedUnits);
			battleResultData.m_iUnitExpBonusRate = 0;
			if (nKMRewardData.UnitExpDataList != null && nKMRewardData.UnitExpDataList.Count > 0)
			{
				battleResultData.m_iUnitExpBonusRate = nKMRewardData.UnitExpDataList[0].m_BonusRatio;
			}
			battleResultData.m_RewardData = nKMRewardData;
			if (phaseClearData.missionReward != null)
			{
				rewardData = phaseClearData.missionReward.DeepCopy();
			}
			if (phaseClearData.oneTimeRewards != null)
			{
				battleResultData.m_OnetimeRewardData = phaseClearData.oneTimeRewards.DeepCopy();
			}
		}
		else if (!flag2)
		{
			battleResultData.m_lstUnitLevelupData = MakeUnitLevelupExpData(armyData, null, deckIndex, sPacket.updatedUnits);
			battleResultData.m_iUnitExpBonusRate = 0;
		}
		NKMDungeonClearData dungeonClearData = sPacket.dungeonClearData;
		if (dungeonClearData != null)
		{
			battleResultData.m_iUnitExp = dungeonClearData.unitExp;
			if (dungeonClearData.rewardData != null)
			{
				if (battleResultData.m_RewardData == null)
				{
					battleResultData.m_RewardData = dungeonClearData.rewardData.DeepCopy();
				}
				else
				{
					battleResultData.m_RewardData.AddRewardDataForRepeatOperation(dungeonClearData.rewardData);
				}
			}
			if (dungeonClearData.missionReward != null)
			{
				if (rewardData == null)
				{
					rewardData = dungeonClearData.missionReward.DeepCopy();
				}
				else
				{
					rewardData.AddRewardDataForRepeatOperation(dungeonClearData.missionReward);
				}
			}
			if (dungeonClearData.oneTimeRewards != null)
			{
				if (battleResultData.m_OnetimeRewardData == null)
				{
					battleResultData.m_OnetimeRewardData = dungeonClearData.oneTimeRewards.DeepCopy();
				}
				else
				{
					battleResultData.m_OnetimeRewardData.AddRewardDataForRepeatOperation(dungeonClearData.oneTimeRewards);
				}
			}
		}
		if (flag5 && firstRewardData != null)
		{
			battleResultData.m_firstRewardData = GetRewardItemAfterFilter(ref battleResultData.m_RewardData, firstRewardData.Type, firstRewardData.RewardId, firstRewardData.RewardQuantity);
		}
		if (flag4 && rewardData != null)
		{
			battleResultData.m_firstAllClearData = GetRewardItemAfterFilter(ref rewardData, nKMStageTempletV.MissionReward.rewardType, nKMStageTempletV.MissionReward.ID, nKMStageTempletV.MissionReward.Count);
		}
		battleResultData.m_battleData = battleData;
		battleResultData.m_multiply = multiplyReward;
		return battleResultData;
	}

	public static BattleResultData MakeShadowResultData(NKMArmyData armyData, NKMDeckIndex deckIndex, int dungeonID, bool bWin, NKMDungeonClearData dungeonClearData, NKMShadowGameResult shadowResult, NKMShadowPalace shadowPalaceData, NKCUIBattleStatistics.BattleData battleData, List<UnitLoyaltyUpdateData> lstUnitUpdateData = null)
	{
		BattleResultData battleResultData = new BattleResultData();
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(dungeonID);
		if (dungeonTempletBase == null)
		{
			Debug.LogError("DungeonTemplet Not Found!!");
			return battleResultData;
		}
		battleResultData.m_NKM_GAME_TYPE = NKM_GAME_TYPE.NGT_SHADOW_PALACE;
		battleResultData.m_lstMissionData = new List<NKCUIResultSubUIDungeon.MissionData>();
		if (dungeonTempletBase.IsUsingEventDeck())
		{
			deckIndex = NKMDeckIndex.None;
		}
		battleResultData.m_firstRewardData = null;
		battleResultData.m_firstAllClearData = null;
		if (bWin)
		{
			battleResultData.m_BATTLE_RESULT_TYPE = BATTLE_RESULT_TYPE.BRT_WIN;
		}
		else
		{
			battleResultData.m_BATTLE_RESULT_TYPE = BATTLE_RESULT_TYPE.BRT_LOSE;
		}
		battleResultData.m_bShadowAllClear = shadowResult.rewardData != null;
		battleResultData.m_ShadowPrevLife = shadowPalaceData.life;
		battleResultData.m_ShadowCurrLife = shadowResult.life;
		battleResultData.m_ShadowBestClearTime = 0;
		battleResultData.m_ShadowCurrClearTime = ((shadowResult.dungeonData != null) ? shadowResult.dungeonData.recentTime : 0);
		NKMPalaceData nKMPalaceData = shadowPalaceData.palaceDataList.Find((NKMPalaceData v) => v.palaceId == shadowPalaceData.currentPalaceId);
		if (nKMPalaceData != null)
		{
			NKMPalaceDungeonData nKMPalaceDungeonData = nKMPalaceData.dungeonDataList.Find((NKMPalaceDungeonData v) => v.dungeonId == dungeonID);
			if (nKMPalaceDungeonData != null)
			{
				battleResultData.m_ShadowBestClearTime = nKMPalaceDungeonData.bestTime;
			}
		}
		battleResultData.m_bShowMedal = false;
		battleResultData.m_bShowBonus = dungeonTempletBase.BonusResult && bWin;
		if (false)
		{
			NKCUIResultSubUIDungeon.MissionData missionData = new NKCUIResultSubUIDungeon.MissionData();
			missionData.bSuccess = bWin;
			missionData.eMissionType = DUNGEON_GAME_MISSION_TYPE.DGMT_CLEAR;
			battleResultData.m_lstMissionData.Add(missionData);
			NKCUIResultSubUIDungeon.MissionData missionData2 = new NKCUIResultSubUIDungeon.MissionData();
			missionData2.bSuccess = dungeonClearData?.missionResult1 ?? false;
			missionData2.eMissionType = dungeonTempletBase.m_DGMissionType_1;
			missionData2.iMissionValue = dungeonTempletBase.m_DGMissionValue_1;
			battleResultData.m_lstMissionData.Add(missionData2);
			NKCUIResultSubUIDungeon.MissionData missionData3 = new NKCUIResultSubUIDungeon.MissionData();
			missionData3.bSuccess = dungeonClearData?.missionResult2 ?? false;
			missionData3.eMissionType = dungeonTempletBase.m_DGMissionType_2;
			missionData3.iMissionValue = dungeonTempletBase.m_DGMissionValue_2;
			battleResultData.m_lstMissionData.Add(missionData3);
		}
		if (dungeonClearData != null)
		{
			NKMRewardData nKMRewardData;
			if (dungeonClearData.rewardData != null)
			{
				nKMRewardData = dungeonClearData.rewardData.DeepCopy();
			}
			else
			{
				nKMRewardData = new NKMRewardData();
				Debug.LogWarning("Dungeon Reward Data is null !!");
			}
			battleResultData.m_iUnitExp = dungeonClearData.unitExp;
			battleResultData.m_lstUnitLevelupData = MakeUnitLevelupExpData(armyData, nKMRewardData.UnitExpDataList, deckIndex, lstUnitUpdateData);
			battleResultData.m_iUnitExpBonusRate = 0;
			if (nKMRewardData.UnitExpDataList != null && nKMRewardData.UnitExpDataList.Count > 0)
			{
				battleResultData.m_iUnitExpBonusRate = nKMRewardData.UnitExpDataList[0].m_BonusRatio;
			}
			battleResultData.m_RewardData = nKMRewardData;
			if (dungeonClearData.oneTimeRewards != null)
			{
				battleResultData.m_OnetimeRewardData = dungeonClearData.oneTimeRewards.DeepCopy();
			}
		}
		else
		{
			battleResultData.m_lstUnitLevelupData = MakeUnitLevelupExpData(armyData, null, deckIndex, lstUnitUpdateData);
			battleResultData.m_iUnitExpBonusRate = 0;
		}
		battleResultData.m_battleData = battleData;
		battleResultData.m_multiply = shadowPalaceData.rewardMultiply;
		return battleResultData;
	}

	public static BattleResultData MakeDefenceResultData(NKMArmyData armyData, NKMDeckIndex deckIndex, int dungeonID, bool bWin, NKMDungeonClearData dungeonClearData, NKMDefenceClearData defenceClearData, NKCUIBattleStatistics.BattleData battleData, List<UnitLoyaltyUpdateData> lstUnitUpdateData = null)
	{
		BattleResultData battleResultData = new BattleResultData();
		if (defenceClearData == null)
		{
			return battleResultData;
		}
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(dungeonID);
		if (dungeonTempletBase == null)
		{
			Debug.LogError("DungeonTemplet Not Found!!");
			return battleResultData;
		}
		if (NKMDefenceTemplet.Find(defenceClearData.defenceTempletId) == null)
		{
			Debug.LogError("DungeonTemplet Not Found!!");
			return battleResultData;
		}
		battleResultData.m_stageID = dungeonClearData.dungeonId;
		battleResultData.m_lstMissionData = new List<NKCUIResultSubUIDungeon.MissionData>();
		bool flag = NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVE_DEFENCE_MEDAL_REWARD);
		bool flag2 = deckIndex.m_eDeckType == NKM_DECK_TYPE.NDT_FRIEND;
		if (flag2)
		{
			flag = false;
		}
		if (deckIndex.m_eDeckType == NKM_DECK_TYPE.NDT_DIVE)
		{
			flag = false;
		}
		if (dungeonTempletBase.IsUsingEventDeck())
		{
			deckIndex = NKMDeckIndex.None;
		}
		NKMStageTempletV2 nKMStageTempletV = NKMEpisodeMgr.FindStageTempletByBattleStrID(dungeonTempletBase.m_DungeonStrID);
		battleResultData.m_firstRewardData = null;
		battleResultData.m_firstAllClearData = null;
		if (bWin)
		{
			battleResultData.m_BATTLE_RESULT_TYPE = BATTLE_RESULT_TYPE.BRT_WIN;
		}
		else
		{
			battleResultData.m_BATTLE_RESULT_TYPE = BATTLE_RESULT_TYPE.BRT_LOSE;
		}
		battleResultData.m_bShowMedal = nKMStageTempletV != null;
		battleResultData.m_bShowBonus = dungeonTempletBase.BonusResult && bWin;
		bool flag3 = false;
		FirstRewardData firstRewardData = nKMStageTempletV?.GetFirstRewardData();
		NKMRewardData rewardData = null;
		if (nKMStageTempletV != null && firstRewardData != null && bWin && firstRewardData.RewardId > 0 && !NKCScenManager.CurrentUserData().CheckDungeonClear(nKMStageTempletV.DungeonTempletBase.m_DungeonID))
		{
			flag3 = true;
		}
		bool flag4;
		if (!(flag4 = !NKCScenManager.GetScenManager().GetMyUserData().CheckDungeonClear(dungeonID)) && dungeonClearData != null && (!dungeonClearData.missionResult1 || !dungeonClearData.missionResult2 || !dungeonClearData.missionRewardResult || (dungeonClearData != null && dungeonClearData.missionReward != null)))
		{
			flag4 = true;
		}
		if (flag)
		{
			NKCUIResultSubUIDungeon.MissionData missionData = new NKCUIResultSubUIDungeon.MissionData();
			missionData.bSuccess = bWin;
			missionData.eMissionType = DUNGEON_GAME_MISSION_TYPE.DGMT_CLEAR;
			battleResultData.m_lstMissionData.Add(missionData);
			NKCUIResultSubUIDungeon.MissionData missionData2 = new NKCUIResultSubUIDungeon.MissionData();
			missionData2.bSuccess = dungeonClearData?.missionResult1 ?? false;
			missionData2.eMissionType = dungeonTempletBase.m_DGMissionType_1;
			missionData2.iMissionValue = dungeonTempletBase.m_DGMissionValue_1;
			battleResultData.m_lstMissionData.Add(missionData2);
			NKCUIResultSubUIDungeon.MissionData missionData3 = new NKCUIResultSubUIDungeon.MissionData();
			missionData3.bSuccess = dungeonClearData?.missionResult2 ?? false;
			missionData3.eMissionType = dungeonTempletBase.m_DGMissionType_2;
			missionData3.iMissionValue = dungeonTempletBase.m_DGMissionValue_2;
			battleResultData.m_lstMissionData.Add(missionData3);
		}
		if (dungeonClearData != null)
		{
			NKMRewardData nKMRewardData;
			if (dungeonClearData.rewardData != null)
			{
				nKMRewardData = dungeonClearData.rewardData.DeepCopy();
			}
			else
			{
				nKMRewardData = new NKMRewardData();
				Debug.LogWarning("Dungeon Reward Data is null !!");
			}
			battleResultData.m_iUnitExp = dungeonClearData.unitExp;
			battleResultData.m_lstUnitLevelupData = MakeUnitLevelupExpData(armyData, nKMRewardData.UnitExpDataList, deckIndex, lstUnitUpdateData);
			battleResultData.m_iUnitExpBonusRate = 0;
			if (nKMRewardData.UnitExpDataList != null && nKMRewardData.UnitExpDataList.Count > 0)
			{
				battleResultData.m_iUnitExpBonusRate = nKMRewardData.UnitExpDataList[0].m_BonusRatio;
			}
			battleResultData.m_RewardData = nKMRewardData;
			if (dungeonClearData.missionReward != null)
			{
				rewardData = dungeonClearData.missionReward.DeepCopy();
			}
			if (dungeonClearData.oneTimeRewards != null)
			{
				battleResultData.m_OnetimeRewardData = dungeonClearData.oneTimeRewards.DeepCopy();
			}
		}
		else if (!flag2)
		{
			battleResultData.m_lstUnitLevelupData = MakeUnitLevelupExpData(armyData, null, deckIndex, lstUnitUpdateData);
			battleResultData.m_iUnitExpBonusRate = 0;
		}
		if (flag3 && firstRewardData != null)
		{
			battleResultData.m_firstRewardData = GetRewardItemAfterFilter(ref battleResultData.m_RewardData, firstRewardData.Type, firstRewardData.RewardId, firstRewardData.RewardQuantity);
		}
		if (flag4 && rewardData != null)
		{
			battleResultData.m_firstAllClearData = GetRewardItemAfterFilter(ref rewardData, nKMStageTempletV.MissionReward.rewardType, nKMStageTempletV.MissionReward.ID, nKMStageTempletV.MissionReward.Count);
		}
		battleResultData.m_battleData = battleData;
		battleResultData.m_multiply = 1;
		return battleResultData;
	}

	public void OpenBattleResult(BattleResultData data, OnClose onClose, bool bAutoSkip = false)
	{
		if (data == null)
		{
			onClose?.Invoke();
			return;
		}
		UnHide();
		dOnClose = onClose;
		base.gameObject.SetActive(value: true);
		ShowBonusType();
		CloseAllSubUI();
		NKCUtil.SetGameobjectActive(m_objShareBtn, bValue: false);
		NKCUtil.SetGameobjectActive(m_objShare, bValue: false);
		if (data.m_BATTLE_RESULT_TYPE == BATTLE_RESULT_TYPE.BRT_WIN)
		{
			SelectTitle(eTitleType.Win);
		}
		else
		{
			SelectTitle(eTitleType.Lose);
		}
		NKCUtil.SetGameobjectActive(m_objUserBuff, NKCScenManager.CurrentUserData().m_companyBuffDataList.Count > 0);
		NKCUtil.SetGameobjectActive(m_objBottomButton, bValue: false);
		NKCUtil.SetGameobjectActive(m_btnBattleStatistics, data.m_battleData != null);
		NKCUtil.SetGameobjectActive(m_btnReplayBattleStatistics, bValue: false);
		NKCUtil.SetGameobjectActive(m_objDoubleToken, bValue: false);
		SetTrimRetryButton(data.m_NKM_GAME_TYPE);
		SetOperationMultiply(data.m_multiply, IsDisplayMultiplyUI());
		SetContractReward();
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME_RESULT)
		{
			if (NKCRepeatOperaion.CheckVisible(NKCScenManager.GetScenManager().Get_NKC_SCEN_GAME_RESULT().GetStageID()))
			{
				NKCRepeatOperaion nKCRepeatOperaion = NKCScenManager.GetScenManager().GetNKCRepeatOperaion();
				if (nKCRepeatOperaion != null)
				{
					NKCUtil.SetGameobjectActive(m_objRepeatOperation, nKCRepeatOperaion.GetIsOnGoing());
					NKCUtil.SetGameobjectActive(m_objRepeatOperationCountDown, bValue: false);
					if (NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing())
					{
						NKCUtil.SetLabelText(m_lbRepeatOperation, $"({nKCRepeatOperaion.GetCurrRepeatCount()}/{nKCRepeatOperaion.GetMaxRepeatCount()})");
					}
				}
			}
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objRepeatOperation, bValue: false);
		}
		if (data.m_battleData != null)
		{
			dOnTouchGameRecord = delegate
			{
				OnClickBattleStatistics(data.m_battleData);
			};
		}
		UIOpened();
		m_uiSubUIMiddle.SetDataBattleResult(data, bAutoSkip ? 2 : 0);
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		List<NKMRewardData> list = new List<NKMRewardData>();
		list.Add(data.m_RewardData);
		list.Add(data.m_OnetimeRewardData);
		SetUnitGetOpenData(list, armyData);
		NKCUIResultSubUIReward.Data data2 = new NKCUIResultSubUIReward.Data();
		data2.rewardData = data.m_RewardData;
		data2.armyData = armyData;
		data2.bAutoSkipUnitGain = bAutoSkip;
		data2.firstRewardData = data.m_firstRewardData;
		data2.firstAllClearRewardData = data.m_firstAllClearData;
		data2.bIgnoreAutoClose = false;
		data2.selectRewardData = null;
		data2.selectSlotText = "";
		data2.bAllowRewardDataNull = false;
		data2.onetimeRewardData = data.m_OnetimeRewardData;
		data2.additionalReward = data.m_additionalReward;
		data2.battleResultType = data.m_BATTLE_RESULT_TYPE;
		m_uiReward.SetData(data2);
		m_uiKillCount.SetData(data);
		m_uiMiscContract.SetData(data.m_RewardData?.ContractList);
		if (data.m_NKM_GAME_TYPE != NKM_GAME_TYPE.NGT_FIERCE)
		{
			m_uiTip.SetData(data.m_BATTLE_RESULT_TYPE);
		}
		m_uiWorldmap.SetData(bBigSuccess: false, null);
		m_uiShadowTime.SetData(data.m_BATTLE_RESULT_TYPE, data.m_ShadowCurrClearTime, data.m_ShadowBestClearTime);
		m_uiShadowLife.SetData(data.m_BATTLE_RESULT_TYPE, data.m_ShadowPrevLife, data.m_ShadowCurrLife);
		NKCUtil.SetGameobjectActive(m_WIN, data.m_NKM_GAME_TYPE != NKM_GAME_TYPE.NGT_FIERCE && data.m_NKM_GAME_TYPE != NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS && data.m_NKM_GAME_TYPE != NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS_PRACTICE);
		NKCUtil.SetGameobjectActive(m_LOSE, data.m_NKM_GAME_TYPE != NKM_GAME_TYPE.NGT_FIERCE && data.m_NKM_GAME_TYPE != NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS && data.m_NKM_GAME_TYPE != NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS_PRACTICE);
		NKCUtil.SetGameobjectActive(m_WIN_BATTLE_REPORT, data.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_FIERCE);
		NKCUtil.SetGameobjectActive(m_LOSE_BATTLE_REPORT, data.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_FIERCE);
		NKCUtil.SetGameobjectActive(m_ASSIST, data.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS || data.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_GUILD_DUNGEON_BOSS_PRACTICE);
		if (data.m_NKM_GAME_TYPE == NKM_GAME_TYPE.NGT_FIERCE)
		{
			m_uiSubUIMiddle.SetDataNull();
			m_ui_FierceBattle.SetData(data);
		}
		m_LastCoroutine = StartCoroutine(ProcessResultUI(bAutoSkip));
	}

	public static BattleResultData MakeFierceResultData(int dungeonID, bool bWin, NKMDungeonClearData dungeonClearData, NKMFierceResultData fierceResult, NKCUIBattleStatistics.BattleData battleData)
	{
		BattleResultData battleResultData = new BattleResultData();
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(dungeonID);
		if (dungeonTempletBase == null)
		{
			Debug.LogError("DungeonTemplet Not Found!!");
			return battleResultData;
		}
		battleResultData.m_NKM_GAME_TYPE = NKM_GAME_TYPE.NGT_FIERCE;
		battleResultData.m_lstMissionData = new List<NKCUIResultSubUIDungeon.MissionData>();
		battleResultData.m_firstRewardData = null;
		battleResultData.m_firstAllClearData = null;
		battleResultData.m_BATTLE_RESULT_TYPE = ((!bWin) ? BATTLE_RESULT_TYPE.BRT_LOSE : BATTLE_RESULT_TYPE.BRT_WIN);
		battleResultData.m_iFierceBestScore = fierceResult.bestPoint;
		battleResultData.m_iFierceScore = fierceResult.accquirePoint;
		battleResultData.m_fFierceLastBossHPPercent = fierceResult.hpPercent;
		battleResultData.m_fFierceRestTime = fierceResult.restTime;
		battleResultData.m_battleData = battleData;
		if (true)
		{
			NKCUIResultSubUIDungeon.MissionData missionData = new NKCUIResultSubUIDungeon.MissionData();
			missionData.bSuccess = bWin;
			missionData.eMissionType = DUNGEON_GAME_MISSION_TYPE.DGMT_CLEAR;
			battleResultData.m_lstMissionData.Add(missionData);
			NKCUIResultSubUIDungeon.MissionData missionData2 = new NKCUIResultSubUIDungeon.MissionData();
			missionData2.bSuccess = dungeonClearData?.missionResult1 ?? false;
			missionData2.eMissionType = dungeonTempletBase.m_DGMissionType_1;
			missionData2.iMissionValue = dungeonTempletBase.m_DGMissionValue_1;
			battleResultData.m_lstMissionData.Add(missionData2);
			NKCUIResultSubUIDungeon.MissionData missionData3 = new NKCUIResultSubUIDungeon.MissionData();
			missionData3.bSuccess = dungeonClearData?.missionResult2 ?? false;
			missionData3.eMissionType = dungeonTempletBase.m_DGMissionType_2;
			missionData3.iMissionValue = dungeonTempletBase.m_DGMissionValue_2;
			battleResultData.m_lstMissionData.Add(missionData3);
		}
		return battleResultData;
	}

	public static BattleResultData MakeGuildCoopArenaResultData(NKMArmyData armyData, int dungeonID, bool bWin, NKMDungeonClearData dungeonClearData, NKMDeckIndex deckIndex, NKCUIBattleStatistics.BattleData battleData)
	{
		BattleResultData battleResultData = new BattleResultData();
		NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(dungeonID);
		if (dungeonTempletBase == null)
		{
			Debug.LogError("DungeonTemplet Not Found!!");
			return battleResultData;
		}
		battleResultData.m_lstMissionData = new List<NKCUIResultSubUIDungeon.MissionData>();
		if (dungeonTempletBase.IsUsingEventDeck())
		{
			deckIndex = NKMDeckIndex.None;
		}
		if (bWin)
		{
			battleResultData.m_BATTLE_RESULT_TYPE = BATTLE_RESULT_TYPE.BRT_WIN;
		}
		else
		{
			battleResultData.m_BATTLE_RESULT_TYPE = BATTLE_RESULT_TYPE.BRT_LOSE;
		}
		battleResultData.m_bShowMedal = true;
		battleResultData.m_bShowBonus = dungeonTempletBase.BonusResult && bWin;
		NKCUIResultSubUIDungeon.MissionData missionData = new NKCUIResultSubUIDungeon.MissionData();
		missionData.bSuccess = bWin;
		missionData.eMissionType = DUNGEON_GAME_MISSION_TYPE.DGMT_CLEAR;
		battleResultData.m_lstMissionData.Add(missionData);
		NKCUIResultSubUIDungeon.MissionData missionData2 = new NKCUIResultSubUIDungeon.MissionData();
		missionData2.bSuccess = dungeonClearData?.missionResult1 ?? false;
		missionData2.eMissionType = dungeonTempletBase.m_DGMissionType_1;
		missionData2.iMissionValue = dungeonTempletBase.m_DGMissionValue_1;
		battleResultData.m_lstMissionData.Add(missionData2);
		NKCUIResultSubUIDungeon.MissionData missionData3 = new NKCUIResultSubUIDungeon.MissionData();
		missionData3.bSuccess = dungeonClearData?.missionResult2 ?? false;
		missionData3.eMissionType = dungeonTempletBase.m_DGMissionType_2;
		missionData3.iMissionValue = dungeonTempletBase.m_DGMissionValue_2;
		battleResultData.m_lstMissionData.Add(missionData3);
		if (dungeonClearData != null)
		{
			battleResultData.m_iUnitExp = dungeonClearData.unitExp;
			NKMRewardData rewardData;
			if (dungeonClearData.rewardData != null)
			{
				rewardData = dungeonClearData.rewardData.DeepCopy();
			}
			else
			{
				rewardData = new NKMRewardData();
				Debug.LogWarning("Dungeon Reward Data is null !!");
			}
			battleResultData.m_RewardData = rewardData;
		}
		else
		{
			battleResultData.m_lstUnitLevelupData = MakeUnitLevelupExpData(armyData, null, deckIndex);
			battleResultData.m_iUnitExpBonusRate = 0;
		}
		battleResultData.m_firstRewardData = null;
		battleResultData.m_firstAllClearData = null;
		battleResultData.m_battleData = battleData;
		battleResultData.m_multiply = 1;
		battleResultData.m_bShowClearPoint = true;
		int num = 0;
		foreach (KeyValuePair<int, GuildDungeonInfoTemplet> item in NKCGuildCoopManager.m_dicGuildDungeonInfoTemplet)
		{
			if (item.Value.GetSeasonDungeonId() == dungeonTempletBase.Key)
			{
				num = item.Key;
				break;
			}
		}
		if (num == 0)
		{
			return battleResultData;
		}
		battleResultData.m_fArenaClearPoint = NKCGuildCoopManager.GetClearPointPercentage(num);
		return battleResultData;
	}

	private bool IsDisplayMultiplyUI()
	{
		bool result = false;
		NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
		if (gameClient != null && gameClient.GetGameData() != null)
		{
			NKMStageTempletV2 nKMStageTempletV = null;
			switch (gameClient.GetGameData().GetGameType())
			{
			case NKM_GAME_TYPE.NGT_WARFARE:
			{
				NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(gameClient.GetGameData().m_WarfareID);
				if (nKMWarfareTemplet != null)
				{
					nKMStageTempletV = nKMWarfareTemplet.StageTemplet;
				}
				break;
			}
			case NKM_GAME_TYPE.NGT_DUNGEON:
			{
				NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(gameClient.GetGameData().m_DungeonID);
				if (dungeonTempletBase != null)
				{
					nKMStageTempletV = dungeonTempletBase.StageTemplet;
				}
				break;
			}
			case NKM_GAME_TYPE.NGT_PHASE:
			case NKM_GAME_TYPE.NGT_TRIM:
				return false;
			case NKM_GAME_TYPE.NGT_SHADOW_PALACE:
				result = NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.SHADOW_PALACE_MULTIPLY);
				break;
			}
			if (nKMStageTempletV != null)
			{
				switch (nKMStageTempletV.EpisodeCategory)
				{
				case EPISODE_CATEGORY.EC_MAINSTREAM:
				case EPISODE_CATEGORY.EC_DAILY:
				case EPISODE_CATEGORY.EC_SIDESTORY:
				case EPISODE_CATEGORY.EC_FIELD:
				case EPISODE_CATEGORY.EC_EVENT:
				case EPISODE_CATEGORY.EC_SUPPLY:
				case EPISODE_CATEGORY.EC_CHALLENGE:
				case EPISODE_CATEGORY.EC_SEASONAL:
					result = true;
					break;
				}
			}
		}
		return result;
	}

	private void ShowBonusType()
	{
		NKCGameClient gameClient = NKCScenManager.GetScenManager().GetGameClient();
		if (gameClient == null)
		{
			return;
		}
		NKMGameData gameData = gameClient.GetGameData();
		if (gameData == null)
		{
			return;
		}
		RewardTuningType type = RewardTuningType.None;
		switch (gameData.GetGameType())
		{
		case NKM_GAME_TYPE.NGT_WARFARE:
		{
			NKMWarfareTemplet nKMWarfareTemplet = NKMWarfareTemplet.Find(gameData.m_WarfareID);
			if (nKMWarfareTemplet != null)
			{
				type = nKMWarfareTemplet.StageTemplet.m_BuffType;
			}
			break;
		}
		case NKM_GAME_TYPE.NGT_PHASE:
		{
			NKMStageTempletV2 stageTemplet = NKCPhaseManager.GetStageTemplet();
			if (stageTemplet != null)
			{
				type = stageTemplet.m_BuffType;
			}
			break;
		}
		case NKM_GAME_TYPE.NGT_DUNGEON:
		{
			NKMDungeonTempletBase dungeonTempletBase = NKMDungeonManager.GetDungeonTempletBase(gameData.m_DungeonID);
			if (dungeonTempletBase != null)
			{
				type = dungeonTempletBase.StageTemplet.m_BuffType;
			}
			break;
		}
		}
		NKCUtil.SetGameobjectActive(m_RESULT_WIN_Bonus_Type, !type.Equals(RewardTuningType.None));
		NKCUtil.SetImageSprite(m_RESULT_WIN_Bonus_Type_icon, NKCUtil.GetBounsTypeIcon(type, big: false));
	}

	public static CityMissionResultData MakeCityMissionCompleteUIData(NKMArmyData armyData, NKMWorldMapCityData cityData, NKMRewardData rewardData, int cityNewExp, int cityNewLevel, bool bGreatSuccess)
	{
		CityMissionResultData cityMissionResultData = new CityMissionResultData();
		cityMissionResultData.m_CityID = cityData.cityID;
		cityMissionResultData.m_CityLevelOld = cityData.level;
		cityMissionResultData.m_CityLevelNew = cityNewLevel;
		cityMissionResultData.m_CityExpOld = cityData.exp;
		cityMissionResultData.m_CityExpNew = cityNewExp;
		cityMissionResultData.m_bGreatSuccess = bGreatSuccess;
		cityMissionResultData.m_LeaderUnitData = armyData.GetUnitFromUID(cityData.leaderUnitUID);
		List<NKMRewardUnitExpData> list = new List<NKMRewardUnitExpData>(rewardData.UnitExpDataList);
		int num = list.FindIndex((NKMRewardUnitExpData x) => x.m_UnitUid == cityData.leaderUnitUID);
		if (num >= 0)
		{
			NKMUnitData unitFromUID = armyData.GetUnitFromUID(cityData.leaderUnitUID);
			NKMRewardUnitExpData unitExpData = list[num];
			cityMissionResultData.LeaderUnitLevelupData = MakeLevelupData(unitFromUID, unitExpData);
		}
		cityMissionResultData.m_lstUnitLevelupData = MakeUnitLevelupExpData(armyData, list, NKMDeckIndex.None);
		cityMissionResultData.m_RewardData = rewardData.DeepCopy();
		if (bGreatSuccess)
		{
			NKMWorldMapMissionTemplet missionTemplet = NKMWorldMapManager.GetMissionTemplet(cityData.worldMapMission.currentMissionID);
			if (missionTemplet != null && missionTemplet.m_CompleteRewardQuantity > 0 && NKMRewardTemplet.IsValidReward(missionTemplet.m_CompleteRewardType, missionTemplet.m_CompleteRewardID))
			{
				NKMRewardData nKMRewardData = new NKMRewardData();
				switch (missionTemplet.m_CompleteRewardType)
				{
				case NKM_REWARD_TYPE.RT_OPERATOR:
				{
					NKMOperator nKMOperator = cityMissionResultData.m_RewardData.OperatorList.Find((NKMOperator v) => v.id == missionTemplet.m_CompleteRewardID);
					if (nKMOperator != null)
					{
						cityMissionResultData.m_RewardData.OperatorList.Remove(nKMOperator);
						nKMRewardData.OperatorList.Add(nKMOperator);
					}
					break;
				}
				case NKM_REWARD_TYPE.RT_UNIT:
				case NKM_REWARD_TYPE.RT_SHIP:
				{
					NKMUnitData nKMUnitData = cityMissionResultData.m_RewardData.UnitDataList.Find((NKMUnitData v) => v.m_UnitID == missionTemplet.m_CompleteRewardID);
					if (nKMUnitData != null)
					{
						cityMissionResultData.m_RewardData.UnitDataList.Remove(nKMUnitData);
						nKMRewardData.UnitDataList.Add(nKMUnitData);
					}
					break;
				}
				case NKM_REWARD_TYPE.RT_MISC:
				{
					NKMItemMiscData nKMItemMiscData = cityMissionResultData.m_RewardData.MiscItemDataList.Find((NKMItemMiscData v) => v.ItemID == missionTemplet.m_CompleteRewardID);
					if (nKMItemMiscData != null)
					{
						cityMissionResultData.m_RewardData.MiscItemDataList.Remove(nKMItemMiscData);
						nKMRewardData.MiscItemDataList.Add(nKMItemMiscData);
					}
					break;
				}
				case NKM_REWARD_TYPE.RT_EQUIP:
				{
					NKMEquipItemData nKMEquipItemData = cityMissionResultData.m_RewardData.EquipItemDataList.Find((NKMEquipItemData v) => v.m_ItemEquipID == missionTemplet.m_CompleteRewardID);
					if (nKMEquipItemData != null)
					{
						cityMissionResultData.m_RewardData.EquipItemDataList.Remove(nKMEquipItemData);
						nKMRewardData.EquipItemDataList.Add(nKMEquipItemData);
					}
					break;
				}
				case NKM_REWARD_TYPE.RT_MOLD:
				{
					NKMMoldItemData nKMMoldItemData = cityMissionResultData.m_RewardData.MoldItemDataList.Find((NKMMoldItemData v) => v.m_MoldID == missionTemplet.m_CompleteRewardID);
					if (nKMMoldItemData != null)
					{
						cityMissionResultData.m_RewardData.MoldItemDataList.Remove(nKMMoldItemData);
						nKMRewardData.MoldItemDataList.Add(nKMMoldItemData);
					}
					break;
				}
				case NKM_REWARD_TYPE.RT_SKIN:
					if (cityMissionResultData.m_RewardData.SkinIdList.Contains(missionTemplet.m_CompleteRewardID))
					{
						cityMissionResultData.m_RewardData.SkinIdList.Remove(missionTemplet.m_CompleteRewardID);
						nKMRewardData.SkinIdList.Add(missionTemplet.m_CompleteRewardID);
					}
					break;
				default:
					Debug.LogError("NKMWorldMapMissionTemplet.m_CompleteRewardType : " + missionTemplet.m_CompleteRewardType.ToString() + " ???");
					break;
				}
				cityMissionResultData.m_SuccessRewardData = nKMRewardData;
				cityMissionResultData.m_SuccessSlotText = NKCUtilString.GET_STRING_WORLDMAP_CITY_MISSION_REWARD_ADD_TEXT;
			}
		}
		return cityMissionResultData;
	}

	public void OpenCityMissionResult(List<CityMissionResultData> lstResultData, OnClose onClose)
	{
		if (lstResultData == null || lstResultData.Count() < 1)
		{
			onClose?.Invoke();
			return;
		}
		UnHide();
		dOnClose = onClose;
		CloseAllSubUI();
		NKCUtil.SetGameobjectActive(m_objShareBtn, bValue: false);
		NKCUtil.SetGameobjectActive(m_objShare, bValue: false);
		SelectTitle(eTitleType.Get);
		base.gameObject.SetActive(value: true);
		NKCUtil.SetGameobjectActive(m_objUserBuff, NKCScenManager.CurrentUserData().m_companyBuffDataList.Count > 0);
		SetOperationMultiply(0);
		SetContractReward();
		HideCommonObjects();
		NKCUtil.SetLabelText(m_lbGetTitle, NKCUtilString.GET_STRING_RESULT_CITY_MISSION);
		NKCUtil.SetGameobjectActive(m_objTitle, !string.IsNullOrEmpty(m_lbGetTitle.text));
		UIOpened();
		m_uiSubUIMiddle.SetDataWorldMapMissionResult(lstResultData, 2f);
		NKMArmyData armyData = NKCScenManager.GetScenManager().GetMyUserData().m_ArmyData;
		NKMRewardData nKMRewardData = new NKMRewardData();
		NKMRewardData selectRewardData = new NKMRewardData();
		foreach (CityMissionResultData lstResultDatum in lstResultData)
		{
			nKMRewardData += lstResultDatum.m_RewardData;
			if (lstResultDatum.m_SuccessRewardData != null)
			{
				selectRewardData += lstResultDatum.m_SuccessRewardData;
			}
		}
		SetUnitGetOpenData(nKMRewardData, armyData);
		NKCUIResultSubUIReward.Data data = new NKCUIResultSubUIReward.Data();
		data.rewardData = nKMRewardData;
		data.armyData = armyData;
		data.bAutoSkipUnitGain = false;
		data.firstRewardData = null;
		data.bIgnoreAutoClose = false;
		data.selectRewardData = selectRewardData;
		data.selectSlotText = NKCUtilString.GET_STRING_WORLDMAP_CITY_MISSION_REWARD_ADD_TEXT;
		m_uiReward.SetData(data);
		m_uiKillCount.SetData(null);
		m_uiMiscContract.SetData(nKMRewardData?.ContractList);
		m_uiTip.SetData(BATTLE_RESULT_TYPE.BRT_WIN);
		m_uiWorldmap.SetData(bBigSuccess: false, null);
		m_uiShadowTime.SetData(BATTLE_RESULT_TYPE.BRT_WIN, 0, 0);
		m_uiShadowLife.SetData(BATTLE_RESULT_TYPE.BRT_WIN, 0, 0);
		m_ui_FierceBattle?.SetData(null);
		m_ui_RearmExtract?.SetData(null, null);
		m_LastCoroutine = StartCoroutine(ProcessResultUI());
	}

	public void OpenMailResult(NKMArmyData armyData, NKMRewardData rewardData, OnClose onClose = null)
	{
		dOnClose = onClose;
		base.gameObject.SetActive(value: true);
		UnHide();
		CloseAllSubUI();
		NKCUtil.SetGameobjectActive(m_objShareBtn, bValue: false);
		NKCUtil.SetGameobjectActive(m_objShare, bValue: false);
		SelectTitle(eTitleType.Get);
		m_uiSubUIMiddle.SetDataNull();
		m_ui_FierceBattle.SetData(null);
		m_ui_RearmExtract?.SetData(null, null);
		SetUnitGetOpenData(rewardData, armyData);
		NKCUIResultSubUIReward.Data data = new NKCUIResultSubUIReward.Data();
		data.rewardData = rewardData;
		data.armyData = armyData;
		data.bAutoSkipUnitGain = false;
		m_uiReward.SetData(data);
		m_uiKillCount.SetData(null);
		m_uiMiscContract.SetData(rewardData?.ContractList);
		m_uiTip.SetData(BATTLE_RESULT_TYPE.BRT_WIN);
		m_uiWorldmap.SetData(bBigSuccess: false, null);
		m_uiShadowTime.SetData(BATTLE_RESULT_TYPE.BRT_WIN, 0, 0);
		m_uiShadowLife.SetData(BATTLE_RESULT_TYPE.BRT_WIN, 0, 0);
		if (!m_uiReward.ProcessRequired && !m_uiMiscContract.ProcessRequired)
		{
			base.gameObject.SetActive(value: false);
			Debug.Log("Mail uiReward Skip");
			onClose?.Invoke();
			return;
		}
		NKCUtil.SetGameobjectActive(m_objUserBuff, NKCScenManager.CurrentUserData().m_companyBuffDataList.Count > 0);
		SetOperationMultiply(0);
		SetContractReward();
		HideCommonObjects();
		NKCUtil.SetLabelText(m_lbGetTitle, NKCUtilString.GET_STRING_MAIL);
		NKCUtil.SetGameobjectActive(m_objTitle, !string.IsNullOrEmpty(m_lbGetTitle.text));
		UIOpened();
		m_LastCoroutine = StartCoroutine(ProcessResultUI());
	}

	public void OpenPrivatePvpResult(NKMArmyData armyData, BattleResultData battleResultData, OnClose onClose, NKCUIBattleStatistics.BattleData battleData)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_objShareBtn, bValue: false);
		dOnClose = onClose;
		UnHide();
		CloseAllSubUI();
		NKCUtil.SetGameobjectActive(m_objShareBtn, bValue: false);
		NKCUtil.SetGameobjectActive(m_objShare, bValue: false);
		SelectTitle(eTitleType.Get);
		base.gameObject.SetActive(value: true);
		m_uiSubUIMiddle.SetDataNull();
		m_ui_FierceBattle.SetData(null);
		m_ui_RearmExtract?.SetData(null, null);
		SetUnitGetOpenData(battleResultData.m_RewardData, armyData);
		NKCUIResultSubUIReward.Data data = new NKCUIResultSubUIReward.Data();
		data.rewardData = battleResultData.m_RewardData;
		data.armyData = armyData;
		data.bAutoSkipUnitGain = false;
		data.firstRewardData = null;
		data.bIgnoreAutoClose = true;
		data.selectRewardData = null;
		data.selectSlotText = "";
		data.bAllowRewardDataNull = true;
		data.additionalReward = null;
		m_uiReward.SetData(data);
		m_uiKillCount.SetData(null);
		m_uiMiscContract.SetData(battleResultData.m_RewardData?.ContractList);
		switch (battleResultData.m_BATTLE_RESULT_TYPE)
		{
		case BATTLE_RESULT_TYPE.BRT_WIN:
			SelectTitle(eTitleType.WinPrivate);
			break;
		case BATTLE_RESULT_TYPE.BRT_DRAW:
			SelectTitle(eTitleType.DrawPrivate);
			break;
		case BATTLE_RESULT_TYPE.BRT_LOSE:
			SelectTitle(eTitleType.LosePrivate);
			break;
		}
		NKCUtil.SetGameobjectActive(m_objUserBuff, NKCScenManager.CurrentUserData().m_companyBuffDataList.Count > 0);
		SetOperationMultiply(0);
		SetContractReward();
		NKCUtil.SetGameobjectActive(m_objBottomButton, bValue: false);
		NKCUtil.SetGameobjectActive(m_objDoubleToken, bValue: false);
		NKCUtil.SetGameobjectActive(m_objRepeatOperation, bValue: false);
		NKCUtil.SetGameobjectActive(m_csbtnTrimRetry, bValue: false);
		NKCUtil.SetLabelText(m_lbDoubleTokenCount, "");
		NKCUtil.SetLabelText(m_lbGetTitle, NKCUtilString.GET_STRING_RESULT_MISSION);
		NKCUtil.SetGameobjectActive(m_objTitle, !string.IsNullOrEmpty(m_lbGetTitle.text));
		if (NKCReplayMgr.IsPlayingReplay())
		{
			NKCReplayMgr.GetNKCReplaMgr()?.StopPlaying();
		}
		if (battleData != null)
		{
			NKCUtil.SetGameobjectActive(m_btnBattleStatistics, battleResultData.m_RewardData != null);
			NKCUtil.SetGameobjectActive(m_btnReplayBattleStatistics, battleResultData.m_RewardData == null);
			dOnTouchGameRecord = delegate
			{
				OnClickBattleStatistics(battleData);
			};
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_btnBattleStatistics, bValue: false);
		}
		UIOpened();
		if (NKCPrivatePVPRoomMgr.LobbyData != null)
		{
			NKCPacketSender.Send_NKMPacket_PRIVATE_PVP_STATE_REQ(LobbyPlayerState.Result);
		}
		m_LastCoroutine = StartCoroutine(ProcessResultUI());
	}

	public void OpenComplexResult(NKMArmyData armyData, NKMRewardData rewardData, OnClose onClose = null, long orgDoubleTokenCount = 0L, NKCUIBattleStatistics.BattleData battleData = null, bool bIgnoreAutoClose = false, bool bAllowRewardDataNull = false)
	{
		OpenComplexResultFull(armyData, rewardData, null, onClose, orgDoubleTokenCount, battleData, bIgnoreAutoClose, bAllowRewardDataNull);
	}

	public void OpenComplexResultFull(NKMArmyData armyData, NKMRewardData rewardData, NKMAdditionalReward additionalReward, OnClose onClose = null, long orgDoubleTokenCount = 0L, NKCUIBattleStatistics.BattleData battleData = null, bool bIgnoreAutoClose = false, bool bAllowRewardDataNull = false)
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: true);
		NKCUtil.SetGameobjectActive(m_objShareBtn, bValue: false);
		dOnClose = onClose;
		UnHide();
		CloseAllSubUI();
		NKCUtil.SetGameobjectActive(m_objShareBtn, bValue: false);
		NKCUtil.SetGameobjectActive(m_objShare, bValue: false);
		SelectTitle(eTitleType.Get);
		base.gameObject.SetActive(value: true);
		m_uiSubUIMiddle.SetDataNull();
		m_ui_FierceBattle.SetData(null);
		m_ui_RearmExtract.SetData(null, null);
		SetUnitGetOpenData(rewardData, armyData);
		NKCUIResultSubUIReward.Data data = new NKCUIResultSubUIReward.Data();
		data.rewardData = rewardData;
		data.armyData = armyData;
		data.bAutoSkipUnitGain = false;
		data.firstRewardData = null;
		data.bIgnoreAutoClose = bIgnoreAutoClose;
		data.selectRewardData = null;
		data.selectSlotText = "";
		data.bAllowRewardDataNull = bAllowRewardDataNull;
		data.additionalReward = additionalReward;
		m_uiReward.SetData(data);
		m_uiKillCount.SetData(null);
		m_uiMiscContract.SetData(rewardData?.ContractList);
		if (orgDoubleTokenCount > 0)
		{
			m_uiReward.SetReservedDoubleTokenAddCount(orgDoubleTokenCount - NKCScenManager.CurrentUserData().m_InventoryData.GetCountMiscItem(301));
		}
		m_uiTip.SetData(BATTLE_RESULT_TYPE.BRT_WIN);
		m_uiWorldmap.SetData(bBigSuccess: false, null);
		m_uiShadowTime.SetData(BATTLE_RESULT_TYPE.BRT_WIN, 0, 0);
		m_uiShadowLife.SetData(BATTLE_RESULT_TYPE.BRT_WIN, 0, 0);
		if (!m_uiReward.ProcessRequired && !m_uiMiscContract.ProcessRequired)
		{
			base.gameObject.SetActive(value: false);
			Debug.Log("PVPResult uiReward Skip");
			onClose?.Invoke();
			return;
		}
		NKCUtil.SetGameobjectActive(m_objUserBuff, NKCScenManager.CurrentUserData().m_companyBuffDataList.Count > 0);
		SetOperationMultiply(0);
		SetContractReward();
		HideCommonObjects();
		if (battleData != null)
		{
			NKCUtil.SetGameobjectActive(m_btnBattleStatistics, !NKCReplayMgr.IsPlayingReplay());
			NKCUtil.SetGameobjectActive(m_btnReplayBattleStatistics, NKCReplayMgr.IsPlayingReplay());
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_btnBattleStatistics, bValue: false);
		}
		NKCUtil.SetLabelText(m_lbDoubleTokenCount, orgDoubleTokenCount.ToString());
		NKCUtil.SetLabelText(m_lbGetTitle, NKCUtilString.GET_STRING_RESULT_MISSION);
		NKCUtil.SetGameobjectActive(m_objTitle, !string.IsNullOrEmpty(m_lbGetTitle.text));
		if (NKCReplayMgr.IsPlayingReplay())
		{
			NKCReplayMgr.GetNKCReplaMgr()?.StopPlaying();
		}
		if (battleData != null)
		{
			dOnTouchGameRecord = delegate
			{
				OnClickBattleStatistics(battleData);
			};
		}
		UIOpened();
		m_LastCoroutine = StartCoroutine(ProcessResultUI());
	}

	public void OpenItemGain(List<NKMItemMiscData> lstItemData, string Title, string subTitle, OnClose onClose = null)
	{
		dOnClose = onClose;
		NKMRewardData nKMRewardData = new NKMRewardData();
		nKMRewardData.SetMiscItemData(lstItemData);
		UnHide();
		CloseAllSubUI();
		NKCUtil.SetGameobjectActive(m_objShareBtn, bValue: false);
		NKCUtil.SetGameobjectActive(m_objShare, bValue: false);
		SelectTitle(eTitleType.Get);
		base.gameObject.SetActive(value: true);
		m_uiSubUIMiddle.SetDataNull();
		m_ui_FierceBattle.SetData(null);
		m_ui_RearmExtract?.SetData(null, null);
		SetUnitGetOpenData(nKMRewardData, NKCScenManager.CurrentUserData().m_ArmyData);
		NKCUIResultSubUIReward.Data data = new NKCUIResultSubUIReward.Data();
		data.rewardData = nKMRewardData;
		data.armyData = null;
		m_uiReward.SetData(data);
		m_uiKillCount.SetData(null);
		m_uiMiscContract.SetData(nKMRewardData?.ContractList);
		m_uiTip.SetData(BATTLE_RESULT_TYPE.BRT_WIN);
		m_uiWorldmap.SetData(bBigSuccess: false, null);
		m_uiShadowTime.SetData(BATTLE_RESULT_TYPE.BRT_WIN, 0, 0);
		m_uiShadowLife.SetData(BATTLE_RESULT_TYPE.BRT_WIN, 0, 0);
		if (!m_uiReward.ProcessRequired && !m_uiMiscContract.ProcessRequired)
		{
			onClose?.Invoke();
			return;
		}
		NKCUtil.SetGameobjectActive(m_objUserBuff, NKCScenManager.CurrentUserData().m_companyBuffDataList.Count > 0);
		HideCommonObjects();
		SetOperationMultiply(0);
		SetContractReward();
		NKCUtil.SetLabelText(m_lbGetTitle, Title);
		NKCUtil.SetGameobjectActive(m_objTitle, !string.IsNullOrEmpty(m_lbGetTitle.text));
		UIOpened();
		m_LastCoroutine = StartCoroutine(ProcessResultUI());
	}

	public void OpenBoxGain(NKMArmyData armyData, NKMRewardData rewardData, int boxItemID, OnClose onClose = null)
	{
		OpenBoxGain(armyData, new List<NKMRewardData> { rewardData }, boxItemID, onClose);
	}

	public void OpenBoxGain(NKMArmyData armyData, List<NKMRewardData> lstRewardData, int boxItemID, OnClose onClose = null)
	{
		NKMItemMiscTemplet itemMiscTempletByID = NKMItemManager.GetItemMiscTempletByID(boxItemID);
		string title = ((itemMiscTempletByID != null) ? itemMiscTempletByID.GetItemName() : "");
		OpenBoxGain(armyData, lstRewardData, title, "", onClose, bDisplayUnitGet: false);
	}

	public void OpenBoxGain(NKMArmyData armyData, List<NKMRewardData> lstRewardData, string Title, OnClose onClose = null)
	{
		OpenBoxGain(armyData, lstRewardData, Title, "", onClose, bDisplayUnitGet: false);
	}

	public void OpenBoxGain(NKMArmyData armyData, NKMRewardData rewardData, string Title, OnClose onClose = null, bool bDisplayUnitGet = true, int requestCount = 1, bool bDefaultSort = true)
	{
		OpenBoxGain(armyData, new List<NKMRewardData> { rewardData }, Title, "", onClose, bDisplayUnitGet, null, requestCount, bDefaultSort);
	}

	public void OpenBoxGain(NKMArmyData armyData, NKMRewardData rewardData, string Title, string subTitle = "", OnClose onClose = null)
	{
		OpenBoxGain(armyData, new List<NKMRewardData> { rewardData }, Title, subTitle, onClose, bDisplayUnitGet: false);
	}

	public void OpenBoxGain(NKMArmyData armyData, List<NKMRewardData> lstRewardData, string Title, string subTitle, OnClose onClose = null, bool bDisplayUnitGet = true, List<NKMAdditionalReward> lstAdditionalRewards = null, int requestCount = 1, bool bDefaultSort = true)
	{
		dOnClose = onClose;
		UnHide();
		CloseAllSubUI();
		SelectTitle(eTitleType.Get);
		NKCUtil.SetGameobjectActive(m_objContractMiscReward, bValue: false);
		base.gameObject.SetActive(value: true);
		if (bDisplayUnitGet)
		{
			foreach (NKMRewardData lstRewardDatum in lstRewardData)
			{
				if (lstRewardDatum.MiscItemDataList != null && lstRewardDatum.MiscItemDataList.Count > 0)
				{
					int num = 0;
					if (num < lstRewardDatum.MiscItemDataList.Count)
					{
						SetContractReward(lstRewardDatum.MiscItemDataList[num]);
						lstRewardDatum.MiscItemDataList.RemoveAt(num);
					}
				}
			}
		}
		List<NKCUISlot.SlotData> list = new List<NKCUISlot.SlotData>();
		HashSet<int> hashSet = new HashSet<int>();
		foreach (NKMRewardData lstRewardDatum2 in lstRewardData)
		{
			list.AddRange(NKCUISlot.MakeSlotDataListFromReward(lstRewardDatum2));
			if (lstRewardDatum2.SkinIdList != null)
			{
				hashSet.UnionWith(lstRewardDatum2.SkinIdList);
			}
		}
		if (lstAdditionalRewards != null)
		{
			foreach (NKMAdditionalReward lstAdditionalReward in lstAdditionalRewards)
			{
				list.AddRange(NKCUISlot.MakeSlotDataListFromReward(lstAdditionalReward));
			}
		}
		m_ui_FierceBattle.SetData(null);
		m_ui_RearmExtract?.SetData(null, null);
		m_uiSubUIMiddle.SetDataNull();
		if (bDisplayUnitGet)
		{
			SetUnitGetOpenData(lstRewardData, armyData, bSkipDuplicateUnitGain: false, bDefaultSort);
		}
		else
		{
			m_lstUnitGainRewardData = null;
		}
		m_uiReward.SetBoxGainData(list);
		m_uiMiscContract.SetData(lstRewardData);
		m_uiTip.SetData(BATTLE_RESULT_TYPE.BRT_WIN);
		m_uiWorldmap.SetData(bBigSuccess: false, null);
		m_uiShadowTime.SetData(BATTLE_RESULT_TYPE.BRT_WIN, 0, 0);
		m_uiShadowLife.SetData(BATTLE_RESULT_TYPE.BRT_WIN, 0, 0);
		if (!m_uiReward.ProcessRequired && !m_uiMiscContract.ProcessRequired)
		{
			base.gameObject.SetActive(value: false);
			onClose?.Invoke();
			return;
		}
		NKCUtil.SetLabelText(m_lbGetTitle, Title);
		NKCUtil.SetGameobjectActive(m_objTitle, !string.IsNullOrEmpty(m_lbGetTitle.text));
		NKCUtil.SetGameobjectActive(m_objUserBuff, NKCScenManager.CurrentUserData().m_companyBuffDataList.Count > 0);
		HideCommonObjects();
		SetOperationMultiply(0);
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		NKCUtil.SetLabelText(m_lbLevel, NKCStringTable.GetString("SI_DP_LEVEL_ONE_PARAM"), nKMUserData.m_UserLevel);
		NKCUtil.SetLabelText(m_lbUserName, nKMUserData.m_UserNickName);
		NKCUtil.SetLabelText(m_lbUserUID, NKCUtilString.GetFriendCode(nKMUserData.m_FriendCode));
		bool bValue = NKCPublisherModule.Marketing.IsUseSnsShareOn10SeqContract();
		NKCUtil.SetGameobjectActive(m_objShareBtn, bValue);
		NKCUtil.SetGameobjectActive(m_objShare, bValue: false);
		UIOpened();
		m_LastCoroutine = StartCoroutine(ProcessResultUI());
	}

	public void OpenRewardGain(NKMArmyData armyData, NKMRewardData rewardData, string Title, string subTitle = "", OnClose onClose = null)
	{
		OpenRewardGain(armyData, rewardData, null, Title, subTitle, onClose);
	}

	public void OpenRewardGain(NKMArmyData armyData, NKMRewardData rewardData, NKMAdditionalReward additionalReward, string Title, string subTitle = "", OnClose onClose = null)
	{
		dOnClose = onClose;
		UnHide();
		CloseAllSubUI();
		NKCUtil.SetGameobjectActive(m_objShareBtn, bValue: false);
		NKCUtil.SetGameobjectActive(m_objShare, bValue: false);
		SelectTitle(eTitleType.Get);
		base.gameObject.SetActive(value: true);
		m_ui_FierceBattle.SetData(null);
		m_ui_RearmExtract?.SetData(null, null);
		m_uiSubUIMiddle.SetDataNull();
		SetUnitGetOpenData(rewardData, armyData);
		NKCUIResultSubUIReward.Data data = new NKCUIResultSubUIReward.Data();
		data.rewardData = rewardData;
		data.armyData = armyData;
		data.additionalReward = additionalReward;
		m_uiReward.SetData(data);
		m_uiKillCount.SetData(null);
		m_uiMiscContract.SetData(rewardData?.ContractList);
		m_uiTip.SetData(BATTLE_RESULT_TYPE.BRT_WIN);
		m_uiWorldmap.SetData(bBigSuccess: false, null);
		m_uiShadowTime.SetData(BATTLE_RESULT_TYPE.BRT_WIN, 0, 0);
		m_uiShadowLife.SetData(BATTLE_RESULT_TYPE.BRT_WIN, 0, 0);
		if (!m_uiReward.ProcessRequired && !m_uiMiscContract.ProcessRequired)
		{
			base.gameObject.SetActive(value: false);
			onClose?.Invoke();
			return;
		}
		NKCUtil.SetGameobjectActive(m_objUserBuff, NKCScenManager.CurrentUserData().m_companyBuffDataList.Count > 0);
		HideCommonObjects();
		SetOperationMultiply(0);
		SetContractReward();
		NKCUtil.SetLabelText(m_lbGetTitle, Title);
		NKCUtil.SetGameobjectActive(m_objTitle, !string.IsNullOrEmpty(m_lbGetTitle.text));
		UIOpened();
		m_LastCoroutine = StartCoroutine(ProcessResultUI());
	}

	public void OpenRewardRearmExtract(NKMRewardData rewardData, NKMRewardData synergyRewardData, string Title, string subTitle = "", OnClose onClose = null)
	{
		dOnClose = onClose;
		UnHide();
		CloseAllSubUI();
		NKCUtil.SetGameobjectActive(m_objShareBtn, bValue: false);
		NKCUtil.SetGameobjectActive(m_objShare, bValue: false);
		SelectTitle(eTitleType.Get);
		base.gameObject.SetActive(value: true);
		m_ui_FierceBattle?.SetData(null);
		m_ui_RearmExtract.SetData(rewardData, synergyRewardData);
		m_uiSubUIMiddle.SetDataNull();
		SetUnitGetOpenData(rewardData, NKCScenManager.CurrentUserData().m_ArmyData);
		m_uiReward.SetData(null);
		m_uiKillCount.SetData(null);
		m_uiMiscContract.SetData(rewardData?.ContractList);
		m_uiTip.SetData(BATTLE_RESULT_TYPE.BRT_WIN);
		m_uiWorldmap.SetData(bBigSuccess: false, null);
		m_uiShadowTime.SetData(BATTLE_RESULT_TYPE.BRT_WIN, 0, 0);
		m_uiShadowLife.SetData(BATTLE_RESULT_TYPE.BRT_WIN, 0, 0);
		if (!m_ui_RearmExtract.ProcessRequired && !m_uiMiscContract.ProcessRequired)
		{
			base.gameObject.SetActive(value: false);
			onClose?.Invoke();
			return;
		}
		NKCUtil.SetGameobjectActive(m_objUserBuff, bValue: false);
		HideCommonObjects();
		SetOperationMultiply(0);
		SetContractReward();
		NKCUtil.SetLabelText(m_lbGetTitle, Title);
		NKCUtil.SetGameobjectActive(m_objTitle, !string.IsNullOrEmpty(m_lbGetTitle.text));
		UIOpened();
		m_LastCoroutine = StartCoroutine(ProcessResultUI());
	}

	public void OpenRewardGainWithUnitSD(NKMArmyData armyData, NKMRewardData rewardData, NKMAdditionalReward additionalReward, int unitID, int skinID, string Title, string subTitle = "", OnClose onClose = null)
	{
		dOnClose = onClose;
		UnHide();
		CloseAllSubUI();
		NKCUtil.SetGameobjectActive(m_objShareBtn, bValue: false);
		NKCUtil.SetGameobjectActive(m_objShare, bValue: false);
		SelectTitle(eTitleType.Get);
		base.gameObject.SetActive(value: true);
		m_ui_FierceBattle.SetData(null);
		m_ui_RearmExtract?.SetData(null, null);
		m_uiSubUIMiddle.SetDataNull();
		SetUnitGetOpenData(rewardData, armyData);
		NKCUIResultSubUIReward.Data data = new NKCUIResultSubUIReward.Data();
		data.rewardData = rewardData;
		data.armyData = armyData;
		data.additionalReward = additionalReward;
		m_uiReward.SetData(data);
		m_uiKillCount.SetData(null);
		m_uiMiscContract.SetData(rewardData?.ContractList);
		m_uiTip.SetData(BATTLE_RESULT_TYPE.BRT_WIN);
		m_uiWorldmap.SetData(bBigSuccess: true, unitID, skinID);
		m_uiShadowTime.SetData(BATTLE_RESULT_TYPE.BRT_WIN, 0, 0);
		m_uiShadowLife.SetData(BATTLE_RESULT_TYPE.BRT_WIN, 0, 0);
		if (!m_uiReward.ProcessRequired && !m_uiMiscContract.ProcessRequired)
		{
			base.gameObject.SetActive(value: false);
			onClose?.Invoke();
			return;
		}
		NKCUtil.SetGameobjectActive(m_objUserBuff, NKCScenManager.CurrentUserData().m_companyBuffDataList.Count > 0);
		HideCommonObjects();
		SetOperationMultiply(0);
		SetContractReward();
		NKCUtil.SetLabelText(m_lbGetTitle, Title);
		NKCUtil.SetGameobjectActive(m_objTitle, !string.IsNullOrEmpty(m_lbGetTitle.text));
		UIOpened();
		m_LastCoroutine = StartCoroutine(ProcessResultUI());
	}

	public static List<NKCUIResultSubUIUnitExp.UnitLevelupUIData> MakeUnitLevelupExpData(NKMArmyData armyData, IReadOnlyList<NKMRewardUnitExpData> lstRewardUnitExpData, NKMDeckIndex deckIndex, List<UnitLoyaltyUpdateData> lstUnitUpdateData = null)
	{
		if (deckIndex.m_eDeckType == NKM_DECK_TYPE.NDT_TRIM)
		{
			return new List<NKCUIResultSubUIUnitExp.UnitLevelupUIData>();
		}
		List<NKCUIResultSubUIUnitExp.UnitLevelupUIData> list = new List<NKCUIResultSubUIUnitExp.UnitLevelupUIData>();
		Dictionary<long, UnitLoyaltyUpdateData> dictionary = new Dictionary<long, UnitLoyaltyUpdateData>();
		if (lstUnitUpdateData != null)
		{
			foreach (UnitLoyaltyUpdateData lstUnitUpdateDatum in lstUnitUpdateData)
			{
				dictionary.Add(lstUnitUpdateDatum.unitUid, lstUnitUpdateDatum);
			}
		}
		NKMDeckData deckData = armyData.GetDeckData(deckIndex);
		if (deckData != null)
		{
			foreach (long UID in deckData.m_listDeckUnitUID)
			{
				NKMUnitData unitFromUID = armyData.GetUnitFromUID(UID);
				if (unitFromUID == null)
				{
					continue;
				}
				NKMRewardUnitExpData nKMRewardUnitExpData = null;
				if (lstRewardUnitExpData != null)
				{
					nKMRewardUnitExpData = lstRewardUnitExpData.FirstOrDefault((NKMRewardUnitExpData x) => x.m_UnitUid == UID);
				}
				if (nKMRewardUnitExpData == null)
				{
					nKMRewardUnitExpData = new NKMRewardUnitExpData
					{
						m_UnitUid = UID,
						m_Exp = 0,
						m_BonusExp = 0
					};
				}
				int newLoyalty = -1;
				if (dictionary.TryGetValue(UID, out var value))
				{
					newLoyalty = value.loyalty;
				}
				NKCUIResultSubUIUnitExp.UnitLevelupUIData item = MakeLevelupData(unitFromUID, nKMRewardUnitExpData, newLoyalty);
				list.Add(item);
			}
		}
		else if (lstRewardUnitExpData != null)
		{
			foreach (NKMRewardUnitExpData lstRewardUnitExpDatum in lstRewardUnitExpData)
			{
				NKMUnitData unitFromUID2 = armyData.GetUnitFromUID(lstRewardUnitExpDatum.m_UnitUid);
				if (unitFromUID2 != null)
				{
					int newLoyalty2 = -1;
					if (dictionary.TryGetValue(unitFromUID2.m_UnitUID, out var value2))
					{
						newLoyalty2 = value2.loyalty;
					}
					NKCUIResultSubUIUnitExp.UnitLevelupUIData item2 = MakeLevelupData(unitFromUID2, lstRewardUnitExpDatum, newLoyalty2);
					list.Add(item2);
				}
			}
		}
		else
		{
			foreach (KeyValuePair<long, UnitLoyaltyUpdateData> item4 in dictionary)
			{
				NKMUnitData unitFromUID3 = armyData.GetUnitFromUID(item4.Key);
				if (unitFromUID3 != null)
				{
					int loyalty = item4.Value.loyalty;
					NKMRewardUnitExpData unitExpData = new NKMRewardUnitExpData
					{
						m_UnitUid = item4.Key,
						m_Exp = 0,
						m_BonusExp = 0
					};
					NKCUIResultSubUIUnitExp.UnitLevelupUIData item3 = MakeLevelupData(unitFromUID3, unitExpData, loyalty);
					list.Add(item3);
				}
			}
		}
		return list;
	}

	private static NKCUIResultSubUIUnitExp.UnitLevelupUIData MakeLevelupData(NKMUnitData unitData, NKMRewardUnitExpData unitExpData, int newLoyalty = -1)
	{
		NKCUIResultSubUIUnitExp.UnitLevelupUIData unitLevelupUIData = new NKCUIResultSubUIUnitExp.UnitLevelupUIData();
		unitLevelupUIData.m_bIsLeader = false;
		unitLevelupUIData.m_UnitData = unitData;
		unitLevelupUIData.m_iExpOld = unitData.m_iUnitLevelEXP;
		unitLevelupUIData.m_iLevelOld = unitData.m_UnitLevel;
		NKCExpManager.CalculateFutureUnitExpAndLevel(unitData, unitExpData.m_Exp + unitExpData.m_BonusExp, out unitLevelupUIData.m_iLevelNew, out unitLevelupUIData.m_iExpNew);
		unitLevelupUIData.m_iTotalExpGain = unitExpData.m_Exp + unitExpData.m_BonusExp;
		NKCUIResultSubUIUnitExp.UNIT_LOYALTY loyalty = NKCUIResultSubUIUnitExp.UNIT_LOYALTY.None;
		if (newLoyalty >= 0)
		{
			if (unitData.loyalty < newLoyalty)
			{
				loyalty = NKCUIResultSubUIUnitExp.UNIT_LOYALTY.Up;
			}
			else if (unitData.loyalty > newLoyalty)
			{
				loyalty = NKCUIResultSubUIUnitExp.UNIT_LOYALTY.Down;
			}
		}
		unitLevelupUIData.m_loyalty = loyalty;
		return unitLevelupUIData;
	}

	private void GetUnitProcessEnd()
	{
		m_bWaitForUnitGain = false;
	}

	private IEnumerator ProcessResultUI(bool bAutoSkip = false)
	{
		if (NKCScenManager.GetScenManager().GetNKCPowerSaveMode().GetEnable() && NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing())
		{
			Close();
			yield break;
		}
		m_bPause = false;
		m_bUserLevelUpPopupOpened = false;
		OpenTitleAndBackground();
		yield return WaitTimeOrUserInput(UI_TITLE_ANI_END_DELAY);
		NKCUtil.SetGameobjectActive(m_objBottomButton, bValue: true);
		if (m_uiMiscContract.ProcessRequired)
		{
			while (m_uiMiscContract.WillProcess())
			{
				yield return ProcessSubUI(m_uiMiscContract, bAutoSkip);
				while (m_bPause)
				{
					yield return null;
				}
				while (!m_uiMiscContract.IsProcessFinished())
				{
					yield return null;
				}
				m_uiMiscContract.CleanUpData();
			}
			m_uiMiscContract.Close();
		}
		List<NKCUIResultSubUIBase> list = new List<NKCUIResultSubUIBase> { m_uiWorldmap, m_uiSubUIMiddle };
		foreach (NKCUIResultSubUIBase subUI in list)
		{
			if (subUI.ProcessRequired)
			{
				yield return ProcessSubUI(subUI, bAutoSkip);
				while (m_bPause)
				{
					yield return null;
				}
				while (!subUI.IsProcessFinished())
				{
					yield return null;
				}
				subUI.Close();
			}
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_OPERATION && NKCContentManager.CheckLevelChanged() && !m_bUserLevelUpPopupOpened)
		{
			NKMUserData myUserData = NKCScenManager.GetScenManager().GetMyUserData();
			NKCPopupUserLevelUp.instance.Open(myUserData, OnCloseUserLevelUpPopup);
			m_bUserLevelUpPopupOpened = true;
			while (m_bUserLevelUpPopupOpened)
			{
				yield return null;
			}
		}
		if (m_lstUnitGainRewardData != null && m_lstUnitGainRewardData.Count > 0)
		{
			m_bWaitForUnitGain = true;
			yield return null;
			if (NKCUIGameResultGetUnit.Instance == null)
			{
				Log.Error("NKCUIResult - NKCUIGameResultGetUnit.Instance is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/NKCUIResult.cs", 2963);
				yield break;
			}
			NKCUIGameResultGetUnit.ShowNewUnitGetUI(m_lstUnitGainRewardData, GetUnitProcessEnd, bAutoSkip, m_bDefaultSort, m_bSkipDuplicateUnitGain);
		}
		while (m_bWaitForUnitGain)
		{
			yield return null;
		}
		List<NKCUIResultSubUIBase> obj = new List<NKCUIResultSubUIBase> { m_uiReward, m_uiTip, m_uiShadowTime, m_uiShadowLife, m_uiTrim, m_ui_FierceBattle, m_uiKillCount, m_ui_RearmExtract };
		List<NKCUIResultSubUIBase> lstProcessedSet = new List<NKCUIResultSubUIBase>();
		foreach (NKCUIResultSubUIBase item in obj)
		{
			if (item.ProcessRequired)
			{
				lstProcessedSet.Add(item);
			}
		}
		for (int i = 0; i < lstProcessedSet.Count; i++)
		{
			bool bCountdownRequired = bAutoSkip && i == lstProcessedSet.Count - 1;
			NKCUIResultSubUIBase subUI = lstProcessedSet[i];
			subUI.SetReserveCountdown(bCountdownRequired);
			yield return ProcessSubUI(subUI, bAutoSkip);
			while (m_bPause)
			{
				yield return null;
			}
			while (!subUI.IsProcessFinished())
			{
				yield return null;
			}
			if (bCountdownRequired)
			{
				yield return ProcessCountDown();
			}
			subUI.Close();
		}
		bool flag = false;
		foreach (NKCUIResultSubUIBase item2 in GetSubUIEnumerator())
		{
			if (item2.ProcessRequired)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			while (m_bPause)
			{
				yield return null;
			}
			if (!bAutoSkip)
			{
				yield return WaitTimeOrUserInput();
			}
		}
		yield return new WaitForSeconds(NoSkipSecond);
		while (NKCPopupSnsShareMenu.Instance.IsOpen)
		{
			yield return new WaitForSeconds(3f);
		}
		yield return CloseTitleAndBackground();
		Close();
	}

	private IEnumerator ProcessCountDown()
	{
		float m_fWaitTimeForCloseAnimation = 1f;
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GAME_RESULT && NKCRepeatOperaion.CheckVisible(NKCScenManager.GetScenManager().Get_NKC_SCEN_GAME_RESULT().GetStageID()) && NKCScenManager.GetScenManager().GetNKCRepeatOperaion().GetIsOnGoing())
		{
			m_fWaitTimeForCloseAnimation = 5f;
			NKCUtil.SetGameobjectActive(m_objRepeatOperationCountDown, bValue: true);
			NKCUtil.SetLabelText(m_lbRepeatOperationCountDown, Mathf.CeilToInt(m_fWaitTimeForCloseAnimation).ToString());
			m_imgRepeatOperationCountDown.fillAmount = 0f;
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objRepeatOperationCountDown, bValue: false);
		}
		float currentTime = 0f;
		m_bProcessingCountDown = true;
		while (m_fWaitTimeForCloseAnimation > currentTime)
		{
			if (m_bHadUserInput)
			{
				currentTime += 1f;
				m_bHadUserInput = false;
			}
			if (!m_bPause)
			{
				currentTime += Time.deltaTime;
			}
			float num = m_fWaitTimeForCloseAnimation - currentTime;
			if (m_objRepeatOperationCountDown.activeSelf)
			{
				int num2 = Mathf.CeilToInt(num);
				NKCUtil.SetLabelText(m_lbRepeatOperationCountDown, num2.ToString());
				m_imgRepeatOperationCountDown.fillAmount = (float)num2 - num;
			}
			yield return null;
		}
		m_bProcessingCountDown = false;
		if (m_amtorRepeatOperation.gameObject.activeSelf)
		{
			m_amtorRepeatOperation.Play("NKM_UI_RESULT_REPEAT_OUTRO");
		}
	}

	private void OpenTitleAndBackground()
	{
		NKCUtil.SetGameobjectActive(m_aniTitleWin.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_aniTitleLose.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_aniTitleGet.gameObject, bValue: false);
		NKCUtil.SetGameobjectActive(m_aniTitleWin.gameObject, m_eCurrentTitleType == eTitleType.Win);
		NKCUtil.SetGameobjectActive(m_aniTitleLose.gameObject, m_eCurrentTitleType == eTitleType.Lose);
		NKCUtil.SetGameobjectActive(m_aniTitleGet.gameObject, m_eCurrentTitleType == eTitleType.Get);
		if (m_aniTitleReplay != null)
		{
			NKCUtil.SetGameobjectActive(m_aniTitleReplay.gameObject, m_eCurrentTitleType == eTitleType.Replay);
		}
		if (m_aniTitleWinPrivate != null)
		{
			NKCUtil.SetGameobjectActive(m_aniTitleWinPrivate.gameObject, m_eCurrentTitleType == eTitleType.WinPrivate);
		}
		if (m_aniTitleLosePrivate != null)
		{
			NKCUtil.SetGameobjectActive(m_aniTitleLosePrivate.gameObject, m_eCurrentTitleType == eTitleType.LosePrivate);
		}
		if (m_aniTitleDrawPrivate != null)
		{
			NKCUtil.SetGameobjectActive(m_aniTitleDrawPrivate.gameObject, m_eCurrentTitleType == eTitleType.DrawPrivate);
		}
	}

	private IEnumerator CloseTitleAndBackground()
	{
		NKCUtil.SetGameobjectActive(m_objBottomButton, bValue: false);
		NKCUtil.SetGameobjectActive(m_btnBattleStatistics, bValue: false);
		NKCUtil.SetGameobjectActive(m_btnReplayBattleStatistics, bValue: false);
		NKCUtil.SetGameobjectActive(m_RESULT_WIN_Bonus_Type, bValue: false);
		NKCUtil.SetGameobjectActive(m_csbtnTrimRetry, bValue: false);
		if (null != m_uiCurrentPlayingTitleAni)
		{
			m_uiCurrentPlayingTitleAni.Play("OUTRO");
		}
		yield return null;
		yield return WaitTimeOrUserInput(m_uiCurrentPlayingTitleAni.GetCurrentAnimatorStateInfo(0).length / m_uiCurrentPlayingTitleAni.GetCurrentAnimatorStateInfo(0).speedMultiplier);
	}

	private IEnumerator ProcessSubUI(NKCUIResultSubUIBase subUI, bool bAutoSkip)
	{
		if (!(subUI == null))
		{
			m_bHadUserInput = false;
			NKCUtil.SetGameobjectActive(subUI.gameObject, bValue: true);
			yield return subUI.Process(bAutoSkip);
			yield return null;
		}
	}

	private IEnumerator WaitTimeOrUserInput(float waitTime = 5f)
	{
		float currentTime = 0f;
		m_bHadUserInput = false;
		if (waitTime == 0f)
		{
			yield break;
		}
		if (waitTime < 0f)
		{
			while (!m_bHadUserInput)
			{
				yield return null;
			}
			yield break;
		}
		while (currentTime < waitTime)
		{
			currentTime += Time.deltaTime;
			if (!m_bHadUserInput)
			{
				yield return null;
				continue;
			}
			break;
		}
	}

	public override void OnBackButton()
	{
		OnClickContinue();
	}

	public override void CloseInternal()
	{
		base.gameObject.SetActive(value: false);
		if (m_LastCoroutine != null)
		{
			StopCoroutine(m_LastCoroutine);
			m_LastCoroutine = null;
		}
		NKCUIGameResultGetUnit.CheckInstanceAndClose();
		NKCUIBattleStatistics.CheckInstanceAndClose();
		dOnTouchGameRecord = null;
		m_lstUnitGainRewardData = null;
		if (m_crtShare != null)
		{
			StopCoroutine(m_crtShare);
			m_crtShare = null;
		}
		if (dOnClose != null)
		{
			dOnClose();
		}
	}

	public void OnCloseUserLevelUpPopup()
	{
		m_bUserLevelUpPopupOpened = false;
		NKCContentManager.SetLevelChanged(bValue: false);
	}

	public void SetPause(bool bSet)
	{
		m_bPause = bSet;
		foreach (NKCUIResultSubUIBase item in GetSubUIEnumerator())
		{
			item.SetPause(bSet);
		}
	}

	private void SetUnitGetOpenData(NKMRewardData rewardData, NKMArmyData armyData, bool bSkipDuplicateUnitGain = true)
	{
		List<NKMRewardData> list = new List<NKMRewardData>();
		list.Add(rewardData);
		SetUnitGetOpenData(list, armyData, bSkipDuplicateUnitGain);
	}

	private void SetUnitGetOpenData(List<NKMRewardData> lstRewardData, NKMArmyData armyData, bool bSkipDuplicateUnitGain = true, bool bDefaultSort = true)
	{
		m_lstUnitGainRewardData = lstRewardData;
		m_bSkipDuplicateUnitGain = bSkipDuplicateUnitGain;
		m_bDefaultSort = bDefaultSort;
		if (armyData == null)
		{
			return;
		}
		foreach (NKMRewardData lstRewardDatum in lstRewardData)
		{
			if (lstRewardDatum == null)
			{
				continue;
			}
			if (lstRewardDatum.UnitDataList != null && lstRewardDatum.UnitDataList.Count > 0)
			{
				foreach (NKMUnitData unitData in lstRewardDatum.UnitDataList)
				{
					if (armyData.IsFirstGetUnit(unitData.m_UnitID))
					{
						NKCUIGameResultGetUnit.AddFirstGetUnit(unitData.m_UnitID);
					}
				}
			}
			if (lstRewardDatum.OperatorList == null || lstRewardDatum.OperatorList.Count <= 0)
			{
				continue;
			}
			foreach (NKMOperator @operator in lstRewardDatum.OperatorList)
			{
				if (armyData.IsFirstGetUnit(@operator.id))
				{
					NKCUIGameResultGetUnit.AddFirstGetUnit(@operator.id);
				}
			}
		}
	}

	private void SetOperationMultiply(int multiply, bool bShow = false)
	{
		if (bShow)
		{
			NKCUtil.SetGameobjectActive(m_objOperationMultiply, multiply > 1);
			NKCUtil.SetLabelText(m_txtOperationMultiply, NKCUtilString.GET_MULTIPLY_REWARD_RESULT_ONE_PARAM, multiply);
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objOperationMultiply, bValue: false);
		}
	}

	private void SetContractReward(NKMItemMiscData ContractMiscReward = null)
	{
		if (ContractMiscReward != null)
		{
			NKMItemMiscTemplet nKMItemMiscTemplet = NKMItemMiscTemplet.Find(ContractMiscReward.ItemID);
			if (nKMItemMiscTemplet != null)
			{
				NKCUtil.SetGameobjectActive(m_objContractMiscReward, bValue: true);
				NKCUtil.SetImageSprite(m_ImgContractMiscRewardIcon, NKCResourceUtility.GetOrLoadMiscItemSmallIcon(nKMItemMiscTemplet));
				NKCUtil.SetLabelText(m_lbContractMiscReward, string.Format(NKCUtilString.GET_STRING_CONTRACT_MISC_REWARD_DESC_01, nKMItemMiscTemplet.GetItemName(), ContractMiscReward.TotalCount));
				return;
			}
		}
		NKCUtil.SetGameobjectActive(m_objContractMiscReward, bValue: false);
	}

	private void SetTrimRetryButton(NKM_GAME_TYPE gameType)
	{
		bool flag = false;
		if (gameType == NKM_GAME_TYPE.NGT_TRIM)
		{
			NKMTrimIntervalTemplet nKMTrimIntervalTemplet = NKMTrimIntervalTemplet.IntervalList.FirstOrDefault(delegate(NKMTrimIntervalTemplet e)
			{
				NKMIntervalTemplet nKMIntervalTemplet = NKMIntervalTemplet.Find(e.DateStrId);
				if (nKMIntervalTemplet == null)
				{
					return false;
				}
				return nKMIntervalTemplet.IsValidTime(NKCSynchronizedTime.ServiceTime) ? true : false;
			});
			flag = nKMTrimIntervalTemplet != null && !nKMTrimIntervalTemplet.IsResetUnLimit;
		}
		NKCUtil.SetGameobjectActive(m_csbtnTrimRetry, gameType == NKM_GAME_TYPE.NGT_TRIM && flag);
		if (m_csbtnTrimRetry != null)
		{
			m_csbtnTrimRetry.UnLock();
		}
	}

	public override bool OnHotkey(HotkeyEventType hotkey)
	{
		switch (hotkey)
		{
		case HotkeyEventType.Confirm:
			OnClickContinue();
			return true;
		case HotkeyEventType.ShowHotkey:
			if (m_btnContinue != null)
			{
				NKCUIComHotkeyDisplay.OpenInstance(m_btnContinue.transform, HotkeyEventType.Confirm, HotkeyEventType.Skip);
			}
			break;
		}
		return false;
	}

	private void Update()
	{
		if (Input.touchCount > 0 || Input.GetMouseButton(0))
		{
			m_fCurrentHoldTime += Time.unscaledDeltaTime;
			if (m_fCurrentHoldTime > 0.3f)
			{
				OnClickContinue();
			}
		}
		else
		{
			m_fCurrentHoldTime = 0f;
		}
		if (m_bProcessingCountDown && Input.anyKeyDown)
		{
			m_bHadUserInput = true;
		}
	}

	public override void OnHotkeyHold(HotkeyEventType hotkey)
	{
		if (hotkey == HotkeyEventType.Skip)
		{
			OnClickContinue();
		}
	}

	private void OnTrimRetry()
	{
		SetPause(bSet: true);
		if (NKCScenManager.CurrentUserData().TrimData.TrimIntervalData.trimRetryCount > 0)
		{
			string content = NKCStringTable.GetString("SI_PF_TRIM_DUNGEON_RESULT_RESET_COUNT_TEXT", NKCScenManager.CurrentUserData().TrimData.TrimIntervalData.trimRetryCount);
			NKCPopupOKCancel.OpenOKCancelBox(NKCUtilString.GET_STRING_NOTICE, content, OnTrimRetryConfirm, OnTrimRetryCancel);
		}
		else
		{
			NKCPopupOKCancel.OpenOKBox(NKCUtilString.GET_STRING_NOTICE, NKCStringTable.GetString("SI_PF_TRIM_DUNGEON_RESULT_RESET_NO_COUNT"), OnTrimRetryCancel);
		}
	}

	private void OnTrimRetryConfirm()
	{
		if (NKMTrimIntervalTemplet.Find(NKCSynchronizedTime.ServiceTime) == null)
		{
			SetPause(bSet: false);
			return;
		}
		m_csbtnTrimRetry.Lock();
		NKCPacketSender.Send_NKMPacket_TRIM_RETRY_REQ();
	}

	public void OnTrimRetryAck()
	{
		SetPause(bSet: false);
	}

	private void OnTrimRetryCancel()
	{
		SetPause(bSet: false);
	}
}
