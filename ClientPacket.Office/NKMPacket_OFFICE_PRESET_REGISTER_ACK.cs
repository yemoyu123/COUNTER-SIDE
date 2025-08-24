using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_PRESET_REGISTER_ACK)]
public sealed class NKMPacket_OFFICE_PRESET_REGISTER_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMOfficePreset preset = new NKMOfficePreset();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref preset);
	}
}
