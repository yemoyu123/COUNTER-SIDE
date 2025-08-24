using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_SHOP_FIX_SHOP_CASH_BUY_POSSIBLE_REQ)]
public sealed class NKMPacket_SHOP_FIX_SHOP_CASH_BUY_POSSIBLE_REQ : ISerializable
{
	public string productMarketID;

	public List<int> selectIndices = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref productMarketID);
		stream.PutOrGet(ref selectIndices);
	}
}
