using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMLeaguePvpRankSeasonRewardTemplet : INKMTemplet
{
	public const int RewardCount = 5;

	private int idx;

	private int seasonRewardGroupId;

	private int minRank;

	private int maxRank;

	public int Key => seasonRewardGroupId;

	public int SeasonRewardGroupId => seasonRewardGroupId;

	public int MinRank => minRank;

	public int MaxRank => maxRank;

	public int Index => idx;

	public List<NKMRewardInfo> Rewards { get; private set; }

	public static NKMLeaguePvpRankSeasonRewardTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMLeaguePvpRankSeasonRewardTemplet.cs", 26))
		{
			return null;
		}
		bool flag = true;
		NKMLeaguePvpRankSeasonRewardTemplet nKMLeaguePvpRankSeasonRewardTemplet = new NKMLeaguePvpRankSeasonRewardTemplet();
		lua.GetData("IDX", ref nKMLeaguePvpRankSeasonRewardTemplet.idx);
		lua.GetData("SeasonRewardGroupId", ref nKMLeaguePvpRankSeasonRewardTemplet.seasonRewardGroupId);
		lua.GetData("MinRank", ref nKMLeaguePvpRankSeasonRewardTemplet.minRank);
		lua.GetData("MaxRank", ref nKMLeaguePvpRankSeasonRewardTemplet.maxRank);
		nKMLeaguePvpRankSeasonRewardTemplet.Rewards = new List<NKMRewardInfo>();
		for (int i = 0; i < 5; i++)
		{
			NKMRewardInfo nKMRewardInfo = new NKMRewardInfo();
			if (lua.GetData($"m_RewardID_{i + 1}", ref nKMRewardInfo.ID))
			{
				lua.GetData($"m_RewardType_{i + 1}", ref nKMRewardInfo.rewardType);
				lua.GetData($"m_RewardValue_{i + 1}", ref nKMRewardInfo.Count);
				nKMLeaguePvpRankSeasonRewardTemplet.Rewards.Add(nKMRewardInfo);
			}
		}
		if (!flag)
		{
			return null;
		}
		return nKMLeaguePvpRankSeasonRewardTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
