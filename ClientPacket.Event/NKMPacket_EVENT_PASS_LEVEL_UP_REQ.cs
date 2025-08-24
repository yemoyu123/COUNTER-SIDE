using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_PASS_LEVEL_UP_REQ)]
public sealed class NKMPacket_EVENT_PASS_LEVEL_UP_REQ : ISerializable
{
	public int increaseLv = 1;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref increaseLv);
	}
}
