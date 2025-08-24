using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_SHOP_FIX_SHOP_CASH_BUY_POSSIBLE_ACK)]
public sealed class NKMPacket_SHOP_FIX_SHOP_CASH_BUY_POSSIBLE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public string productMarketID;

	public NKMShopPurchaseHistory histroy = new NKMShopPurchaseHistory();

	public List<int> selectIndices = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref productMarketID);
		stream.PutOrGet(ref histroy);
		stream.PutOrGet(ref selectIndices);
	}
}
