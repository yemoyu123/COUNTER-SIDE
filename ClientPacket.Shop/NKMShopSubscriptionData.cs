using System;
using Cs.Protocol;

namespace ClientPacket.Shop;

public sealed class NKMShopSubscriptionData : ISerializable
{
	public int productId;

	public int rewardCount;

	public DateTime lastUpdateDate;

	public DateTime startDate;

	public DateTime endDate;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref productId);
		stream.PutOrGet(ref rewardCount);
		stream.PutOrGet(ref lastUpdateDate);
		stream.PutOrGet(ref startDate);
		stream.PutOrGet(ref endDate);
	}
}
