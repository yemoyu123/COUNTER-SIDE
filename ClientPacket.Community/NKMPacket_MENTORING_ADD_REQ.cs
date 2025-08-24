using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_MENTORING_ADD_REQ)]
public sealed class NKMPacket_MENTORING_ADD_REQ : ISerializable
{
	public MentoringIdentity identity;

	public long userUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref identity);
		stream.PutOrGet(ref userUid);
	}
}
