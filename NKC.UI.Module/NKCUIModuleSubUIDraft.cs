using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Core.Util;
using Cs.Logging;
using NKC.PacketHandler;
using NKC.UI.Component;
using NKC.UI.Fierce;
using NKC.UI.Gauntlet;
using NKM;
using NKM.Event;
using NKM.Templet;
using TMPro;
using UnityEngine;

namespace NKC.UI.Module;

public class NKCUIModuleSubUIDraft : NKCUIModuleSubUIBase
{
	public class EventModuleMessageDataDraft : NKCUIModuleHome.EventModuleMessageDataBase
	{
		public bool m_bOpenRewardPopup;
	}

	[Header("Top")]
	public TMP_Text m_lbSeasonName;

	public TMP_Text m_lbSeasonDate;

	public NKCUIComStateButton m_btnRule;

	public NKCUIComStateButton m_btnMagazine;

	public GameObject m_objMaxLevel;

	public GameObject m_objBanEquip;

	[Header("Left")]
	public GameObject m_objMyInfo;

	public CanvasGroup m_cgMyInfo;

	public NKCUILeagueTier m_NKCUILeagueTierMy;

	public TMP_Text m_lbScore;

	public TMP_Text m_lbTier;

	public TMP_Text m_lbLeagueRank;

	public GameObject m_objNoRecord;

	public TMP_Text m_lbWinCount;

	public TMP_Text m_lbLoseCount;

	public TMP_Text m_lbWinPercent;

	public NKCUIComStateButton m_btnMusic;

	public NKCUIComStateButton m_btnRewardInfo;

	public NKCUIComStateButton m_btnLeaderboard;

	public NKCUIComStateButton m_btnRankHistory;

	[Header("Right")]
	public NKCUIComStateButton m_btnHistory;

	public NKCUIComStateButton m_btnEmoticon;

	public NKCUIComStateButton m_btnGlobalBan;

	public NKCUIComStateButton m_btnReady;

	[Header("BattleCondition")]
	public NKCUIComBattleEnvironmentList m_BattleCond;

	[Header("Center")]
	public GameObject m_objTopPlayer;

	public NKCUILeaderBoardSlot m_slotTopPlayer;

	[Header("Mission")]
	public NKCUIComStateButton m_btnMission;

	public GameObject m_objMissionReddot;

	public NKCUIModuleSubUIDraftMission m_MissionUI;

	private NKCUIModuleSubUIDraftReward m_NKCUIModuleSubUIDraftReward;

	private NKCPopupImage m_NKCPopupImage;

	private NKMLeaguePvpRankSeasonTemplet m_leagueTemplet;

	private Dictionary<int, List<NKCUIPopupFierceBattleRewardInfo.RankUIRewardData>> m_dicRewardData = new Dictionary<int, List<NKCUIPopupFierceBattleRewardInfo.RankUIRewardData>>();

	private List<string> m_lstRewardTabStrID = new List<string>();

	private List<string> m_lstRewardTitleStrID = new List<string>();

	private List<string> m_lstRewardDescStr = new List<string>();

	private NKCUIModuleSubUITournamentRank m_NKCUIModuleSubUITournamentRank;

	private bool m_bWaitForRewardPopup;

	private bool m_bWaitForRankerPopup;

	private NKM_GAME_TYPE m_GameType;

