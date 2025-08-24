using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_MENTORING_MENTOR_REWARD_NOT)]
public sealed class NKMPacket_MENTORING_MENTOR_REWARD_NOT : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
