using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Guild;

public class GuildDungeonTempletManager
{
	private class ArenaArtifact
	{
		public Dictionary<int, int> dic = new Dictionary<int, int>();
	}

	private class SessionArenaArtifact
	{
		public Dictionary<int, ArenaArtifact> dic = new Dictionary<int, ArenaArtifact>();
	}

	private const int IntervalMinuts = 10;

	private static Dictionary<int, List<GuildSeasonRewardTemplet>> dicSeasonRewardTemplet = new Dictionary<int, List<GuildSeasonRewardTemplet>>();

	private static Dictionary<int, List<GuildDungeonInfoTemplet>> dicDungeonInfoTemplet = new Dictionary<int, List<GuildDungeonInfoTemplet>>();

	private static Dictionary<int, List<GuildDungeonScheduleTemplet>> dicDungeonScheduleTemplet = new Dictionary<int, List<GuildDungeonScheduleTemplet>>();

	private static Dictionary<int, List<GuildDungeonArtifactTemplet>> dicDungeonArtifactTemplet = new Dictionary<int, List<GuildDungeonArtifactTemplet>>();

	private static Dictionary<int, List<GuildRaidTemplet>> dicRaidTemplet = new Dictionary<int, List<GuildRaidTemplet>>();

	private static Dictionary<int, SessionArenaArtifact> dicGetDungeonIdByArenaId = new Dictionary<int, SessionArenaArtifact>();

	public static IEnumerable<GuildSeasonTemplet> OrderedSeasons { get; private set; }

	public static List<GuildSeasonRewardTemplet> GetSeasonRewardList(int rewardGroup)
	{
		if (dicSeasonRewardTemplet.TryGetValue(rewardGroup, out var value))
		{
			return value;
		}
		return null;
	}

	public static List<GuildDungeonInfoTemplet> GetDungeonInfoList(int dungeonGroup)
	{
		if (dicDungeonInfoTemplet.TryGetValue(dungeonGroup, out var value))
		{
			return value;
		}
		return null;
	}

	public static List<GuildDungeonScheduleTemplet> GetDungeonScheduleList(int dungeonGroup)
	{
		if (dicDungeonScheduleTemplet.TryGetValue(dungeonGroup, out var value))
		{
			return value;
		}
		return null;
	}

	public static List<GuildDungeonArtifactTemplet> GetDungeonArtifactList(int artifactGroup)
	{
		if (dicDungeonArtifactTemplet.TryGetValue(artifactGroup, out var value))
		{
			return value;
		}
		return null;
	}

	public static List<GuildRaidTemplet> GetRaidTempletList(int raidGroup)
	{
		if (dicRaidTemplet.TryGetValue(raidGroup, out var value))
		{
			return value;
		}
		return null;
	}

	public static bool LoadFromLua()
	{
		dicSeasonRewardTemplet = NKMTempletLoader<GuildSeasonRewardTemplet>.LoadGroup("AB_SCRIPT", "LUA_GUILD_SEASON_REWARD_TEMPLET", "GUILD_SEASON_REWARD_TEMPLET", GuildSeasonRewardTemplet.LoadFromLua);
		if (dicSeasonRewardTemplet == null)
		{
			Log.Error("[GuildDungeonManager] dicSeasonRewardTemplet is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonTempletManager.cs", 34);
			return false;
		}
		dicDungeonInfoTemplet = NKMTempletLoader<GuildDungeonInfoTemplet>.LoadGroup("AB_SCRIPT", "LUA_GUILD_DUNGEON_INFO_TEMPLET", "GUILD_DUNGEON_INFO_TEMPLET", GuildDungeonInfoTemplet.LoadFromLua);
		if (dicDungeonInfoTemplet == null)
		{
			Log.Error("[GuildDungeonManager] dicDungeonInfoTemplet is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonTempletManager.cs", 41);
			return false;
		}
		dicDungeonScheduleTemplet = NKMTempletLoader<GuildDungeonScheduleTemplet>.LoadGroup("AB_SCRIPT", "LUA_GUILD_DUNGEON_SCHEDULE_TEMPLET", "GUILD_DUNGEON_SCHEDULE_TEMPLET", GuildDungeonScheduleTemplet.LoadFromLua);
		if (dicDungeonScheduleTemplet == null)
		{
			Log.Error("[GuildDungeonManager] dicDungeonScheduleTemplet is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonTempletManager.cs", 48);
			return false;
		}
		dicDungeonArtifactTemplet = NKMTempletLoader<GuildDungeonArtifactTemplet>.LoadGroup("AB_SCRIPT", "LUA_GUILD_DUNGEON_ARTIFACT_TEMPLET", "GUILD_DUNGEON_ARTIFACT_TEMPLET", GuildDungeonArtifactTemplet.LoadFromLua);
		if (dicDungeonArtifactTemplet == null)
		{
			Log.Error("[GuildDungeonManager] dicDungeonArtifactTemplet is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonTempletManager.cs", 55);
			return false;
		}
		dicRaidTemplet = NKMTempletLoader<GuildRaidTemplet>.LoadGroup("AB_SCRIPT", "LUA_GUILD_RAID_TEMPLET", "GUILD_RAID_TEMPLET", GuildRaidTemplet.LoadFromLua);
		if (dicRaidTemplet == null)
		{
			Log.Error("[GuildDungeonManager] dicRaidTemplet is null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonTempletManager.cs", 62);
			return false;
		}
		return true;
	}

