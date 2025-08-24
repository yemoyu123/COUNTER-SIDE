using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMDefenceScoreRewardTemplet : INKMTemplet
{
	private static Dictionary<int, List<NKMDefenceScoreRewardTemplet>> ScoreRewardGroups = new Dictionary<int, List<NKMDefenceScoreRewardTemplet>>();

	public const int RewardCount = 3;

	public int DefenceScoreRewardGroupID { get; private set; }

	public int DefenceScoreRewardID { get; private set; }

	public int Step { get; private set; }

	public int Score { get; private set; }

	public List<NKMRewardInfo> Rewards { get; private set; }

	public string ScoreDescStrID { get; private set; }

	public int Key => DefenceScoreRewardID;

	public static IEnumerable<NKMDefenceScoreRewardTemplet> Values => NKMTempletContainer<NKMDefenceScoreRewardTemplet>.Values;

	public static IReadOnlyDictionary<int, List<NKMDefenceScoreRewardTemplet>> Groups => ScoreRewardGroups;

	public static NKMDefenceScoreRewardTemplet Find(int key)
	{
		return NKMTempletContainer<NKMDefenceScoreRewardTemplet>.Values.Where((NKMDefenceScoreRewardTemplet e) => e.Key == key).FirstOrDefault();
	}

	public static NKMDefenceScoreRewardTemplet LoadFromLua(NKMLua lua)
	{
		bool flag = true;
		int rValue = 0;
		int rValue2 = 0;
		int rValue3 = 0;
		int rValue4 = 0;
		string rValue5 = "";
		flag |= lua.GetData("DefenceScoreRewardGroupID", ref rValue);
		flag |= lua.GetData("DefenceScoreRewardID", ref rValue2);
		flag |= lua.GetData("Step", ref rValue3);
		flag |= lua.GetData("Score", ref rValue4);
		lua.GetData("ScoreDescStrID", ref rValue5);
		List<NKMRewardInfo> list = new List<NKMRewardInfo>();
		for (int i = 0; i < 3; i++)
		{
			NKMRewardInfo nKMRewardInfo = new NKMRewardInfo();
			if (lua.GetData($"ScoreRewardID_{i + 1}", ref nKMRewardInfo.ID))
			{
				flag &= lua.GetData($"ScoreRewardType_{i + 1}", ref nKMRewardInfo.rewardType);
				flag &= lua.GetData($"ScoreRewardQuantity_{i + 1}", ref nKMRewardInfo.Count);
				list.Add(nKMRewardInfo);
			}
		}
		if (!flag)
		{
			NKMTempletError.Add($"[NKMDefenceScoreRewardTemplet :{rValue2}] data is invalid", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDefenceScoreRewardTemplet.cs", 60);
			return null;
		}
		return new NKMDefenceScoreRewardTemplet
		{
			DefenceScoreRewardGroupID = rValue,
			DefenceScoreRewardID = rValue2,
			Step = rValue3,
			Score = rValue4,
			Rewards = list,
			ScoreDescStrID = rValue5
		};
	}

	public void Join()
	{
		if (!ScoreRewardGroups.ContainsKey(DefenceScoreRewardGroupID))
		{
			ScoreRewardGroups.Add(DefenceScoreRewardGroupID, new List<NKMDefenceScoreRewardTemplet>());
			if (ScoreRewardGroups[DefenceScoreRewardGroupID].Find((NKMDefenceScoreRewardTemplet x) => x.DefenceScoreRewardID == DefenceScoreRewardID) == null)
			{
				ScoreRewardGroups[DefenceScoreRewardGroupID].Add(this);
			}
			else
			{
				Log.Error($"NKMDefenceScoreRewardTemplet.DefenceScoreRewardID is duplecated - {DefenceScoreRewardID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDefenceScoreRewardTemplet.cs", 83);
			}
		}
		else if (ScoreRewardGroups[DefenceScoreRewardGroupID].Find((NKMDefenceScoreRewardTemplet x) => x.DefenceScoreRewardID == DefenceScoreRewardID) == null)
		{
			ScoreRewardGroups[DefenceScoreRewardGroupID].Add(this);
		}
		else
		{
			Log.Error($"NKMDefenceScoreRewardTemplet.DefenceScoreRewardID is duplecated - {DefenceScoreRewardID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDefenceScoreRewardTemplet.cs", 90);
		}
	}

	public void Validate()
	{
		if (Rewards.Where((NKMRewardInfo e) => e.ID <= 0).Any())
		{
			NKMTempletError.Add($"[NKMDefenceRankRewardTemplet{Key}] RewardID 가 올바르지 않은 데이터가 존재합니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDefenceScoreRewardTemplet.cs", 98);
		}
		else if (Rewards.Where((NKMRewardInfo e) => e.Count <= 0).Any())
		{
			NKMTempletError.Add($"[NKMDefenceRankRewardTemplet{Key}] RewardQuantity 가 올바르지 않은 데이터가 존재합니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDefenceScoreRewardTemplet.cs", 104);
		}
	}
}
