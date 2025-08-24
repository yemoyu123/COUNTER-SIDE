using Cs.Core.Util;
using Cs.Logging;

namespace NKM;

public class NKMRandom
{
	public static int Range(int min, int max)
	{
		return PerThreadRandom.Instance.Next(min, max);
	}

	public static float Range(float min, float max)
	{
		if (min > max)
		{
			Log.Error($"NKMRandom min({min}) > max({max})", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMRandom.cs", 17);
		}
		return (float)PerThreadRandom.Instance.NextDouble() * (max - min) + min;
	}

	public static int RandomInt()
	{
		return PerThreadRandom.Instance.Next();
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
