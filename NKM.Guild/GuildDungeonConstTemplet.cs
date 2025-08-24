using System.Collections.Generic;
using NKM.Templet.Base;

namespace NKM.Guild;

public class GuildDungeonConstTemplet
{
	public class SessionItemLimit
	{
		public int itemId;

		public int limitValue;

		public SessionItemLimit(int itemId, int limitValue)
		{
			this.itemId = itemId;
			this.limitValue = limitValue;
		}
	}

	public int ArenaTicketBuyCount = 1;

	public int ArenaPlayCountBasic = 3;

	public int BossPlayCountBasic = 1;

	public int ArtifactFulificationCount = 10;

	public int TicketCost = 50;

	public int AdjustTime = 10;

	public List<SessionItemLimit> sessionRewardLimitDatas = new List<SessionItemLimit>();

	public string arenaEndString = "STR_GUILD_ARENA_END";

	public string artifactString = "STR_GUILD_ARTIFACT_ACHIEVE";

	public string bossEndString = "STR_GUILD_BOSS_END";

	public string bossClearString = "STR_GUILD_BOSS_CLEAR";

	public int ArenaMaxPlayCount()
	{
		return ArenaPlayCountBasic + ArenaTicketBuyCount;
	}

	public void LoadFromLua(NKMLua lua)
	{
		using (lua.OpenTable("SESSION_REWARD_LIMIT", "GUILD_DUNGEON SESSION_REWARD_LIMIT table failed", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonConstTemplet.cs", 28))
		{
			int num = 1;
			while (lua.OpenTable(num++))
			{
				int @int = lua.GetInt32("ITEM_ID");
				int int2 = lua.GetInt32("ITEM_LIMIT_VALUE");
				sessionRewardLimitDatas.Add(new SessionItemLimit(@int, int2));
				lua.CloseTable();
			}
		}
		using (lua.OpenTable("BASIC_CONST", "Guild_Dungeon Const table failed.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonConstTemplet.cs", 40))
		{
			lua.GetData("ARENA_TICKET_BUY_COUNT", ref ArenaTicketBuyCount);
			lua.GetData("ARENA_PLAY_COUNT_BASIC", ref ArenaPlayCountBasic);
			lua.GetData("BOSS_PLAY_COUNT_BASIC", ref BossPlayCountBasic);
			lua.GetData("ARTIFACT_FULIFICATION_COUNT", ref ArtifactFulificationCount);
			lua.GetData("TICKET_COST", ref TicketCost);
		}
	}

	public void Validate()
	{
		if (ArenaTicketBuyCount <= 0)
		{
			NKMTempletError.Add($"ArenaTicketBuyCount is invalid. arenaTicketBuyCount:{ArenaTicketBuyCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonConstTemplet.cs", 55);
		}
		if (ArenaPlayCountBasic < 1)
		{
			NKMTempletError.Add($"ArenaPlayCountBasic is invalid. ArenaPlayCountBasic:{ArenaPlayCountBasic}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonConstTemplet.cs", 60);
		}
		if (BossPlayCountBasic < 1)
		{
			NKMTempletError.Add($"BossPlayCountBasic is invalid. BossPlayCountBasic:{BossPlayCountBasic}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonConstTemplet.cs", 65);
		}
	}
}
