using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_PRESET_APPLY_REQ)]
public sealed class NKMPacket_EQUIP_PRESET_APPLY_REQ : ISerializable
{
	public int presetIndex;

	public long applyUnitUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref presetIndex);
		stream.PutOrGet(ref applyUnitUid);
	}
}
