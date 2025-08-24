using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_POST_SEND_ACK)]
public sealed class NKMPacket_OFFICE_POST_SEND_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long receiverUserUid;

	public NKMOfficePostState postState = new NKMOfficePostState();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref receiverUserUid);
		stream.PutOrGet(ref postState);
	}
}
