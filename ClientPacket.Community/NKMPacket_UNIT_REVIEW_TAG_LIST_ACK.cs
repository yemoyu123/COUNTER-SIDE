using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_UNIT_REVIEW_TAG_LIST_ACK)]
public sealed class NKMPacket_UNIT_REVIEW_TAG_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMUnitReviewTagData> unitReviewTagDataList = new List<NKMUnitReviewTagData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref unitReviewTagDataList);
	}
}
