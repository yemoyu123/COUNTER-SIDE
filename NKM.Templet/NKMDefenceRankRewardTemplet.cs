using System.Collections.Generic;
using System.Linq;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMDefenceRankRewardTemplet : INKMTemplet
{
	public const int RewardCount = 5;

	public int DefenceRankRewardGroupID { get; private set; }

	public int DefenceRankRewardID { get; private set; }

	public bool PercentCheck { get; private set; }

	public int RankValue { get; private set; }

	public List<NKMRewardInfo> Rewards { get; private set; }

	public string RankDescStrID { get; private set; }

	public int Key => DefenceRankRewardID;

	public static IEnumerable<NKMDefenceRankRewardTemplet> Values => NKMTempletContainer<NKMDefenceRankRewardTemplet>.Values;

	public static NKMDefenceRankRewardTemplet Find(int key)
	{
		return NKMTempletContainer<NKMDefenceRankRewardTemplet>.Values.Where((NKMDefenceRankRewardTemplet e) => e.Key == key).FirstOrDefault();
	}

	public static NKMDefenceRankRewardTemplet LoadFromLua(NKMLua lua)
	{
		bool flag = true;
		int rValue = 0;
		int rValue2 = 0;
		bool rbValue = false;
		int rValue3 = 0;
		string rValue4 = "";
		flag |= lua.GetData("DefenceRankRewardGroupID", ref rValue);
		flag |= lua.GetData("DefenceRankRewardID", ref rValue2);
		flag |= lua.GetData("PercentCheck", ref rbValue);
		flag |= lua.GetData("RankValue", ref rValue3);
		lua.GetData("RankDescStrID", ref rValue4);
		List<NKMRewardInfo> list = new List<NKMRewardInfo>();
		for (int i = 0; i < 5; i++)
		{
			NKMRewardInfo nKMRewardInfo = new NKMRewardInfo();
			if (lua.GetData($"RankRewardID_{i + 1}", ref nKMRewardInfo.ID))
			{
				flag &= lua.GetData($"RankRewardType_{i + 1}", ref nKMRewardInfo.rewardType);
				flag &= lua.GetData($"RankRewardQuantity_{i + 1}", ref nKMRewardInfo.Count);
				list.Add(nKMRewardInfo);
			}
		}
		if (!flag)
		{
			NKMTempletError.Add($"[NKMDefenceRankRewardTemplet :{rValue2}] data is invalid", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDefenceRankRewardTemplet.cs", 56);
			return null;
		}
		return new NKMDefenceRankRewardTemplet
		{
			DefenceRankRewardGroupID = rValue,
			DefenceRankRewardID = rValue2,
			PercentCheck = rbValue,
			RankValue = rValue3,
			Rewards = list,
			RankDescStrID = rValue4
		};
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (PercentCheck && RankValue > 100)
		{
			NKMTempletError.Add($"[NKMDefenceRankRewardTemplet{Key}] RankValue 100을 넘어섰습니다 RankValue:{RankValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDefenceRankRewardTemplet.cs", 81);
		}
		else if (Rewards.Where((NKMRewardInfo e) => e.ID <= 0).Any())
		{
			NKMTempletError.Add($"[NKMDefenceRankRewardTemplet{Key}] RewardID 가 올바르지 않은 데이터가 존재합니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDefenceRankRewardTemplet.cs", 88);
		}
		else if (Rewards.Where((NKMRewardInfo e) => e.Count <= 0).Any())
		{
			NKMTempletError.Add($"[NKMDefenceRankRewardTemplet{Key}] RewardQuantity 가 올바르지 않은 데이터가 존재합니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMDefenceRankRewardTemplet.cs", 94);
		}
	}
}
