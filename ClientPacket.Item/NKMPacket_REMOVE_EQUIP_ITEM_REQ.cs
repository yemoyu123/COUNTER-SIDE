using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_REMOVE_EQUIP_ITEM_REQ)]
public sealed class NKMPacket_REMOVE_EQUIP_ITEM_REQ : ISerializable
{
	public List<long> removeEquipItemUIDList = new List<long>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref removeEquipItemUIDList);
	}
}
