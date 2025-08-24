using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_SHOP_FIX_SHOP_BUY_REQ)]
public sealed class NKMPacket_SHOP_FIX_SHOP_BUY_REQ : ISerializable
{
	public int productID;

	public int productCount;

	public List<int> selectIndices = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref productID);
		stream.PutOrGet(ref productCount);
		stream.PutOrGet(ref selectIndices);
	}
}
