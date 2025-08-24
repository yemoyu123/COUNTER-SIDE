using System;
using NKM;

namespace NKC;

public static class NKMTime
{
	public enum TimePeriod
	{
		Day,
		Week,
		Month
	}

	private const DayOfWeek WEEKLY_RESET_DAY = DayOfWeek.Sunday;

	public static TimeSpan s_tsServiceTimeOffset;

	public static DateTime UtcEpoch => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	public static DateTime LocalEpoch => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

	public static int INTERVAL_FROM_UTC => (int)s_tsServiceTimeOffset.TotalHours;

	public static int RESET_TIME_BASE_UTC
	{
		get
		{
			if (4 >= INTERVAL_FROM_UTC)
			{
				return 4 - INTERVAL_FROM_UTC;
			}
			return 28 - INTERVAL_FROM_UTC;
		}
	}

	public static long ToUnixTime(this DateTime datetime)
	{
		if (datetime.Kind == DateTimeKind.Utc)
		{
			return (long)(datetime - UtcEpoch).TotalSeconds;
		}
		return (long)(datetime - LocalEpoch).TotalSeconds;
	}

	public static DateTime GetNextResetTime(DateTime baseTimeUTC, NKM_MISSION_RESET_INTERVAL resetInterval)
	{
		TimePeriod timePeriod = TimePeriod.Day;
		switch (resetInterval)
		{
		case NKM_MISSION_RESET_INTERVAL.DAILY:
			timePeriod = TimePeriod.Day;
			break;
		case NKM_MISSION_RESET_INTERVAL.WEEKLY:
			timePeriod = TimePeriod.Week;
			break;
		case NKM_MISSION_RESET_INTERVAL.MONTHLY:
			timePeriod = TimePeriod.Month;
			break;
		}
		return GetNextResetTime(baseTimeUTC, timePeriod);
	}

	public static DateTime GetNextResetTime(DateTime baseTimeUTC, TimePeriod timePeriod)
	{
		switch (timePeriod)
		{
		case TimePeriod.Month:
		{
			DateTime dateTime4 = new DateTime(baseTimeUTC.Year, baseTimeUTC.Month, DateTime.DaysInMonth(baseTimeUTC.Year, baseTimeUTC.Month), RESET_TIME_BASE_UTC, 0, 0);
			if (baseTimeUTC < dateTime4)
			{
				return dateTime4;
			}
			DateTime dateTime5 = dateTime4.AddMonths(1);
			return new DateTime(dateTime5.Year, dateTime5.Month, DateTime.DaysInMonth(dateTime5.Year, dateTime5.Month), RESET_TIME_BASE_UTC, 0, 0);
		}
		case TimePeriod.Week:
		{
			DateTime dateTime2 = new DateTime(baseTimeUTC.Year, baseTimeUTC.Month, baseTimeUTC.Day, RESET_TIME_BASE_UTC, 0, 0);
			int num = (0 - dateTime2.DayOfWeek + 7) % 7;
			DateTime dateTime3 = dateTime2.AddDays(num);
			if (baseTimeUTC < dateTime3)
			{
				return dateTime3;
			}
			return dateTime3.AddDays(7.0);
		}
		default:
		{
			DateTime dateTime = new DateTime(baseTimeUTC.Year, baseTimeUTC.Month, baseTimeUTC.Day, RESET_TIME_BASE_UTC, 0, 0);
			if (baseTimeUTC < dateTime)
			{
				return dateTime;
			}
			return dateTime.AddDays(1.0);
		}
		}
	}

	public static DateTime GetNextResetTime(DateTime baseTimeUTC, TimePeriod timePeriod, int resetHour)
	{
		switch (timePeriod)
		{
		case TimePeriod.Month:
		{
			DateTime dateTime4 = new DateTime(baseTimeUTC.Year, baseTimeUTC.Month, DateTime.DaysInMonth(baseTimeUTC.Year, baseTimeUTC.Month), resetHour, 0, 0);
			if (baseTimeUTC < dateTime4)
			{
				return dateTime4;
			}
			DateTime dateTime5 = dateTime4.AddMonths(1);
			return new DateTime(dateTime5.Year, dateTime5.Month, DateTime.DaysInMonth(dateTime5.Year, dateTime5.Month), RESET_TIME_BASE_UTC, 0, 0);
		}
		case TimePeriod.Week:
		{
			DateTime dateTime2 = new DateTime(baseTimeUTC.Year, baseTimeUTC.Month, baseTimeUTC.Day, resetHour, 0, 0);
			int num = (0 - dateTime2.DayOfWeek + 7) % 7;
			DateTime dateTime3 = dateTime2.AddDays(num);
			if (baseTimeUTC < dateTime3)
			{
				return dateTime3;
			}
			return dateTime3.AddDays(7.0);
		}
		default:
		{
			DateTime dateTime = new DateTime(baseTimeUTC.Year, baseTimeUTC.Month, baseTimeUTC.Day, resetHour, 0, 0);
			if (baseTimeUTC < dateTime)
			{
				return dateTime;
			}
			return dateTime.AddDays(1.0);
		}
		}
	}

	public static DateTime GetResetTime(DateTime baseTimeUTC, TimePeriod timePeriod)
	{
		return timePeriod switch
		{
			TimePeriod.Month => GetNextResetTime(baseTimeUTC, timePeriod).AddMonths(-1), 
			TimePeriod.Week => GetNextResetTime(baseTimeUTC, timePeriod).AddDays(-7.0), 
			_ => GetNextResetTime(baseTimeUTC, timePeriod).AddDays(-1.0), 
		};
	}

	[Obsolete("NKCSynchronizedTime.IsEventTime 사용할 것. interval 관련 인터페이스들이 구현되어 있음.")]
	public static bool IsEventTime(DateTime currentTime, DateTime StartTime, DateTime finishTime)
	{
		return NKCSynchronizedTime.IsEventTime(currentTime, StartTime, finishTime);
	}

	public static DateTime LocalToUTC(DateTime koreaLocalTime, int fAddHour = 0)
	{
		if (koreaLocalTime.Ticks == 0L || koreaLocalTime == DateTime.MinValue || koreaLocalTime == DateTime.MaxValue)
		{
			return koreaLocalTime;
		}
		return koreaLocalTime.AddHours(-INTERVAL_FROM_UTC).AddHours(fAddHour);
	}

	public static DateTime UTCtoLocal(DateTime utcTime, int fAddHour = 0)
	{
		if (utcTime.Ticks == 0L || utcTime == DateTime.MinValue || utcTime == DateTime.MaxValue)
		{
			return utcTime;
		}
		return utcTime.AddHours(INTERVAL_FROM_UTC).AddHours(fAddHour);
	}
}
