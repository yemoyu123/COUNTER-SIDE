using System;
using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_FRIEND_ACCEPT_NOT)]
public sealed class NKMPacket_FRIEND_ACCEPT_NOT : ISerializable
{
	public bool isAllow;

	public FriendListData friendProfileData = new FriendListData();

	public DateTime regDate;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref isAllow);
		stream.PutOrGet(ref friendProfileData);
		stream.PutOrGet(ref regDate);
	}
}
