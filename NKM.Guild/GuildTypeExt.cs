using System;
using ClientPacket.Guild;
using Cs.Shared.Time;

namespace NKM.Guild;

public static class GuildTypeExt
{
	public static NKMGuildAttendanceData GetYesterdayAttendance(this NKMGuildData self, DateTime current)
	{
		DateTime dateTime = DailyReset.GetDateExpression(current).AddDays(-1.0);
		foreach (NKMGuildAttendanceData attendance in self.attendanceList)
		{
			if (attendance.date == dateTime)
			{
				return attendance;
			}
		}
		return null;
	}

	public static NKMGuildAttendanceData GetTodayAttendance(this NKMGuildData self, DateTime current)
	{
		DateTime dateExpression = DailyReset.GetDateExpression(current);
		foreach (NKMGuildAttendanceData attendance in self.attendanceList)
		{
			if (attendance.date == dateExpression)
			{
				return attendance;
			}
		}
		return null;
	}

	public static bool HasAttendanceData(this NKMGuildMemberData self, DateTime current)
	{
		return !DailyReset.IsOutOfDate(current, self.lastAttendanceDate);
	}
}
