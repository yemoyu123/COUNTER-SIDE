using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_PASS_LEVEL_COMPLETE_REQ)]
public sealed class NKMPacket_EVENT_PASS_LEVEL_COMPLETE_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
