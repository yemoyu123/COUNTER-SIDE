using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMTrimIntervalData : ISerializable
{
	public int trimTryCount;

	public int trimRetryCount;

	public int trimRestoreCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref trimTryCount);
		stream.PutOrGet(ref trimRetryCount);
		stream.PutOrGet(ref trimRestoreCount);
	}
}
