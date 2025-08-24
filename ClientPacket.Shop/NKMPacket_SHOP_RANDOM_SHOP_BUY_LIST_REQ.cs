using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_SHOP_RANDOM_SHOP_BUY_LIST_REQ)]
public sealed class NKMPacket_SHOP_RANDOM_SHOP_BUY_LIST_REQ : ISerializable
{
	public List<int> slotIndexes = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref slotIndexes);
	}
}
