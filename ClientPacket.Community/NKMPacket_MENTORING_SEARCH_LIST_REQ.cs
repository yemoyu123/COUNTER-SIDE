using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_MENTORING_SEARCH_LIST_REQ)]
public sealed class NKMPacket_MENTORING_SEARCH_LIST_REQ : ISerializable
{
	public MentoringIdentity identity;

	public string keyword;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref identity);
		stream.PutOrGet(ref keyword);
	}
}
