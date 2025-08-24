using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_UNIT_REVIEW_COMMENT_AND_SCORE_REQ)]
public sealed class NKMPacket_UNIT_REVIEW_COMMENT_AND_SCORE_REQ : ISerializable
{
	public int unitID;

	public bool isOrderByVotedCount;

	public int pageNumber;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitID);
		stream.PutOrGet(ref isOrderByVotedCount);
		stream.PutOrGet(ref pageNumber);
	}
}
