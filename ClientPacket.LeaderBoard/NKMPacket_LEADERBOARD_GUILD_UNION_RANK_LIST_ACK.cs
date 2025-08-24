using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.LeaderBoard;

[PacketId(ClientPacketId.kNKMPacket_LEADERBOARD_GUILD_UNION_RANK_LIST_ACK)]
public sealed class NKMPacket_LEADERBOARD_GUILD_UNION_RANK_LIST_ACK : ISerializable
{
	public int seasonId;

	public NKMLeaderBoardGuildData leaderBoard = new NKMLeaderBoardGuildData();

	public NKMRankData myRankData = new NKMRankData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref seasonId);
		stream.PutOrGet(ref leaderBoard);
		stream.PutOrGet(ref myRankData);
	}
}