	public static bool Join()
	{
		foreach (KeyValuePair<int, List<GuildDungeonArtifactTemplet>> item in dicDungeonArtifactTemplet)
		{
			foreach (GuildDungeonArtifactTemplet item2 in item.Value)
			{
				item2.Join();
			}
		}
		OrderedSeasons = NKMTempletContainer<GuildSeasonTemplet>.Values.OrderBy((GuildSeasonTemplet e) => e.GetSeasonStartDate()).ToList();
		return true;
	}

	public static bool Validation()
	{
		foreach (KeyValuePair<int, List<GuildRaidTemplet>> item in dicRaidTemplet)
		{
			List<int> list = item.Value.Select((GuildRaidTemplet e) => e.GetStageIndex()).ToList();
			list.Sort();
			if (list.First() != 1 || list.Last() != list.Count() || list.Count() != list.Distinct().Count())
			{
				NKMTempletError.Add($"[dicRaidTemplet] invalid index Order. seasonRaidGroup:{item.Key} totalCount:{list.Count()} firstIndex:{list.First()} lastIndex:{list.Last()} distinctCount:{list.Distinct().Count()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonTempletManager.cs", 100);
			}
			list = item.Value.Select((GuildRaidTemplet e) => e.GetStageId()).ToList();
			if (list.Count() != list.Distinct().Count())
			{
				NKMTempletError.Add($"[dicRaidTemplet] invalid raid count. exist duplicated stageid. seasonRaidGroup:{item.Key} totalCount:{list.Count()} distinctCount:{list.Distinct().Count()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonTempletManager.cs", 107);
			}
		}
		foreach (KeyValuePair<int, List<GuildDungeonScheduleTemplet>> item2 in dicDungeonScheduleTemplet)
		{
			List<int> list2 = item2.Value.Select((GuildDungeonScheduleTemplet e) => e.GetSessionIndex()).ToList();
			list2.Sort();
			if (list2.First() != 1 || list2.Last() != list2.Count() || list2.Count() != list2.Distinct().Count())
			{
				NKMTempletError.Add($"[dicDungeonScheduleTemplet] Invalid Index Order. SeasonDungeonGroup:{item2.Key} totalCount:{list2.Count()} firstIndex:{list2.First()} lastIndex:{list2.Last()} distinctCount:{list2.Distinct().Count()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonTempletManager.cs", 122);
			}
		}
		foreach (KeyValuePair<int, List<GuildDungeonInfoTemplet>> item3 in dicDungeonInfoTemplet)
		{
			List<int> source = item3.Value.Select((GuildDungeonInfoTemplet e) => e.GetSeasonDungeonId()).ToList();
			if (source.Count() != source.Distinct().Count())
			{
				NKMTempletError.Add($"[dicDungeonInfoTemplet] dicDungeonInfoTemplet StageId Duplicate. SeasonDungeonGroup:{item3.Key} duplicateCount:{source.Count() - source.Distinct().Count()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonTempletManager.cs", 133);
			}
			foreach (IGrouping<int, GuildDungeonInfoTemplet> item4 in from e in item3.Value
				group e by e.GetArenaIndex())
			{
				if (item4.Count() != item4.Select((GuildDungeonInfoTemplet e) => e.GetLevelIndex()).ToList().Distinct()
					.Count())
				{
					int num = item4.Count();
					int num2 = item4.Select((GuildDungeonInfoTemplet e) => e.GetLevelIndex()).ToList().Distinct()
						.Count();
					NKMTempletError.Add($"[dicDungeonInfoTemplet] exist duplicate level index data. SeasonDungeonGroup:{item3.Key} arenaId:{item4.Key} duplicateCount:{num - num2}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonTempletManager.cs", 144);
				}
			}
			foreach (int item5 in item3.Value.Select((GuildDungeonInfoTemplet e) => e.GetSeasonDungeonId()).ToList())
			{
				if (NKMDungeonManager.GetDungeonTempletBase(item5) == null)
				{
					NKMTempletError.Add($"SeasonDungeonId is not in DungeonTempletBase. seasonDungeonId:{item5}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonTempletManager.cs", 153);
				}
			}
		}
		foreach (KeyValuePair<int, List<GuildDungeonArtifactTemplet>> item6 in dicDungeonArtifactTemplet)
		{
			foreach (GuildDungeonArtifactTemplet item7 in item6.Value)
			{
				item7.Validate();
			}
		}
		foreach (int key in dicDungeonScheduleTemplet.Keys)
		{
			foreach (GuildDungeonScheduleTemplet item8 in dicDungeonScheduleTemplet[key])
			{
				foreach (int data2 in item8.GetDungeonList())
				{
					GuildDungeonInfoTemplet guildDungeonInfoTemplet = dicDungeonInfoTemplet[key].Find((GuildDungeonInfoTemplet e) => e.GetSeasonDungeonId() == data2);
					if (!dicGetDungeonIdByArenaId.TryGetValue(key, out var value))
					{
						value = new SessionArenaArtifact();
						dicGetDungeonIdByArenaId.Add(key, value);
					}
					if (!value.dic.TryGetValue(item8.GetSessionIndex(), out var value2))
					{
						value2 = new ArenaArtifact();
						value.dic.Add(item8.GetSessionIndex(), value2);
					}
					if (value2.dic.ContainsKey(guildDungeonInfoTemplet.GetArenaIndex()))
					{
						Log.ErrorAndExit($"그룹:{key} 세션:{item8.GetSessionIndex()} 아레나:{guildDungeonInfoTemplet.GetArenaIndex()} 위치에 2개이상 중복된 던전값이 있음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonTempletManager.cs", 190);
					}
					else
					{
						value2.dic.Add(guildDungeonInfoTemplet.GetArenaIndex(), guildDungeonInfoTemplet.GetStageRewardArtifactGroup());
					}
				}
			}
		}
		foreach (KeyValuePair<int, List<GuildDungeonInfoTemplet>> item9 in dicDungeonInfoTemplet)
		{
			foreach (GuildDungeonInfoTemplet item10 in item9.Value)
			{
				if (!dicDungeonArtifactTemplet.Keys.Contains(item10.GetStageRewardArtifactGroup()))
				{
					NKMTempletError.Add($"[GuildDungeon] Error. ArtifactGroup값으로 된 ArtifactTempet 정보가 없음. m_SeasonDungeonGroup:{item10.GetSeasonDungeonGroup()} m_StageRewardArtifactGroup:{item10.GetStageRewardArtifactGroup()} ", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildDungeonTempletManager.cs", 207);
				}
			}
		}
		return true;
	}

	public static int GetArtifactGroupIdByArenaIndex(int groupId, int sessionId, int arenaId)
	{
		if (!dicGetDungeonIdByArenaId.TryGetValue(groupId, out var value))
		{
			return 0;
		}
		if (!value.dic.TryGetValue(sessionId, out var value2))
		{
			return 0;
		}
		if (!value2.dic.TryGetValue(arenaId, out var value3))
		{
			return 0;
		}
		return value3;
	}

	public static GuildDungeonArtifactTemplet GetArtifactTemplet(int artifactID)
	{
		foreach (List<GuildDungeonArtifactTemplet> value in dicDungeonArtifactTemplet.Values)
		{
			GuildDungeonArtifactTemplet guildDungeonArtifactTemplet = value.Find((GuildDungeonArtifactTemplet x) => x.GetArtifactId() == artifactID);
			if (guildDungeonArtifactTemplet != null)
			{
				return guildDungeonArtifactTemplet;
			}
		}
		return null;
	}

	public static GuildSeasonTemplet GetGuildSeasonTemplet(int seasonId)
	{
		return NKMTempletContainer<GuildSeasonTemplet>.Find(seasonId);
	}

	public static GuildSeasonTemplet GetCurrentSeasonTemplet(DateTime current)
	{
		GuildSeasonTemplet guildSeasonTemplet = null;
		foreach (GuildSeasonTemplet value in NKMTempletContainer<GuildSeasonTemplet>.Values)
		{
			if (value.EnableByTag)
			{
				if (current >= value.GetSeasonStartDate() && (guildSeasonTemplet == null || guildSeasonTemplet.GetSeasonStartDate() < value.GetSeasonStartDate()))
				{
					guildSeasonTemplet = value;
				}
				if (current < value.GetSeasonEndDate().AddMinutes(-10.0) && current > value.GetSeasonStartDate())
				{
					return value;
				}
			}
		}
		return guildSeasonTemplet;
	}

	public static GuildRaidTemplet GetGuildRaidTemplet(int raidGroup, int bossStageId)
	{
		return GetRaidTempletList(raidGroup)?.Find((GuildRaidTemplet x) => x.GetStageId() == bossStageId);
	}
}
