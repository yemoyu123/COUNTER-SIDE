using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM.Templet;

public static class NKMPvpRankSeasonRewardTempletManager
{
	public static Dictionary<int, List<NKMPvpRankSeasonRewardTemplet>> PvpRankSeasonRewardTemplets = new Dictionary<int, List<NKMPvpRankSeasonRewardTemplet>>();

	public static void LoadFromLua()
	{
		if (NKMOpenTagManager.IsSystemOpened(SystemOpenTagType.RANK_SEASON_REWARD))
		{
			PvpRankSeasonRewardTemplets.Clear();
			NKMTempletContainer<NKMPvpRankSeasonRewardTemplet>.Load("AB_SCRIPT", "LUA_PVP_RANK_SEASON_REWARD", "PVP_RANK_SEASON_REWARD", NKMPvpRankSeasonRewardTemplet.LoadFromLua);
		}
	}

	public static void Add(NKMPvpRankSeasonRewardTemplet templet)
	{
		if (PvpRankSeasonRewardTemplets.TryGetValue(templet.SeasonRewardGroupId, out var value))
		{
			value.Add(templet);
			return;
		}
		value = new List<NKMPvpRankSeasonRewardTemplet>();
		value.Add(templet);
		PvpRankSeasonRewardTemplets.Add(templet.SeasonRewardGroupId, value);
	}

	public static NKMPvpRankSeasonRewardTemplet GetPvpSeasonReward(int seasonRewardGroupId, long userRank)
	{
		if (userRank == 0L)
		{
			return null;
		}
		if (PvpRankSeasonRewardTemplets.TryGetValue(seasonRewardGroupId, out var value))
		{
			foreach (NKMPvpRankSeasonRewardTemplet item in value)
			{
				if (item.EnableByTag && item.MinRank <= userRank && item.MaxRank >= userRank)
				{
					return item;
				}
			}
		}
		return null;
	}
}
