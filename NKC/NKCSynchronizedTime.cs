using System;
using ClientPacket.Service;
using NKM.Templet;
using UnityEngine;

namespace NKC;

public static class NKCSynchronizedTime
{
	public static int UNLIMITD_REMAIN_DAYS = 36000;

	private static float s_fTimeSinceLastUpdate = float.MaxValue;

	private static TimeSpan s_tsServerTimeDifference;

	private static TimeSpan s_tsServiceTimeOffset;

	private const float SERVER_TIME_SYNC_INTERVAL = 300f;

	private const long THIRTY_SECOND = 300000000L;

	private static long s_lastTick;

	public static ref readonly TimeSpan ServiceTimeOffet => ref s_tsServiceTimeOffset;

	public static DateTime ServiceTime => GetServerUTCTime() + s_tsServiceTimeOffset;

	public static void Update(float deltaTime)
	{
		long ticks = DateTime.Now.Ticks;
		if (Math.Abs(ticks - s_lastTick) > 300000000)
		{
			s_fTimeSinceLastUpdate = 298f;
		}
		s_lastTick = ticks;
		s_fTimeSinceLastUpdate += deltaTime;
		if (300f < s_fTimeSinceLastUpdate && NKCScenManager.GetScenManager().GetConnectGame().HasLoggedIn && NKCScenManager.GetScenManager().GetConnectGame().IsConnected)
		{
			NKMPacket_SERVER_TIME_REQ();
		}
	}

	public static void ForceUpdateTime()
	{
		s_fTimeSinceLastUpdate = float.MaxValue;
	}

	private static void NKMPacket_SERVER_TIME_REQ()
	{
		NKMPacket_SERVER_TIME_REQ packet = new NKMPacket_SERVER_TIME_REQ();
		s_fTimeSinceLastUpdate = 270f;
		NKCScenManager.GetScenManager().GetConnectGame().Send(packet, NKC_OPEN_WAIT_BOX_TYPE.NOWBT_INVALID);
	}

	public static void OnRecv(DateTime utcServerTime, TimeSpan tsServiceTimeOffset)
	{
		s_tsServiceTimeOffset = tsServiceTimeOffset;
		NKMTime.s_tsServiceTimeOffset = s_tsServiceTimeOffset;
		SetUTCServerTime(utcServerTime);
	}

	private static void SetUTCServerTime(DateTime utcServerTime)
	{
		DateTime utcNow = DateTime.UtcNow;
		s_tsServerTimeDifference = utcServerTime - utcNow;
		s_fTimeSinceLastUpdate = 0f;
	}

	public static void OnRecv(NKMPacket_SERVER_TIME_ACK sPacket)
	{
		SetUTCServerTime(new DateTime(sPacket.utcServerTimeTicks));
	}

	public static DateTime GetServerUTCTime(double intervalSeconds = 0.0)
	{
		return DateTime.UtcNow.Add(s_tsServerTimeDifference).AddSeconds(intervalSeconds);
	}

	public static DateTime ToServerServiceTime(DateTime UtcTime)
	{
		return UtcTime.Add(s_tsServiceTimeOffset);
	}

	public static DateTime ToUtcTime(DateTime serverServiceTime)
	{
		if (serverServiceTime.Ticks == 0L || serverServiceTime == DateTime.MinValue || serverServiceTime == DateTime.MaxValue)
		{
			return serverServiceTime;
		}
		return serverServiceTime.Subtract(s_tsServiceTimeOffset);
	}

	public static TimeSpan GetTimeLeft(long finishTimeTicks)
	{
		return GetTimeLeft(new DateTime(finishTimeTicks));
	}

	public static TimeSpan GetTimeLeft(DateTime finishTime)
	{
		DateTime serverUTCTime = GetServerUTCTime();
		if (finishTime > serverUTCTime)
		{
			return finishTime - serverUTCTime;
		}
		return new TimeSpan(0, 0, 0);
	}

	public static string GetTimeLeftString(long finishTimeTicks)
	{
		return GetTimeLeftString(new DateTime(finishTimeTicks));
	}

