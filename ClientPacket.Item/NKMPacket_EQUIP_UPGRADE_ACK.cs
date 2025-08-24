using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_UPGRADE_ACK)]
public sealed class NKMPacket_EQUIP_UPGRADE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMEquipItemData equipItemData;

	public List<long> consumeEquipItemUidList = new List<long>();

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref equipItemData);
		stream.PutOrGet(ref consumeEquipItemUidList);
		stream.PutOrGet(ref costItemDataList);
	}
}
