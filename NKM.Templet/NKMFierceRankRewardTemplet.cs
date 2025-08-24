using System.Collections.Generic;
using System.Linq;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMFierceRankRewardTemplet : INKMTemplet, IFierceRankReward
{
	private static Dictionary<int, List<NKMFierceRankRewardTemplet>> rankRewardGroups;

	public static Dictionary<int, NKMFierceRankRewardTemplet[]> NumericRankGroupMap { get; private set; }

	public static Dictionary<int, NKMFierceRankRewardTemplet[]> PercentRankGroupMap { get; private set; }

	public int FierceRankRewardID { get; private set; }

	public int FierceRankRewardGroupID { get; private set; }

	public int ShowIndex { get; private set; }

	public bool PercentCheck { get; private set; }

	public int RankValue { get; private set; }

	public string RankDescStrID { get; private set; }

	public List<NKM_REWARD_DATA> Rewards { get; private set; }

	public int Key => FierceRankRewardID;

	public static IEnumerable<NKMFierceRankRewardTemplet> Values => NKMTempletContainer<NKMFierceRankRewardTemplet>.Values;

	public static IReadOnlyDictionary<int, List<NKMFierceRankRewardTemplet>> Groups => rankRewardGroups;

	public NKMFierceRankRewardTemplet(int fierceRankRewardId, int fierceRankRewardGroupID, int showIndex, bool percentCheck, int rankValue, string rankDescStrId, List<NKM_REWARD_DATA> rewards)
	{
		FierceRankRewardID = fierceRankRewardId;
		FierceRankRewardGroupID = fierceRankRewardGroupID;
		ShowIndex = showIndex;
		PercentCheck = percentCheck;
		RankValue = rankValue;
		RankDescStrID = rankDescStrId;
		Rewards = rewards;
	}

	public static NKMFierceRankRewardTemplet Find(int fierceRankRewardId)
	{
		return NKMTempletContainer<NKMFierceRankRewardTemplet>.Find(fierceRankRewardId);
	}

	public static NKMFierceRankRewardTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceRankRewardTemplet.cs", 54))
		{
			return null;
		}
		bool flag = true;
		int rValue = 0;
		flag &= lua.GetData("FierceRankRewardID", ref rValue);
		int rValue2 = 0;
		flag &= lua.GetData("FierceRankRewardGroupID", ref rValue2);
		int rValue3 = 0;
		flag &= lua.GetData("ShowIndex", ref rValue3);
		bool rbValue = false;
		flag &= lua.GetData("PercentCheck", ref rbValue);
		int rValue4 = 0;
		flag &= lua.GetData("RankValue", ref rValue4);
		string rValue5 = string.Empty;
		flag &= lua.GetData("RankDescStrID", ref rValue5);
		List<NKM_REWARD_DATA> list = new List<NKM_REWARD_DATA>();
		for (int i = 1; i <= 5; i++)
		{
			NKM_REWARD_DATA nKM_REWARD_DATA = new NKM_REWARD_DATA();
			if (lua.GetData($"RankRewardID_{i}", ref nKM_REWARD_DATA.RewardID))
			{
				flag &= lua.GetData($"RankRewardType_{i}", ref nKM_REWARD_DATA.RewardType);
				flag &= lua.GetData($"RankRewardQuantity_{i}", ref nKM_REWARD_DATA.RewardQuantity);
				list.Add(nKM_REWARD_DATA);
			}
		}
		if (!flag)
		{
			NKMTempletError.Add($"[FierceRankRewardTemplet:{rValue}] data is invalid", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceRankRewardTemplet.cs", 95);
			return null;
		}
		return new NKMFierceRankRewardTemplet(rValue, rValue2, rValue3, rbValue, rValue4, rValue5, list);
	}

	public void Join()
	{
		if (!Values.Where((NKMFierceRankRewardTemplet e) => e.PercentCheck).Any((NKMFierceRankRewardTemplet e) => e.RankValue == 100))
		{
			string text = string.Join(",", (from e in Values
				where e.PercentCheck && e.RankValue != 100
				select e.Key).ToList());
			NKMTempletError.Add("[FierceRankRewardTemplet] rankValue가\u0080 100이 아닌 템플릿이 존재합니다. targetTempletIds:" + text, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceRankRewardTemplet.cs", 117);
			return;
		}
		rankRewardGroups = (from e in Values
			group e by e.FierceRankRewardGroupID).ToDictionary((IGrouping<int, NKMFierceRankRewardTemplet> e) => e.Key, (IGrouping<int, NKMFierceRankRewardTemplet> e) => e.ToList());
		NumericRankGroupMap = (from e in Values
			where !e.PercentCheck
			group e by e.FierceRankRewardGroupID).ToDictionary((IGrouping<int, NKMFierceRankRewardTemplet> e) => e.Key, (IGrouping<int, NKMFierceRankRewardTemplet> e) => e.OrderBy((NKMFierceRankRewardTemplet a) => a.RankValue).ToArray());
		PercentRankGroupMap = (from e in Values
			where e.PercentCheck
			group e by e.FierceRankRewardGroupID).ToDictionary((IGrouping<int, NKMFierceRankRewardTemplet> e) => e.Key, (IGrouping<int, NKMFierceRankRewardTemplet> e) => e.OrderBy((NKMFierceRankRewardTemplet a) => a.RankValue).ToArray());
	}

	public void Validate()
	{
		if (PercentCheck && RankValue > 100)
		{
			NKMTempletError.Add($"[FierceRankRewardTemplet{Key}] RankValue 100을 넘어섰습니다 RankValue:{RankValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceRankRewardTemplet.cs", 142);
		}
		else if (Rewards.Where((NKM_REWARD_DATA e) => e.RewardID <= 0).Any())
		{
			NKMTempletError.Add($"[FierceRankRewardTemplet{Key}] RewardID 가 올바르지 않은 데이터가 존재합니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceRankRewardTemplet.cs", 149);
		}
		else if (Rewards.Where((NKM_REWARD_DATA e) => e.RewardQuantity <= 0).Any())
		{
			NKMTempletError.Add($"[FierceRankRewardTemplet{Key}] RewardQuantity 가 올바르지 않은 데이터가 존재합니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMFierceRankRewardTemplet.cs", 155);
		}
	}
}
