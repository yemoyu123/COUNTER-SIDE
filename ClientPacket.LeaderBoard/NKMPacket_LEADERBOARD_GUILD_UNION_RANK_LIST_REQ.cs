using Cs.Protocol;
using Protocol;

namespace ClientPacket.LeaderBoard;

[PacketId(ClientPacketId.kNKMPacket_LEADERBOARD_GUILD_UNION_RANK_LIST_REQ)]
public sealed class NKMPacket_LEADERBOARD_GUILD_UNION_RANK_LIST_REQ : ISerializable
{
	public int seasonId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref seasonId);
	}
}
