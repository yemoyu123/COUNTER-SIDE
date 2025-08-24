using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_FRIEND_REQUEST_NOT)]
public sealed class NKMPacket_FRIEND_REQUEST_NOT : ISerializable
{
	public FriendListData friendProfileData = new FriendListData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref friendProfileData);
	}
}
