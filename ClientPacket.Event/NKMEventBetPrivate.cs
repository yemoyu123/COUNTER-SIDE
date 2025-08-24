using Cs.Protocol;

namespace ClientPacket.Event;

public sealed class NKMEventBetPrivate : ISerializable
{
	public int eventIndex;

	public EventBetTeam selectTeam;

	public long currentBetCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref eventIndex);
		stream.PutOrGetEnum(ref selectTeam);
		stream.PutOrGet(ref currentBetCount);
	}
}
