using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_FRIEND_LIST_ACK)]
public sealed class NKMPacket_FRIEND_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKM_FRIEND_LIST_TYPE friendListType;

	public List<FriendListData> list = new List<FriendListData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGetEnum(ref friendListType);
		stream.PutOrGet(ref list);
	}
}
