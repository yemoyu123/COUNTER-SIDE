using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_LEAGUE_PVP_SEASON_INFO_REQ)]
public sealed class NKMPacket_LEAGUE_PVP_SEASON_INFO_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
