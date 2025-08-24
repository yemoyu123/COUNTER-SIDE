using System;
using NKM.Templet.Base;

namespace NKM.Guild;

public sealed class GuildDungeonTemplet : INKMTemplet
{
	private int SeasonId;

	private DateTime SeasonStartDate;

	private DateTime SeasonEndDate;

	private int SeaseonDungeonGroup;

	private int SeasonRaidGroup;

	public int Key => SeasonId;

	public static GuildDungeonTemplet LoadFromLua(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonTemplet.cs", 22))
		{
			return null;
		}
		GuildDungeonTemplet guildDungeonTemplet = new GuildDungeonTemplet();
		if ((1u & (cNKMLua.GetData("m_SeasonID", ref guildDungeonTemplet.SeasonId) ? 1u : 0u) & (cNKMLua.GetData("m_SeasonStartDate", ref guildDungeonTemplet.SeasonStartDate) ? 1u : 0u) & (cNKMLua.GetData("m_SeasonEndDate", ref guildDungeonTemplet.SeasonEndDate) ? 1u : 0u) & (cNKMLua.GetData("m_SeasonDungeonGroup", ref guildDungeonTemplet.SeaseonDungeonGroup) ? 1u : 0u) & (cNKMLua.GetData("m_SeasonRaidGroup", ref guildDungeonTemplet.SeasonRaidGroup) ? 1u : 0u)) == 0)
		{
			return null;
		}
		return guildDungeonTemplet;
	}

	public static GuildDungeonTemplet Find(int key)
	{
		return NKMTempletContainer<GuildDungeonTemplet>.Find(key);
	}

	public void Join()
	{
	}

	public void Validate()
	{
	}
}
