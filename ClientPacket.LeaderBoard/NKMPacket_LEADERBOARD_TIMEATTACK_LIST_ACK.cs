using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.LeaderBoard;

[PacketId(ClientPacketId.kNKMPacket_LEADERBOARD_TIMEATTACK_LIST_ACK)]
public sealed class NKMPacket_LEADERBOARD_TIMEATTACK_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMLeaderBoardTimeAttackData leaderBoardTimeAttackData = new NKMLeaderBoardTimeAttackData();

	public int userRank;

	public int stageId;

	public bool isAll;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref leaderBoardTimeAttackData);
		stream.PutOrGet(ref userRank);
		stream.PutOrGet(ref stageId);
		stream.PutOrGet(ref isAll);
	}
}
