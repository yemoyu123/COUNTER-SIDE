using Cs.Protocol;
using Protocol;

namespace ClientPacket.Defence;

[PacketId(ClientPacketId.kNKMPacket_DEFENCE_RANK_REWARD_REQ)]
public sealed class NKMPacket_DEFENCE_RANK_REWARD_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
