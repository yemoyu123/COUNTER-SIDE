using System;
using System.Collections.Generic;
using ClientPacket.Common;
using ClientPacket.Guild;
using ClientPacket.LeaderBoard;
using ClientPacket.Mode;
using ClientPacket.Pvp;
using Cs.Core.Util;
using Cs.Logging;
using NKC.UI;
using NKC.UI.Module;
using NKM;
using NKM.Templet;

namespace NKC;

public class NKCLeaderBoardManager
{
	public const int GUILD_LEVEL_RANK_CRITERIA = 1;

	public static Dictionary<int, List<LeaderBoardSlotData>> m_dicLeaderBoardData = new Dictionary<int, List<LeaderBoardSlotData>>();

	public static Dictionary<int, LeaderBoardSlotData> m_dicMyRankSlotData = new Dictionary<int, LeaderBoardSlotData>();

	public static Dictionary<int, DateTime> m_dicLastUpdateTime = new Dictionary<int, DateTime>();

	public static Dictionary<int, bool> m_dicAllReq = new Dictionary<int, bool>();

	private static float m_fRefreshInterval = 10f;

	private static LeaderBoardType m_LastType = LeaderBoardType.BT_NONE;

	public static void Initialize()
	{
		m_dicLeaderBoardData.Clear();
		m_dicLastUpdateTime.Clear();
		m_dicAllReq.Clear();
		m_dicMyRankSlotData.Clear();
	}

	public static bool HasLeaderBoardData(NKMLeaderBoardTemplet boardTemplet)
	{
		if (NeedRefreshData(boardTemplet))
		{
			return false;
		}
		return m_dicLeaderBoardData.ContainsKey(boardTemplet.m_BoardID);
	}

	public static List<LeaderBoardSlotData> GetLeaderBoardData(int boardId)
	{
		if (m_dicLeaderBoardData.ContainsKey(boardId))
		{
			return m_dicLeaderBoardData[boardId];
		}
		return new List<LeaderBoardSlotData>();
	}

	public static LeaderBoardSlotData GetMyRankSlotData(int boardId)
	{
		if (m_dicMyRankSlotData.ContainsKey(boardId))
		{
			return m_dicMyRankSlotData[boardId];
		}
		return new LeaderBoardSlotData();
	}

	private static DateTime GetLastUpdateTime(int boardId)
	{
		if (m_dicLastUpdateTime.ContainsKey(boardId))
		{
			return m_dicLastUpdateTime[boardId];
		}
		return DateTime.MinValue;
	}

	public static DateTime GetNextResetTime(NKMLeaderBoardTemplet templet)
	{
		LeaderBoardType boardTab = templet.m_BoardTab;
		if (boardTab != LeaderBoardType.BT_ACHIEVE && boardTab != LeaderBoardType.BT_SHADOW)
		{
			_ = 7;
		}
		return DateTime.MaxValue;
	}

	public static bool GetReceivedAllData(int boardId)
	{
		if (m_dicAllReq.ContainsKey(boardId))
		{
			return m_dicAllReq[boardId];
		}
		return false;
	}

