using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_ITEM_ENCHANT_USING_MISC_ITEM_ACK)]
public sealed class NKMPacket_EQUIP_ITEM_ENCHANT_USING_MISC_ITEM_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long equipItemUID;

	public int enchantLevel;

	public int enchantExp;

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref equipItemUID);
		stream.PutOrGet(ref enchantLevel);
		stream.PutOrGet(ref enchantExp);
		stream.PutOrGet(ref costItemDataList);
	}
}
