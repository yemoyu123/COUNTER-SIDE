using ClientPacket.Common;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Guild;

public sealed class GuildSeasonRewardTemplet : INKMTemplet
{
	private int SeasonRewardGroup;

	private GuildDungeonRewardCategory RewardCategory;

	private int RewardCountValue;

	private NKM_REWARD_TYPE RewardItemType;

	private int RewardItemId;

	private int RewardItemValue;

	public int Key => SeasonRewardGroup;

	public static GuildSeasonRewardTemplet LoadFromLua(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildSeasonRewardTemplet.cs", 22))
		{
			return null;
		}
		GuildSeasonRewardTemplet guildSeasonRewardTemplet = new GuildSeasonRewardTemplet();
		if ((1u & (cNKMLua.GetData("m_SeasonRewardGroup", ref guildSeasonRewardTemplet.SeasonRewardGroup) ? 1u : 0u) & (cNKMLua.GetData("m_RewardCategory", ref guildSeasonRewardTemplet.RewardCategory) ? 1u : 0u) & (cNKMLua.GetData("m_RewardCountValue", ref guildSeasonRewardTemplet.RewardCountValue) ? 1u : 0u) & (cNKMLua.GetData("m_RewardItemType", ref guildSeasonRewardTemplet.RewardItemType) ? 1u : 0u) & (cNKMLua.GetData("m_RewardItemID", ref guildSeasonRewardTemplet.RewardItemId) ? 1u : 0u) & (cNKMLua.GetData("m_RewardItemValue", ref guildSeasonRewardTemplet.RewardItemValue) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return guildSeasonRewardTemplet;
	}

	public bool IsCorrectRewardGroup(int rewardGroup)
	{
		return SeasonRewardGroup == rewardGroup;
	}

	public GuildDungeonRewardCategory GetRewardCategory()
	{
		return RewardCategory;
	}

	public int GetRewardCountValue()
	{
		return RewardCountValue;
	}

	public NKM_REWARD_TYPE GetRewardItemType()
	{
		return RewardItemType;
	}

	public int GetRewardItemId()
	{
		return RewardItemId;
	}

	public int GetRewardItemValue()
	{
		return RewardItemValue;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (RewardCategory < GuildDungeonRewardCategory.RANK)
		{
			NKMTempletError.Add($"[GuildSeasonRewardTemplet] RewardCategory Error. SeasonRewardGroup:{SeasonRewardGroup} RewardCategory:{RewardCategory}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildSeasonRewardTemplet.cs", 60);
		}
		if (RewardItemId <= 0 || RewardItemValue <= 0)
		{
			NKMTempletError.Add($"[GuildSeasonRewardTemplet] reward item Error. SeasonRewardGroup:{SeasonRewardGroup} RewardItemId:{RewardItemId} RewardItemValue:{RewardItemValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildSeasonRewardTemplet.cs", 65);
		}
		if (!NKMRewardTemplet.IsValidReward(RewardItemType, RewardItemId))
		{
			NKMTempletError.Add($"[GuildSeasonRewardTemplet] invalid m_RewardItemType - m_RewardItemID. SeasonRewardGroup:{SeasonRewardGroup} m_RewardItemType:{RewardItemType} m_RewardItemID:{RewardItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildSeasonRewardTemplet.cs", 70);
		}
	}

	public (int itemId, int itemCount) GetRewardData()
	{
		return (itemId: RewardItemId, itemCount: RewardItemValue);
	}
}
