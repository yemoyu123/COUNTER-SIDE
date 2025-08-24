using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_FIERCE_COMPLETE_POINT_REWARD_ALL_REQ)]
public sealed class NKMPacket_FIERCE_COMPLETE_POINT_REWARD_ALL_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
