using System;
using System.Collections.Generic;
using System.Linq;
using ClientPacket.Common;
using Cs.Core.Util;
using Cs.Logging;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Guild;

public class GuildSeasonTemplet : INKMTemplet, INKMTempletEx
{
	public struct SessionData
	{
		public int SessionId;

		public DateTime StartDate;

		public DateTime EndDate;

		public GuildDungeonScheduleTemplet templet;
	}

	private int SeasonId;

	private string SeasonName;

	private string SeasonBgPrefabName;

	private string SeasonDateStrId;

	private DateTime SeasonStartDate;

	private DateTime SeasonEndDate;

	private int SeasonDungeonGroup;

	private int SeasonRaidGroup;

	private int SeasonRewardGroup;

	private string m_OpenTag;

	private string m_SeasonBgBlurName;

	private List<SessionData> sessionDatas;

	public int Key => SeasonId;

	public static IEnumerable<GuildSeasonTemplet> Values => GuildDungeonTempletManager.OrderedSeasons;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public static GuildSeasonTemplet LoadFromLua(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildSeasonTemplet.cs", 34))
		{
			return null;
		}
		GuildSeasonTemplet guildSeasonTemplet = new GuildSeasonTemplet();
		bool flag = true;
		flag &= cNKMLua.GetData("m_SeasonID", ref guildSeasonTemplet.SeasonId);
		flag &= cNKMLua.GetData("m_SeasonName", ref guildSeasonTemplet.SeasonName);
		flag &= cNKMLua.GetData("m_SeasonBgPrefabName", ref guildSeasonTemplet.SeasonBgPrefabName);
		flag &= cNKMLua.GetData("m_DateStrID", ref guildSeasonTemplet.SeasonDateStrId);
		flag &= cNKMLua.GetData("m_SeasonDungeonGroup", ref guildSeasonTemplet.SeasonDungeonGroup);
		flag &= cNKMLua.GetData("m_SeasonRaidGroup", ref guildSeasonTemplet.SeasonRaidGroup);
		flag &= cNKMLua.GetData("m_SeasonRewardGroup", ref guildSeasonTemplet.SeasonRewardGroup);
		cNKMLua.GetData("m_SeasonBgBlurName", ref guildSeasonTemplet.m_SeasonBgBlurName);
		if (NKMContentsVersionManager.HasDFChangeTagType(DataFormatChangeTagType.OPEN_TAG_GUILD_DUNGEON))
		{
			flag &= cNKMLua.GetData("m_OpenTag", ref guildSeasonTemplet.m_OpenTag);
		}
		if (!flag)
		{
			return null;
		}
		return guildSeasonTemplet;
	}

	public static GuildSeasonTemplet Find(int key)
	{
		return NKMTempletContainer<GuildSeasonTemplet>.Find(key);
	}

	public string GetSeasonNameID()
	{
		return SeasonName;
	}

	public int GetSeasonDungeonGroup()
	{
		return SeasonDungeonGroup;
	}

	public int GetSeasonRaidGroup()
	{
		return SeasonRaidGroup;
	}

	public int GetSeasonRewardGroup()
	{
		return SeasonRewardGroup;
	}

	public string GetSeasonBgPrefabName()
	{
		return SeasonBgPrefabName;
	}

	public DateTime GetSeasonStartDate()
	{
		return SeasonStartDate;
	}

	public DateTime GetSeasonEndDate()
	{
		return SeasonEndDate;
	}

	public SessionData GetLastSession()
	{
		return sessionDatas.Last();
	}

	public string GetSeasonBgBlurName()
	{
		return m_SeasonBgBlurName;
	}

	public void Join()
	{
		if (NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
		}
	}

	public void JoinIntervalTemplet()
	{
		NKMIntervalTemplet nKMIntervalTemplet = NKMIntervalTemplet.Find(SeasonDateStrId);
		if (nKMIntervalTemplet == null)
		{
			NKMTempletError.Add($"[GuildSeasonTemplet] Date Setting Fail. IntervalTemplet is null. SeasonId:{SeasonId} intervalTemplet.Key:{SeasonDateStrId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildSeasonTemplet.cs", 90);
		}
		else
		{
			SeasonStartDate = nKMIntervalTemplet.StartDate;
			SeasonEndDate = nKMIntervalTemplet.EndDate;
		}
		sessionDatas = MakeSessionData(SeasonDungeonGroup);
		if (sessionDatas == null)
		{
			NKMTempletError.Add($"[GuildSeasonTemplet] MakeSessionData Fail. SeasonId:{SeasonId} SeasonDungeonGroup:{SeasonDungeonGroup}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildSeasonTemplet.cs", 101);
		}
	}