	public static void SendReq(NKMLeaderBoardTemplet templet, bool bAllReq)
	{
		switch (templet.m_BoardTab)
		{
		case LeaderBoardType.BT_ACHIEVE:
			NKCPacketSender.Send_NKMPacket_LEADERBOARD_ACHIEVE_LIST_REQ(bAllReq);
			break;
		case LeaderBoardType.BT_SHADOW:
			NKCPacketSender.Send_NKMPacket_LEADERBOARD_SHADOWPALACE_LIST_REQ(templet.m_BoardCriteria, bAllReq);
			break;
		case LeaderBoardType.BT_FIERCE:
		{
			NKCFierceBattleSupportDataMgr nKCFierceBattleSupportDataMgr = NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr();
			if (nKCFierceBattleSupportDataMgr != null && nKCFierceBattleSupportDataMgr.FierceTemplet != null)
			{
				if (nKCFierceBattleSupportDataMgr.GetStatus() != NKCFierceBattleSupportDataMgr.FIERCE_STATUS.FS_UNUSABLE)
				{
					NKCPacketSender.Send_NKMPacket_LEADERBOARD_FIERCE_LIST_REQ(bAllReq);
				}
				else if (NKCUILeaderBoard.IsInstanceOpen)
				{
					NKCUILeaderBoard.Instance.RefreshUI(bResetScroll: true);
				}
			}
			break;
		}
		case LeaderBoardType.BT_GUILD:
			if (templet.m_BoardCriteria == 1)
			{
				NKCPacketSender.Send_NKMPacket_LEADERBOARD_GUILD_LEVEL_RANK_LIST_REQ();
			}
			else
			{
				NKCPacketSender.Send_NKMPacket_LEADERBOARD_GUILD_UNION_RANK_LIST_REQ(templet.m_BoardCriteria);
			}
			break;
		case LeaderBoardType.BT_TIMEATTACK:
			NKCPacketSender.Send_NKMPacket_LEADERBOARD_TIMEATTACK_LIST_REQ(templet.m_BoardCriteria, bAllReq);
			break;
		case LeaderBoardType.BT_DEFENCE:
			if (NKMDefenceTemplet.GetCurrentDefenceDungeonTemplet(ServiceTime.Now) != null)
			{
				NKCPacketSender.Send_NKMPacket_LEADERBOARD_DEFENCE_LIST_REQ(bAllReq);
			}
			else if (NKCUILeaderBoard.IsInstanceOpen)
			{
				NKCUILeaderBoard.Instance.RefreshUI(bResetScroll: true);
			}
			break;
		case LeaderBoardType.BT_LEAGUE:
		case LeaderBoardType.BT_UNLIMITED:
			m_LastType = templet.m_BoardTab;
			NKCPacketSender.Send_NKMPacket_LEAGUE_PVP_RANK_LIST_REQ(RANK_TYPE.ALL, (!bAllReq) ? LeaderBoardRangeType.TOP10 : LeaderBoardRangeType.ALL);
			break;
		case LeaderBoardType.BT_COLLECTION:
		case LeaderBoardType.BT_PVP_RANK:
		case LeaderBoardType.BT_PVP_LEAGUE_TOP:
		case LeaderBoardType.BT_TOURNAMENT:
			break;
		}
	}

	public static bool NeedRefreshData(NKMLeaderBoardTemplet templet)
	{
		if (NKCSynchronizedTime.GetServerUTCTime() - GetLastUpdateTime(templet.m_BoardID) < TimeSpan.FromSeconds(m_fRefreshInterval))
		{
			return false;
		}
		return true;
	}

	private static void SaveLeaderBoardData(LeaderBoardType boardType, int boardID, int userRank, long myScore, bool bIsAll, List<LeaderBoardSlotData> lstSlotData)
	{
		if (m_dicLastUpdateTime.ContainsKey(boardID))
		{
			m_dicLastUpdateTime[boardID] = NKCSynchronizedTime.GetServerUTCTime();
		}
		else
		{
			m_dicLastUpdateTime.Add(boardID, NKCSynchronizedTime.GetServerUTCTime());
		}
		if (m_dicLeaderBoardData.ContainsKey(boardID))
		{
			m_dicLeaderBoardData[boardID] = lstSlotData;
		}
		else
		{
			m_dicLeaderBoardData.Add(boardID, lstSlotData);
		}
		if (m_dicMyRankSlotData.ContainsKey(boardID))
		{
			m_dicMyRankSlotData[boardID].rank = userRank;
			m_dicMyRankSlotData[boardID].score = LeaderBoardSlotData.GetScoreByBoardType(boardType, myScore);
		}
		else
		{
			LeaderBoardSlotData value = LeaderBoardSlotData.MakeMySlotData(boardType, userRank, LeaderBoardSlotData.GetScoreByBoardType(boardType, myScore), boardType == LeaderBoardType.BT_GUILD);
			m_dicMyRankSlotData.Add(boardID, value);
		}
		if (m_dicAllReq.ContainsKey(boardID))
		{
			m_dicAllReq[boardID] = bIsAll;
		}
		else
		{
			m_dicAllReq.Add(boardID, bIsAll);
		}
	}

