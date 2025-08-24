using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_UNIT_REVIEW_COMMENT_VOTE_ACK)]
public sealed class NKMPacket_UNIT_REVIEW_COMMENT_VOTE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int unitID;

	public NKMUnitReviewCommentData unitReviewCommentData = new NKMUnitReviewCommentData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref unitID);
		stream.PutOrGet(ref unitReviewCommentData);
	}
}
