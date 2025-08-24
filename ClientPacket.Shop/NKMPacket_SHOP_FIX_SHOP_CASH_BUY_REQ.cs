using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_SHOP_FIX_SHOP_CASH_BUY_REQ)]
public sealed class NKMPacket_SHOP_FIX_SHOP_CASH_BUY_REQ : ISerializable
{
	public string productMarketID;

	public string validationToken;

	public double realCash;

	public int currencyType;

	public string currencyCode;

	public List<int> selectIndices = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref productMarketID);
		stream.PutOrGet(ref validationToken);
		stream.PutOrGet(ref realCash);
		stream.PutOrGet(ref currencyType);
		stream.PutOrGet(ref currencyCode);
		stream.PutOrGet(ref selectIndices);
	}
}