	public override void Init()
	{
		base.Init();
		if (m_btnRule != null)
		{
			m_btnRule.PointerClick.RemoveAllListeners();
			m_btnRule.PointerClick.AddListener(OnClickRule);
		}
		if (m_btnMagazine != null)
		{
			m_btnMagazine.PointerClick.RemoveAllListeners();
			m_btnMagazine.PointerClick.AddListener(OnClickMagazine);
		}
		if (m_btnMusic != null)
		{
			m_btnMusic.PointerClick.RemoveAllListeners();
			m_btnMusic.PointerClick.AddListener(OnClickMusic);
		}
		if (m_btnRewardInfo != null)
		{
			m_btnRewardInfo.PointerClick.RemoveAllListeners();
			m_btnRewardInfo.PointerClick.AddListener(OnClickRewardInfo);
		}
		if (m_btnLeaderboard != null)
		{
			m_btnLeaderboard.PointerClick.RemoveAllListeners();
			m_btnLeaderboard.PointerClick.AddListener(OnClickLeaderboard);
		}
		if (m_btnRankHistory != null)
		{
			m_btnRankHistory.PointerClick.RemoveAllListeners();
			m_btnRankHistory.PointerClick.AddListener(OnClickRankHistory);
		}
		if (m_btnHistory != null)
		{
			m_btnHistory.PointerClick.RemoveAllListeners();
			m_btnHistory.PointerClick.AddListener(OnClickHistory);
		}
		if (m_btnEmoticon != null)
		{
			m_btnEmoticon.PointerClick.RemoveAllListeners();
			m_btnEmoticon.PointerClick.AddListener(OnClickEmoticon);
		}
		if (m_btnGlobalBan != null)
		{
			m_btnGlobalBan.PointerClick.RemoveAllListeners();
			m_btnGlobalBan.PointerClick.AddListener(OnClickGlobalBan);
			m_btnGlobalBan.m_bGetCallbackWhileLocked = true;
		}
		if (m_btnReady != null)
		{
			m_btnReady.PointerClick.RemoveAllListeners();
			m_btnReady.PointerClick.AddListener(OnClickReady);
			m_btnReady.m_bGetCallbackWhileLocked = true;
		}
		if (m_btnMission != null)
		{
			m_btnMission.PointerClick.RemoveAllListeners();
			m_btnMission.PointerClick.AddListener(OnClickMission);
		}
		if (m_MissionUI != null)
		{
			m_MissionUI.InitUI();
		}
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.PVP_REPLAY))
		{
			NKCScenManager.GetScenManager().GetNKCReplayMgr().ReadReplayData();
		}
	}

	public override void OnOpen(NKMEventCollectionIndexTemplet templet)
	{
		ModuleID = templet.Key;
		m_bWaitForRankerPopup = false;
		m_bWaitForRewardPopup = false;
		int intValue = NKCUtil.GetIntValue(templet.m_Option, "PvpLeagueSeasonID", 0);
		m_leagueTemplet = NKMLeaguePvpRankSeasonTemplet.Find(intValue);
		m_GameType = m_leagueTemplet.GameType;
		if (m_leagueTemplet == null)
		{
			NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_HOME);
			return;
		}
		NKCUtil.SetGameobjectActive(m_btnMission, m_leagueTemplet.MissionIdList.Count > 0);
		if (!NKCPVPManager.m_bLeagueDataReceived)
		{
			NKCPacketSender.Send_NKMPacket_LEAGUE_PVP_SEASON_INFO_REQ();
		}
		NKCUtil.SetLabelText(m_lbSeasonName, m_leagueTemplet.GetSeasonStrId());
		if (m_BattleCond != null)
		{
			bool flag = m_BattleCond.InitData(null, m_leagueTemplet.BattleConditionTemplets, null);
			NKCUtil.SetGameobjectActive(m_BattleCond, flag);
			if (flag)
			{
				m_BattleCond.Open();
			}
		}
		SetData();
	}

	public override void OnClose()
	{
		NKCUtil.SetGameobjectActive(base.gameObject, bValue: false);
		m_dicRewardData.Clear();
		m_lstRewardTabStrID.Clear();
		m_lstRewardTitleStrID.Clear();
		m_lstRewardDescStr.Clear();
		m_NKCPopupImage = null;
		m_NKCUIModuleSubUIDraftReward = null;
		m_NKCUIModuleSubUITournamentRank = null;
		m_bWaitForRewardPopup = false;
		m_bWaitForRankerPopup = false;
	}

	private void SetData()
	{
		NKCUtil.SetGameobjectActive(m_objMaxLevel, m_leagueTemplet.UnitMaxLevel);
		NKCUtil.SetGameobjectActive(m_objBanEquip, m_leagueTemplet.ForceBanEquip);
		SetMyLeagueInfo();
		SetTopPlayer();
		if (m_leagueTemplet.SeasonGameEnable(ServiceTime.Now))
		{
			NKCUtil.SetLabelText(m_lbSeasonDate, m_leagueTemplet.StartDateUTC.ToLocalTime().ToString("yyyy-MM-dd HH:mm") + " ~ " + m_leagueTemplet.EndDateUTC.ToLocalTime().ToString("yyyy-MM-dd HH:mm"));
			if (m_btnGlobalBan != null)
			{
				m_btnGlobalBan.UnLock();
			}
			m_btnReady.UnLock();
		}
		else if (m_leagueTemplet.SeasonRewardEnable(ServiceTime.Now))
		{
			NKCUtil.SetLabelText(m_lbSeasonDate, m_leagueTemplet.SeasonRewardInterval.StartDate.ToString("yyyy-MM-dd HH:mm") + " ~ " + m_leagueTemplet.SeasonRewardInterval.EndDate.ToString("yyyy-MM-dd HH:mm"));
			if (m_btnGlobalBan != null)
			{
				m_btnGlobalBan.Lock();
			}
			m_btnReady.Lock();
		}
		else
		{
			if (m_btnGlobalBan != null)
			{
				m_btnGlobalBan.Lock();
			}
			m_btnReady.Lock();
		}
		if (m_btnMagazine != null)
		{
			NKCUtil.SetGameobjectActive(m_btnMagazine, m_leagueTemplet.SeasonRewardEnable(ServiceTime.Now));
		}
		if (m_MissionUI != null)
		{
			m_MissionUI.CloseImmediately();
		}
		NKCUtil.SetGameobjectActive(m_objMissionReddot, CanGetReward());
		ResetBGMTitle();
		if (!NKCPVPManager.m_bLeagueSeasonRewardReceived && NKCScenManager.CurrentUserData().m_LeagueData.WinCount + NKCScenManager.CurrentUserData().m_LeagueData.LoseCount > 0 && NKCPVPManager.m_bLeagueDataReceived && !m_bWaitForRewardPopup && m_leagueTemplet.SeasonRewardEnable(ServiceTime.Now))
		{
			m_bWaitForRewardPopup = true;
			NKCPacketSender.Send_NKMPacket_LEAGUE_PVP_SEASON_REWARD_REQ();
		}
	}

	private void SetMyLeagueInfo()
	{
		PvpState leagueData = NKCScenManager.CurrentUserData().m_LeagueData;
		if (leagueData == null || leagueData.WinCount + leagueData.LoseCount <= 0)
		{
			NKCUtil.SetGameobjectActive(m_objNoRecord, bValue: true);
			NKCUtil.SetGameobjectActive(m_cgMyInfo.gameObject, bValue: false);
			return;
		}
		NKCUtil.SetGameobjectActive(m_objNoRecord, bValue: false);
		NKCUtil.SetGameobjectActive(m_cgMyInfo.gameObject, bValue: true);
		if (NKMLeaguePvpRankTemplet.FindByTier(m_leagueTemplet.SeasonId, leagueData.LeagueTierID, out var templet))
		{
			m_NKCUILeagueTierMy.SetUI(templet);
			NKCUtil.SetLabelText(m_lbTier, NKCStringTable.GetString(templet.LeagueName));
			NKCUtil.SetLabelText(m_lbLeagueRank, string.Format(NKCUtilString.GET_STRING_TOTAL_RANK_ONE_PARAM, leagueData.Rank));
			NKCUtil.SetLabelText(m_lbScore, leagueData.Score.ToString());
			NKCUtil.SetLabelText(m_lbWinCount, leagueData.WinCount.ToString());
			NKCUtil.SetLabelText(m_lbLoseCount, leagueData.LoseCount.ToString());
			NKCUtil.SetLabelText(m_lbWinPercent, $"{leagueData.WinCount * 100 / (leagueData.WinCount + leagueData.LoseCount)}%");
		}
	}

	private void SetTopPlayer()
	{
		bool flag = m_leagueTemplet.SeasonRewardEnable(ServiceTime.Now);
		NKCUtil.SetGameobjectActive(m_objTopPlayer, flag);
		if (!flag)
		{
			return;
		}
		NKMLeaderBoardTemplet nKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(GetLeaderBoardType(m_GameType), 0);
		if (nKMLeaderBoardTemplet != null)
		{
			List<LeaderBoardSlotData> leaderBoardData = NKCLeaderBoardManager.GetLeaderBoardData(nKMLeaderBoardTemplet.m_BoardID);
			if (leaderBoardData.Count > 0)
			{
				m_slotTopPlayer.SetData(leaderBoardData[0], 0, null, bUsePercentRank: false, bShowMyRankIcon: false);
			}
			else if (!NKCLeaderBoardManager.GetReceivedAllData(nKMLeaderBoardTemplet.m_BoardID))
			{
				NKCLeaderBoardManager.SendReq(nKMLeaderBoardTemplet, bAllReq: true);
			}
		}
	}

	public override void Refresh()
	{
		SetData();
		ResetBGMTitle();
		if (m_MissionUI != null && m_MissionUI.IsOpened())
		{
			m_MissionUI.Refresh();
		}
	}

	private void ResetBGMTitle()
	{
		NKCBGMInfoTemplet nKCBGMInfoTemplet = NKCBGMInfoTemplet.Find(NKCScenManager.GetScenManager().GetMyUserData().m_JukeboxData.GetJukeboxBgmId(NKM_BGM_TYPE.PVP_INGAME));
		if (nKCBGMInfoTemplet != null)
		{
			m_btnMusic.SetTitleText(NKCStringTable.GetString(nKCBGMInfoTemplet.m_BgmNameStringID));
		}
		else
		{
			m_btnMusic.SetTitleText(NKCUtilString.GET_STRING_JUKEBOX_MUSIC_DEFAULT);
		}
	}

	public override void PassData(NKCUIModuleHome.EventModuleMessageDataBase passData)
	{
		Refresh();
		if (!(passData is EventModuleMessageDataDraft eventModuleMessageDataDraft))
		{
			return;
		}
		if (m_bWaitForRankerPopup)
		{
			m_bWaitForRankerPopup = false;
			OnClickRankHistory();
		}
		if (eventModuleMessageDataDraft.m_bOpenRewardPopup)
		{
			if (m_GameType == NKM_GAME_TYPE.NGT_PVP_LEAGUE)
			{
				m_NKCUIModuleSubUIDraftReward = NKCUIModuleSubUIDraftReward.OpenInstance("UI_SINGLE_CHAMPIONSHIP", "UI_SINGLE_POPUP_CHAMPIONSHIP_REWARD");
			}
			else if (m_GameType == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
			{
				m_NKCUIModuleSubUIDraftReward = NKCUIModuleSubUIDraftReward.OpenInstance("UI_SINGLE_UNLIMITED", "UI_SINGLE_POPUP_UNLIMITED_REWARD");
			}
			m_NKCUIModuleSubUIDraftReward.Open(m_leagueTemplet.SeasonId);
		}
	}

	private void OnClickRule()
	{
		if (m_GameType == NKM_GAME_TYPE.NGT_PVP_LEAGUE)
		{
			m_NKCPopupImage = NKCPopupImage.OpenInstance("UI_SINGLE_CHAMPIONSHIP", "UI_SINGLE_POPUP_CHAMPIONSHIP_RULE");
		}
		else if (m_GameType == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
		{
			m_NKCPopupImage = NKCPopupImage.OpenInstance("UI_SINGLE_UNLIMITED", "UI_SINGLE_POPUP_UNLIMITED_RULE");
		}
		else
		{
			m_NKCPopupImage = null;
		}
		if (m_NKCPopupImage != null)
		{
			m_NKCPopupImage.Open(string.Empty, m_leagueTemplet.GetSeasonRule());
		}
	}

	private void OnClickMagazine()
	{
		if (NKCLeaguePVPMgr.PickRateData == null || NKCLeaguePVPMgr.PickRateData.Count == 0)
		{
			NKCPacketSender.NKMPacket_PVP_PICK_RATE_REQ(m_GameType);
		}
		else
		{
			NKCUIPopupDraftMagazine.Instance.Open();
		}
	}

	private void OnClickMusic()
	{
		if (!NKCContentManager.IsContentsUnlocked(ContentsType.BASE_PERSONNAL))
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_JUKEBOX_CONTENTS_UNLOCK);
			return;
		}
		int jukeboxBgmId = NKCScenManager.GetScenManager().GetMyUserData().m_JukeboxData.GetJukeboxBgmId(NKM_BGM_TYPE.PVP_INGAME);
		NKCUIJukeBox.Instance.Open(jukeboxBgmId);
	}

	private void OnClickRewardInfo()
	{
		SetRewardData();
		NKCPopupRewardInfoToggle.Instance.Open(NKCUIPopupFierceBattleRewardInfo.REWARD_SLOT_TYPE.GauntletLeague, m_dicRewardData, m_lstRewardTabStrID, m_lstRewardTitleStrID, m_lstRewardDescStr, bShowTitle: true, bShowDesc: true, bShowMyRank: false);
	}

	private void SetRewardData()
	{
		m_dicRewardData.Clear();
		List<NKMLeaguePvpRankSeasonRewardTemplet> leaguePvpSeasonRewardList = NKCPVPManager.GetLeaguePvpSeasonRewardList(m_leagueTemplet.RankSeasonRewardGroup);
		List<NKCUIPopupFierceBattleRewardInfo.RankUIRewardData> list = new List<NKCUIPopupFierceBattleRewardInfo.RankUIRewardData>();
		for (int i = 0; i < leaguePvpSeasonRewardList.Count; i++)
		{
			list.Add(new NKCUIPopupFierceBattleRewardInfo.RankUIRewardData(leaguePvpSeasonRewardList[i]));
		}
		m_dicRewardData.Add(0, list);
		NKMLeaguePvpRankGroupTemplet rankGroup = m_leagueTemplet.RankGroup;
		if (rankGroup != null)
		{
			List<NKMLeaguePvpRankTemplet> list2 = rankGroup.List.ToList();
			list2.Sort(SortByTier);
			List<NKCUIPopupFierceBattleRewardInfo.RankUIRewardData> list3 = new List<NKCUIPopupFierceBattleRewardInfo.RankUIRewardData>();
			for (int j = 0; j < list2.Count; j++)
			{
				list3.Add(new NKCUIPopupFierceBattleRewardInfo.RankUIRewardData(list2[j]));
			}
			m_dicRewardData.Add(1, list3);
		}
		m_lstRewardTabStrID.Clear();
		m_lstRewardTitleStrID.Clear();
		m_lstRewardDescStr.Clear();
		m_lstRewardTabStrID.Add("SI_PF_LEAGUE_SEASON_RANK_REWARD_TITLE");
		m_lstRewardTabStrID.Add("SI_PF_LEAGUE_SEASON_REWARD_TITLE");
		if (m_GameType == NKM_GAME_TYPE.NGT_PVP_LEAGUE)
		{
			m_lstRewardDescStr.Add(NKCUtilString.GET_STRING_POPUP_LEAGUE_REWARD_INFO_DESC);
			m_lstRewardDescStr.Add(NKCUtilString.GET_STRING_POPUP_LEAGUE_REWARD_INFO_DESC);
		}
		else if (m_GameType == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
		{
			m_lstRewardDescStr.Add(NKCUtilString.GET_STRING_POPUP_UNLIMITED_REWARD_INFO_DESC);
			m_lstRewardDescStr.Add(NKCUtilString.GET_STRING_POPUP_UNLIMITED_REWARD_INFO_DESC);
		}
	}

	private int SortByTier(NKMLeaguePvpRankTemplet lItem, NKMLeaguePvpRankTemplet rItem)
	{
		return rItem.LeagueTier.CompareTo(lItem.LeagueTier);
	}

	private void OnClickLeaderboard()
	{
		NKMLeaderBoardTemplet nKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(GetLeaderBoardType(m_GameType), 0);
		if (nKMLeaderBoardTemplet != null)
		{
			NKCPopupLeaderBoardSingle.Instance.OpenSingle(nKMLeaderBoardTemplet);
		}
	}

	private void OnClickRankHistory()
	{
		if (!NKCPVPManager.m_bLeagueDataReceived)
		{
			m_bWaitForRankerPopup = true;
			NKCPacketSender.Send_NKMPacket_LEAGUE_PVP_SEASON_INFO_REQ();
			return;
		}
		if (NKCPVPManager.m_bLeagueDataReceived && NKCPVPManager.m_lstLeagueRankerData.Count == 0)
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GE_STRING_TOURNAMENT_HOF_NO_RECORD);
			return;
		}
		if (m_GameType == NKM_GAME_TYPE.NGT_PVP_LEAGUE)
		{
			m_NKCUIModuleSubUITournamentRank = NKCUIModuleSubUITournamentRank.OpenInstance("UI_SINGLE_CHAMPIONSHIP", "UI_SINGLE_POPUP_CHAMPIONSHIP_RANK");
		}
		else if (m_GameType == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
		{
			m_NKCUIModuleSubUITournamentRank = NKCUIModuleSubUITournamentRank.OpenInstance("UI_SINGLE_UNLIMITED", "UI_SINGLE_POPUP_UNLIMITED_RANK");
		}
		if (m_NKCUIModuleSubUITournamentRank != null)
		{
			m_NKCUIModuleSubUITournamentRank.Open(NKCPVPManager.m_lstLeagueRankerData, GetLeaderBoardType(m_GameType));
		}
	}

	private void OnClickHistory()
	{
		NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_LOBBY()?.OpenBattleRecord(m_GameType);
	}

	private void OnClickEmoticon()
	{
		if (!NKCEmoticonManager.m_bWaitForPopup)
		{
			if (NKCEmoticonManager.m_bReceivedEmoticonData)
			{
				NKCPopupEmoticonSetting.Instance.Open();
				return;
			}
			NKCEmoticonManager.m_bWaitForPopup = true;
			NKCPacketSender.Send_NKMPacket_EMOTICON_DATA_REQ();
		}
	}

	private void OnClickGlobalBan()
	{
		if (!m_btnGlobalBan.m_bLock)
		{
			NKCPopupGauntletBan.Instance.Open();
		}
	}

	private void OnClickReady()
	{
		if (m_GameType == NKM_GAME_TYPE.NGT_PVP_LEAGUE && !NKCBanManager.IsTryDraftBan())
		{
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_GAUNTLET_GLOBAL_BAN_NOT_SELECT_UNIT);
			return;
		}
		if (NKMPvpCommonConst.Instance.LeaguePvp.CalculateTimeStart <= ServiceTime.Now.TimeOfDay)
		{
			TimeSpan ts = new TimeSpan(NKMPvpCommonConst.Instance.LeaguePvp.CalculateTimeInterval, 0, 0);
			if (NKMPvpCommonConst.Instance.LeaguePvp.CalculateTimeStart.Add(ts) > ServiceTime.Now.TimeOfDay)
			{
				NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_POPUP_LEAGUE_CALCULATE);
				return;
			}
		}
		int seasonID = NKCUtil.FindPVPSeasonIDForLeague(NKCSynchronizedTime.GetServerUTCTime());
		NKM_ERROR_CODE nKM_ERROR_CODE = NKM_ERROR_CODE.NEC_OK;
		if (m_GameType == NKM_GAME_TYPE.NGT_PVP_LEAGUE)
		{
			nKM_ERROR_CODE = NKCPVPManager.CanPlayPVPLeagueGame(NKCScenManager.CurrentUserData(), seasonID, NKCSynchronizedTime.GetServerUTCTime());
		}
		else if (m_GameType == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
		{
			nKM_ERROR_CODE = NKCPVPManager.CanPlayUnlimitedGame(NKCScenManager.CurrentUserData(), seasonID, NKCSynchronizedTime.GetServerUTCTime());
		}
		switch (nKM_ERROR_CODE)
		{
		case NKM_ERROR_CODE.NEC_FAIL_DRAFT_PVP_NOT_ENOUGH_UNIT_COUNT:
		case NKM_ERROR_CODE.NEC_FAIL_DRAFT_PVP_NOT_ENOUGH_SHIP_COUNT:
			NKCPopupGauntletLeagueEnterCondition.Instance.Open();
			break;
		case NKM_ERROR_CODE.NEC_FAIL_DRAFT_PVP_INVALID_TIME:
			NKCPopupMessageManager.AddPopupMessage(NKCUtilString.GET_STRING_POPUP_LEAGUE_CALCULATE);
			return;
		default:
			NKCPacketHandlers.Check_NKM_ERROR_CODE(nKM_ERROR_CODE);
			break;
		}
		if (!m_btnReady.m_bLock && nKM_ERROR_CODE == NKM_ERROR_CODE.NEC_OK)
		{
			NKCScenManager.GetScenManager().Get_SCEN_HOME().SetReservedOpenUI(NKC_SCEN_HOME.RESERVE_OPEN_TYPE.ROT_EVENT_COLLECTION, ModuleID);
			if (m_GameType == NKM_GAME_TYPE.NGT_PVP_LEAGUE)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_MATCH().SetReservedGameType(NKM_GAME_TYPE.NGT_PVP_LEAGUE);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_MATCH);
			}
			else if (m_GameType == NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
			{
				NKCScenManager.GetScenManager().Get_NKC_SCEN_GAUNTLET_MATCH_READY().SetReservedGameType(NKM_GAME_TYPE.NGT_PVP_UNLIMITED);
				NKCScenManager.GetScenManager().ScenChangeFade(NKM_SCEN_ID.NSI_GAUNTLET_MATCH_READY);
			}
			else
			{
				Log.Error($"Invalid GameType - {m_GameType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/UI/Module/NKCUIModuleSubUIDraft.cs", 584);
			}
		}
	}

	private LeaderBoardType GetLeaderBoardType(NKM_GAME_TYPE gameType)
	{
		return gameType switch
		{
			NKM_GAME_TYPE.NGT_PVP_LEAGUE => LeaderBoardType.BT_LEAGUE, 
			NKM_GAME_TYPE.NGT_PVP_UNLIMITED => LeaderBoardType.BT_UNLIMITED, 
			_ => LeaderBoardType.BT_NONE, 
		};
	}

	private void OnClickMission()
	{
		if (!(m_MissionUI != null) || m_leagueTemplet.MissionIdList.Count <= 0)
		{
			return;
		}
		if (m_MissionUI.IsOpened())
		{
			m_MissionUI.Close();
			return;
		}
		List<NKMMissionTemplet> list = new List<NKMMissionTemplet>();
		for (int i = 0; i < m_leagueTemplet.MissionIdList.Count; i++)
		{
			list.Add(NKMMissionManager.GetMissionTemplet(m_leagueTemplet.MissionIdList[i]));
		}
		m_MissionUI.Open(list);
	}

	private bool CanGetReward()
	{
		bool flag = false;
		NKMUserData userData = NKCScenManager.CurrentUserData();
		for (int i = 0; i < m_leagueTemplet.MissionIdList.Count; i++)
		{
			NKMMissionTemplet missionTemplet = NKMMissionManager.GetMissionTemplet(m_leagueTemplet.MissionIdList[i]);
			NKMMissionData missionData = NKMMissionManager.GetMissionData(missionTemplet);
			flag |= NKMMissionManager.CanComplete(missionTemplet, userData, missionData) == NKM_ERROR_CODE.NEC_OK;
		}
		return flag;
	}
}
