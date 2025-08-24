using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_UPGRADE_REQ)]
public sealed class NKMPacket_EQUIP_UPGRADE_REQ : ISerializable
{
	public long equipUid;

	public List<long> consumeEquipItemUidList = new List<long>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref equipUid);
		stream.PutOrGet(ref consumeEquipItemUidList);
	}
}
