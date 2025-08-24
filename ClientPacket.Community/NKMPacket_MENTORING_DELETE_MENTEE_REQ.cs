using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_MENTORING_DELETE_MENTEE_REQ)]
public sealed class NKMPacket_MENTORING_DELETE_MENTEE_REQ : ISerializable
{
	public long menteeUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref menteeUid);
	}
}
