using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_MENTORING_RECEIVE_LIST_REQ)]
public sealed class NKMPacket_MENTORING_RECEIVE_LIST_REQ : ISerializable
{
	public MentoringIdentity identity;

	public bool isForce;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref identity);
		stream.PutOrGet(ref isForce);
	}
}
