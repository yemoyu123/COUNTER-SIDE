using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_MENTORING_MATCH_LIST_REQ)]
public sealed class NKMPacket_MENTORING_MATCH_LIST_REQ : ISerializable
{
	public bool isForce;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref isForce);
	}
}
