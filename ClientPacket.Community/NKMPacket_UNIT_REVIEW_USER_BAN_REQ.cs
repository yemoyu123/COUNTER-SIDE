using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_UNIT_REVIEW_USER_BAN_REQ)]
public sealed class NKMPacket_UNIT_REVIEW_USER_BAN_REQ : ISerializable
{
	public long targetUserUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref targetUserUid);
	}
}
