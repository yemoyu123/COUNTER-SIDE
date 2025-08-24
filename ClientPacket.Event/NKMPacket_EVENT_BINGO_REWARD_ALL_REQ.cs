using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_BINGO_REWARD_ALL_REQ)]
public sealed class NKMPacket_EVENT_BINGO_REWARD_ALL_REQ : ISerializable
{
	public int eventId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref eventId);
	}
}