	public static string GetTimeLeftString(DateTime finishTime)
	{
		return GetTimeSpanString(GetTimeLeft(finishTime));
	}

	public static string GetTimeSpanString(TimeSpan timeSpan)
	{
		if (timeSpan.Days > 0)
		{
			return $"{string.Format(NKCUtilString.GET_STRING_TIME_DAY_ONE_PARAM, timeSpan.Days)} {timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
		}
		return $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
	}

	public static bool IsStarted(long startTimeUTCTicks)
	{
		return IsFinished(startTimeUTCTicks);
	}

	public static bool IsStarted(DateTime startTimeUTC)
	{
		return IsFinished(startTimeUTC);
	}

	public static bool IsFinished(long finishTimeUTCTicks)
	{
		return IsFinished(new DateTime(finishTimeUTCTicks));
	}

	public static bool IsFinished(DateTime finishTimeUTC)
	{
		DateTime serverUTCTime = GetServerUTCTime();
		return finishTimeUTC < serverUTCTime;
	}

	public static bool IsEventTime(DateTime StartTimeUTC, DateTime finishTimeUTC)
	{
		if ((StartTimeUTC.Ticks == 0L && finishTimeUTC.Ticks == 0L) || (StartTimeUTC == DateTime.MinValue && finishTimeUTC == DateTime.MaxValue))
		{
			return true;
		}
		DateTime serverUTCTime = GetServerUTCTime();
		if (serverUTCTime < StartTimeUTC)
		{
			return false;
		}
		if (finishTimeUTC < serverUTCTime)
		{
			return false;
		}
		return true;
	}

	public static bool IsEventTime(DateTime currentTime, DateTime StartTimeUTC, DateTime finishTimeUTC)
	{
		if ((StartTimeUTC.Ticks == 0L && finishTimeUTC.Ticks == 0L) || (StartTimeUTC == DateTime.MinValue && finishTimeUTC == DateTime.MaxValue))
		{
			return true;
		}
		if (currentTime < StartTimeUTC)
		{
			return false;
		}
		if (finishTimeUTC < currentTime)
		{
			return false;
		}
		return true;
	}

	public static bool IsEventTime(NKMIntervalTemplet interval)
	{
		return interval?.IsValidTime(ServiceTime) ?? true;
	}

	public static bool IsEventTime(DateTime utcCurrent, NKMIntervalTemplet interval)
	{
		return interval?.IsValidTime(ToServerServiceTime(utcCurrent)) ?? true;
	}

	public static bool IsEventTime(string intervalStrID)
	{
		if (string.IsNullOrEmpty(intervalStrID))
		{
			return true;
		}
		NKMIntervalTemplet nKMIntervalTemplet = NKMIntervalTemplet.Find(intervalStrID);
		if (nKMIntervalTemplet == null)
		{
			Debug.LogError("intervalTemplet " + intervalStrID + " not found!");
			return false;
		}
		return IsEventTime(nKMIntervalTemplet);
	}

	public static bool IsEventTime(DateTime utcNow, string intervalStrID)
	{
		if (string.IsNullOrEmpty(intervalStrID))
		{
			return true;
		}
		NKMIntervalTemplet nKMIntervalTemplet = NKMIntervalTemplet.Find(intervalStrID);
		if (nKMIntervalTemplet == null)
		{
			Debug.LogError("intervalTemplet " + intervalStrID + " not found!");
			return false;
		}
		return IsEventTime(utcNow, nKMIntervalTemplet);
	}

	public static bool IsEventTime(NKMIntervalTemplet interval, DateTime startUtc, DateTime finishUtc)
	{
		if (interval != null)
		{
			return IsEventTime(interval);
		}
		return IsEventTime(startUtc, finishUtc);
	}

	public static bool IsEventTime(DateTime utcCurrent, NKMIntervalTemplet interval, DateTime startUtc, DateTime finishUtc)
	{
		if (interval != null)
		{
			return IsEventTime(utcCurrent, interval);
		}
		return IsEventTime(utcCurrent, startUtc, finishUtc);
	}

