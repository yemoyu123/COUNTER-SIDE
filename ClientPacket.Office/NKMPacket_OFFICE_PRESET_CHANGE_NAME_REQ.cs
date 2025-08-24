using Cs.Protocol;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_PRESET_CHANGE_NAME_REQ)]
public sealed class NKMPacket_OFFICE_PRESET_CHANGE_NAME_REQ : ISerializable
{
	public int presetId;

	public string newPresetName;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref presetId);
		stream.PutOrGet(ref newPresetName);
	}
}