	public void Validate()
	{
		if (SeasonStartDate > SeasonEndDate)
		{
			NKMTempletError.Add($"[GuildSeasonTemplet] Invalid Date. SeasonId:{SeasonId} SeasonStartDate:{SeasonStartDate} SeasonEndDate:{SeasonEndDate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildSeasonTemplet.cs", 109);
		}
		if (SeasonStartDate.DayOfWeek != DayOfWeek.Wednesday)
		{
			NKMTempletError.Add($"[GuildSeasonTemplet] Invalid Start Date. StartDate:{SeasonStartDate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildSeasonTemplet.cs", 115);
		}
		if (sessionDatas.Count != 0 && sessionDatas.Count != GetSeasionScheduleCount(SeasonDungeonGroup))
		{
			NKMTempletError.Add($"[GuildSeasonTemplet] session Count Data error. SeasonId:{SeasonId} calSessionCount:{sessionDatas.Count} GuildDungeonScheduleCount:{GetSeasionScheduleCount(SeasonDungeonGroup)}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildSeasonTemplet.cs", 121);
		}
		if (!dungeonValidation())
		{
			NKMTempletError.Add("[GuildSeasonTemplet] schedule 내 던전 중 info 에 지정되지 않은 던전이 있음.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildSeasonTemplet.cs", 127);
		}
		if (SeasonEndDate != sessionDatas.Last().EndDate)
		{
			NKMTempletError.Add($"[GuildSeasonTemplet] seasonEndDate != LastSessionEndDate. seasonEndDate:{SeasonEndDate}, LastSessionEndDate:{sessionDatas.Last().EndDate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildSeasonTemplet.cs", 133);
		}
		if (!raidValidation())
		{
			NKMTempletError.Add($"[GuildSeasonTemplet] seasonRaidTemplet Error. Invalid raid index. seasonId:{SeasonId} SeasonRaidGroup:{SeasonRaidGroup}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildSeasonTemplet.cs", 139);
		}
		GuildSeasonTemplet guildSeasonTemplet = NKMTempletContainer<GuildSeasonTemplet>.Values.Where((GuildSeasonTemplet e) => e.Key != Key).FirstOrDefault((GuildSeasonTemplet e) => IsOverlapped(e));
		if (guildSeasonTemplet != null)
		{
			NKMTempletError.Add($"Invalid GuildSeasonTemplet Exist. idA:{Key} idB:{guildSeasonTemplet.Key}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildSeasonTemplet.cs", 149);
		}
	}

	public bool isBetween(DateTime current)
	{
		return current.IsBetween(SeasonStartDate, SeasonEndDate);
	}

	private bool IsOverlapped(GuildSeasonTemplet other)
	{
		if (!other.isBetween(SeasonStartDate) && !other.isBetween(SeasonEndDate) && (!(SeasonStartDate < other.SeasonStartDate) || !(other.SeasonEndDate < SeasonEndDate)))
		{
			if (other.SeasonStartDate < SeasonStartDate)
			{
				return SeasonEndDate < other.SeasonEndDate;
			}
			return false;
		}
		return true;
	}

	public SessionData GetCurrentSession(DateTime current)
	{
		if (sessionDatas.Last().EndDate < current)
		{
			return sessionDatas.Last();
		}
		foreach (SessionData data in sessionDatas)
		{
			if (!(current <= data.EndDate))
			{
				continue;
			}
			if (current < data.StartDate)
			{
				return sessionDatas.Find((SessionData e) => e.SessionId == data.SessionId - 1);
			}
			return data;
		}
		return new SessionData
		{
			SessionId = 0
		};
	}

	public GuildSeasonRewardTemplet GetRewardData(GuildDungeonRewardCategory category, int index)
	{
		List<GuildSeasonRewardTemplet> seasonRewardList = GuildDungeonTempletManager.GetSeasonRewardList(SeasonRaidGroup);
		if (seasonRewardList == null)
		{
			return null;
		}
		GuildSeasonRewardTemplet guildSeasonRewardTemplet = seasonRewardList.Where((GuildSeasonRewardTemplet e) => e.GetRewardCategory() == category && e.GetRewardCountValue() == index).First();
		if (guildSeasonRewardTemplet == null)
		{
			return null;
		}
		return guildSeasonRewardTemplet;
	}

	public SessionData GetNextSession(int sessionIndex)
	{
		if (sessionDatas.Count() < sessionIndex)
		{
			return sessionDatas.Find((SessionData e) => e.SessionId == sessionDatas.Count());
		}
		SessionData result = sessionDatas.Find((SessionData e) => e.SessionId == sessionIndex);
		if (result.SessionId == 0)
		{
			return new SessionData
			{
				SessionId = 0
			};
		}
		return result;
	}

	public SessionData GetSessionDate(int sessionId)
	{
		SessionData result = sessionDatas.Find((SessionData e) => e.SessionId == sessionId);
		if (result.SessionId != sessionId)
		{
			return new SessionData
			{
				SessionId = 0
			};
		}
		return result;
	}

