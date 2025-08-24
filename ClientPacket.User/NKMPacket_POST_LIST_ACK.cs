using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_POST_LIST_ACK)]
public sealed class NKMPacket_POST_LIST_ACK : ISerializable
{
	public List<NKMPostData> postDataList = new List<NKMPostData>();

	public int postCount;

	public NKM_ERROR_CODE errorCode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref postDataList);
		stream.PutOrGet(ref postCount);
		stream.PutOrGetEnum(ref errorCode);
	}
}
