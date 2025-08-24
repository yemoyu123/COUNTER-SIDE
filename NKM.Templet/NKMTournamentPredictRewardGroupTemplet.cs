using System.Collections.Generic;
using System.Linq;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMTournamentPredictRewardGroupTemplet : INKMTemplet
{
	private int groupId;

	private List<NKMTournamentPredictRewardTemplet> rewardTemplets;

	public List<NKMTournamentPredictRewardTemplet> RewardTemplets => rewardTemplets;

	public int Key => groupId;

	private NKMTournamentPredictRewardGroupTemplet(IGrouping<int, NKMTournamentPredictRewardTemplet> group)
	{
		groupId = group.Key;
		rewardTemplets = (from e in @group.ToList()
			orderby e.index
			select e).ToList();
	}

	public static void LoadFromLua()
	{
		(from e in NKMTempletLoader.LoadCommonPath("AB_SCRIPT", "LUA_TOURNAMENT_PREDICT_REWARD", "TOURNAMENT_PREDICT_REWARD", NKMTournamentPredictRewardTemplet.LoadFromLua)
			group e by e.groupId into e
			select new NKMTournamentPredictRewardGroupTemplet(e)).AddToContainer();
	}

	public static NKMTournamentPredictRewardGroupTemplet Find(int key)
	{
		return NKMTempletContainer<NKMTournamentPredictRewardGroupTemplet>.Find(key);
	}

	public NKMTournamentPredictRewardTemplet GetPredictRewardTemplet(int predicionHitCount)
	{
		foreach (NKMTournamentPredictRewardTemplet rewardTemplet in rewardTemplets)
		{
			if (rewardTemplet.CanReward(predicionHitCount))
			{
				return rewardTemplet;
			}
		}
		return null;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		rewardTemplets.Any();
		foreach (NKMTournamentPredictRewardTemplet rewardTemplet in rewardTemplets)
		{
			rewardTemplet.Validate();
			foreach (NKMTournamentPredictRewardTemplet rewardTemplet2 in rewardTemplets)
			{
				if (rewardTemplet != rewardTemplet2 && (rewardTemplet.CanReward(rewardTemplet2.predictCountMin) || rewardTemplet.CanReward(rewardTemplet2.predictCountMax)))
				{
					NKMTempletError.Add($"[NKMTournamentPredictRewardTemplet : {rewardTemplet.Key}] \ufffd\ufffd\ufffdø\ufffd \ufffdε\ufffd\ufffd\ufffd {rewardTemplet2.Key}\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffdħ ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentPredictRewardGroupTemplet.cs", 67);
				}
			}
		}
	}
}
