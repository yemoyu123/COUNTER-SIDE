using Cs.Protocol;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_PRESET_APPLY_REQ)]
public sealed class NKMPacket_OFFICE_PRESET_APPLY_REQ : ISerializable
{
	public int roomId;

	public int presetId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref roomId);
		stream.PutOrGet(ref presetId);
	}
}
