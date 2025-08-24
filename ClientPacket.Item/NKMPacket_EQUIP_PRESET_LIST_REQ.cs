using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_PRESET_LIST_REQ)]
public sealed class NKMPacket_EQUIP_PRESET_LIST_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
