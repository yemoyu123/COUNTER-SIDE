using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_CRAFT_UNLOCK_SLOT_REQ)]
public sealed class NKMPacket_CRAFT_UNLOCK_SLOT_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
