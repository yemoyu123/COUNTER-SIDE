using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_ITEM_ENCHANT_REQ)]
public sealed class NKMPacket_EQUIP_ITEM_ENCHANT_REQ : ISerializable
{
	public long equipItemUID;

	public List<long> consumeEquipItemUIDList = new List<long>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref equipItemUID);
		stream.PutOrGet(ref consumeEquipItemUIDList);
	}
}
