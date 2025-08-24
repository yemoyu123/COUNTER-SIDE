using System;
using System.Text;

namespace Cs.Engine.Util;

public static class StringExt
{
	public static string ToByteFormat(this ushort bytes)
	{
		string[] array = new string[9] { "b", "Kb", "Mb", "Gb", "Tb", "Pb", "Eb", "Zb", "Yb" };
		long num = 0L;
		long num2 = bytes;
		int num3 = 0;
		while (num2 >= 1024)
		{
			num = num2 & 0x3FF;
			num2 >>= 10;
			num3++;
		}
		if (num > 100)
		{
			return $"{num2}.{num / 10:##}{array[num3]}";
		}
		return $"{num2}{array[num3]}";
	}

	public static string ToByteFormat(this int bytes)
	{
		string[] array = new string[9] { "b", "Kb", "Mb", "Gb", "Tb", "Pb", "Eb", "Zb", "Yb" };
		long num = 0L;
		long num2 = bytes;
		int num3 = 0;
		while (num2 >= 1024)
		{
			num = num2 & 0x3FF;
			num2 >>= 10;
			num3++;
		}
		if (num > 100)
		{
			return $"{num2}.{num / 10:##}{array[num3]}";
		}
		return $"{num2}{array[num3]}";
	}

	public static string ToByteFormat(this long bytes)
	{
		string[] array = new string[9] { "b", "Kb", "Mb", "Gb", "Tb", "Pb", "Eb", "Zb", "Yb" };
		long num = 0L;
		long num2 = bytes;
		int num3 = 0;
		while (num2 >= 1024)
		{
			num = num2 & 0x3FF;
			num2 >>= 10;
			num3++;
		}
		if (num > 100)
		{
			return $"{num2}.{num / 10:##}{array[num3]}";
		}
		return $"{num2}{array[num3]}";
	}

	public static string ToByteFormat(this ulong bytes)
	{
		string[] array = new string[9] { "b", "Kb", "Mb", "Gb", "Tb", "Pb", "Eb", "Zb", "Yb" };
		ulong num = 0uL;
		ulong num2 = bytes;
		int num3 = 0;
		while (num2 >= 1024)
		{
			num = num2 & 0x3FF;
			num2 >>= 10;
			num3++;
		}
		if (num > 100)
		{
			return $"{num2}.{num / 10:##}{array[num3]}";
		}
		return $"{num2}{array[num3]}";
	}

	public static string ToTimeFormat(this long msec)
	{
		if (msec < 1000)
		{
			return msec + "ms";
		}
		float num = (float)msec / 1000f;
		if (num < 60f)
		{
			return $"{num:.##}s";
		}
		TimeSpan timeSpan = TimeSpan.FromMilliseconds(msec);
		if (timeSpan.TotalHours < 1.0)
		{
			return timeSpan.ToString("mm\\:ss\\.ff");
		}
		return timeSpan.ToString("hh\\:mm\\:ss\\.ff");
	}

	public static string EncodeBase64(this string data)
	{
		return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
	}

	public static string DecodeBase64(this string data)
	{
		return Encoding.UTF8.GetString(Convert.FromBase64String(data));
	}
}
