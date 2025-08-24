using System;

namespace Cs.Shared.Time;

public static class DailyReset
{
	public static bool IsOutOfDate(DateTime current, DateTime dateTime)
	{
		return dateTime <= CalcLastReset(current);
	}

	public static DateTime CalcNextReset(DateTime current)
	{
		return CalcLastReset(current).AddDays(1.0);
	}

	public static DateTime CalcNextMidnight(DateTime current)
	{
		return current.Date.AddDays(1.0);
	}

	public static DateTime CalcLastReset(DateTime current)
	{
		DateTime result = current.Date + TimeReset.ResetHourSpan;
		if (current.TimeOfDay < TimeReset.ResetHourSpan)
		{
			result -= TimeSpan.FromDays(1.0);
		}
		return result;
	}

	public static DateTime GetDateExpression(DateTime current)
	{
		DateTime date = current.Date;
		if (current.TimeOfDay < TimeReset.ResetHourSpan)
		{
			date -= TimeSpan.FromDays(1.0);
		}
		return date;
	}
}
