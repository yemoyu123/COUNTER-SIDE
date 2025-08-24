using System;

namespace Cs.Shared.Time;

public static class WeeklyReset
{
	public static bool IsOutOfDate(DateTime current, DateTime dateTime, DayOfWeek dayOfWeek)
	{
		return dateTime <= CalcLastReset(current, dayOfWeek);
	}

	public static DateTime CalcNextReset(DateTime current, DayOfWeek dayOfWeek)
	{
		return CalcLastReset(current, dayOfWeek).AddDays(7.0);
	}

	internal static DateTime CalcLastReset(DateTime current, DayOfWeek dayOfWeek)
	{
		DateTime dateTime = new DateTime(current.Year, current.Month, current.Day, 4, 0, 0);
		int num = (dayOfWeek - dateTime.DayOfWeek + 7) % 7;
		DateTime dateTime2 = dateTime.AddDays(num);
		if (current < dateTime2)
		{
			return dateTime2.AddDays(-7.0);
		}
		return dateTime2;
	}

	public static DateTime CalcNextReset(DateTime current, DayOfWeek dayOfWeek, TimeSpan resetHourSpan)
	{
		return CalcLastReset(current, dayOfWeek, resetHourSpan).AddDays(7.0);
	}

	internal static DateTime CalcLastReset(DateTime current, DayOfWeek dayOfWeek, TimeSpan ResetHourSpan)
	{
		DateTime dateTime = current.Date + ResetHourSpan + TimeSpan.FromDays(dayOfWeek - current.DayOfWeek);
		if (current < dateTime)
		{
			return dateTime.AddDays(-7.0);
		}
		return dateTime;
	}
}
