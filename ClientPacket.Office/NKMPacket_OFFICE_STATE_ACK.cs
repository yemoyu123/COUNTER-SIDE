using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_STATE_ACK)]
public sealed class NKMPacket_OFFICE_STATE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long userUid;

	public NKMOfficeState officeState = new NKMOfficeState();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref userUid);
		stream.PutOrGet(ref officeState);
	}
}
