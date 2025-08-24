using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_UNIT_REVIEW_COMMENT_AND_SCORE_ACK)]
public sealed class NKMPacket_UNIT_REVIEW_COMMENT_AND_SCORE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int unitID;

	public List<NKMUnitReviewCommentData> bestUnitReviewCommentDataList = new List<NKMUnitReviewCommentData>();

	public List<NKMUnitReviewCommentData> unitReviewCommentDataList = new List<NKMUnitReviewCommentData>();

	public NKMUnitReviewCommentData myUnitReviewCommentData = new NKMUnitReviewCommentData();

	public NKMUnitReviewScoreData unitReviewScoreData = new NKMUnitReviewScoreData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref unitID);
		stream.PutOrGet(ref bestUnitReviewCommentDataList);
		stream.PutOrGet(ref unitReviewCommentDataList);
		stream.PutOrGet(ref myUnitReviewCommentData);
		stream.PutOrGet(ref unitReviewScoreData);
	}
}
