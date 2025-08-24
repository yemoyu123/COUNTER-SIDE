using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_FIERCE_COMPLETE_RANK_REWARD_REQ)]
public sealed class NKMPacket_FIERCE_COMPLETE_RANK_REWARD_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
