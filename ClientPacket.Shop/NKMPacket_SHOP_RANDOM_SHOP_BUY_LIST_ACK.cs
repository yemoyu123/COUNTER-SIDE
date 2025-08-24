using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Shop;

[PacketId(ClientPacketId.kNKMPacket_SHOP_RANDOM_SHOP_BUY_LIST_ACK)]
public sealed class NKMPacket_SHOP_RANDOM_SHOP_BUY_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<int> slotIndexes = new List<int>();

	public NKMRewardData rewardData;

	public List<NKMItemMiscData> costItemDatas = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref slotIndexes);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref costItemDatas);
	}
}
