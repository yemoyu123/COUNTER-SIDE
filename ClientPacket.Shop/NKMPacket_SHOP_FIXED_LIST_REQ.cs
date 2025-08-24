using Cs.Protocol;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_SHOP_FIXED_LIST_REQ)]
public sealed class NKMPacket_SHOP_FIXED_LIST_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
