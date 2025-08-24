using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_ITEM_EQUIP_REQ)]
public sealed class NKMPacket_EQUIP_ITEM_EQUIP_REQ : ISerializable
{
	public bool isEquip;

	public long unitUID;

	public long equipItemUID;

	public ITEM_EQUIP_POSITION equipPosition;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref isEquip);
		stream.PutOrGet(ref unitUID);
		stream.PutOrGet(ref equipItemUID);
		stream.PutOrGetEnum(ref equipPosition);
	}
}
