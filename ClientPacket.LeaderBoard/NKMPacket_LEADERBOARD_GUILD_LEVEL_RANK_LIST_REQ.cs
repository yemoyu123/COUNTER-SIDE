using Cs.Protocol;
using Protocol;

namespace ClientPacket.LeaderBoard;

[PacketId(ClientPacketId.kNKMPacket_LEADERBOARD_GUILD_LEVEL_RANK_LIST_REQ)]
public sealed class NKMPacket_LEADERBOARD_GUILD_LEVEL_RANK_LIST_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
