using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_SHIP_SLOT_OPTION_CANCEL_REQ)]
public sealed class NKMPacket_SHIP_SLOT_OPTION_CANCEL_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
