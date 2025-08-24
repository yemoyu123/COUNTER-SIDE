using System;
using NKC;

namespace Cs.Core.Util;

public static class ServiceTime
{
	public static DateTime Now => NKCSynchronizedTime.ServiceTime;

	public static DateTime Recent => NKCSynchronizedTime.ServiceTime;

	public static DateTime UtcNow => DateTime.UtcNow;

	public static DateTime Forever { get; } = DateTime.Parse("9000-1-1");

	public static DateTime FromUtcTime(DateTime utcTime)
	{
		return utcTime + NKCSynchronizedTime.ServiceTimeOffet;
	}

	public static DateTime ToUtcTime(DateTime serviceTime)
	{
		return serviceTime - NKCSynchronizedTime.ServiceTimeOffet;
	}
}
