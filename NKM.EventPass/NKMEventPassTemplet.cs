using System;
using System.Linq;
using ClientPacket.Event;
using Cs.Logging;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.EventPass;

public class NKMEventPassTemplet : INKMTemplet, INKMTempletEx
{
	private string dateStrId;

	private string m_OpenTag;

	public int EventPassId { get; private set; }

	public DateTime EventPassStartDate => IntervalTemplet.StartDate;

	public DateTime EventPassEndDate => IntervalTemplet.EndDate;

	public string EventPassTitleStrId { get; private set; }

	public NKM_REWARD_TYPE EventPassMainRewardType { get; private set; }

	public int EventPassMainReward { get; private set; }

	public NKMEventPassCoreProductTemplet CorePassProduct { get; private set; }

	public NKMEventPassCorePlusProductTemplet CorePassPlusPrdouct { get; private set; }

	public int PassMaxLevel { get; private set; }

	public int PassMaxExp { get; private set; }

	public int PassLevelUpExp { get; private set; }

	public int PassRewardGroupId { get; private set; }

	public int DailyMissionGroupId { get; private set; }

	public int DailyMissionMaxSlot { get; private set; }

	public int DailyMissionClearCount { get; private set; }

	public int DailyMissionClearRewardExp { get; private set; }

	public int WeeklyMissionGroupId { get; private set; }

	public int WeeklyMissionMaxSlot { get; private set; }

	public int WeeklyMissionClearCount { get; private set; }

	public int WeeklyMissionClearRewardExp { get; private set; }

	public int PassLevelUpMiscId { get; private set; }

	public int PassLevelUpMiscCount { get; private set; }

	public float CorePassDiscountPercent { get; private set; }

	public NKM_SHORTCUT_TYPE m_ShortCutType { get; private set; }

	public string m_ShortCut { get; private set; }

	public NKMIntervalTemplet IntervalTemplet { get; internal set; }

	public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public int Key => EventPassId;

