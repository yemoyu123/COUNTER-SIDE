namespace Cs.Protocol.Detail;

internal static class ZigZag
{
	internal static uint Encode32(int n)
	{
		return (uint)((n << 1) ^ (n >> 31));
	}

	internal static ulong Encode64(long n)
	{
		return (ulong)((n << 1) ^ (n >> 63));
	}

	internal static int Decode32(uint n)
	{
		return (int)((n >> 1) ^ (0 - (n & 1)));
	}

	internal static long Decode64(ulong n)
	{
		return (long)((n >> 1) ^ (0L - (n & 1)));
	}
}
