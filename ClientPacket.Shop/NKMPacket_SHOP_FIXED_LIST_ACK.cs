using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_SHOP_FIXED_LIST_ACK)]
public sealed class NKMPacket_SHOP_FIXED_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<int> shopList = new List<int>();

	public List<InstantProduct> InstantProductList = new List<InstantProduct>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref shopList);
		stream.PutOrGet(ref InstantProductList);
	}
}
