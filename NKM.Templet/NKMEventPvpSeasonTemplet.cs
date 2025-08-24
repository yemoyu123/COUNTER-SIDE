using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Core.Util;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public sealed class NKMEventPvpSeasonTemplet : INKMTemplet
{
	private enum DAY_OF_WEEK
	{
		SUN,
		MON,
		TUE,
		WED,
		THU,
		FRI,
		SAT
	}

	private readonly List<DayOfWeek> openDaysOfWeek = new List<DayOfWeek>();

	private readonly List<string> battleConditionIds = new List<string>();

	private readonly List<int> rewardGroupIds = new List<int>();

	private int seasonId;

	private string openTag;

	private string seasonDesc;

	private string seasonName;

	private string seasonRule;

	private string lobbyArtResource;

	private string eventDeckThumbnail;

	private string intervalStrId;

	private string gameStateRateID;

	private int eventDeckId;

	private TimeSpan openTimeStart;

	private TimeSpan openTimeEnd;

	private bool noShuffleDeck;

	private bool forcedBanIgnore;

	private bool forcedBanEquip;

	private bool forceGetWinPoint;

	private bool forcedAuto;

	private bool unitMaxLevel;

	private int winPoint;

	private int losePoint;

	public List<int> preconditionGroups = new List<int>();

	private int respawnCountMaxSameTime;

	private bool useTournamentBan;

	private int mappingTournamentId;

	private int mapGroupId;

	private int dungeonId;

	private bool draftBanPick;

	private NKMDeckCondition m_DeckCondition;

	public List<NKMBattleConditionTemplet> BattleConditionTemplets = new List<NKMBattleConditionTemplet>();

	public Dictionary<int, List<NKMEventPvpRewardTemplet>> EventPvpRewardTemplets = new Dictionary<int, List<NKMEventPvpRewardTemplet>>();

	public int Key => seasonId;

	public static IEnumerable<NKMEventPvpSeasonTemplet> Values => NKMTempletContainer<NKMEventPvpSeasonTemplet>.Values;

	public List<DayOfWeek> OpenDaysOfWeek => openDaysOfWeek;

	public int SeasonId => seasonId;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(openTag);

	public string SeasonDesc => seasonDesc;

	public string SeasonName => seasonName;

	public string SeasonRule => seasonRule;

	public string LobbyArtResource => lobbyArtResource;

	public string EventDeckThumbnail => eventDeckThumbnail;

	public string IntervalStrId => intervalStrId;

	public TimeSpan OpenTimeStart => openTimeStart;

	public TimeSpan OpenTimeEnd => openTimeEnd;

	public bool NoShuffleDeck => noShuffleDeck;

	public bool ForcedBanIgnore => forcedBanIgnore;

	public bool ForceBanEquip => forcedBanEquip;

	public bool ForceAuto => forcedAuto;

	public bool ForceGetWinPoint => forceGetWinPoint;

	public int WinPoint => winPoint;

	public int LosePoint => losePoint;

	public int RespawnCountMaxSameTime => respawnCountMaxSameTime;

	public int DungeonId => dungeonId;

	public bool DraftBanPick => draftBanPick;

	public bool UnitMaxLevel => unitMaxLevel;

	public int MapGroupId => mapGroupId;

	public bool UseTournamentBan => useTournamentBan;

	public int MappingTournamentId => mappingTournamentId;

	public bool EnableTournamentBan
	{
		get
		{
			if (useTournamentBan)
			{
				return mappingTournamentId > 0;
			}
			return false;
		}
	}

	public List<int> BCPreconditionGroups => preconditionGroups;

	public NKMIntervalTemplet IntervalTemplet { get; private set; }

	public NKMDungeonEventDeckTemplet EventDeckTemplet { get; private set; }

	public NKMDeckCondition DeckCondition => m_DeckCondition;

	public NKMGameStatRateTemplet GameStatRateTemplet { get; private set; }

	public static NKMEventPvpSeasonTemplet Find(int key)
	{
		return NKMTempletContainer<NKMEventPvpSeasonTemplet>.Find(key);
	}

	public static NKMEventPvpSeasonTemplet Find(DateTime dateTime)
	{
		return Values.FirstOrDefault((NKMEventPvpSeasonTemplet e) => e.IntervalTemplet.IsValidTime(ServiceTime.Recent));
	}

	public static NKMEventPvpSeasonTemplet LoadFromLua(NKMLua lua)
	{
		NKMEventPvpSeasonTemplet nKMEventPvpSeasonTemplet = new NKMEventPvpSeasonTemplet();
		bool flag = true;
		flag &= lua.GetData("seasonID", ref nKMEventPvpSeasonTemplet.seasonId);
		flag &= lua.GetData("OpenTag", ref nKMEventPvpSeasonTemplet.openTag);
		string rValue = "";
		string rValue2 = "";
		flag &= lua.GetData("OpenTimeStart", ref rValue);
		flag &= lua.GetData("OpenTimeEnd", ref rValue2);
		if (lua.GetDataList("EnterLimitDays", out List<string> result, nullIfEmpty: false))
		{
			foreach (string item2 in result)
			{
				if (!Enum.TryParse<DAY_OF_WEEK>(item2, out var result2))
				{
					NKMTempletError.Add("[NKMEventPvpSeasonTemplet] " + nKMEventPvpSeasonTemplet.seasonName + "의 요일 입력이 올바르지 않음: " + item2, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventPvpSeasonTemplet.cs", 125);
					continue;
				}
				DayOfWeek item = (DayOfWeek)result2;
				if (nKMEventPvpSeasonTemplet.openDaysOfWeek.Contains(item))
				{
					NKMTempletError.Add("[NKMEventPvpSeasonTemplet] " + nKMEventPvpSeasonTemplet.seasonName + "의 요일 중복 설정: " + item2, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventPvpSeasonTemplet.cs", 132);
				}
				else
				{
					nKMEventPvpSeasonTemplet.openDaysOfWeek.Add(item);
				}
			}
		}
		flag &= nKMEventPvpSeasonTemplet.ParseTime(rValue, out nKMEventPvpSeasonTemplet.openTimeStart);
		flag &= nKMEventPvpSeasonTemplet.ParseTime(rValue2, out nKMEventPvpSeasonTemplet.openTimeEnd);
		flag &= lua.GetData("bNoShuffleDeck", ref nKMEventPvpSeasonTemplet.noShuffleDeck);
		flag &= lua.GetData("bForcedBanIgnore", ref nKMEventPvpSeasonTemplet.forcedBanIgnore);
		flag &= lua.GetData("bForcedBanEquip", ref nKMEventPvpSeasonTemplet.forcedBanEquip);
		flag &= lua.GetData("Interval", ref nKMEventPvpSeasonTemplet.intervalStrId);
		flag &= lua.GetData("EventDeckID", ref nKMEventPvpSeasonTemplet.eventDeckId);
		flag &= lua.GetData("GameStateRateID", ref nKMEventPvpSeasonTemplet.gameStateRateID);
		flag &= lua.GetData("bGetWinPoint", ref nKMEventPvpSeasonTemplet.forceGetWinPoint);
		flag &= lua.GetData("EventPVPWinPoint", ref nKMEventPvpSeasonTemplet.winPoint);
		flag &= lua.GetData("EventPVPLosePoint", ref nKMEventPvpSeasonTemplet.losePoint);
		lua.GetData("TournamentBan", ref nKMEventPvpSeasonTemplet.useTournamentBan);
		lua.GetData("TournamentID", ref nKMEventPvpSeasonTemplet.mappingTournamentId);
		lua.GetData("m_RespawnCountMaxSameTime", ref nKMEventPvpSeasonTemplet.respawnCountMaxSameTime);
		flag &= lua.GetDataList("BattleConditionID", out List<string> result3, nullIfEmpty: false);
		if (result3 != null)
		{
			nKMEventPvpSeasonTemplet.battleConditionIds.AddRange(result3);
		}
		lua.GetData("DungeonId", ref nKMEventPvpSeasonTemplet.dungeonId);
		flag &= lua.GetDataList("RewardGroupID", out List<int> result4, nullIfEmpty: false);
		if (result4 != null)
		{
			nKMEventPvpSeasonTemplet.rewardGroupIds.AddRange(result4);
		}
		lua.GetData("LobbyArtResource", ref nKMEventPvpSeasonTemplet.lobbyArtResource);
		lua.GetData("EventDeckThumbnail", ref nKMEventPvpSeasonTemplet.eventDeckThumbnail);
		lua.GetData("SeasonName", ref nKMEventPvpSeasonTemplet.seasonName);
		lua.GetData("SeasonDesc", ref nKMEventPvpSeasonTemplet.seasonDesc);
		lua.GetData("SeasonRule", ref nKMEventPvpSeasonTemplet.seasonRule);
		lua.GetData("MapGroupId", ref nKMEventPvpSeasonTemplet.mapGroupId);
		lua.GetData("bForcedAuto", ref nKMEventPvpSeasonTemplet.forcedAuto);
		lua.GetData("DraftBanPick", ref nKMEventPvpSeasonTemplet.draftBanPick);
		lua.GetData("UnitMaxLevel", ref nKMEventPvpSeasonTemplet.unitMaxLevel);
		lua.GetDataList("m_PreConditionGroup", out nKMEventPvpSeasonTemplet.preconditionGroups, nullIfEmpty: false);
		NKMDeckCondition.LoadFromLua(lua, "DECK_CONDITION", out nKMEventPvpSeasonTemplet.m_DeckCondition, $"NKMEventPvpSeasonTemplet DeckCondition Parse Fail. seasonID : {nKMEventPvpSeasonTemplet.SeasonId}");
		return nKMEventPvpSeasonTemplet;
	}

	public void Join()
	{
		if (NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
		}
		if (!draftBanPick)
		{
			EventDeckTemplet = NKMDungeonManager.GetEventDeckTemplet(eventDeckId);
		}
		foreach (string battleConditionId in battleConditionIds)
		{
			NKMBattleConditionTemplet templetByStrID = NKMBattleConditionManager.GetTempletByStrID(battleConditionId);
			if (templetByStrID == null)
			{
				NKMTempletError.Add("[EventPvpSeasonTemplet:" + SeasonName + "] invalid battleConditionId:" + battleConditionId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventPvpSeasonTemplet.cs", 207);
			}
			else
			{
				BattleConditionTemplets.Add(templetByStrID);
			}
		}
		foreach (int rewardGroupId in rewardGroupIds)
		{
			IEnumerable<NKMEventPvpRewardTemplet> enumerable = NKMEventPvpRewardTemplet.Values.Where((NKMEventPvpRewardTemplet e) => e.RewardGroupId == rewardGroupId);
			if (enumerable != null)
			{
				List<NKMEventPvpRewardTemplet> list = enumerable.ToList();
				if (list.Count != 0)
				{
					EventPvpRewardTemplets.Add(rewardGroupId, list);
				}
			}
		}
		GameStatRateTemplet = NKMGameStatRateTemplet.Find(gameStateRateID);
		if (GameStatRateTemplet == null)
		{
			NKMTempletError.Add("[EventPvpSeason] invalid GameStatRateId:" + gameStateRateID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventPvpSeasonTemplet.cs", 231);
		}
		if (!forceGetWinPoint)
		{
			winPoint = 0;
			losePoint = 0;
		}
	}

	public void Validate()
	{
		if (openTimeStart > openTimeEnd)
		{
			NKMTempletError.Add($"[EventPvpSeasonTemplet:{SeasonName}] EndTime이 StartTime보다 작음 openTimeEnd :{openTimeEnd}, openTimeStart : {openTimeStart}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventPvpSeasonTemplet.cs", 245);
		}
		if (openTimeEnd - openTimeStart > TimeSpan.FromDays(1.0))
		{
			NKMTempletError.Add($"[EventPvpSeasonTemplet:{SeasonName}] TimeSpan 간격이 하루가 넘음 openTimeEnd :{openTimeEnd}, openTimeStart : {openTimeStart}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventPvpSeasonTemplet.cs", 250);
		}
		if (!draftBanPick && EventDeckTemplet == null)
		{
			NKMTempletError.Add($"[EventPvpSeasonTemplet:{SeasonName}] 이벤트 덱이 존재하지 않음 {eventDeckId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventPvpSeasonTemplet.cs", 256);
		}
		if (rewardGroupIds.Count > 0)
		{
			if (EventPvpRewardTemplets.Count != rewardGroupIds.Count)
			{
				NKMTempletError.Add($"[EventPvpSeasonTemplet:{SeasonName}] 보상정보 Group ID 일치하지 않음 WriteCount : {rewardGroupIds.Count}, TempletCount : {EventPvpRewardTemplets.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventPvpSeasonTemplet.cs", 263);
			}
			int num = -1;
			int num2 = -1;
			foreach (int rewardGroupId in rewardGroupIds)
			{
				if (!EventPvpRewardTemplets.TryGetValue(rewardGroupId, out var value))
				{
					NKMTempletError.Add($"[EventPvpSeasonTemplet:{SeasonName}] 보상정보 Group ID에 맞는 Tempelt이 존재하지 않음 RewardGroup : {rewardGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventPvpSeasonTemplet.cs", 272);
				}
				int type = (int)value.FirstOrDefault().Type;
				int countCondition = (int)value.FirstOrDefault().CountCondition;
				if (num == type && num2 == countCondition)
				{
					NKMTempletError.Add("[EventPvpSeasonTemplet:" + SeasonName + "] 시즌 내 보상 그룹에 중복되는 타입 존재", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventPvpSeasonTemplet.cs", 281);
				}
				num = type;
				num2 = countCondition;
				int num3 = 0;
				int num4 = 0;
				foreach (NKMEventPvpRewardTemplet item in value)
				{
					if (num3 == item.Step)
					{
						NKMTempletError.Add($"[EventPvpSeasonTemplet:{SeasonName}] 보상 그룹에 중복되는 스텝 존재 Step : {item.Step}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventPvpSeasonTemplet.cs", 292);
					}
					if (num4 == item.RewardId)
					{
						NKMTempletError.Add($"[EventPvpSeasonTemplet:{SeasonName}] 보상 그룹에 중복되는 ID 존재 RewardID : {item.RewardId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventPvpSeasonTemplet.cs", 297);
					}
					num3 = item.Step;
					num4 = item.RewardId;
				}
			}
		}
		if (forceGetWinPoint && winPoint + losePoint <= 0)
		{
			NKMTempletError.Add("[EventPvpSeasonTemplet:" + SeasonName + "] 획득할 수 있는 점수가 존재하지 않음.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventPvpSeasonTemplet.cs", 310);
		}
		if (draftBanPick && !forcedBanIgnore)
		{
			NKMTempletError.Add("[EventPvpSeasonTemplet:" + SeasonName + "] 드래프트 밴픽 옵션이 활성화 되었으나 bForcedBanIgnore이 true가 아님.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventPvpSeasonTemplet.cs", 317);
		}
		if (useTournamentBan)
		{
			if (mappingTournamentId <= 0)
			{
				NKMTempletError.Add("[EventPvpSeasonTemplet:" + SeasonName + "] 토너먼트 옵션이 활성화 되었으나 연결된 시즌 ID가 없음", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventPvpSeasonTemplet.cs", 324);
			}
			NKMTournamentTemplet nKMTournamentTemplet = NKMTournamentTemplet.Find(mappingTournamentId);
			if (nKMTournamentTemplet == null)
			{
				NKMTempletError.Add($"[EventPvpSeasonTemplet:{SeasonName}] 토너먼트 벤 옵션이 활성화 되었지만 지정된 토너먼트 id의 토너먼트 템플릿을 찾을 수 없음. mappingTournamentId:{mappingTournamentId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventPvpSeasonTemplet.cs", 330);
			}
			else if (IntervalTemplet.StartDate.Date.Day != nKMTournamentTemplet.IntervalTemplet.StartDate.Date.Day)
			{
				if (EnableByTag)
				{
					NKMTempletError.Add($"[EventPvpSeasonTemplet:{SeasonName}] 토너먼트 벤 옵션이 활성화 되었지만 지정된 토너먼트 템플릿과 시작 일(Day) 값이 다름. mappingTournamentId:{mappingTournamentId} / EventPvp:{IntervalTemplet.StartDate} ~ {IntervalTemplet.EndDate} / Tournament:{nKMTournamentTemplet.IntervalTemplet.StartDate} ~ {nKMTournamentTemplet.IntervalTemplet.EndDate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventPvpSeasonTemplet.cs", 339);
				}
				else
				{
					Log.Error($"[EventPvpSeasonTemplet:{SeasonName}] 토너먼트 벤 옵션이 활성화 되었지만 지정된 토너먼트 템플릿과 시작 일(Day) 값이 다름. mappingTournamentId:{mappingTournamentId} / EventPvp:{IntervalTemplet.StartDate} ~ {IntervalTemplet.EndDate} / Tournament:{nKMTournamentTemplet.IntervalTemplet.StartDate} ~ {nKMTournamentTemplet.IntervalTemplet.EndDate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventPvpSeasonTemplet.cs", 343);
				}
			}
		}
		else if (mappingTournamentId > 0)
		{
			NKMTempletError.Add($"[EventPvpSeasonTemplet:{SeasonName}] 토너먼트 옵션이 비활성화 되었으나 연결된 ID가 존재함. mappingTournamentId :{mappingTournamentId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventPvpSeasonTemplet.cs", 352);
		}
	}

	public bool ValidateTime(DateTime current)
	{
		TimeSpan timeSpan = new TimeSpan(current.Hour, current.Minute, current.Second);
		TimeSpan timeSpan2 = new TimeSpan(openTimeEnd.Hours, openTimeEnd.Minutes, openTimeEnd.Seconds);
		if (timeSpan2 <= openTimeStart)
		{
			if (timeSpan < timeSpan2)
			{
				timeSpan += TimeSpan.FromDays(1.0);
			}
			timeSpan2 += TimeSpan.FromDays(1.0);
		}
		if (timeSpan >= openTimeStart && timeSpan < timeSpan2)
		{
			return true;
		}
		return false;
	}

	public bool ValidateDayOfWeek(DateTime current)
	{
		DayOfWeek dayOfWeek = current.DayOfWeek;
		TimeSpan timeSpan = new TimeSpan(current.Hour, current.Minute, current.Second);
		TimeSpan timeSpan2 = new TimeSpan(openTimeEnd.Hours, openTimeEnd.Minutes, openTimeEnd.Seconds);
		if (timeSpan2 < openTimeStart && timeSpan < timeSpan2)
		{
			dayOfWeek = current.AddDays(-1.0).DayOfWeek;
		}
		if (openDaysOfWeek.Contains(dayOfWeek))
		{
			return true;
		}
		return false;
	}

	private bool ParseTime(string timeStr, out TimeSpan timeSpan)
	{
		timeSpan = TimeSpan.MinValue;
		if (timeStr == "")
		{
			timeStr = timeStr.PadRight(4, '0');
		}
		if (!int.TryParse(timeStr.Substring(0, 2), out var result) || !int.TryParse(timeStr.Substring(2, 2), out var result2))
		{
			return false;
		}
		if (result2 < 0 || result2 >= 60)
		{
			return false;
		}
		timeSpan = new TimeSpan(result, result2, 0);
		return true;
	}

	private void JoinIntervalTemplet()
	{
		IntervalTemplet = NKMIntervalTemplet.Find(intervalStrId);
		if (IntervalTemplet == null)
		{
			IntervalTemplet = NKMIntervalTemplet.Invalid;
			NKMTempletError.Add("[EventPvpSeasonTemplet:" + SeasonName + "] invalid seasonDateStrId:" + intervalStrId, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventPvpSeasonTemplet.cs", 430);
		}
	}
}
