using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_UNIT_REVIEW_COMMENT_LIST_ACK)]
public sealed class NKMPacket_UNIT_REVIEW_COMMENT_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMUnitReviewCommentData> unitReviewCommentDataList = new List<NKMUnitReviewCommentData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref unitReviewCommentDataList);
	}
}
