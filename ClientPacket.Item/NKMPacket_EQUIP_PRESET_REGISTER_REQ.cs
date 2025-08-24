using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_PRESET_REGISTER_REQ)]
public sealed class NKMPacket_EQUIP_PRESET_REGISTER_REQ : ISerializable
{
	public int presetIndex;

	public ITEM_EQUIP_POSITION equipPosition;

	public long equipUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref presetIndex);
		stream.PutOrGetEnum(ref equipPosition);
		stream.PutOrGet(ref equipUid);
	}
}
