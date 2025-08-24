using Cs.Protocol;
using Protocol;

namespace ClientPacket.Item;

[PacketId(ClientPacketId.kNKMPacket_EQUIP_PRESET_REGISTER_ALL_REQ)]
public sealed class NKMPacket_EQUIP_PRESET_REGISTER_ALL_REQ : ISerializable
{
	public long unitUid;

	public int presetIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUid);
		stream.PutOrGet(ref presetIndex);
	}
}
