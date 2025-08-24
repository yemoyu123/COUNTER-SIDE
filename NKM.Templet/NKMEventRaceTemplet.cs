using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using NKM.Templet.Base;

namespace NKM.Templet;

public class NKMEventRaceTemplet : INKMTemplet, INKMTempletEx
{
	private int eventId;

	private string openTag;

	private string dateStrId;

	private int raceItemId;

	private int raceTryItemValue;

	private int eventRavceIndexCount;

	private string eventRaceId;

	private string eventRaceName;

	private List<int> raceMissionID;

	private string teamWinMailTitle;

	private string teamWinMailDesc;

	private string teamLoseMailTitle;

	private string teamLoseMailDesc;

	private string teamABubbleHappy;

	private string teamABubbleSad;

	private string teamBBubbleHappy;

	private string teamBBubbleSad;

	private string shopShortCut;

	private NKMIntervalTemplet intervalTemplet;

	public static IEnumerable<NKMEventRaceTemplet> Values => NKMTempletContainer<NKMEventRaceTemplet>.Values;

	public int Key => eventId;

	public bool EnableByTag => NKMOpenTagManager.IsOpened(openTag);

	public int IndexCount => eventRavceIndexCount;

	public int RaceItemId => raceItemId;

	public int RaceTryItemValue => raceTryItemValue;

	public string RaceName => eventRaceName;

	public DateTime StartDate => intervalTemplet.StartDate;

	public DateTime EndDate => intervalTemplet.EndDate;

	public string TeamWinMailTitle => teamWinMailTitle;

	public string TeamWinMailDesc => teamWinMailDesc;

	public string TeamLoseMailTitle => teamLoseMailTitle;

	public string TeamLoseMailDesc => teamLoseMailDesc;

	public List<int> RaceMissionID => raceMissionID;

	public string TeamABubbleHappy => teamABubbleHappy;

	public string TeamABubbleSad => teamABubbleSad;

	public string TeamBBubbleHappy => teamBBubbleHappy;

	public string TeamBBubbleSad => teamBBubbleSad;

	public int MaxBetCount => raceTryItemValue;

	public string ShopShortCutParam => shopShortCut;

	public static NKMEventRaceTemplet Find(int key)
	{
		return NKMTempletContainer<NKMEventRaceTemplet>.Find((NKMEventRaceTemplet x) => x.eventId == key);
	}

	public static NKMEventRaceTemplet GetByTime(DateTime time)
	{
		return Values.FirstOrDefault((NKMEventRaceTemplet e) => e.IsOnRace(time));
	}

	public static NKMEventRaceTemplet LoadFromLua(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventRaceTemplet.cs", 52))
		{
			return null;
		}
		bool flag = true;
		NKMEventRaceTemplet nKMEventRaceTemplet = new NKMEventRaceTemplet();
		flag &= lua.GetData("m_EventID", ref nKMEventRaceTemplet.eventId);
		flag &= lua.GetData("m_OpenTag", ref nKMEventRaceTemplet.openTag);
		flag &= lua.GetData("m_DateStrID", ref nKMEventRaceTemplet.dateStrId);
		flag &= lua.GetData("m_RaceTryItemID", ref nKMEventRaceTemplet.raceItemId);
		flag &= lua.GetData("m_RaceTryItemValue", ref nKMEventRaceTemplet.raceTryItemValue);
		lua.GetData("m_EventRaceID", ref nKMEventRaceTemplet.eventRaceId);
		lua.GetData("m_EventRaceName", ref nKMEventRaceTemplet.eventRaceName);
		nKMEventRaceTemplet.raceMissionID = new List<int>();
		if (lua.OpenTable("m_RaceMissionID"))
		{
			int i = 1;
			for (int rValue = 0; lua.GetData(i, ref rValue); i++)
			{
				nKMEventRaceTemplet.raceMissionID.Add(rValue);
			}
			lua.CloseTable();
		}
		lua.GetData("m_TeamWinMailTitle", ref nKMEventRaceTemplet.teamWinMailTitle);
		lua.GetData("m_TeamWinMailDesc", ref nKMEventRaceTemplet.teamWinMailDesc);
		lua.GetData("m_TeamLoseMailTitle", ref nKMEventRaceTemplet.teamLoseMailTitle);
		lua.GetData("m_TeamLoseMailDesc", ref nKMEventRaceTemplet.teamLoseMailDesc);
		lua.GetData("m_TeamABubbleHappy", ref nKMEventRaceTemplet.teamABubbleHappy);
		lua.GetData("m_TeamABubbleSad", ref nKMEventRaceTemplet.teamABubbleSad);
		lua.GetData("m_TeamBBubbleHappy", ref nKMEventRaceTemplet.teamBBubbleHappy);
		lua.GetData("m_TeamBBubbleSad", ref nKMEventRaceTemplet.teamBBubbleSad);
		lua.GetData("m_ShortCutShop", ref nKMEventRaceTemplet.shopShortCut);
		if (!flag)
		{
			return null;
		}
		return nKMEventRaceTemplet;
	}

