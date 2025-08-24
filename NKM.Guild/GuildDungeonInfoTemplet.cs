using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Guild;

public sealed class GuildDungeonInfoTemplet : INKMTemplet
{
	private int SeasonDungeonGroup;

	private int StageArenaIndex;

	private int StageLevelIndex;

	private int SeasonDungeonId;

	private int StageRewardArtifactGroup;

	public int Key => SeasonDungeonGroup;

	public static GuildDungeonInfoTemplet LoadFromLua(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonInfoTemplet.cs", 19))
		{
			return null;
		}
		GuildDungeonInfoTemplet guildDungeonInfoTemplet = new GuildDungeonInfoTemplet();
		if ((1u & (cNKMLua.GetData("m_SeasonDungeonGroup", ref guildDungeonInfoTemplet.SeasonDungeonGroup) ? 1u : 0u) & (cNKMLua.GetData("m_StageArenaIndex", ref guildDungeonInfoTemplet.StageArenaIndex) ? 1u : 0u) & (cNKMLua.GetData("m_StageLevelIndex", ref guildDungeonInfoTemplet.StageLevelIndex) ? 1u : 0u) & (cNKMLua.GetData("m_SeasonDungeonID", ref guildDungeonInfoTemplet.SeasonDungeonId) ? 1u : 0u) & (cNKMLua.GetData("m_StageRewardArtifactGroup", ref guildDungeonInfoTemplet.StageRewardArtifactGroup) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return guildDungeonInfoTemplet;
	}

	public int GetSeasonDungeonId()
	{
		return SeasonDungeonId;
	}

	public int GetArenaIndex()
	{
		return StageArenaIndex;
	}

	public int GetLevelIndex()
	{
		return StageLevelIndex;
	}

	public int GetSeasonDungeonGroup()
	{
		return SeasonDungeonGroup;
	}

	public int GetStageRewardArtifactGroup()
	{
		return StageRewardArtifactGroup;
	}

	public void Join()
	{
	}

	public void Validate()
	{
		if (NKMDungeonManager.GetDungeonTempletBase(SeasonDungeonId) == null)
		{
			Log.Error($"SeasonDungeonId is not in DungeonTempletBase. seasonDungeonId:{SeasonDungeonId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonInfoTemplet.cs", 55);
		}
	}
}
