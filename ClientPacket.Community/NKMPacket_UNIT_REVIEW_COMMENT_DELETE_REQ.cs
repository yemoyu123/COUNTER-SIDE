using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_UNIT_REVIEW_COMMENT_DELETE_REQ)]
public sealed class NKMPacket_UNIT_REVIEW_COMMENT_DELETE_REQ : ISerializable
{
	public int unitID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitID);
	}
}