	public static NKMEventPassTemplet LoadFromLUA(NKMLua lua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(lua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassTemplet.cs", 142))
		{
			return null;
		}
		int rValue = 0;
		bool data = lua.GetData("EventPassID", ref rValue);
		string rValue2 = string.Empty;
		data &= lua.GetData("EventPassTitleStrID", ref rValue2);
		NKM_REWARD_TYPE result = NKM_REWARD_TYPE.RT_NONE;
		data &= lua.GetData("EventPassMainRewardType", ref result);
		int rValue3 = 0;
		data &= lua.GetData("EventPassMainReward", ref rValue3);
		NKMEventPassCoreProductTemplet nKMEventPassCoreProductTemplet = NKMEventPassCoreProductTemplet.LoadFromLUA(lua);
		data = data && nKMEventPassCoreProductTemplet != null;
		NKMEventPassCorePlusProductTemplet nKMEventPassCorePlusProductTemplet = NKMEventPassCorePlusProductTemplet.LoadFromLUA(lua);
		data = data && nKMEventPassCorePlusProductTemplet != null;
		int rValue4 = 0;
		data &= lua.GetData("PassMaxLevel", ref rValue4);
		int rValue5 = 0;
		data &= lua.GetData("PassLevelUpExp", ref rValue5);
		int rValue6 = 0;
		data &= lua.GetData("PassRewardGroupID", ref rValue6);
		int rValue7 = 0;
		data &= lua.GetData("DailyMissionGroupID", ref rValue7);
		int rValue8 = 0;
		data &= lua.GetData("DailyMissionMaxSlot", ref rValue8);
		int rValue9 = 0;
		data &= lua.GetData("DailyMissionClearCount", ref rValue9);
		int rValue10 = 0;
		data &= lua.GetData("DailyMissionClearRewardExp", ref rValue10);
		int rValue11 = 0;
		data &= lua.GetData("WeeklyMissionGroupID", ref rValue11);
		int rValue12 = 0;
		data &= lua.GetData("WeeklyMissionMaxSlot", ref rValue12);
		int rValue13 = 0;
		data &= lua.GetData("WeeklyMissionClearCount", ref rValue13);
		int rValue14 = 0;
		data &= lua.GetData("WeeklyMissionClearRewardExp", ref rValue14);
		int rValue15 = 0;
		data &= lua.GetData("PassLevelUpMiscID", ref rValue15);
		int rValue16 = 0;
		data &= lua.GetData("PassLevelUpMiscCount", ref rValue16);
		float rValue17 = 0f;
		data &= lua.GetData("CorePassDiscountPercent", ref rValue17);
		NKM_SHORTCUT_TYPE result2 = NKM_SHORTCUT_TYPE.SHORTCUT_NONE;
		lua.GetData("m_ShortCutType", ref result2);
		string rValue18 = string.Empty;
		lua.GetData("m_ShortCut", ref rValue18);
		string rValue19 = null;
		if (NKMContentsVersionManager.HasDFChangeTagType(DataFormatChangeTagType.OPEN_TAG_EVENTPASS))
		{
			data &= lua.GetData("m_OpenTag", ref rValue19);
			if (!data)
			{
				Log.Error($"[EventPassTemplet] ID[{rValue}] - no OpenTag", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassTemplet.cs", 216);
			}
		}
		NKMEventPassTemplet nKMEventPassTemplet = new NKMEventPassTemplet
		{
			EventPassId = rValue,
			EventPassTitleStrId = rValue2,
			EventPassMainRewardType = result,
			EventPassMainReward = rValue3,
			CorePassProduct = nKMEventPassCoreProductTemplet,
			CorePassPlusPrdouct = nKMEventPassCorePlusProductTemplet,
			PassMaxLevel = rValue4,
			PassMaxExp = (rValue4 - 1) * rValue5,
			PassLevelUpExp = rValue5,
			PassRewardGroupId = rValue6,
			DailyMissionGroupId = rValue7,
			DailyMissionMaxSlot = rValue8,
			DailyMissionClearRewardExp = rValue10,
			DailyMissionClearCount = rValue9,
			WeeklyMissionGroupId = rValue11,
			WeeklyMissionMaxSlot = rValue12,
			WeeklyMissionClearCount = rValue13,
			WeeklyMissionClearRewardExp = rValue14,
			PassLevelUpMiscId = rValue15,
			PassLevelUpMiscCount = rValue16,
			CorePassDiscountPercent = rValue17,
			m_ShortCutType = result2,
			m_ShortCut = rValue18,
			m_OpenTag = rValue19
		};
		if (!(data & lua.GetData("m_DateStrID", ref nKMEventPassTemplet.dateStrId)))
		{
			Log.ErrorAndExit($"[EventPassTemplet] data is invalid, event pass id: {rValue}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassTemplet.cs", 251);
			return null;
		}
		return nKMEventPassTemplet;
	}

	public static NKMEventPassTemplet GetCurrentTemplet(DateTime current)
	{
		foreach (NKMEventPassTemplet value in NKMTempletContainer<NKMEventPassTemplet>.Values)
		{
			if (!value.IsSeasonOut(current))
			{
				return value;
			}
		}
		return null;
	}

	public static NKMEventPassTemplet Find(int eventPassId)
	{
		return NKMTempletContainer<NKMEventPassTemplet>.Find(eventPassId);
	}

	public bool IsSeasonOut(DateTime current)
	{
		if (NKMContentsVersionManager.HasDFChangeTagType(DataFormatChangeTagType.OPEN_TAG_EVENTPASS) && !EnableByTag)
		{
			return true;
		}
		if (current < EventPassStartDate || current > EventPassEndDate)
		{
			return true;
		}
		return false;
	}

	public static NKMEventPassTemplet GetPervTemplet(int eventPassId)
	{
		return NKMTempletContainer<NKMEventPassTemplet>.Values.LastOrDefault((NKMEventPassTemplet e) => e.EventPassId < eventPassId);
	}

	public int ConvertLevel(int totalExp)
	{
		return totalExp / PassLevelUpExp + 1;
	}

	public int GetWeekSinceEventStart(DateTime current)
	{
		TimeSpan timeSpan = current - EventPassStartDate;
		if (timeSpan <= TimeSpan.Zero)
		{
			return 1;
		}
		int num = (int)timeSpan.TotalDays;
		int num2 = num / 7;
		int num3 = num % 7;
		int num4 = (int)(current.DayOfWeek - 1);
		if (num4 < 0)
		{
			num4 += 7;
		}
		if (num3 >= num4)
		{
			num2++;
		}
		if (EventPassStartDate.DayOfWeek != DayOfWeek.Monday)
		{
			num2++;
		}
		return num2;
	}

	public int GetCorePassPriceDiscounted(int priceCount)
	{
		if (CorePassDiscountPercent <= 0f)
		{
			return priceCount;
		}
		priceCount -= (int)((float)(CorePassPlusPrdouct.PassExp / PassLevelUpExp * PassLevelUpMiscCount) * CorePassDiscountPercent);
		return Math.Max(0, priceCount);
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
		IntervalTemplet = NKMIntervalTemplet.Find(dateStrId);
		if (IntervalTemplet == null)
		{
			IntervalTemplet = NKMIntervalTemplet.Unuseable;
			NKMTempletError.Add($"[EventPass:{Key}] invalid interval id:{dateStrId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassTemplet.cs", 361);
		}
		else if (IntervalTemplet.IsRepeatDate)
		{
			Log.ErrorAndExit($"[EventPass:{Key}] 반복 기간설정 사용 불가. id:{dateStrId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassTemplet.cs", 367);
		}
	}

	public void Validate()
	{
		if (!EnableByTag)
		{
			return;
		}
		if (EventPassStartDate >= EventPassEndDate)
		{
			Log.Error($"[EvetPass] 시작 시간이 종료시간 보다 크거나 같습니다. eventPassId: {EventPassId}, startDate: {EventPassStartDate}, endDate: {EventPassEndDate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassTemplet.cs", 380);
			return;
		}
		NKMEventPassTemplet nKMEventPassTemplet = NKMTempletContainer<NKMEventPassTemplet>.Values.Where((NKMEventPassTemplet e) => e.EventPassId != EventPassId).FirstOrDefault((NKMEventPassTemplet e) => e.EventPassStartDate == EventPassStartDate || e.EventPassEndDate == EventPassEndDate);
		if (nKMEventPassTemplet != null)
		{
			Log.Error($"[EventPass] 이벤트 패스 시작 혹은 종료 시간이 동일한 내역이 있습니다. eventPassId: {EventPassId}, startDate: {EventPassStartDate}, endDate: {EventPassEndDate}" + $"중복된 eventPassId: {nKMEventPassTemplet.EventPassId}, startDate: {nKMEventPassTemplet.EventPassStartDate}, endDate: {nKMEventPassTemplet.EventPassEndDate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassTemplet.cs", 389);
		}
		else if (NKMEventPassMissionGroupTemplet.GetMissionGroupData(EventPassMissionType.Daily, DailyMissionGroupId) == null)
		{
			Log.ErrorAndExit($"[EventPass] 일일 미션 그룹 데이터가 존재하지 않습니다. dailyMissionGroupId: {DailyMissionGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassTemplet.cs", 396);
		}
		else if (NKMEventPassMissionGroupTemplet.GetMissionGroupData(EventPassMissionType.Weekly, WeeklyMissionGroupId) == null)
		{
			Log.ErrorAndExit($"[EventPass] 주간 미션 그룹 데이터가 존재하지 않습니다. weeklyMissionGroupId: {WeeklyMissionGroupId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassTemplet.cs", 402);
		}
		else if (NKMItemManager.GetItemMiscTempletByID(PassLevelUpMiscId) == null)
		{
			Log.ErrorAndExit($"[EventPass] 패스 레벨업 misc id가 유효하지 않습니다. misc id: {PassLevelUpMiscId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassTemplet.cs", 408);
		}
		else if (PassLevelUpMiscCount <= 0)
		{
			Log.ErrorAndExit($"[EventPass] 패스 레벨업 misc count가 유효하지 않습니다. count: {PassLevelUpMiscCount}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/EventPass/NKMEventPassTemplet.cs", 414);
		}
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
	}
}
