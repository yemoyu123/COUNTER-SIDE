using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_FRIEND_LIST_REQ)]
public sealed class NKMPacket_FRIEND_LIST_REQ : ISerializable
{
	public NKM_FRIEND_LIST_TYPE friendListType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref friendListType);
	}
}
