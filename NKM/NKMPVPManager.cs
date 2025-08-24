using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKMPVPManager
{
	public static Dictionary<int, Dictionary<int, NKMPvpRankTemplet>> dicPvpRankTemplet = new Dictionary<int, Dictionary<int, NKMPvpRankTemplet>>();

	public static Dictionary<int, List<NKMPvpRankTemplet>> listPvpRankTemplet = new Dictionary<int, List<NKMPvpRankTemplet>>();

	public static Dictionary<int, NKMPvpRankSeasonTemplet> dicPvpRankSeasonTemplet = new Dictionary<int, NKMPvpRankSeasonTemplet>();

	public static Dictionary<int, NKMPvpRankSeasonTemplet> dicAsyncPvpSeasonTemplet = new Dictionary<int, NKMPvpRankSeasonTemplet>();

	public static DateTime WeekCalcStartDateUtc { get; set; }

	public static DateTime WeekCalcEndDateUtc { get; set; }

	public static DateTime ScoreBaseTime { get; private set; } = new DateTime(2080, 1, 1, 0, 0, 0);

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
			Log.ErrorAndExit("PvpRankSeason load failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPManager.cs", 56);
			return false;
		}
		dicAsyncPvpSeasonTemplet = NKMTempletLoader.LoadDictionary("AB_SCRIPT", "LUA_PVP_ASYNC_SEASON", "PVP_ASYNC_SEASON", NKMPvpRankSeasonTemplet.LoadFromLUA);
		if (dicAsyncPvpSeasonTemplet == null)
		{
			Log.ErrorAndExit("PvpAsyncSeason load failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPManager.cs", 65);
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
				Log.ErrorAndExit($"Invalid Async SeasonId. Prev:{nKMPvpRankSeasonTemplet.SeasonID}, Cur:{value3.SeasonID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPManager.cs", 82);
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
		return flag;
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

	public static void Validate()
	{
		foreach (NKMPvpRankTemplet item in listPvpRankTemplet.SelectMany((KeyValuePair<int, List<NKMPvpRankTemplet>> e) => e.Value))
		{
			if (NKMGameStatRateTemplet.Find(item.m_GameStatRateID) == null)
			{
				NKMTempletError.Add("PvpRankTemplet 의 m_GameStatRateID(" + item.m_GameStatRateID + ") 에 해당하는 GameStatRateTemplet 이 존재하지 않음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPManager.cs", 148);
			}
		}
		NKMLeaguePvpRankSeasonTemplet nKMLeaguePvpRankSeasonTemplet = null;
		foreach (NKMLeaguePvpRankSeasonTemplet value in NKMTempletContainer<NKMLeaguePvpRankSeasonTemplet>.Values)
		{
			if (nKMLeaguePvpRankSeasonTemplet == null)
			{
				nKMLeaguePvpRankSeasonTemplet = value;
				continue;
			}
			if (nKMLeaguePvpRankSeasonTemplet.SeasonId != value.SeasonId - 1)
			{
				NKMTempletError.Add($"Invalid League SeasonId. Prev:{nKMLeaguePvpRankSeasonTemplet.SeasonId}, Cur:{value.SeasonId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPManager.cs", 163);
			}
			if (value.Interval.StartDate - nKMLeaguePvpRankSeasonTemplet.Interval.EndDate > TimeSpan.FromDays(1.0))
			{
				NKMTempletError.Add($"[LeaguePvp] {nKMLeaguePvpRankSeasonTemplet.Name} 종료:{nKMLeaguePvpRankSeasonTemplet.Interval.EndDate}, {value.Name} 시작:{value.Interval.StartDate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPManager.cs", 168);
			}
			nKMLeaguePvpRankSeasonTemplet = value;
		}
		NKMPvpRankSeasonTemplet nKMPvpRankSeasonTemplet = null;
		foreach (NKMPvpRankSeasonTemplet value2 in dicPvpRankSeasonTemplet.Values)
		{
			value2.Validate();
			if (nKMPvpRankSeasonTemplet == null)
			{
				nKMPvpRankSeasonTemplet = value2;
				continue;
			}
			if (nKMPvpRankSeasonTemplet.SeasonID != value2.SeasonID - 1)
			{
				NKMTempletError.Add($"Invalid Rank SeasonId. Prev:{nKMPvpRankSeasonTemplet.SeasonID} Cur:{value2.SeasonID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPManager.cs", 187);
			}
			if (value2.Interval.StartDate - nKMPvpRankSeasonTemplet.Interval.EndDate > TimeSpan.FromDays(1.0))
			{
				NKMTempletError.Add($"[RankPvp] {nKMPvpRankSeasonTemplet.Name} 종료:{nKMPvpRankSeasonTemplet.Interval.EndDate}, {value2.Name} 시작:{value2.Interval.StartDate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMPVPManager.cs", 192);
			}
			nKMPvpRankSeasonTemplet = value2;
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
		if (!value2.TryGetValue(leagueTier, out var value3))
		{
			return null;
		}
		return value3;
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
		if (userData.m_PvpData.SeasonID != 0 && userData.m_PvpData.SeasonID == seasonID && userData.m_PvpData.WeekID != 0 && userData.m_PvpData.WeekID != weekID)
		{
			return NKM_ERROR_CODE.NEC_FAIL_HAVE_NOT_BEEN_REWARDED;
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

	public static NKM_ERROR_CODE CanRewardWeek(PvpState pvpData, int seasonID, int weekID, DateTime today)
	{
		if (pvpData.SeasonID == 0)
		{
			return NKM_ERROR_CODE.NEC_FAIL_PVP_SEASON_ID_0;
		}
		if (pvpData.WeekID == 0)
		{
			return NKM_ERROR_CODE.NEC_FAIL_PVP_WEEK_ID_0;
		}
		if (pvpData.SeasonID != seasonID)
		{
			return NKM_ERROR_CODE.NEC_FAIL_INVALID_PVP_SEASON_DATA;
		}
		if (pvpData.WeekID == weekID)
		{
			return NKM_ERROR_CODE.NEC_FAIL_ALREADY_REWARD_WEEK_DATA;
		}
		if (WeekCalcStartDateUtc < today && today < WeekCalcEndDateUtc)
		{
			return NKM_ERROR_CODE.NEC_FAIL_END_WEEK;
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static NKM_ERROR_CODE CanRewardSeason(PvpState pvpData, int seasonID, DateTime today)
	{
		if (pvpData.SeasonID == 0 || pvpData.SeasonID != seasonID - 1)
		{
			return NKM_ERROR_CODE.NEC_FAIL_INVALID_PVP_SEASON_DATA;
		}
		if (pvpData.SeasonID == seasonID)
		{
			return NKM_ERROR_CODE.NEC_FAIL_ALREADY_REWARD_SEASON_DATA;
		}
		if (WeekCalcStartDateUtc < today && today < WeekCalcEndDateUtc)
		{
			return NKM_ERROR_CODE.NEC_FAIL_END_WEEK;
		}
		return NKM_ERROR_CODE.NEC_OK;
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
		if (!value2.TryGetValue(leagueTier, out var value3))
		{
			return null;
		}
		return value3;
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
}
