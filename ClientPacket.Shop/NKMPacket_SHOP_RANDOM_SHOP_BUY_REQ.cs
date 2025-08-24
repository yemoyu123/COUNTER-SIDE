using Cs.Protocol;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_SHOP_RANDOM_SHOP_BUY_REQ)]
public sealed class NKMPacket_SHOP_RANDOM_SHOP_BUY_REQ : ISerializable
{
	public int slotIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref slotIndex);
	}
}
