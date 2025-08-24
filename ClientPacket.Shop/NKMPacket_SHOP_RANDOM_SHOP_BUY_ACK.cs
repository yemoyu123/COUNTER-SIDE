using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_SHOP_RANDOM_SHOP_BUY_ACK)]
public sealed class NKMPacket_SHOP_RANDOM_SHOP_BUY_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int slotIndex;

	public NKMRewardData rewardData;

	public NKMItemMiscData costItemData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref slotIndex);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref costItemData);
	}
}
