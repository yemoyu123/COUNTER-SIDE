using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMTournamentRankRewardTemplet : INKMTemplet
{
	private int index;

	private int rankRewardGroupId;

	private string RankRewardType;

	private int rankValue;

	private int groupMatchCount;

	private string rankRewardDesc;

	public List<NKMRewardInfo> Rewards { get; private set; }

	public static IEnumerable<NKMTournamentRankRewardTemplet> Values => NKMTempletContainer<NKMTournamentRankRewardTemplet>.Values;

	public int Key => index;

	public int RankRewardGroupId => rankRewardGroupId;

	public int RankValue => rankValue;

	public int GroupMatchCount => groupMatchCount;

	public string RankRewardDesc => rankRewardDesc;

	public static NKMTournamentRankRewardTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentRankRewardTemplet.cs", 25))
		{
			return null;
		}
		bool flag = true;
		NKMTournamentRankRewardTemplet nKMTournamentRankRewardTemplet = new NKMTournamentRankRewardTemplet();
		flag &= lua.GetData("INDEX", ref nKMTournamentRankRewardTemplet.index);
		flag &= lua.GetData("RankRewardGroupID", ref nKMTournamentRankRewardTemplet.rankRewardGroupId);
		flag &= lua.GetData("RankRewardType", ref nKMTournamentRankRewardTemplet.RankRewardType);
		lua.GetData("RankValue", ref nKMTournamentRankRewardTemplet.rankValue);
		lua.GetData("GroupMatchCount", ref nKMTournamentRankRewardTemplet.groupMatchCount);
		flag &= lua.GetData("RankRewardDesc", ref nKMTournamentRankRewardTemplet.rankRewardDesc);
		nKMTournamentRankRewardTemplet.Rewards = new List<NKMRewardInfo>();
		for (int i = 0; i < 5; i++)
		{
			NKMRewardInfo nKMRewardInfo = new NKMRewardInfo();
			if (lua.GetData($"RewardID_{i + 1}", ref nKMRewardInfo.ID))
			{
				lua.GetData($"RewardType_{i + 1}", ref nKMRewardInfo.rewardType);
				lua.GetData($"RewardValue_{i + 1}", ref nKMRewardInfo.Count);
				nKMTournamentRankRewardTemplet.Rewards.Add(nKMRewardInfo);
			}
		}
		if (!flag)
		{
			return null;
		}
		return nKMTournamentRankRewardTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		foreach (NKMRewardInfo reward in Rewards)
		{
			if (!NKMRewardTemplet.IsValidReward(reward.rewardType, reward.ID))
			{
				NKMTempletError.Add($"[NKMTournamentRankRewardTemplet:{Key}] 보상 아이템 정보가 존재하지 않습니다. RankRewardGroupId:{rankRewardGroupId} type:{reward.rewardType} id:{reward.ID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentRankRewardTemplet.cs", 73);
			}
			if (reward.Count <= 0)
			{
				NKMTempletError.Add($"[NKMTournamentRankRewardTemplet:{Key}] 보상 개수가 0 이하. RankRewardGroupId:{rankRewardGroupId} type:{reward.rewardType} id:{reward.ID} count:{reward.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentRankRewardTemplet.cs", 79);
			}
		}
		if (rankValue > 0 && groupMatchCount > 0)
		{
			NKMTempletError.Add($"[NKMTournamentRankRewardTemplet:{Key}] RankValue와 GroupMatchCount가 중복으로 설정. rankValue:{rankValue} GroupMatchCount:{groupMatchCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentRankRewardTemplet.cs", 86);
		}
		if (rankValue < 0 || groupMatchCount < 0)
		{
			NKMTempletError.Add($"[NKMTournamentRankRewardTemplet:{Key}] RankValue 혹은 GroupMatchCount가 음수로 설정. rankValue:{rankValue} GroupMatchCount:{groupMatchCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentRankRewardTemplet.cs", 93);
		}
		if (string.IsNullOrEmpty(RankRewardType) || (!(RankRewardType == "RANK") && !(RankRewardType == "COUNT")))
		{
			NKMTempletError.Add($"[NKMTournamentRankRewardTemplet:{Key}] 비정상적인 RankRewardType:{RankRewardType}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentRankRewardTemplet.cs", 100);
		}
		if (RankRewardType == "RANK" && RankValue <= 0)
		{
			NKMTempletError.Add($"[NKMTournamentRankRewardTemplet:{Key}] 결선 랭크 보상이 비정상적인 경우. RankRewardType:{RankRewardType} RankValue{RankValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentRankRewardTemplet.cs", 105);
		}
		if (RankRewardType == "COUNT" && groupMatchCount < 0)
		{
			NKMTempletError.Add($"[NKMTournamentRankRewardTemplet:{Key}] 그룹 랭크 보상이 비정상적인 경우. RankRewardType:{RankRewardType} RankValue{RankValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMTournamentRankRewardTemplet.cs", 110);
		}
	}
}
