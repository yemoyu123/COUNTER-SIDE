using Cs.Protocol;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_ZLONG_PAYMENT_NOTIFY)]
public sealed class NKMPacket_ZLONG_PAYMENT_NOTIFY : ISerializable
{
	public NKMRewardData rewardData;

	public int productID;

	public NKMShopPurchaseHistory history = new NKMShopPurchaseHistory();

	public NKMShopSubscriptionData subScriptionData = new NKMShopSubscriptionData();

	public double totalPaidAmount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref productID);
		stream.PutOrGet(ref history);
		stream.PutOrGet(ref subScriptionData);
		stream.PutOrGet(ref totalPaidAmount);
	}
}
