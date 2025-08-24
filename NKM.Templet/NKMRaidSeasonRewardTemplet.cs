using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMRaidSeasonRewardTemplet : INKMTemplet
{
	private int id;

	private int rewardBoardId;

	private int raidPoint;

	private NKM_REWARD_TYPE rewardType;

	private int rewardId;

	private int rewardValue;

	private int extraRewardPoint;

	private NKM_REWARD_TYPE extraRewardType;

	private int extraRewardId;

	private int extraRewardValue;

	public int RewardBoardId => rewardBoardId;

	public int RaidPoint => raidPoint;

	public NKM_REWARD_TYPE RewardType => rewardType;

	public int RewardId => rewardId;

	public int RewardValue => rewardValue;

	public int ExtraRewardPoint => extraRewardPoint;

	public NKM_REWARD_TYPE ExtraRewardType => extraRewardType;

	public int ExtraRewardId => extraRewardId;

	public int ExtraRewardValue => extraRewardValue;

	public int Key => id;

	public static IEnumerable<NKMRaidSeasonRewardTemplet> Values => NKMTempletContainer<NKMRaidSeasonRewardTemplet>.Values;

	public static NKMRaidSeasonRewardTemplet Find(int key)
	{
		return NKMTempletContainer<NKMRaidSeasonRewardTemplet>.Find((NKMRaidSeasonRewardTemplet x) => x.Key == key);
	}

	public static NKMRaidSeasonRewardTemplet LoadFromLua(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRaidSeasonRewardTemplet.cs", 39))
		{
			return null;
		}
		NKMRaidSeasonRewardTemplet nKMRaidSeasonRewardTemplet = new NKMRaidSeasonRewardTemplet();
		int num = (int)(1u & (cNKMLua.GetData("ID", ref nKMRaidSeasonRewardTemplet.id) ? 1u : 0u) & (cNKMLua.GetData("Reward_Board_ID", ref nKMRaidSeasonRewardTemplet.rewardBoardId) ? 1u : 0u) & (cNKMLua.GetData("Raid_Point", ref nKMRaidSeasonRewardTemplet.raidPoint) ? 1u : 0u) & (cNKMLua.GetData("Reward_Type", ref nKMRaidSeasonRewardTemplet.rewardType) ? 1u : 0u) & (cNKMLua.GetData("Reward_ID", ref nKMRaidSeasonRewardTemplet.rewardId) ? 1u : 0u)) & (cNKMLua.GetData("Reward_Value", ref nKMRaidSeasonRewardTemplet.rewardValue) ? 1 : 0);
		cNKMLua.GetData("ExtraReward_Point", ref nKMRaidSeasonRewardTemplet.extraRewardPoint);
		cNKMLua.GetData("ExtraReward_Type", ref nKMRaidSeasonRewardTemplet.extraRewardType);
		cNKMLua.GetData("ExtraReward_ID", ref nKMRaidSeasonRewardTemplet.extraRewardId);
		cNKMLua.GetData("ExtraReward_Value", ref nKMRaidSeasonRewardTemplet.extraRewardValue);
		if (num == 0)
		{
			return null;
		}
		return nKMRaidSeasonRewardTemplet;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (!NKMRewardTemplet.IsValidReward(rewardType, rewardId))
		{
			NKMTempletError.Add($"NKMRaidSeasonRewardTemplet: Invalid reward data. type:{rewardType} id:{rewardId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRaidSeasonRewardTemplet.cs", 68);
		}
		if (extraRewardId > 0 && !NKMRewardTemplet.IsValidReward(extraRewardType, extraRewardId))
		{
			NKMTempletError.Add($"NKMRaidSeasonRewardTemplet: Invalid reward data. type:{rewardType} id:{rewardId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMRaidSeasonRewardTemplet.cs", 75);
		}
	}
}
