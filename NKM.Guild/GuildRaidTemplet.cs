using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Guild;

public sealed class GuildRaidTemplet : INKMTemplet
{
	private int seasonRaidGroup;

	private int raidStageIndex;

	private int stageId;

	private int raidRewardPoint;

	private int raidRewardId;

	private int raidRewardValue;

	private string raidBossSDName;

	private string raidBossFaceCardName;

	private string guideShortCut;

	public int Key => seasonRaidGroup;

	public int RaidRewardId => raidRewardId;

	public int RaidRewardValue => raidRewardValue;

	public static GuildRaidTemplet LoadFromLua(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildRaidTemplet.cs", 23))
		{
			return null;
		}
		GuildRaidTemplet guildRaidTemplet = new GuildRaidTemplet();
		int num = (int)(1u & (cNKMLua.GetData("m_SeasonRaidGroup", ref guildRaidTemplet.seasonRaidGroup) ? 1u : 0u) & (cNKMLua.GetData("m_RaidStageIndex", ref guildRaidTemplet.raidStageIndex) ? 1u : 0u) & (cNKMLua.GetData("m_StageID", ref guildRaidTemplet.stageId) ? 1u : 0u) & (cNKMLua.GetData("m_RaidRewardPoint", ref guildRaidTemplet.raidRewardPoint) ? 1u : 0u) & (cNKMLua.GetData("m_RaidRewardID", ref guildRaidTemplet.raidRewardId) ? 1u : 0u) & (cNKMLua.GetData("m_RaidRewardValue", ref guildRaidTemplet.raidRewardValue) ? 1u : 0u) & (cNKMLua.GetData("m_RaidBossSDName", ref guildRaidTemplet.raidBossSDName) ? 1u : 0u)) & (cNKMLua.GetData("m_RaidBossFaceCardName", ref guildRaidTemplet.raidBossFaceCardName) ? 1 : 0);
		cNKMLua.GetData("GuideShortCut", ref guildRaidTemplet.guideShortCut);
		if (num == 0)
		{
			Log.Error("GuildRaidTemplet data error", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildRaidTemplet.cs", 44);
			return null;
		}
		return guildRaidTemplet;
	}

	public int GetSeasonRaidGrouop()
	{
		return seasonRaidGroup;
	}

	public int GetStageIndex()
	{
		return raidStageIndex;
	}

	public int GetStageId()
	{
		return stageId;
	}

	public int GetRewardPoint()
	{
		return raidRewardPoint;
	}

	public string GetRaidBossSDName()
	{
		return raidBossSDName;
	}

	public string GetRaidBossFaceCardName()
	{
		return raidBossFaceCardName;
	}

	public string GetGuideShortCut()
	{
		return guideShortCut;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
