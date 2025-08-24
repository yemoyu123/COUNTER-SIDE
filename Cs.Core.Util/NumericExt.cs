using System;

namespace Cs.Core.Util;

public static class NumericExt
{
	public static int Clamp(this int val, int min, int max)
	{
		return System.Math.Min(System.Math.Max(val, min), max);
	}

	public static long Clamp(this long val, long min, long max)
	{
		return System.Math.Min(System.Math.Max(val, min), max);
	}

	public static T Clamp<T>(this T val, T min, T max) where T : class, IComparable<T>
	{
		if (val.CompareTo(min) < 0)
		{
			return min;
		}
		if (val.CompareTo(max) <= 0)
		{
			return val;
		}
		return max;
	}

	public static ushort DirectToUint16(this byte[] buffer, int startIndex)
	{
		return (ushort)((buffer[startIndex + 1] << 8) | buffer[startIndex]);
	}

	public static uint DirectToUint32(this byte[] buffer, int startIndex)
	{
		return (uint)((((((buffer[startIndex + 3] << 8) | buffer[startIndex + 2]) << 8) | buffer[startIndex + 1]) << 8) | buffer[startIndex]);
	}

	public static ulong DirectToUint64(this byte[] buffer, int startIndex)
	{
		return ((((((((((((((ulong)buffer[startIndex + 7] << 8) | buffer[startIndex + 6]) << 8) | buffer[startIndex + 5]) << 8) | buffer[startIndex + 4]) << 8) | buffer[startIndex + 3]) << 8) | buffer[startIndex + 2]) << 8) | buffer[startIndex + 1]) << 8) | buffer[startIndex];
	}

	public static void DirectWriteTo(this int data, byte[] buffer, int position)
	{
		buffer[position] = (byte)data;
		buffer[position + 1] = (byte)(data >> 8);
		buffer[position + 2] = (byte)(data >> 16);
		buffer[position + 3] = (byte)(data >> 24);
	}

	public static void DirectWriteTo(this long data, byte[] buffer, int position)
	{
		buffer[position] = (byte)data;
		buffer[position + 1] = (byte)(data >> 8);
		buffer[position + 2] = (byte)(data >> 16);
		buffer[position + 3] = (byte)(data >> 24);
		buffer[position + 4] = (byte)(data >> 32);
		buffer[position + 5] = (byte)(data >> 40);
		buffer[position + 6] = (byte)(data >> 48);
		buffer[position + 7] = (byte)(data >> 56);
	}

	public static void DirectWriteTo(this ulong data, byte[] buffer, int position)
	{
		buffer[position] = (byte)data;
		buffer[position + 1] = (byte)(data >> 8);
		buffer[position + 2] = (byte)(data >> 16);
		buffer[position + 3] = (byte)(data >> 24);
		buffer[position + 4] = (byte)(data >> 32);
		buffer[position + 5] = (byte)(data >> 40);
		buffer[position + 6] = (byte)(data >> 48);
		buffer[position + 7] = (byte)(data >> 56);
	}
}
