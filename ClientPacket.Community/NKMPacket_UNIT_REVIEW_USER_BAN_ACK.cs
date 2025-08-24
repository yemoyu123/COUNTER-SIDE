using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_UNIT_REVIEW_USER_BAN_ACK)]
public sealed class NKMPacket_UNIT_REVIEW_USER_BAN_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long targetUserUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref targetUserUid);
	}
}
