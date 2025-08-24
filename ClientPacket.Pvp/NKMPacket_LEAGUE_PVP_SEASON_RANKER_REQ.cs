using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_LEAGUE_PVP_SEASON_RANKER_REQ)]
public sealed class NKMPacket_LEAGUE_PVP_SEASON_RANKER_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
