using Cs.Protocol;

namespace ClientPacket.Shop;

public sealed class NKMShopPurchaseHistory : ISerializable
{
	public int shopId;

	public int purchaseCount;

	public int purchaseTotalCount;

	public long nextResetDate;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref shopId);
		stream.PutOrGet(ref purchaseCount);
		stream.PutOrGet(ref purchaseTotalCount);
		stream.PutOrGet(ref nextResetDate);
	}
}
