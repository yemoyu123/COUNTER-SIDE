using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMTournamentPredictRewardTemplet : INKMTemplet
{
	public const int RewardCount = 5;

	public int index;

	public int groupId;

	public int predictCountMin;

	public int predictCountMax;

	public string predictRewardDesc;

	public static IEnumerable<NKMTournamentPredictRewardTemplet> Values => NKMTempletContainer<NKMTournamentPredictRewardTemplet>.Values;

	public List<NKMRewardInfo> Rewards { get; private set; }

	public string PredictRewardDesc => predictRewardDesc;

	public int Key => index;

	public static NKMTournamentPredictRewardTemplet LoadFromLua(NKMLua lua)
	{
		NKMTournamentPredictRewardTemplet nKMTournamentPredictRewardTemplet = new NKMTournamentPredictRewardTemplet();
		bool flag = true;
		flag &= lua.GetData("INDEX", ref nKMTournamentPredictRewardTemplet.index);
		flag &= lua.GetData("PredictRewardGroupID", ref nKMTournamentPredictRewardTemplet.groupId);
		flag &= lua.GetData("PredictCountMin", ref nKMTournamentPredictRewardTemplet.predictCountMin);
		flag &= lua.GetData("PredictCountMax", ref nKMTournamentPredictRewardTemplet.predictCountMax);
		flag &= lua.GetData("PredictRewardDesc", ref nKMTournamentPredictRewardTemplet.predictRewardDesc);
		nKMTournamentPredictRewardTemplet.Rewards = new List<NKMRewardInfo>();
		for (int i = 0; i < 5; i++)
		{
			NKMRewardInfo nKMRewardInfo = new NKMRewardInfo();
			if (lua.GetData($"RewardID_{i + 1}", ref nKMRewardInfo.ID))
			{
				flag &= lua.GetData($"RewardType_{i + 1}", ref nKMRewardInfo.rewardType);
				flag &= lua.GetData($"RewardValue_{i + 1}", ref nKMRewardInfo.Count);
				nKMTournamentPredictRewardTemplet.Rewards.Add(nKMRewardInfo);
			}
		}
		return nKMTournamentPredictRewardTemplet;
	}

	public bool CanReward(int count)
	{
		if (predictCountMin <= count && count <= predictCountMax)
		{
			return true;
		}
		return false;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
