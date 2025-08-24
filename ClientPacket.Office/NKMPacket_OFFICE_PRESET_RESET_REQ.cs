using Cs.Protocol;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_PRESET_RESET_REQ)]
public sealed class NKMPacket_OFFICE_PRESET_RESET_REQ : ISerializable
{
	public int presetId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref presetId);
	}
}
