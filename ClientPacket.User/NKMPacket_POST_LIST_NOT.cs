using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_POST_LIST_NOT)]
public sealed class NKMPacket_POST_LIST_NOT : ISerializable
{
	public List<NKMPostData> postDataList = new List<NKMPostData>();

	public int postCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref postDataList);
		stream.PutOrGet(ref postCount);
	}
}
