using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_MENTORING_MATCH_LIST_ACK)]
public sealed class NKMPacket_MENTORING_MATCH_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<MenteeInfo> matchList = new List<MenteeInfo>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref matchList);
	}
}
