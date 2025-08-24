using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPAcket_EQUIP_ITEM_ENCHANT_ACK)]
public sealed class NKMPAcket_EQUIP_ITEM_ENCHANT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long equipItemUID;

	public int enchantLevel;

	public int enchantExp;

	public List<long> consumeEquipItemUIDList = new List<long>();

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref equipItemUID);
		stream.PutOrGet(ref enchantLevel);
		stream.PutOrGet(ref enchantExp);
		stream.PutOrGet(ref consumeEquipItemUIDList);
		stream.PutOrGet(ref costItemDataList);
	}
}
