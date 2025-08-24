using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_TUNING_CANCEL_REQ)]
public sealed class NKMPacket_EQUIP_TUNING_CANCEL_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
