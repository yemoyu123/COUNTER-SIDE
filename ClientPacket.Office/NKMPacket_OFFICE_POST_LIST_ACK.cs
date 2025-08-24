using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_POST_LIST_ACK)]
public sealed class NKMPacket_OFFICE_POST_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMOfficePost> postList = new List<NKMOfficePost>();

	public int postCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref postList);
		stream.PutOrGet(ref postCount);
	}
}
