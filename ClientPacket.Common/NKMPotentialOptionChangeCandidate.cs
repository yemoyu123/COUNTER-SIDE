using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMPotentialOptionChangeCandidate : ISerializable
{
	public long equipUid;

	public int precision;

	public int socketIndex;

	public int accumulateCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref equipUid);
		stream.PutOrGet(ref precision);
		stream.PutOrGet(ref socketIndex);
		stream.PutOrGet(ref accumulateCount);
	}
}
