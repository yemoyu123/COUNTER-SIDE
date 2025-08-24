using Cs.Protocol;

namespace ClientPacket.Event;

public sealed class NKMRaceResult : ISerializable
{
	public int RaceIndex;

	public long TeamAPoint;

	public long TeamBPoint;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref RaceIndex);
		stream.PutOrGet(ref TeamAPoint);
		stream.PutOrGet(ref TeamBPoint);
	}
}