	private List<SessionData> MakeSessionData(int seasonDungeonGroupId)
	{
		List<SessionData> list = new List<SessionData>();
		DateTime dateTime = SeasonStartDate;
		int num = 5;
		int num2 = 2;
		int num3 = 1;
		while (dateTime < SeasonEndDate)
		{
			SessionData item = new SessionData
			{
				SessionId = num3,
				StartDate = dateTime,
				templet = GetSessionScheduleTemplet(seasonDungeonGroupId, num3)
			};
			if (item.templet == null)
			{
				Log.Error($"[MakeSessionData] GetSessionScheduleTempelt Fail. seasonDungeonGroupId:{seasonDungeonGroupId} sessionIdex:{num3},", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildSeasonTemplet.cs", 247);
				return null;
			}
			dateTime = (item.EndDate = dateTime.AddDays(num));
			list.Add(item);
			dateTime = dateTime.AddDays(num2);
			num3++;
		}
		return list;
	}

	private GuildDungeonScheduleTemplet GetSessionScheduleTemplet(int seasonDungeonGroupId, int sessionId)
	{
		if (sessionId == -1)
		{
			Log.Info($"No Data. session id {sessionId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildSeasonTemplet.cs", 266);
			return null;
		}
		List<GuildDungeonScheduleTemplet> dungeonScheduleList = GuildDungeonTempletManager.GetDungeonScheduleList(seasonDungeonGroupId);
		if (dungeonScheduleList == null)
		{
			Log.Error($"sessionSchedule(seasonDungeonGroupId) is null. seasonId:{SeasonId} seasonDungeonGroupId:{seasonDungeonGroupId} sessionId:{sessionId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildSeasonTemplet.cs", 273);
			return null;
		}
		if (!dungeonScheduleList.Any((GuildDungeonScheduleTemplet e) => e.GetSessionIndex() == sessionId) || dungeonScheduleList.Select((GuildDungeonScheduleTemplet e) => e.GetSessionIndex() == sessionId).Count((bool e) => e) > 1)
		{
			Log.Error($"session schedule is invalid. duplicate sessionId. seasonId:{SeasonId} seasonDungeonGroupId:{seasonDungeonGroupId} sessionId:{sessionId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildSeasonTemplet.cs", 280);
			return null;
		}
		return dungeonScheduleList.Find((GuildDungeonScheduleTemplet e) => e.GetSessionIndex() == sessionId);
	}

	private int GetSeasionScheduleCount(int sessionDungeonGroup)
	{
		return GuildDungeonTempletManager.GetDungeonScheduleList(sessionDungeonGroup)?.Count ?? 0;
	}

	private bool dungeonValidation()
	{
		List<int> list = new List<int>();
		foreach (SessionData sessionData in sessionDatas)
		{
			list.AddRange(sessionData.templet.GetDungeonList());
		}
		list = list.Distinct().ToList();
		List<GuildDungeonInfoTemplet> dungeonInfoList = GuildDungeonTempletManager.GetDungeonInfoList(SeasonDungeonGroup);
		if (dungeonInfoList == null)
		{
			return false;
		}
		List<int> seasonDungeonList = dungeonInfoList.Select((GuildDungeonInfoTemplet e) => e.GetSeasonDungeonId()).Distinct().ToList();
		bool num = list.Where((int e) => seasonDungeonList.Contains(e)).Count() == list.Count();
		if (!num)
		{
			Log.Debug($"DungeonCountError. scheduleDungeonList.Where(e => seasonDungeonList.Contains(e)).Count():{list.Where((int e) => seasonDungeonList.Contains(e)).Count()} scheduleDungeonList.Count:{list.Count()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildSeasonTemplet.cs", 322);
		}
		return num;
	}

	private bool raidValidation()
	{
		List<GuildRaidTemplet> raidTempletList = GuildDungeonTempletManager.GetRaidTempletList(SeasonRaidGroup);
		if (raidTempletList == null)
		{
			return false;
		}
		List<int> list = raidTempletList.Select((GuildRaidTemplet e) => e.GetStageIndex()).ToList();
		list.Sort();
		if (list.First() != 1 || list.Last() != list.Count() || list.Count() != list.Distinct().Count())
		{
			Log.Error($"[GetSeasonRaidTempletList] invalid index Data. seasonRaidGroup:{SeasonRaidGroup} totalCount:{list.Count()} firstIndex:{list.First()} lastIndex:{list.Last()} distinctCount:{list.Distinct().Count()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildSeasonTemplet.cs", 344);
			return false;
		}
		list = raidTempletList.Select((GuildRaidTemplet e) => e.GetStageId()).ToList();
		if (list.Count() != list.Distinct().Count())
		{
			Log.Error($"[GetSeasonRaidTempletList] invalid raid stageId data. seasonRaidGroup:{SeasonRaidGroup} totalCount:{list.Count()} distinctCount:{list.Distinct().Count()}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Guild/GuildSeasonTemplet.cs", 352);
			return false;
		}
		return true;
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
	}
}
