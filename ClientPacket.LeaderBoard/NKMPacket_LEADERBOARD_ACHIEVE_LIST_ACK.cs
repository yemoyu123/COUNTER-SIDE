using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.LeaderBoard;

[PacketId(ClientPacketId.kNKMPacket_LEADERBOARD_ACHIEVE_LIST_ACK)]
public sealed class NKMPacket_LEADERBOARD_ACHIEVE_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMLeaderBoardAchieveData leaderBoardAchieveData = new NKMLeaderBoardAchieveData();

	public int userRank;

	public bool isAll;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref leaderBoardAchieveData);
		stream.PutOrGet(ref userRank);
		stream.PutOrGet(ref isAll);
	}
}
