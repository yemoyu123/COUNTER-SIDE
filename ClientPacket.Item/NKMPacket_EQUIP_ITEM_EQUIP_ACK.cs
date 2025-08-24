using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_ITEM_EQUIP_ACK)]
public sealed class NKMPacket_EQUIP_ITEM_EQUIP_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long unitUID;

	public long equipItemUID;

	public long unequipItemUID;

	public ITEM_EQUIP_POSITION equipPosition;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref unitUID);
		stream.PutOrGet(ref equipItemUID);
		stream.PutOrGet(ref unequipItemUID);
		stream.PutOrGetEnum(ref equipPosition);
	}
}
