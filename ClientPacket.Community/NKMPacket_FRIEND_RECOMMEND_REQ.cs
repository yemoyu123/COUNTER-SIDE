using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_FRIEND_RECOMMEND_REQ)]
public sealed class NKMPacket_FRIEND_RECOMMEND_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
