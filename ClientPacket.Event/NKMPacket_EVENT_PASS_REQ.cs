using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_PASS_REQ)]
public sealed class NKMPacket_EVENT_PASS_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
