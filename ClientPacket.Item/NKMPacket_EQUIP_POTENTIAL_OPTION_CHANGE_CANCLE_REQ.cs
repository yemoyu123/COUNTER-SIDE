using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_CANCLE_REQ)]
public sealed class NKMPacket_EQUIP_POTENTIAL_OPTION_CHANGE_CANCLE_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
