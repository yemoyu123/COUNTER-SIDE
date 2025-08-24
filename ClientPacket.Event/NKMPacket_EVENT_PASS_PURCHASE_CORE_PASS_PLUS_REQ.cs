using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_PASS_PURCHASE_CORE_PASS_PLUS_REQ)]
public sealed class NKMPacket_EVENT_PASS_PURCHASE_CORE_PASS_PLUS_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
