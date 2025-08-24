using System;
using Cs.Core.Util;

namespace Cs.Shared.Time;

public static class TimeReset
{
	public const int ResetHour = 4;

	public static readonly TimeSpan ResetHourSpan = TimeSpan.FromHours(4.0);

	public static DateTime CalcNextResetUtc(TimeResetType resetType)
	{
		DateTime recent = ServiceTime.Recent;
		return resetType switch
		{
			TimeResetType.Month => ServiceTime.ToUtcTime(MonthlyReset.CalcNextReset(recent)), 
			TimeResetType.Week => ServiceTime.ToUtcTime(WeeklyReset.CalcNextReset(recent, DayOfWeek.Monday)), 
			TimeResetType.Day => ServiceTime.ToUtcTime(DailyReset.CalcNextReset(recent)), 
			_ => throw new Exception($"[TimeUtil] invalid resetType:{resetType}"), 
		};
	}

	public static bool IsOutOfDateUtc(DateTime utcTime, TimeResetType resetType)
	{
		DateTime serviceTime = CalcLastReset(resetType);
		return utcTime <= ServiceTime.ToUtcTime(serviceTime);
	}

	public static DateTime CalcLastReset(TimeResetType resetType)
	{
		DateTime recent = ServiceTime.Recent;
		return resetType switch
		{
			TimeResetType.Month => MonthlyReset.CalcLastReset(recent), 
			TimeResetType.Week => WeeklyReset.CalcLastReset(recent, DayOfWeek.Monday), 
			TimeResetType.Day => DailyReset.CalcLastReset(recent), 
			_ => throw new Exception($"[TimeUtil] invalid resetType:{resetType}"), 
		};
	}
}
