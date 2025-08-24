using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMPvpRankSeasonRewardTemplet : INKMTemplet
{
	public int Idx;

	public int SeasonRewardGroupId;

	public int MinRank;

	public int MaxRank;

	public List<NKMRewardInfo> RewardList = new List<NKMRewardInfo>();

	public string OpenTag;

	public int Key => Idx;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(OpenTag);

	public void Join()
	{
	}

	public void Validate()
	{
		if (!EnableByTag)
		{
			return;
		}
		foreach (NKMRewardInfo reward in RewardList)
		{
			if (!NKMRewardTemplet.IsOpenedReward(reward.rewardType, reward.ID, useRandomContract: false))
			{
				NKMTempletError.Add($"[NKMPvpRankSeasonRewardTemplet: {Key}] \ufffd\ufffd\ufffd\ufffd \ufffd±װ\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\u05bd\ufffd\ufffdϴ\ufffd RewardId[{reward.ID}]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMPvpRankSeasonRewardTemplet.cs", 95);
			}
		}
		NKMPvpRankSeasonRewardTempletManager.Add(this);
	}

	public static NKMPvpRankSeasonRewardTemplet LoadFromLua(NKMLua lua)
	{
		NKMPvpRankSeasonRewardTemplet nKMPvpRankSeasonRewardTemplet = new NKMPvpRankSeasonRewardTemplet();
		lua.GetData("IDX", ref nKMPvpRankSeasonRewardTemplet.Idx);
		lua.GetData("SeasonRewardGroupId", ref nKMPvpRankSeasonRewardTemplet.SeasonRewardGroupId);
		lua.GetData("MinRank", ref nKMPvpRankSeasonRewardTemplet.MinRank);
		lua.GetData("MaxRank", ref nKMPvpRankSeasonRewardTemplet.MaxRank);
		lua.GetData("OpenTag", ref nKMPvpRankSeasonRewardTemplet.OpenTag);
		nKMPvpRankSeasonRewardTemplet.RewardList.Clear();
		for (int i = 0; i < 3; i++)
		{
			NKMRewardInfo nKMRewardInfo = new NKMRewardInfo
			{
				paymentType = NKM_ITEM_PAYMENT_TYPE.NIPT_FREE
			};
			int num = i + 1;
			if ((1u & (lua.GetData($"m_RewardType_{num}", ref nKMRewardInfo.rewardType) ? 1u : 0u) & (lua.GetData($"m_RewardID_{num}", ref nKMRewardInfo.ID) ? 1u : 0u) & (lua.GetData($"m_RewardValue_{num}", ref nKMRewardInfo.Count) ? 1u : 0u)) != 0 && nKMRewardInfo.Count > 0)
			{
				nKMPvpRankSeasonRewardTemplet.RewardList.Add(nKMRewardInfo);
			}
		}
		return nKMPvpRankSeasonRewardTemplet;
	}
}
