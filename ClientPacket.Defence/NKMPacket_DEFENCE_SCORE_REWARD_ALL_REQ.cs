using Cs.Protocol;
using Protocol;

namespace ClientPacket.Defence;

[PacketId(ClientPacketId.kNKMPacket_DEFENCE_SCORE_REWARD_ALL_REQ)]
public sealed class NKMPacket_DEFENCE_SCORE_REWARD_ALL_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