	public static void OnRecv(NKMPacket_LEADERBOARD_ACHIEVE_LIST_ACK sPacket)
	{
		if (sPacket.leaderBoardAchieveData.achieveData == null)
		{
			sPacket.leaderBoardAchieveData.achieveData = new List<NKMAchieveData>();
		}
		NKMLeaderBoardTemplet nKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(LeaderBoardType.BT_ACHIEVE, 0);
		if (nKMLeaderBoardTemplet == null)
		{
			Log.Error($"NKMLeaderBoardTemplet is null - tab : {LeaderBoardType.BT_ACHIEVE} / criteria : {0}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaderBoardManager.cs", 575);
			return;
		}
		List<LeaderBoardSlotData> list = new List<LeaderBoardSlotData>();
		for (int i = 0; i < sPacket.leaderBoardAchieveData.achieveData.Count; i++)
		{
			LeaderBoardSlotData item = LeaderBoardSlotData.MakeSlotData(sPacket.leaderBoardAchieveData.achieveData[i], i + 1);
			list.Add(item);
		}
		SaveLeaderBoardData(LeaderBoardType.BT_ACHIEVE, nKMLeaderBoardTemplet.m_BoardID, sPacket.userRank, NKCScenManager.CurrentUserData().m_MissionData.GetAchiecePoint(), sPacket.isAll, list);
		if (NKCUILeaderBoard.IsInstanceOpen)
		{
			NKCUILeaderBoard.Instance.RefreshUI(!sPacket.isAll);
		}
	}

	public static int GetMyShadowPalaceTimeByLeaderBoardTemplet(NKMLeaderBoardTemplet templet)
	{
		return GetMyShadowPalaceTime(templet.m_BoardCriteria);
	}

