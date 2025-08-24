using System.Threading;

namespace NKM;

public static class NpcUid
{
	public const long NpcUidStart = 1000000000L;

	private static long NpcUidIndex = 1000000000L;

	public static long Get()
	{
		return Interlocked.Increment(ref NpcUidIndex);
	}
}
