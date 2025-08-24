using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM.Guild;

public sealed class GuildDungeonScheduleTemplet : INKMTemplet
{
	private int SeasonDungeonGroup;

	private int SeasonSessionIndex;

	private int UseSeasonDungeonId1;

	private int UseSeasonDungeonId2;

	private int UseSeasonDungeonId3;

	private int UseSeasonDungeonId4;

	public int Key => SeasonDungeonGroup;

	public static GuildDungeonScheduleTemplet LoadFromLua(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonScheduleTemplet.cs", 20))
		{
			return null;
		}
		GuildDungeonScheduleTemplet guildDungeonScheduleTemplet = new GuildDungeonScheduleTemplet();
		if ((1u & (cNKMLua.GetData("m_SeasonDungeonGroup", ref guildDungeonScheduleTemplet.SeasonDungeonGroup) ? 1u : 0u) & (cNKMLua.GetData("m_SeasonSessionIndex", ref guildDungeonScheduleTemplet.SeasonSessionIndex) ? 1u : 0u) & (cNKMLua.GetData("m_UseSeasonDungeonID_1", ref guildDungeonScheduleTemplet.UseSeasonDungeonId1) ? 1u : 0u) & (cNKMLua.GetData("m_UseSeasonDungeonID_2", ref guildDungeonScheduleTemplet.UseSeasonDungeonId2) ? 1u : 0u) & (cNKMLua.GetData("m_UseSeasonDungeonID_3", ref guildDungeonScheduleTemplet.UseSeasonDungeonId3) ? 1u : 0u) & (cNKMLua.GetData("m_UseSeasonDungeonID_4", ref guildDungeonScheduleTemplet.UseSeasonDungeonId4) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return guildDungeonScheduleTemplet;
	}

	public int GetSessionIndex()
	{
		return SeasonSessionIndex;
	}

	public int GetSeasonDungeonGroupIndex()
	{
		return SeasonDungeonGroup;
	}

	public bool IsValidDungeonId(int dungeonId)
	{
		if (dungeonId != UseSeasonDungeonId1 && dungeonId != UseSeasonDungeonId2 && dungeonId != UseSeasonDungeonId3)
		{
			return dungeonId == UseSeasonDungeonId4;
		}
		return true;
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}

	public List<int> GetDungeonList()
	{
		return new List<int> { UseSeasonDungeonId1, UseSeasonDungeonId2, UseSeasonDungeonId3, UseSeasonDungeonId4 };
	}
}