	public static int GetMyShadowPalaceTime(int palaceId)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return 0;
		}
		if (nKMUserData.m_ShadowPalace == null || nKMUserData.m_ShadowPalace.palaceDataList == null)
		{
			return 0;
		}
		NKMPalaceData nKMPalaceData = nKMUserData.m_ShadowPalace.palaceDataList.Find((NKMPalaceData x) => x.palaceId == palaceId);
		if (nKMPalaceData != null && nKMPalaceData.dungeonDataList.Count == NKMShadowPalaceManager.GetBattleTemplets(palaceId).Count)
		{
			int num = 0;
			for (int num2 = 0; num2 < nKMPalaceData.dungeonDataList.Count; num2++)
			{
				num += nKMPalaceData.dungeonDataList[num2].bestTime;
			}
			return num;
		}
		return 0;
	}

	public static void OnRecv(NKMPacket_LEADERBOARD_SHADOWPALACE_LIST_ACK cPacket)
	{
		if (cPacket.leaderBoardShadowPalaceData.shadowPalaceData == null)
		{
			cPacket.leaderBoardShadowPalaceData.shadowPalaceData = new List<NKMShadowPalaceData>();
		}
		NKMLeaderBoardTemplet nKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(LeaderBoardType.BT_SHADOW, cPacket.actId);
		if (nKMLeaderBoardTemplet == null)
		{
			Log.Error($"NKMLeaderBoardTemplet is null - tab : {LeaderBoardType.BT_SHADOW}, id : {cPacket.actId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaderBoardManager.cs", 632);
			return;
		}
		List<LeaderBoardSlotData> list = new List<LeaderBoardSlotData>();
		for (int i = 0; i < cPacket.leaderBoardShadowPalaceData.shadowPalaceData.Count; i++)
		{
			LeaderBoardSlotData item = LeaderBoardSlotData.MakeSlotData(cPacket.leaderBoardShadowPalaceData.shadowPalaceData[i], i + 1);
			list.Add(item);
		}
		SaveLeaderBoardData(LeaderBoardType.BT_SHADOW, nKMLeaderBoardTemplet.m_BoardID, cPacket.userRank, GetMyShadowPalaceTime(nKMLeaderBoardTemplet.m_BoardCriteria), cPacket.isAll, list);
		if (NKCUILeaderBoard.IsInstanceOpen)
		{
			NKCUILeaderBoard.Instance.RefreshUI(!cPacket.isAll);
		}
	}

	public static void OnRecv(NKMPacket_LEAGUE_PVP_RANK_LIST_ACK sPacket)
	{
		if (sPacket.list == null)
		{
			sPacket.list = new List<NKMUserSimpleProfileData>();
		}
		NKMLeaderBoardTemplet nKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(m_LastType, 0);
		if (nKMLeaderBoardTemplet == null)
		{
			Log.Error($"NKMLeaderBoardTemplet is null - tab : {m_LastType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaderBoardManager.cs", 661);
			return;
		}
		List<LeaderBoardSlotData> list = new List<LeaderBoardSlotData>();
		for (int i = 0; i < sPacket.list.Count; i++)
		{
			LeaderBoardSlotData item = LeaderBoardSlotData.MakeSlotData(m_LastType, sPacket.list[i], i + 1);
			list.Add(item);
		}
		SaveLeaderBoardData(m_LastType, nKMLeaderBoardTemplet.m_BoardID, sPacket.myRank, NKCScenManager.CurrentUserData().m_LeagueData.Score, bIsAll: true, list);
		if (NKCUIModuleHome.IsAnyInstanceOpen())
		{
			NKCUIModuleHome.SendMessage(new NKCUIModuleSubUIDraft.EventModuleMessageDataDraft());
		}
		if (NKCUILeaderBoard.IsInstanceOpen)
		{
			NKCUILeaderBoard.Instance.RefreshUI(bResetScroll: true);
		}
		else if (NKCPopupLeaderBoardSingle.IsInstanceOpen)
		{
			NKCPopupLeaderBoardSingle.Instance.RefreshUI();
		}
	}

	public static void OnRecv(NKMPacket_LEADERBOARD_FIERCE_LIST_ACK cPacket)
	{
		if (cPacket.leaderBoardfierceData == null)
		{
			cPacket.leaderBoardfierceData = new NKMLeaderBoardFierceData();
		}
		NKMLeaderBoardTemplet nKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(LeaderBoardType.BT_FIERCE, 0);
		if (nKMLeaderBoardTemplet == null)
		{
			Log.Error($"NKMLeaderBoardTemplet is null - tab : {LeaderBoardType.BT_FIERCE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaderBoardManager.cs", 698);
			return;
		}
		List<LeaderBoardSlotData> list = new List<LeaderBoardSlotData>();
		for (int i = 0; i < cPacket.leaderBoardfierceData.fierceData.Count; i++)
		{
			LeaderBoardSlotData item = LeaderBoardSlotData.MakeSlotData(cPacket.leaderBoardfierceData.fierceData[i], i + 1);
			list.Add(item);
		}
		int num = cPacket.userRankNumber;
		if (num == 0 || num > 100)
		{
			num = cPacket.userRankPercent;
		}
		NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr().UpdateMyFierceRank(cPacket.userRankNumber, cPacket.userRankPercent);
		NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr().UpdateFierceBosses(cPacket.bossList);
		SaveLeaderBoardData(LeaderBoardType.BT_FIERCE, nKMLeaderBoardTemplet.m_BoardID, num, NKCScenManager.GetScenManager().GetNKCFierceBattleSupportDataMgr().GetTotalPoint(), cPacket.isAll, list);
		if (NKCUILeaderBoard.IsInstanceOpen)
		{
			NKCUILeaderBoard.Instance.RefreshUI(!cPacket.isAll);
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_FIERCE_BATTLE_SUPPORT)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_FIERCE_BATTLE_SUPPORT().RefreshLeaderBoard();
		}
	}

	public static void OnRecv(NKMPacket_LEADERBOARD_GUILD_LEVEL_RANK_LIST_ACK sPacket)
	{
		if (sPacket.leaderBoard.rankDatas == null)
		{
			sPacket.leaderBoard.rankDatas = new List<NKMGuildRankData>();
		}
		NKMLeaderBoardTemplet nKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(LeaderBoardType.BT_GUILD, 1);
		if (nKMLeaderBoardTemplet == null)
		{
			Log.Error($"NKMLeaderBoardTemplet is null - tab : {LeaderBoardType.BT_GUILD}, criteria : {1}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaderBoardManager.cs", 742);
			return;
		}
		List<LeaderBoardSlotData> list = new List<LeaderBoardSlotData>();
		for (int i = 0; i < sPacket.leaderBoard.rankDatas.Count; i++)
		{
			LeaderBoardSlotData item = LeaderBoardSlotData.MakeSlotData(sPacket.leaderBoard.rankDatas[i], i + 1);
			list.Add(item);
		}
		SaveLeaderBoardData(LeaderBoardType.BT_GUILD, nKMLeaderBoardTemplet.m_BoardID, sPacket.myRankData.rank, sPacket.myRankData.score, bIsAll: true, list);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_LOBBY)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_LOBBY().RefreshUI();
		}
		if (NKCUILeaderBoard.IsInstanceOpen)
		{
			NKCUILeaderBoard.Instance.RefreshUI(bResetScroll: true);
		}
	}

	public static void OnRecv(NKMPacket_LEADERBOARD_GUILD_UNION_RANK_LIST_ACK sPacket)
	{
		if (sPacket.leaderBoard.rankDatas == null)
		{
			sPacket.leaderBoard.rankDatas = new List<NKMGuildRankData>();
		}
		NKMLeaderBoardTemplet nKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(LeaderBoardType.BT_GUILD, sPacket.seasonId);
		if (nKMLeaderBoardTemplet == null)
		{
			Log.Error($"NKMLeaderBoardTemplet is null - tab : {LeaderBoardType.BT_GUILD}, criteria : {sPacket.seasonId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaderBoardManager.cs", 770);
			return;
		}
		List<LeaderBoardSlotData> list = new List<LeaderBoardSlotData>();
		for (int i = 0; i < sPacket.leaderBoard.rankDatas.Count; i++)
		{
			LeaderBoardSlotData item = LeaderBoardSlotData.MakeSlotData(sPacket.leaderBoard.rankDatas[i], i + 1);
			list.Add(item);
		}
		SaveLeaderBoardData(LeaderBoardType.BT_GUILD, nKMLeaderBoardTemplet.m_BoardID, sPacket.myRankData.rank, sPacket.myRankData.score, bIsAll: true, list);
		if (NKCScenManager.GetScenManager().GetNowScenID() == NKM_SCEN_ID.NSI_GUILD_LOBBY)
		{
			NKCScenManager.GetScenManager().Get_NKC_SCEN_GUILD_LOBBY().RefreshUI();
		}
		if (NKCUILeaderBoard.IsInstanceOpen)
		{
			NKCUILeaderBoard.Instance.RefreshUI(bResetScroll: true);
		}
	}

	public static NKMGuildRankData MakeMyGuildRankData(int boardId, out int myRank)
	{
		myRank = 0;
		if (!NKCGuildManager.HasGuild())
		{
			return new NKMGuildRankData();
		}
		NKMGuildRankData nKMGuildRankData = new NKMGuildRankData();
		nKMGuildRankData.badgeId = NKCGuildManager.MyGuildData.badgeId;
		nKMGuildRankData.guildLevel = NKCGuildManager.MyGuildData.guildLevel;
		nKMGuildRankData.guildName = NKCGuildManager.MyGuildData.name;
		nKMGuildRankData.guildUid = NKCGuildManager.MyGuildData.guildUid;
		nKMGuildRankData.masterNickname = NKCGuildManager.MyGuildData.members.Find((NKMGuildMemberData x) => x.grade == GuildMemberGrade.Master).commonProfile.nickname;
		nKMGuildRankData.memberCount = NKCGuildManager.MyGuildData.members.Count;
		if (!long.TryParse(GetMyRankSlotData(boardId).score, out nKMGuildRankData.rankValue))
		{
			nKMGuildRankData.rankValue = 0L;
		}
		myRank = GetMyRankSlotData(boardId).rank;
		return nKMGuildRankData;
	}

	public static void OnRecv(NKMPacket_LEADERBOARD_TIMEATTACK_LIST_ACK sPacket)
	{
		if (sPacket.leaderBoardTimeAttackData.timeAttackData == null)
		{
			sPacket.leaderBoardTimeAttackData.timeAttackData = new List<NKMTimeAttackData>();
		}
		NKMLeaderBoardTemplet nKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(LeaderBoardType.BT_TIMEATTACK, sPacket.stageId);
		if (nKMLeaderBoardTemplet == null)
		{
			Log.Error($"NKMLeaderBoardTemplet is null - tab : {LeaderBoardType.BT_TIMEATTACK}, id : {sPacket.stageId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaderBoardManager.cs", 824);
			return;
		}
		List<LeaderBoardSlotData> list = new List<LeaderBoardSlotData>();
		for (int i = 0; i < sPacket.leaderBoardTimeAttackData.timeAttackData.Count; i++)
		{
			LeaderBoardSlotData item = LeaderBoardSlotData.MakeSlotData(sPacket.leaderBoardTimeAttackData.timeAttackData[i], i + 1);
			list.Add(item);
		}
		SaveLeaderBoardData(LeaderBoardType.BT_TIMEATTACK, nKMLeaderBoardTemplet.m_BoardID, sPacket.userRank, NKCScenManager.CurrentUserData().GetStageBestClearSec(nKMLeaderBoardTemplet.m_BoardCriteria), sPacket.isAll, list);
		if (NKCUILeaderBoard.IsInstanceOpen)
		{
			NKCUILeaderBoard.Instance.RefreshUI(!sPacket.isAll);
		}
		else if (NKCPopupLeaderBoardSingle.IsInstanceOpen)
		{
			NKCPopupLeaderBoardSingle.Instance.RefreshUI(!sPacket.isAll);
		}
	}

	public static void OnRecv(NKMPacket_LEADERBOARD_DEFENCE_LIST_ACK sPacket)
	{
		if (sPacket.leaderBoardDefenceData == null)
		{
			sPacket.leaderBoardDefenceData = new NKMLeaderBoardDefenceData();
		}
		NKMLeaderBoardTemplet nKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(LeaderBoardType.BT_DEFENCE, 0);
		if (nKMLeaderBoardTemplet == null)
		{
			Log.Error($"NKMLeaderBoardTemplet is null - tab : {LeaderBoardType.BT_DEFENCE}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCLeaderBoardManager.cs", 855);
			return;
		}
		List<LeaderBoardSlotData> list = new List<LeaderBoardSlotData>();
		for (int i = 0; i < sPacket.leaderBoardDefenceData.rankDatas.Count; i++)
		{
			if (i == 0)
			{
				NKCDefenceDungeonManager.SetTopRankData(sPacket.leaderBoardDefenceData.rankDatas[i]);
			}
			LeaderBoardSlotData item = LeaderBoardSlotData.MakeSlotData(sPacket.leaderBoardDefenceData.rankDatas[i], i + 1);
			list.Add(item);
		}
		int num = sPacket.userRank;
		if (num == 0 || num > 100)
		{
			num = NKCDefenceDungeonManager.m_MyRankPercent;
		}
		SaveLeaderBoardData(LeaderBoardType.BT_DEFENCE, nKMLeaderBoardTemplet.m_BoardID, num, NKCDefenceDungeonManager.m_BestClearScore, sPacket.isAll, list);
		if (NKCUILeaderBoard.IsInstanceOpen)
		{
			NKCUILeaderBoard.Instance.RefreshUI(!sPacket.isAll);
		}
		if (NKCPopupLeaderBoardSingle.IsInstanceOpen)
		{
			NKCPopupLeaderBoardSingle.Instance.RefreshUI();
		}
		if (NKCScenManager.GetScenManager().GetNowScenID() != NKM_SCEN_ID.NSI_HOME)
		{
			return;
		}
		foreach (NKCUIModuleHome item2 in NKCUIManager.GetOpenedUIsByType<NKCUIModuleHome>())
		{
			if (item2.IsOpen)
			{
				item2.UpdateUI();
			}
		}
	}
}
