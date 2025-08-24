using System;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_SHOP_SUBSCRIPTION_NOT)]
public sealed class NKMPacket_SHOP_SUBSCRIPTION_NOT : ISerializable
{
	public int productId;

	public DateTime lastUpdateDate;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref productId);
		stream.PutOrGet(ref lastUpdateDate);
	}
}
