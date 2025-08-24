using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_MENTORING_RECEIVE_LIST_ACK)]
public sealed class NKMPacket_MENTORING_RECEIVE_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<FriendListData> invitedList = new List<FriendListData>();

	public List<FriendListData> recommendList = new List<FriendListData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref invitedList);
		stream.PutOrGet(ref recommendList);
	}
}
