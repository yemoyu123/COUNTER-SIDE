using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClientPacket.Pvp;
using Cs.Core.Util;
using Cs.Logging;
using NKC;
using NKC.UI.Module;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKCPVPManager
{
	private const int WEEK_CALC_START_HOUR = 22;

	private const int WEEK_CALC_END_HOUR = 4;

	public static Dictionary<int, Dictionary<int, NKMPvpRankTemplet>> dicPvpRankTemplet = new Dictionary<int, Dictionary<int, NKMPvpRankTemplet>>();

	public static Dictionary<int, List<NKMPvpRankTemplet>> listPvpRankTemplet = new Dictionary<int, List<NKMPvpRankTemplet>>();

	public static Dictionary<int, NKMPvpRankSeasonTemplet> dicPvpRankSeasonTemplet = new Dictionary<int, NKMPvpRankSeasonTemplet>();

	public static Dictionary<int, NKMPvpRankSeasonTemplet> dicAsyncPvpSeasonTemplet = new Dictionary<int, NKMPvpRankSeasonTemplet>();

	public static bool m_bLeagueDataReceived = false;

	public static bool m_bLeagueSeasonRewardReceived = false;

	public static List<List<LeaderBoardSlotData>> m_lstLeagueRankerData = new List<List<LeaderBoardSlotData>>();

	public static int WEEK_CALC_END_TIME
	{
		get
		{
			if (4 >= NKMTime.INTERVAL_FROM_UTC)
			{
				return 4 - NKMTime.INTERVAL_FROM_UTC;
			}
			return 28 - NKMTime.INTERVAL_FROM_UTC;
		}
	}

	public static int WEEK_CALC_START_TIME
	{
		get
		{
			if (22 >= NKMTime.INTERVAL_FROM_UTC)
			{
				return 22 - NKMTime.INTERVAL_FROM_UTC;
			}
			return 46 - NKMTime.INTERVAL_FROM_UTC;
		}
	}

	public static DateTime WeekCalcStartDateUtc { get; set; }

	public static DateTime WeekCalcEndDateUtc { get; set; }

	private static int WeekID_Rank { get; set; }

	private static int WeekID_Async { get; set; }

	private static int WeekID_League { get; set; }

	private static DateTime RankNextWeekIDResetDate { get; set; }

	private static DateTime AsyncNextWeekIDResetDate { get; set; }

	private static DateTime LeagueNextWeekIDResetDate { get; set; }

	public static void Initialize()
	{
		m_bLeagueDataReceived = false;
		m_bLeagueSeasonRewardReceived = false;
		m_lstLeagueRankerData.Clear();
	}

	public static string GetLeagueTop3Key()
	{
		return string.Format($"{NKCScenManager.CurrentUserData().m_UserUID}_LEAGUE_TOP3_UPDATE_TICK");
	}

	public static bool LoadFromLua()
	{
		bool flag = true;
		NKMLua nKMLua = new NKMLua();
		if (nKMLua.LoadCommonPath("AB_SCRIPT", "LUA_PVP_RANK") && nKMLua.OpenTable("PVP_RANK"))
		{
			int num = 1;
			while (nKMLua.OpenTable(num))
			{
				NKMPvpRankTemplet nKMPvpRankTemplet = new NKMPvpRankTemplet();
				flag &= nKMPvpRankTemplet.LoadFromLUA(nKMLua);
				if (!dicPvpRankTemplet.TryGetValue(nKMPvpRankTemplet.RankGroup, out var value))
				{
					value = new Dictionary<int, NKMPvpRankTemplet>();
					dicPvpRankTemplet.Add(nKMPvpRankTemplet.RankGroup, value);
				}
				value[nKMPvpRankTemplet.LeagueTier] = nKMPvpRankTemplet;
				num++;
				nKMLua.CloseTable();
			}
			nKMLua.CloseTable();
		}
		nKMLua.LuaClose();
		dicPvpRankSeasonTemplet = NKMTempletLoader.LoadDictionary("AB_SCRIPT", "LUA_PVP_RANK_SEASON", "PVP_RANK_SEASON", NKMPvpRankSeasonTemplet.LoadFromLUA);
		if (dicPvpRankSeasonTemplet == null)
		{
			Log.ErrorAndExit("PvpRankSeason load failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPVPManager.cs", 88);
			return false;
		}
		dicAsyncPvpSeasonTemplet = NKMTempletLoader.LoadDictionary("AB_SCRIPT", "LUA_PVP_ASYNC_SEASON", "PVP_ASYNC_SEASON", NKMPvpRankSeasonTemplet.LoadFromLUA);
		if (dicAsyncPvpSeasonTemplet == null)
		{
			Log.ErrorAndExit("PvpAsyncSeason load failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPVPManager.cs", 97);
			return false;
		}
		NKMPvpRankSeasonTemplet nKMPvpRankSeasonTemplet = null;
		foreach (NKMPvpRankSeasonTemplet value3 in dicAsyncPvpSeasonTemplet.Values)
		{
			value3.Validate();
			if (nKMPvpRankSeasonTemplet == null)
			{
				nKMPvpRankSeasonTemplet = value3;
				continue;
			}
			if (nKMPvpRankSeasonTemplet.SeasonID != value3.SeasonID - 1)
			{
				Log.ErrorAndExit($"Invalid Async SeasonId. Prev:{nKMPvpRankSeasonTemplet.SeasonID}, Cur:{value3.SeasonID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPVPManager.cs", 115);
				return false;
			}
			nKMPvpRankSeasonTemplet = value3;
		}
		NKMTempletContainer<NKMLeaguePvpRankSeasonTemplet>.Load("AB_SCRIPT", "LUA_PVP_LEAGUE_SEASON", "PVP_LEAGUE_SEASON", NKMLeaguePvpRankSeasonTemplet.LoadFromLUA);
		NKMLeaguePvpRankGroupTemplet.LoadFile();
		NKMPvpRankTemplet nKMPvpRankTemplet2 = null;
		List<NKMPvpRankTemplet> value2 = null;
		foreach (KeyValuePair<int, Dictionary<int, NKMPvpRankTemplet>> item in dicPvpRankTemplet)
		{
			foreach (KeyValuePair<int, NKMPvpRankTemplet> item2 in item.Value)
			{
				if (nKMPvpRankTemplet2 == null)
				{
					nKMPvpRankTemplet2 = item2.Value;
					continue;
				}
				int num2 = (item2.Value.LeaguePointReq - nKMPvpRankTemplet2.LeaguePointReq) / NKMPvpCommonConst.Instance.SCORE_MIN_INTERVAL_UNIT;
				for (int i = 0; i < num2; i++)
				{
					if (!listPvpRankTemplet.TryGetValue(item.Key, out value2))
					{
						value2 = new List<NKMPvpRankTemplet>();
						listPvpRankTemplet.Add(item.Key, value2);
					}
					value2.Add(nKMPvpRankTemplet2);
				}
				nKMPvpRankTemplet2 = item2.Value;
			}
			value2?.Add(nKMPvpRankTemplet2);
		}
		NKMTempletContainer<NKMLeaguePvpRankSeasonRewardGroupTemplet>.Load(from e in NKMTempletLoader<NKMLeaguePvpRankSeasonRewardTemplet>.LoadGroup("AB_SCRIPT", "LUA_PVP_LEAGUE_SEASON_REWARD", "PVP_LEAGUE_SEASON_REWARD", NKMLeaguePvpRankSeasonRewardTemplet.LoadFromLua)
			select new NKMLeaguePvpRankSeasonRewardGroupTemplet(e.Key, e.Value), null);
		NKMPvpRankSeasonRewardTempletManager.PvpRankSeasonRewardTemplets.Clear();
		NKMTempletContainer<NKMPvpRankSeasonRewardTemplet>.Load("AB_SCRIPT", "LUA_PVP_RANK_SEASON_REWARD", "PVP_RANK_SEASON_REWARD", NKMPvpRankSeasonRewardTemplet.LoadFromLua);
		foreach (NKMPvpRankSeasonRewardTemplet value4 in NKMTempletContainer<NKMPvpRankSeasonRewardTemplet>.Values)
		{
			NKMPvpRankSeasonRewardTempletManager.Add(value4);
		}
		m_bLeagueDataReceived = false;
		m_bLeagueSeasonRewardReceived = false;
		m_lstLeagueRankerData.Clear();
		return flag;
	}

	public static List<NKMLeaguePvpRankSeasonRewardTemplet> GetLeaguePvpSeasonRewardList(int seasonRewardGroupId)
	{
		return NKMLeaguePvpRankSeasonRewardGroupTemplet.Find(seasonRewardGroupId)?.GetRewardTempletList();
	}

	public static NKMLeaguePvpRankSeasonRewardTemplet GetLeaguePvpSeasonRewardTemplet(int seasonRewardGroupId, long userRank)
	{
		if (userRank == 0L)
		{
			return null;
		}
		NKMLeaguePvpRankSeasonRewardGroupTemplet nKMLeaguePvpRankSeasonRewardGroupTemplet = NKMLeaguePvpRankSeasonRewardGroupTemplet.Find(seasonRewardGroupId);
		if (nKMLeaguePvpRankSeasonRewardGroupTemplet != null)
		{
			foreach (NKMLeaguePvpRankSeasonRewardTemplet rewardTemplet in nKMLeaguePvpRankSeasonRewardGroupTemplet.GetRewardTempletList())
			{
				if (rewardTemplet.MinRank <= userRank && rewardTemplet.MaxRank >= userRank)
				{
					return rewardTemplet;
				}
			}
		}
		return null;
	}

	public static void Join()
	{
		foreach (NKMPvpRankSeasonTemplet value in dicPvpRankSeasonTemplet.Values)
		{
			value.Join();
		}
		foreach (NKMPvpRankSeasonTemplet value2 in dicAsyncPvpSeasonTemplet.Values)
		{
			value2.Join();
		}
	}

	public static void PostJoin()
	{
		foreach (NKMPvpRankSeasonTemplet value in dicPvpRankSeasonTemplet.Values)
		{
			value.PostJoin();
		}
		foreach (NKMPvpRankSeasonTemplet value2 in dicAsyncPvpSeasonTemplet.Values)
		{
			value2.PostJoin();
		}
	}

	public static int GetFinalTier(int tier)
	{
		if (tier == 0)
		{
			tier = 1;
		}
		return tier;
	}

	public static LEAGUE_TIER_ICON GetTierIconByScore(NKM_GAME_TYPE gameType, int seasonID, int score)
	{
		switch (gameType)
		{
		case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
		case NKM_GAME_TYPE.NGT_PVP_UNLIMITED:
		{
			NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet = NKMLeaguePvpRankSeasonTemplet.Find(seasonID);
			if (nKMLeaguePvpRankSeasonTemplet != null)
			{
				NKMLeaguePvpRankTemplet byScore = nKMLeaguePvpRankSeasonTemplet.RankGroup.GetByScore(score);
				if (byScore != null)
				{
					return byScore.LeagueTierIcon;
				}
			}
			break;
		}
		case NKM_GAME_TYPE.NGT_PVP_RANK:
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
		{
			NKMPvpRankTemplet rankTempletByScore = GetRankTempletByScore(gameType, seasonID, score);
			if (rankTempletByScore != null)
			{
				return rankTempletByScore.LeagueTierIcon;
			}
			break;
		}
		}
		return LEAGUE_TIER_ICON.LTI_NONE;
	}

	public static LEAGUE_TIER_ICON GetTierIconByTier(NKM_GAME_TYPE gameType, int seasonID, int leagueTier)
	{
		switch (gameType)
		{
		case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
		case NKM_GAME_TYPE.NGT_PVP_UNLIMITED:
		{
			if (NKMLeaguePvpRankTemplet.FindByTier(seasonID, leagueTier, out var templet))
			{
				return templet.LeagueTierIcon;
			}
			break;
		}
		case NKM_GAME_TYPE.NGT_PVP_RANK:
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
		{
			NKMPvpRankTemplet rankTempletByTier = GetRankTempletByTier(gameType, seasonID, leagueTier);
			if (rankTempletByTier != null)
			{
				return rankTempletByTier.LeagueTierIcon;
			}
			break;
		}
		}
		return LEAGUE_TIER_ICON.LTI_NONE;
	}

	public static int GetTierNumberByScore(NKM_GAME_TYPE gameType, int seasonID, int score)
	{
		switch (gameType)
		{
		case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
		case NKM_GAME_TYPE.NGT_PVP_UNLIMITED:
		{
			NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet = NKMLeaguePvpRankSeasonTemplet.Find(ServiceTime.Recent);
			if (nKMLeaguePvpRankSeasonTemplet != null)
			{
				NKMLeaguePvpRankTemplet byScore = nKMLeaguePvpRankSeasonTemplet.RankGroup.GetByScore(score);
				if (byScore != null)
				{
					return byScore.LeagueTierIconNumber;
				}
			}
			break;
		}
		case NKM_GAME_TYPE.NGT_PVP_RANK:
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
		{
			NKMPvpRankTemplet rankTempletByScore = GetRankTempletByScore(gameType, seasonID, score);
			if (rankTempletByScore != null)
			{
				return rankTempletByScore.LeagueTierIconNumber;
			}
			break;
		}
		}
		return 0;
	}

	public static int GetTierNumberByTier(NKM_GAME_TYPE gameType, int seasonID, int leagueTier)
	{
		switch (gameType)
		{
		case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
		case NKM_GAME_TYPE.NGT_PVP_UNLIMITED:
		{
			if (NKMLeaguePvpRankTemplet.FindByTier(seasonID, leagueTier, out var templet))
			{
				return templet.LeagueTierIconNumber;
			}
			break;
		}
		case NKM_GAME_TYPE.NGT_PVP_RANK:
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
		{
			NKMPvpRankTemplet rankTempletByTier = GetRankTempletByTier(gameType, seasonID, leagueTier);
			if (rankTempletByTier != null)
			{
				return rankTempletByTier.LeagueTierIconNumber;
			}
			break;
		}
		}
		return 0;
	}

	public static string GetLeagueNameByTier(NKM_GAME_TYPE gameType, int seasonID, int leagueTier)
	{
		switch (gameType)
		{
		case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
		case NKM_GAME_TYPE.NGT_PVP_UNLIMITED:
		{
			if (NKMLeaguePvpRankTemplet.FindByTier(seasonID, leagueTier, out var templet))
			{
				return NKCStringTable.GetString(templet.LeagueName);
			}
			break;
		}
		case NKM_GAME_TYPE.NGT_PVP_RANK:
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
		{
			NKMPvpRankTemplet rankTempletByTier = GetRankTempletByTier(gameType, seasonID, leagueTier);
			if (rankTempletByTier != null)
			{
				return rankTempletByTier.GetLeagueName();
			}
			break;
		}
		}
		return "";
	}

	public static NKMPvpRankTemplet GetPvpRankTempletByTier(int seasonID, int leagueTier)
	{
		if (!dicPvpRankSeasonTemplet.TryGetValue(seasonID, out var value))
		{
			return null;
		}
		int rankGroup = value.RankGroup;
		if (!dicPvpRankTemplet.TryGetValue(rankGroup, out var value2))
		{
			return null;
		}
		leagueTier = GetFinalTier(leagueTier);
		if (value2.TryGetValue(leagueTier, out var value3))
		{
			return value3;
		}
		int num = dicPvpRankTemplet.Keys.Max();
		if (leagueTier > num && value2.TryGetValue(num, out value3))
		{
			return value3;
		}
		Log.Warn($"[PvpRankTemplet] templet not found. seasonId:{seasonID} tierId:{leagueTier}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPVPManager.cs", 411);
		return null;
	}

	public static NKMPvpRankTemplet GetPvpRankTempletByScore(int seasonID, int leagueScore)
	{
		if (leagueScore < 0)
		{
			return null;
		}
		NKMPvpRankSeasonTemplet pvpRankSeasonTemplet = GetPvpRankSeasonTemplet(seasonID);
		if (pvpRankSeasonTemplet == null)
		{
			return null;
		}
		int rankGroup = pvpRankSeasonTemplet.RankGroup;
		if (rankGroup == 0)
		{
			return null;
		}
		if (!listPvpRankTemplet.TryGetValue(rankGroup, out var value))
		{
			return null;
		}
		int num = leagueScore / NKMPvpCommonConst.Instance.SCORE_MIN_INTERVAL_UNIT;
		if (num >= value.Count)
		{
			return value[value.Count - 1];
		}
		return value[num];
	}

	public static NKMPvpRankSeasonTemplet GetPvpRankSeasonTemplet(int seasonID)
	{
		dicPvpRankSeasonTemplet.TryGetValue(seasonID, out var value);
		return value;
	}

	public static NKM_ERROR_CODE CanPlayPVPRankGame(NKMUserData userData, int seasonID, int weekID, DateTime current)
	{
		if (!userData.m_RankOpen)
		{
			return NKM_ERROR_CODE.NEC_FAIL_PVP_INSUFFICIENT_OPEN_SCORE;
		}
		if (!dicPvpRankSeasonTemplet.TryGetValue(seasonID, out var value))
		{
			return NKM_ERROR_CODE.NEC_FAIL_INVALID_PVP_SEASON_DATA;
		}
		if (value.EndDate < current)
		{
			return NKM_ERROR_CODE.NEC_FAIL_END_SEASON;
		}
		if (WeekCalcStartDateUtc < current && current < WeekCalcEndDateUtc)
		{
			return NKM_ERROR_CODE.NEC_FAIL_END_WEEK;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE CanPlayPVPLeagueGame(NKMUserData userData, int seasonID, DateTime currentUTC)
	{
		if (!userData.m_LeagueOpen)
		{
			return NKM_ERROR_CODE.NEC_FAIL_PVP_INSUFFICIENT_OPEN_SCORE;
		}
		if (userData.m_ArmyData.GetUnitTypeCount() < NKMPvpCommonConst.Instance.DraftBan.MinUnitCount)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DRAFT_PVP_NOT_ENOUGH_UNIT_COUNT;
		}
		if (userData.m_ArmyData.GetShipTypeCount() < NKMPvpCommonConst.Instance.DraftBan.MinShipCount)
		{
			return NKM_ERROR_CODE.NEC_FAIL_DRAFT_PVP_NOT_ENOUGH_SHIP_COUNT;
		}
		NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet = NKMLeaguePvpRankSeasonTemplet.Find(seasonID);
		if (nKMLeaguePvpRankSeasonTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_INVALID_PVP_SEASON_DATA;
		}
		if (!NKMPvpCommonConst.Instance.LeaguePvp.IsValidTime(ServiceTime.Recent))
		{
			return NKM_ERROR_CODE.NEC_FAIL_DRAFT_PVP_INVALID_TIME;
		}
		if (!nKMLeaguePvpRankSeasonTemplet.SeasonGameEnable(ServiceTime.Now))
		{
			return NKM_ERROR_CODE.NEC_FAIL_DRAFT_PVP_INVALID_TIME;
		}
		if (!nKMLeaguePvpRankSeasonTemplet.SeasonEnable(ServiceTime.Now))
		{
			return NKM_ERROR_CODE.NEC_FAIL_END_SEASON;
		}
		if (NKMPvpCommonConst.Instance.LeaguePvp.CalculateTimeStart <= ServiceTime.Now.TimeOfDay)
		{
			TimeSpan ts = new TimeSpan(NKMPvpCommonConst.Instance.LeaguePvp.CalculateTimeInterval, 0, 0);
			if (NKMPvpCommonConst.Instance.LeaguePvp.CalculateTimeStart.Add(ts) > ServiceTime.Now.TimeOfDay)
			{
				return NKM_ERROR_CODE.NEC_FAIL_DRAFT_PVP_INVALID_TIME;
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE CanPlayUnlimitedGame(NKMUserData userData, int seasonID, DateTime currentUTC)
	{
		if (!userData.m_LeagueOpen)
		{
			return NKM_ERROR_CODE.NEC_FAIL_PVP_INSUFFICIENT_OPEN_SCORE;
		}
		NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet = NKMLeaguePvpRankSeasonTemplet.Find(seasonID);
		if (nKMLeaguePvpRankSeasonTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_INVALID_PVP_SEASON_DATA;
		}
		if (!NKMPvpCommonConst.Instance.LeaguePvp.IsValidTime(ServiceTime.Recent))
		{
			return NKM_ERROR_CODE.NEC_FAIL_DRAFT_PVP_INVALID_TIME;
		}
		if (!nKMLeaguePvpRankSeasonTemplet.SeasonGameEnable(ServiceTime.Now))
		{
			return NKM_ERROR_CODE.NEC_FAIL_DRAFT_PVP_INVALID_TIME;
		}
		if (!nKMLeaguePvpRankSeasonTemplet.SeasonEnable(ServiceTime.Now))
		{
			return NKM_ERROR_CODE.NEC_FAIL_END_SEASON;
		}
		if (NKMPvpCommonConst.Instance.LeaguePvp.CalculateTimeStart <= ServiceTime.Now.TimeOfDay)
		{
			TimeSpan ts = new TimeSpan(NKMPvpCommonConst.Instance.LeaguePvp.CalculateTimeInterval, 0, 0);
			if (NKMPvpCommonConst.Instance.LeaguePvp.CalculateTimeStart.Add(ts) > ServiceTime.Now.TimeOfDay)
			{
				return NKM_ERROR_CODE.NEC_FAIL_DRAFT_PVP_INVALID_TIME;
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE CanRewardWeek(NKM_GAME_TYPE gameType, PvpState pvpData, int seasonID, int weekID, DateTime todayUTC)
	{
		if (pvpData.SeasonID == 0)
		{
			return NKM_ERROR_CODE.NEC_FAIL_PVP_SEASON_ID_0;
		}
		if (pvpData.SeasonID != seasonID)
		{
			return NKM_ERROR_CODE.NEC_FAIL_INVALID_PVP_SEASON_DATA;
		}
		if (gameType != NKM_GAME_TYPE.NGT_PVP_LEAGUE && gameType != NKM_GAME_TYPE.NGT_PVP_UNLIMITED)
		{
			if (pvpData.WeekID == 0)
			{
				return NKM_ERROR_CODE.NEC_FAIL_PVP_WEEK_ID_0;
			}
			if (pvpData.WeekID == weekID)
			{
				return NKM_ERROR_CODE.NEC_FAIL_ALREADY_REWARD_WEEK_DATA;
			}
			if (WeekCalcStartDateUtc < todayUTC && todayUTC < WeekCalcEndDateUtc)
			{
				return NKM_ERROR_CODE.NEC_FAIL_END_WEEK;
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE CanRewardSeason(PvpState pvpData, int seasonID, DateTime todayUTC)
	{
		if (pvpData.SeasonID == 0 || pvpData.SeasonID != seasonID - 1)
		{
			return NKM_ERROR_CODE.NEC_FAIL_INVALID_PVP_SEASON_DATA;
		}
		if (pvpData.SeasonID == seasonID)
		{
			return NKM_ERROR_CODE.NEC_FAIL_ALREADY_REWARD_SEASON_DATA;
		}
		if (WeekCalcStartDateUtc < todayUTC && todayUTC < WeekCalcEndDateUtc)
		{
			return NKM_ERROR_CODE.NEC_FAIL_END_WEEK;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static long GetLastUpdateChargePointTicks()
	{
		return NKCScenManager.CurrentUserData().LastPvpPointChargeTimeUTC.Ticks;
	}

	public static NKMPvpRankTemplet GetAsyncPvpRankTempletByTier(int seasonID, int leagueTier)
	{
		if (!dicAsyncPvpSeasonTemplet.TryGetValue(seasonID, out var value))
		{
			return null;
		}
		int rankGroup = value.RankGroup;
		if (!dicPvpRankTemplet.TryGetValue(rankGroup, out var value2))
		{
			return null;
		}
		leagueTier = GetFinalTier(leagueTier);
		if (value2.TryGetValue(leagueTier, out var value3))
		{
			return value3;
		}
		int num = dicPvpRankTemplet.Keys.Max();
		if (leagueTier > num && value2.TryGetValue(num, out value3))
		{
			return value3;
		}
		Log.Warn($"[PvpRankTemplet] templet not found. seasonId:{seasonID} tierId:{leagueTier}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPVPManager.cs", 686);
		return null;
	}

	public static NKMPvpRankTemplet GetAsyncPvpRankTempletByScore(int seasonID, int leagueScore)
	{
		if (leagueScore < 0)
		{
			return null;
		}
		NKMPvpRankSeasonTemplet pvpAsyncSeasonTemplet = GetPvpAsyncSeasonTemplet(seasonID);
		if (pvpAsyncSeasonTemplet == null)
		{
			return null;
		}
		int rankGroup = pvpAsyncSeasonTemplet.RankGroup;
		if (rankGroup == 0)
		{
			return null;
		}
		if (!listPvpRankTemplet.TryGetValue(rankGroup, out var value))
		{
			return null;
		}
		int num = leagueScore / NKMPvpCommonConst.Instance.SCORE_MIN_INTERVAL_UNIT;
		if (num >= value.Count)
		{
			return value[value.Count - 1];
		}
		return value[num];
	}

	public static NKMPvpRankSeasonTemplet GetPvpAsyncSeasonTemplet(int seasonID)
	{
		dicAsyncPvpSeasonTemplet.TryGetValue(seasonID, out var value);
		return value;
	}

	public static void Init(DateTime date)
	{
		ReloadWeekIDForRank(date, NKCUtil.FindPVPSeasonIDForRank(NKCSynchronizedTime.GetServerUTCTime()));
		ReloadWeekIDForAsync(date, NKCUtil.FindPVPSeasonIDForAsync(NKCSynchronizedTime.GetServerUTCTime()));
	}

	public static int GetResetScore(int seasonID, int orgScore, NKM_GAME_TYPE gameType)
	{
		switch (gameType)
		{
		case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
		case NKM_GAME_TYPE.NGT_PVP_UNLIMITED:
			return GetLeagueResetScore(orgScore);
		default:
		{
			NKMPvpRankTemplet rankTempletByScore = GetRankTempletByScore(gameType, seasonID, orgScore);
			if (rankTempletByScore != null && rankTempletByScore.LeagueDemotePoint < orgScore)
			{
				return rankTempletByScore.LeagueDemotePoint;
			}
			return orgScore;
		}
		}
	}

	private static int GetLeagueResetScore(int orgScore)
	{
		if (NKMPvpCommonConst.Instance.LEAGUE_PVP_RESET_SCORE < orgScore)
		{
			return NKMPvpCommonConst.Instance.LEAGUE_PVP_RESET_SCORE;
		}
		return orgScore;
	}

	public static bool IsRewardWeek(int seasonID, DateTime weekCalcStartDate)
	{
		if (!dicPvpRankSeasonTemplet.TryGetValue(seasonID, out var value))
		{
			return false;
		}
		return weekCalcStartDate != value.EndDate;
	}

	public static bool IsRewardWeek(NKMPvpRankSeasonTemplet seasonTemplet, DateTime weekCalcStartDate)
	{
		if (seasonTemplet == null)
		{
			return false;
		}
		return weekCalcStartDate != seasonTemplet.EndDate;
	}

	public static int GetWeekIDForRank(DateTime today, int seasonID)
	{
		if (RankNextWeekIDResetDate < today)
		{
			ReloadWeekIDForRank(today, seasonID);
		}
		return WeekID_Rank;
	}

	public static int GetWeekIDForAsync(DateTime today, int seasonID)
	{
		if (AsyncNextWeekIDResetDate < today)
		{
			ReloadWeekIDForAsync(today, seasonID);
		}
		return WeekID_Async;
	}

	public static int GetWeekIDForLeague(DateTime today, int seasonID)
	{
		if (LeagueNextWeekIDResetDate < today)
		{
			ReloadWeekIDForLeague(today, seasonID);
		}
		return WeekID_League;
	}

	public static NKMPvpRankTemplet GetRankTempletByRankGroupScore(int rankGroup, int leagueScore)
	{
		if (leagueScore < 0)
		{
			return null;
		}
		if (rankGroup == 0)
		{
			return null;
		}
		if (!listPvpRankTemplet.TryGetValue(rankGroup, out var value))
		{
			return null;
		}
		int num = leagueScore / NKMPvpCommonConst.Instance.SCORE_MIN_INTERVAL_UNIT;
		if (num >= value.Count)
		{
			return value[value.Count - 1];
		}
		return value[num];
	}

	public static NKMPvpRankTemplet GetRankTempletByRankGroupTier(int rankGroup, int leagueTier)
	{
		if (!dicPvpRankTemplet.TryGetValue(rankGroup, out var value))
		{
			return null;
		}
		leagueTier = GetFinalTier(leagueTier);
		if (!value.TryGetValue(leagueTier, out var value2))
		{
			return null;
		}
		return value2;
	}

	private static void ReloadWeekIDForRank(DateTime today, int seasonID)
	{
		dicPvpRankSeasonTemplet.TryGetValue(seasonID, out var value);
		if (value == null)
		{
			Log.Error($"Invalid SeasonID seasonID : {seasonID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPVPManager.cs", 855);
			return;
		}
		int num = (int)((today - value.StartDate).TotalDays / 7.0) + 1;
		if (num != WeekID_Rank)
		{
			ReloadWeekCalcDate(today);
			WeekID_Rank = num;
		}
		RankNextWeekIDResetDate = NKMTime.GetNextResetTime(today, NKMTime.TimePeriod.Day, WEEK_CALC_END_TIME);
	}

	private static void ReloadWeekIDForAsync(DateTime today, int seasonID)
	{
		dicAsyncPvpSeasonTemplet.TryGetValue(seasonID, out var value);
		if (value == null)
		{
			Log.Error($"Invalid SeasonID seasonID : {seasonID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPVPManager.cs", 876);
			return;
		}
		int num = (int)((today - value.StartDate).TotalDays / 7.0) + 1;
		if (num != WeekID_Async)
		{
			ReloadWeekCalcDate(today);
			WeekID_Async = num;
		}
		AsyncNextWeekIDResetDate = NKMTime.GetNextResetTime(today, NKMTime.TimePeriod.Day, WEEK_CALC_END_TIME);
	}

	private static void ReloadWeekIDForLeague(DateTime today, int seasonID)
	{
		NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet = NKMLeaguePvpRankSeasonTemplet.Find(seasonID);
		if (nKMLeaguePvpRankSeasonTemplet == null)
		{
			Log.Error($"Invalid League SeasonID seasonID : {seasonID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCPVPManager.cs", 897);
			return;
		}
		int num = (int)((today - nKMLeaguePvpRankSeasonTemplet.StartDateUTC).TotalDays / 7.0) + 1;
		if (num != WeekID_League)
		{
			ReloadWeekCalcDate(today);
			WeekID_League = num;
		}
		LeagueNextWeekIDResetDate = NKMTime.GetNextResetTime(today, NKMTime.TimePeriod.Day, WEEK_CALC_END_TIME);
	}

	private static void ReloadWeekCalcDate(DateTime today)
	{
		WeekCalcEndDateUtc = NKMTime.GetNextResetTime(today, NKMTime.TimePeriod.Week, WEEK_CALC_END_TIME);
		WeekCalcStartDateUtc = WeekCalcEndDateUtc.AddHours(-6.0);
	}

	public static NKM_ERROR_CODE CanPlayPVPAsyncGame(NKMUserData userData, int seasonID, int weekID, DateTime currentDate)
	{
		if (!dicAsyncPvpSeasonTemplet.TryGetValue(seasonID, out var value))
		{
			return NKM_ERROR_CODE.NEC_FAIL_INVALID_PVP_SEASON_DATA;
		}
		if (value.EndDate < currentDate)
		{
			return NKM_ERROR_CODE.NEC_FAIL_END_SEASON;
		}
		if (WeekCalcStartDateUtc < currentDate && currentDate < WeekCalcEndDateUtc)
		{
			return NKM_ERROR_CODE.NEC_FAIL_END_WEEK;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static bool IsPvpRankUnlocked()
	{
		return NKCScenManager.CurrentUserData()?.m_RankOpen ?? false;
	}

	public static bool IsPvpLeagueUnlocked()
	{
		return NKCScenManager.CurrentUserData()?.m_LeagueOpen ?? false;
	}

	public static string GetLeagueOpenDaysString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < NKMPvpCommonConst.Instance.LeaguePvp.OpenDaysOfWeek.Count; i++)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(" / ");
			}
			stringBuilder.Append(NKCUtilString.GetDayString(NKMPvpCommonConst.Instance.LeaguePvp.OpenDaysOfWeek[i]));
		}
		return stringBuilder.ToString();
	}

	public static string GetLeagueOpenTimeString()
	{
		return $"{NKMPvpCommonConst.Instance.LeaguePvp.OpenTimeStart.Hours:00}:{NKMPvpCommonConst.Instance.LeaguePvp.OpenTimeStart.Minutes:00} ~ {NKMPvpCommonConst.Instance.LeaguePvp.OpenTimeEnd.Hours:00}:{NKMPvpCommonConst.Instance.LeaguePvp.OpenTimeEnd.Minutes:00}";
	}

	public static void OnRecv(NKMPacket_LEAGUE_PVP_SEASON_INFO_ACK sPacket)
	{
		m_bLeagueSeasonRewardReceived = sPacket.seasonRewardReceived;
		m_lstLeagueRankerData = new List<List<LeaderBoardSlotData>>();
		for (int i = 0; i < sPacket.rankerDatas.Count; i++)
		{
			List<LeaderBoardSlotData> list = new List<LeaderBoardSlotData>();
			NKMLeaguePvpSeasonRanker nKMLeaguePvpSeasonRanker = sPacket.rankerDatas[i];
			for (int j = 0; j < nKMLeaguePvpSeasonRanker.profiles.Count; j++)
			{
				if (nKMLeaguePvpSeasonRanker.profiles[j].commonProfile != null && nKMLeaguePvpSeasonRanker.profiles[j].commonProfile.userUid > 0)
				{
					LeaderBoardSlotData item = LeaderBoardSlotData.MakeSlotData(LeaderBoardType.BT_LEAGUE, nKMLeaguePvpSeasonRanker.profiles[j], j + 1, nKMLeaguePvpSeasonRanker.seasonId);
					list.Add(item);
				}
			}
			if (list.Count > 0)
			{
				m_lstLeagueRankerData.Add(list);
			}
		}
		m_bLeagueDataReceived = true;
		if (NKCUIModuleHome.IsAnyInstanceOpen())
		{
			NKCUIModuleHome.SendMessage(new NKCUIModuleSubUIDraft.EventModuleMessageDataDraft());
		}
	}

	public static string GetRankingTotalDesc(LeaderBoardType leaderBoardType)
	{
		NKMLeaderBoardTemplet nKMLeaderBoardTemplet = NKMLeaderBoardTemplet.Find(leaderBoardType, 0);
		if (nKMLeaderBoardTemplet != null)
		{
			if (NKCLeaderBoardManager.GetMyRankSlotData(nKMLeaderBoardTemplet.m_BoardID).rank > 0)
			{
				return string.Format(NKCUtilString.GET_FIERCE_RANK_IN_TOP_100_DESC_01, NKCLeaderBoardManager.GetMyRankSlotData(nKMLeaderBoardTemplet.m_BoardID).rank);
			}
			return string.Format(NKCUtilString.GET_FIERCE_RANK_IN_TOP_100_DESC_01, "-");
		}
		return "";
	}

	public static int FindPvPSeasonID(NKM_GAME_TYPE gameType, DateTime nowUTC)
	{
		switch (gameType)
		{
		case NKM_GAME_TYPE.NGT_PVP_RANK:
			return NKCUtil.FindPVPSeasonIDForRank(nowUTC);
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
			return NKCUtil.FindPVPSeasonIDForAsync(nowUTC);
		case NKM_GAME_TYPE.NGT_PVP_LEAGUE:
		case NKM_GAME_TYPE.NGT_PVP_UNLIMITED:
		{
			NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet = NKMLeaguePvpRankSeasonTemplet.Find(NKCSynchronizedTime.ToServerServiceTime(nowUTC));
			if (nKMLeaguePvpRankSeasonTemplet != null)
			{
				return nKMLeaguePvpRankSeasonTemplet.SeasonId;
			}
			break;
		}
		}
		return 0;
	}

	public static NKMPvpRankTemplet GetRankTempletByScore(NKM_GAME_TYPE gameType, int seasonID, int leagueScore)
	{
		switch (gameType)
		{
		case NKM_GAME_TYPE.NGT_PVP_RANK:
			return GetPvpRankTempletByScore(seasonID, leagueScore);
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
			return GetAsyncPvpRankTempletByScore(seasonID, leagueScore);
		default:
			return null;
		}
	}

	public static NKMPvpRankTemplet GetRankTempletByTier(NKM_GAME_TYPE gameType, int seasonID, int leagueTier)
	{
		switch (gameType)
		{
		case NKM_GAME_TYPE.NGT_PVP_RANK:
			return GetPvpRankTempletByTier(seasonID, leagueTier);
		case NKM_GAME_TYPE.NGT_ASYNC_PVP:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_REVENGE:
		case NKM_GAME_TYPE.NGT_PVP_STRATEGY_NPC:
			return GetAsyncPvpRankTempletByTier(seasonID, leagueTier);
		default:
			return null;
		}
	}
}