	public static bool IsEventTime(string intervalStrID, DateTime startUtc, DateTime finishUtc)
	{
		if (string.IsNullOrEmpty(intervalStrID))
		{
			return IsEventTime(startUtc, finishUtc);
		}
		NKMIntervalTemplet nKMIntervalTemplet = NKMIntervalTemplet.Find(intervalStrID);
		if (nKMIntervalTemplet == null)
		{
			Debug.LogError("intervalTemplet " + intervalStrID + " not found!");
			return false;
		}
		return IsEventTime(nKMIntervalTemplet);
	}

	public static bool IsEventTime(DateTime utcCurrent, string intervalStrID, DateTime startUtc, DateTime finishUtc)
	{
		if (string.IsNullOrEmpty(intervalStrID))
		{
			return IsEventTime(utcCurrent, startUtc, finishUtc);
		}
		NKMIntervalTemplet nKMIntervalTemplet = NKMIntervalTemplet.Find(intervalStrID);
		if (nKMIntervalTemplet == null)
		{
			Debug.LogError("intervalTemplet " + intervalStrID + " not found!");
			return false;
		}
		return IsEventTime(utcCurrent, nKMIntervalTemplet);
	}

	public static bool IsBeforeLastServerRefresh(long Ticks)
	{
		return IsBeforeLastServerRefresh(new DateTime(Ticks));
	}

	public static bool IsBeforeLastServerRefresh(DateTime Time)
	{
		DateTime serverUTCTime = GetServerUTCTime();
		DateTime dateTime = serverUTCTime.Date.AddHours(NKMTime.RESET_TIME_BASE_UTC);
		if (serverUTCTime.Hour < NKMTime.RESET_TIME_BASE_UTC)
		{
			dateTime = dateTime.AddDays(-1.0);
		}
		return Time < dateTime;
	}

	public static DateTime GetSystemLocalTime(DateTime UtcTime)
	{
		return UtcTime.ToLocalTime();
	}

	public static DateTime GetSystemLocalTime(DateTime ServiceTime, int UtcInterval)
	{
		return ServiceTime.AddHours(-UtcInterval).ToLocalTime();
	}

	public static DateTime GetIntervalUtc(NKMIntervalTemplet intervalTemplet, bool bStart)
	{
		if (bStart)
		{
			return intervalTemplet.GetStartDateUtc();
		}
		return intervalTemplet.GetEndDateUtc();
	}

	public static DateTime GetIntervalStartUtc(NKMIntervalTemplet intervalTemplet)
	{
		return intervalTemplet.GetStartDateUtc();
	}

	public static DateTime GetIntervalEndUtc(NKMIntervalTemplet intervalTemplet)
	{
		return intervalTemplet.GetEndDateUtc();
	}

	public static DateTime GetIntervalStartUtc(string intervalID)
	{
		return GetIntervalUtc(intervalID, DateTime.MinValue, bStart: true);
	}

	public static DateTime GetIntervalEndUtc(string intervalID)
	{
		return GetIntervalUtc(intervalID, DateTime.MaxValue, bStart: false);
	}

	private static DateTime GetIntervalUtc(string intervalID, DateTime templetServiceTime, bool bStart)
	{
		if (string.IsNullOrEmpty(intervalID))
		{
			return NKMTime.LocalToUTC(templetServiceTime);
		}
		NKMIntervalTemplet nKMIntervalTemplet = NKMIntervalTemplet.Find(intervalID);
		if (nKMIntervalTemplet == null)
		{
			Debug.LogError("IntervalTemplet " + intervalID + " not found");
			return DateTime.MinValue;
		}
		if (bStart)
		{
			return nKMIntervalTemplet.GetStartDateUtc();
		}
		return nKMIntervalTemplet.GetEndDateUtc();
	}

	public static DateTime GetStartOfServiceTime(int dayOffset)
	{
		return ToUtcTime(new DateTime(ServiceTime.Year, ServiceTime.Month, ServiceTime.Day, 0, 0, 0).AddDays(dayOffset));
	}
}
