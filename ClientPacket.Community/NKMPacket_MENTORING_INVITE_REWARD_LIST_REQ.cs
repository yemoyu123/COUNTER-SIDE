using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_MENTORING_INVITE_REWARD_LIST_REQ)]
public sealed class NKMPacket_MENTORING_INVITE_REWARD_LIST_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
