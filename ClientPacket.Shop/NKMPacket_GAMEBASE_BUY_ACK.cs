using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_GAMEBASE_BUY_ACK)]
public sealed class NKMPacket_GAMEBASE_BUY_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMRewardData rewardData;

	public int productId;

	public NKMShopPurchaseHistory histroy = new NKMShopPurchaseHistory();

	public NKMItemMiscData costItemData;

	public NKMShopSubscriptionData subScriptionData = new NKMShopSubscriptionData();

	public double totalPaidAmount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref productId);
		stream.PutOrGet(ref histroy);
		stream.PutOrGet(ref costItemData);
		stream.PutOrGet(ref subScriptionData);
		stream.PutOrGet(ref totalPaidAmount);
	}
}
