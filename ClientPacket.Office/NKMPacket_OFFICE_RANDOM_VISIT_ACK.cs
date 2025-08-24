using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_RANDOM_VISIT_ACK)]
public sealed class NKMPacket_OFFICE_RANDOM_VISIT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMOfficeState officeState = new NKMOfficeState();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref officeState);
	}
}
