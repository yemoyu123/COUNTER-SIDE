using System;

namespace Cs.Core.Util;

public static class DateTimeExt
{
	public static bool IsBetween(this DateTime self, DateTime start, DateTime end)
	{
		if (start <= self)
		{
			return self < end;
		}
		return false;
	}

	public static string ToFileString(this DateTime self)
	{
		return self.ToString("yyyy.MM.dd-HH.mm.ss.fff");
	}
}
