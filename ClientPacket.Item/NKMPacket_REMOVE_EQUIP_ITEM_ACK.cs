using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_REMOVE_EQUIP_ITEM_ACK)]
public sealed class NKMPacket_REMOVE_EQUIP_ITEM_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<long> removeEquipItemUIDList = new List<long>();

	public List<NKMItemMiscData> rewardItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref removeEquipItemUIDList);
		stream.PutOrGet(ref rewardItemDataList);
	}
}
