using System;
using Cs.Core.Util;

namespace Cs.Math;

public static class RandomGenerator
{
	private static class PerThreadRandom
	{
		[ThreadStatic]
		private static Random random;

		public static Random Instance
		{
			get
			{
				if (random == null)
				{
					random = new Random();
				}
				return random;
			}
		}
	}

	public static int Range(int min, int max)
	{
		return PerThreadRandom.Instance.Next(min, max);
	}

	public static int ArrayIndex(int count)
	{
		return PerThreadRandom.Instance.Next(count);
	}

	public static int Next(int maxValue)
	{
		return PerThreadRandom.Instance.Next(maxValue);
	}

	public static float Range(float min, float max)
	{
		if (min > max)
		{
			throw new ArgumentException($"[RandomGenerator] min:{min} max:{max}");
		}
		return (float)PerThreadRandom.Instance.NextDouble() * (max - min) + min;
	}

	public static long LongRandom()
	{
		return (long)(PerThreadRandom.Instance.NextDouble() * 9.223372036854776E+18);
	}

	public static long LongRandom(long min, long max)
	{
		byte[] buffer = new byte[8];
		PerThreadRandom.Instance.NextBytes(buffer);
		return (long)buffer.DirectToUint64(0) % (max - min) + min;
	}
}
