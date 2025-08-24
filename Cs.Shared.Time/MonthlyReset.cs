using System;

namespace Cs.Shared.Time;

public static class MonthlyReset
{
	public static bool IsOutOfDate(DateTime current, DateTime dateTime)
	{
		return dateTime <= CalcLastReset(current);
	}

	public static DateTime CalcNextReset(DateTime current)
	{
		return CalcLastReset(current).AddMonths(1);
	}

	public static DateTime CalcNextMidnight(DateTime current)
	{
		return new DateTime(current.Year, current.Month, 1, 0, 0, 0).AddMonths(1);
	}

	internal static DateTime CalcLastReset(DateTime current)
	{
		DateTime dateTime = new DateTime(current.Year, current.Month, 1, 4, 0, 0);
		if (current < dateTime)
		{
			return dateTime.AddMonths(-1);
		}
		return dateTime;
	}
}
