using System;
using System.Collections.Generic;
using Cs.Core.Util;
using Cs.Logging;
using Cs.Shared.Time;
using NKC;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKMAttendanceTabTemplet : INKMTemplet, INKMTempletEx
{
	private int idx;

	private int tabID;

	private NKM_ATTENDANCE_EVENT_TYPE eventType;

	private int rewardGroup;

	private string tabNameMain;

	private string tabNameSub;

	private string backgroundImage;

	private int limitDayCount;

	private int returnDayCount;

	private string prefabName;

	private string tabIconName;

	private int maxAttCount;

	private string intervalId;

	private string m_OpenTag;

	private NKMIntervalTemplet intervalTemplet = NKMIntervalTemplet.Invalid;

	public int Key => idx;

	public int IDX => idx;

	public int TabID => tabID;

	public NKM_ATTENDANCE_EVENT_TYPE EventType => eventType;

	public int RewardGroup => rewardGroup;

	public string TabNameMain => tabNameMain;

	public string TabNameSub => tabNameSub;

	public string BackgroundImage => backgroundImage;

	public NKMIntervalTemplet IntervalTemplet => intervalTemplet;

	public DateTime StartDateUtc => ServiceTime.ToUtcTime(intervalTemplet.StartDate);

	public DateTime EndDateUtc => ServiceTime.ToUtcTime(intervalTemplet.EndDate);

	public int LimitDayCount => limitDayCount;

	public int ReturnDayCount => returnDayCount;

	public string PrefabName => prefabName;

	public string TabIconName => tabIconName;

	public int MaxAttCount => maxAttCount;

	public IReadOnlyDictionary<int, NKMAttendanceRewardTemplet> RewardTemplets { get; private set; }

	public bool EnableByTag => NKMOpenTagManager.IsOpened(m_OpenTag);

	public static NKMAttendanceTabTemplet LoadFromLUA(NKMLua cNKMLua)
	{
		if (!NKMContentsVersionManager.CheckContentsVersion(cNKMLua, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMAttendanceManager.cs", 89))
		{
			return null;
		}
		NKMAttendanceTabTemplet nKMAttendanceTabTemplet = new NKMAttendanceTabTemplet();
		bool flag = true;
		flag &= cNKMLua.GetData("IDX", ref nKMAttendanceTabTemplet.idx);
		flag &= cNKMLua.GetData("m_TabID", ref nKMAttendanceTabTemplet.tabID);
		flag &= cNKMLua.GetData("m_EventType", ref nKMAttendanceTabTemplet.eventType);
		flag &= cNKMLua.GetData("m_RewardGroup", ref nKMAttendanceTabTemplet.rewardGroup);
		flag &= cNKMLua.GetData("m_TabNameMain", ref nKMAttendanceTabTemplet.tabNameMain);
		flag &= cNKMLua.GetData("m_TabNameSub", ref nKMAttendanceTabTemplet.tabNameSub);
		flag &= cNKMLua.GetData("m_TabBackgroundImage", ref nKMAttendanceTabTemplet.backgroundImage);
		flag &= cNKMLua.GetData("m_LimitDayCount", ref nKMAttendanceTabTemplet.limitDayCount);
		flag &= cNKMLua.GetData("m_ReturnDayCount", ref nKMAttendanceTabTemplet.returnDayCount);
		flag &= cNKMLua.GetData("m_PrefabName", ref nKMAttendanceTabTemplet.prefabName);
		flag &= cNKMLua.GetData("m_TabIconName", ref nKMAttendanceTabTemplet.tabIconName);
		flag &= cNKMLua.GetData("m_MaxAttCount", ref nKMAttendanceTabTemplet.maxAttCount);
		flag &= cNKMLua.GetData("m_DateStrID", ref nKMAttendanceTabTemplet.intervalId);
		if (NKMContentsVersionManager.HasDFChangeTagType(DataFormatChangeTagType.OPEN_TAG_ATTENDANCE))
		{
			flag &= cNKMLua.GetData("m_OpenTag", ref nKMAttendanceTabTemplet.m_OpenTag);
		}
		if (!flag)
		{
			Log.Error("NKMAttendanceTabTemplet LoadFromLUA Fail", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMAttendanceManager.cs", 116);
			return null;
		}
		return nKMAttendanceTabTemplet;
	}

	public void Join()
	{
		if (NKMUtil.IsServer)
		{
			JoinIntervalTemplet();
		}
		RewardTemplets = NKMAttendanceManager.GetAttendanceRewardTemplet(RewardGroup);
		if (RewardTemplets == null)
		{
			NKMTempletError.Add($"Attendance Reward Group is null. Rewardgroup is {RewardGroup}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMAttendanceManager.cs", 132);
		}
	}

	public void JoinIntervalTemplet()
	{
		if (!string.IsNullOrEmpty(intervalId))
		{
			intervalTemplet = NKMIntervalTemplet.Find(intervalId);
			if (intervalTemplet == null)
			{
				intervalTemplet = NKMIntervalTemplet.Unuseable;
				NKMTempletError.Add($"[Attendance:{Key}]\ufffd߸\ufffd\ufffd\ufffd interval id:{intervalId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMAttendanceManager.cs", 144);
			}
			if (intervalTemplet.IsRepeatDate)
			{
				NKMTempletError.Add($"[Attendance:{Key}] \ufffdݺ\ufffd \ufffdⰣ\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd \ufffdҰ\ufffd. id:{intervalId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMAttendanceManager.cs", 149);
			}
		}
	}

	public void Validate()
	{
		if (IntervalTemplet.StartDate.TimeOfDay != TimeReset.ResetHourSpan)
		{
			NKMTempletError.Add($"[Attendance{tabID}] \ufffd⼮ \ufffd\ufffd\ufffd۽ð\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffdð\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd. intervalId:{intervalId} startdate:{intervalTemplet.StartDate}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMAttendanceManager.cs", 158);
		}
	}

	public void PostJoin()
	{
		JoinIntervalTemplet();
	}

	public NKMIntervalTemplet GetIntervalTemplet()
	{
		if (string.IsNullOrEmpty(intervalId))
		{
			return NKMIntervalTemplet.Invalid;
		}
		if (!intervalTemplet.IsValid)
		{
			intervalTemplet = NKMIntervalTemplet.Find(intervalId);
			if (intervalTemplet == null)
			{
				Log.ErrorAndExit($"[Attendance:{Key}] 잘못된 interval id:{intervalId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMAttendanceManagerEx.cs", 204);
				return null;
			}
			if (intervalTemplet.IsRepeatDate)
			{
				Log.ErrorAndExit($"[Attendance:{Key}] 반복 기간설정 사용 불가. id:{intervalId}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMAttendanceManagerEx.cs", 210);
				return null;
			}
		}
		return intervalTemplet;
	}

	public DateTime GetStartTime(bool bUTC)
	{
		if (!IntervalTemplet.IsValid)
		{
			Log.ErrorAndExit("Invalid intervalTemplet. IntervalID [" + intervalId + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMAttendanceManagerEx.cs", 222);
		}
		if (IntervalTemplet == null)
		{
			Log.ErrorAndExit("Null intervalTemplet. IntervalID [" + intervalId + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMAttendanceManagerEx.cs", 227);
		}
		if (bUTC)
		{
			return ServiceTime.ToUtcTime(IntervalTemplet.StartDate);
		}
		return IntervalTemplet.StartDate;
	}

	public DateTime GetEndTime(bool bUTC)
	{
		if (!IntervalTemplet.IsValid)
		{
			Log.ErrorAndExit("Invalid intervalTemplet. IntervalID [" + intervalId + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMAttendanceManagerEx.cs", 242);
		}
		if (IntervalTemplet == null)
		{
			Log.ErrorAndExit("Null intervalTemplet. IntervalID [" + intervalId + "]", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKMEx/NKMAttendanceManagerEx.cs", 247);
		}
		if (bUTC)
		{
			return ServiceTime.ToUtcTime(intervalTemplet.EndDate);
		}
		return intervalTemplet.EndDate;
	}

	public string GetTabNameMain()
	{
		return NKCStringTable.GetString(TabNameMain);
	}

	public string GetTabNameSub()
	{
		return NKCStringTable.GetString(TabNameSub);
	}
}
