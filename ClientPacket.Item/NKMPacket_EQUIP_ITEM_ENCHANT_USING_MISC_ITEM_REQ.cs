using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_ITEM_ENCHANT_USING_MISC_ITEM_REQ)]
public sealed class NKMPacket_EQUIP_ITEM_ENCHANT_USING_MISC_ITEM_REQ : ISerializable
{
	public long equipItemUID;

	public List<MiscItemData> miscItemList = new List<MiscItemData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref equipItemUID);
		stream.PutOrGet(ref miscItemList);
	}
}
