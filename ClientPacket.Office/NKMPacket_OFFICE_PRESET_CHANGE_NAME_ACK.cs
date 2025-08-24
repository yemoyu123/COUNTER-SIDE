using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_PRESET_CHANGE_NAME_ACK)]
public sealed class NKMPacket_OFFICE_PRESET_CHANGE_NAME_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int presetId;

	public string newPresetName;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref presetId);
		stream.PutOrGet(ref newPresetName);
	}
}
