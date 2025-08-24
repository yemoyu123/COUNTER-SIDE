using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_SHOP_FIX_SHOP_BUY_ACK)]
public sealed class NKMPacket_SHOP_FIX_SHOP_BUY_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMRewardData rewardData;

	public int productID;

	public NKMShopPurchaseHistory histroy = new NKMShopPurchaseHistory();

	public NKMItemMiscData costItemData;

	public NKMShopSubscriptionData subScriptionData = new NKMShopSubscriptionData();

	public double totalPaidAmount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref productID);
		stream.PutOrGet(ref histroy);
		stream.PutOrGet(ref costItemData);
		stream.PutOrGet(ref subScriptionData);
		stream.PutOrGet(ref totalPaidAmount);
	}
}
