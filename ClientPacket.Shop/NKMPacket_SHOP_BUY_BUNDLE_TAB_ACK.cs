using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_SHOP_BUY_BUNDLE_TAB_ACK)]
public sealed class NKMPacket_SHOP_BUY_BUNDLE_TAB_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMRewardData rewardData;

	public NKMItemMiscData costItemData;

	public List<NKMShopPurchaseHistory> histroy = new List<NKMShopPurchaseHistory>();

	public List<NKMShopSubscriptionData> subScriptionData = new List<NKMShopSubscriptionData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref costItemData);
		stream.PutOrGet(ref histroy);
		stream.PutOrGet(ref subScriptionData);
	}
}
