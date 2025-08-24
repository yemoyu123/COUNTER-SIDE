using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_LEAGUE_PVP_WEEKLY_REWARD_REQ)]
public sealed class NKMPacket_LEAGUE_PVP_WEEKLY_REWARD_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
