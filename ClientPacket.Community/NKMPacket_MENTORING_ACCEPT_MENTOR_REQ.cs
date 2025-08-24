using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_MENTORING_ACCEPT_MENTOR_REQ)]
public sealed class NKMPacket_MENTORING_ACCEPT_MENTOR_REQ : ISerializable
{
	public long mentorUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref mentorUid);
	}
}
