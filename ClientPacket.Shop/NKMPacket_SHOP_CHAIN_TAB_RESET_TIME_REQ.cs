using Cs.Protocol;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_SHOP_CHAIN_TAB_RESET_TIME_REQ)]
public sealed class NKMPacket_SHOP_CHAIN_TAB_RESET_TIME_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