	public void Join()
	{
		intervalTemplet = NKMIntervalTemplet.Find(dateStrId);
		if (intervalTemplet == null)
		{
			NKMTempletError.Add($"[NKMEventBetTemplet] interval templet을 찾을 수 없음. EventId:{eventId} dateStrId:{dateStrId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventRaceTemplet.cs", 107);
		}
		else
		{
			eventRavceIndexCount = (intervalTemplet.EndDate - intervalTemplet.StartDate).Days;
		}
	}

	public void Validate()
	{
		if (intervalTemplet.StartDate.Hour != 4 || intervalTemplet.EndDate.Hour != 4)
		{
			NKMTempletError.Add($"[NKMEventBetTemplet] 경주 시작, 종료 시간이 4시가 아님. RaceId:{Key} RaceStartDate:{intervalTemplet.StartDate.Hour} RaceEndDate:{intervalTemplet.EndDate.Hour}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventRaceTemplet.cs", 120);
		}
		if (intervalTemplet.EndDate <= intervalTemplet.StartDate)
		{
			NKMTempletError.Add($"[NKMEventBetTemplet] 경주 종료시간이 시작시간보다 작음. RaceId:{Key} RaceEndDate:{intervalTemplet.EndDate} <= RaceStartDate:{intervalTemplet.StartDate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventRaceTemplet.cs", 125);
		}
		if (eventRavceIndexCount <= 0)
		{
			NKMTempletError.Add($"[NKMEventBetTemplet] 경주는 1일 이상 진행되어야 함. RaceId:{Key} RaceEndDate:{intervalTemplet.EndDate} - RaceStartDate:{intervalTemplet.StartDate} <= 0", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventRaceTemplet.cs", 130);
		}
		if (!NKMRewardTemplet.IsValidReward(NKM_REWARD_TYPE.RT_MISC, raceItemId))
		{
			NKMTempletError.Add($"[NKMEventBetTemplet] 경주 소비 아이템  m_reward_type:{NKM_REWARD_TYPE.RT_MISC} m_RewardID:{raceItemId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventRaceTemplet.cs", 136);
		}
		if (raceTryItemValue <= 0)
		{
			NKMTempletError.Add($"[NKMEventBetTemplet] 경주 필요 아이템 개수 이상. RaceId:{Key}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventRaceTemplet.cs", 141);
		}
	}

	public bool RaceIndexValidate(int raceIndex)
	{
		return raceIndex < eventRavceIndexCount;
	}

	public bool IsOnRace(DateTime current)
	{
		if (!EnableByTag)
		{
			return false;
		}
		if (intervalTemplet.StartDate <= current && current <= intervalTemplet.EndDate)
		{
			return true;
		}
		return false;
	}

	public bool CanBetTime(DateTime current)
	{
		if (!EnableByTag)
		{
			return false;
		}
		if (intervalTemplet.StartDate < current && current <= intervalTemplet.EndDate.AddDays(-1.0))
		{
			DateTime dateTime = new DateTime(current.Year, current.Month, current.Day, 4, 0, 0);
			if (dateTime.AddMinutes(-10.0) <= current && current <= dateTime.AddMinutes(10.0))
			{
				Log.Debug($"[NKMEventRaceTemplet] Invalid Race Time. restTime:{dateTime.AddMinutes(-10.0)}~{dateTime.AddMinutes(10.0)} current:{current}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Templet/NKMEventRaceTemplet.cs", 176);
				return false;
			}
		}
		if (intervalTemplet.StartDate <= current && current <= intervalTemplet.EndDate.AddDays(-1.0))
		{
			return true;
		}
		return false;
	}

	public int GetRaceIndex(DateTime current)
	{
		if (!intervalTemplet.IsValidTime(current))
		{
			return -1;
		}
		return (current - intervalTemplet.StartDate).Days;
	}

	public void PostJoin()
	{
		Join();
	}
}
