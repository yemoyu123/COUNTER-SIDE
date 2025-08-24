using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.LeaderBoard;

[PacketId(ClientPacketId.kNKMPacket_LEADERBOARD_FIERCE_BOSSGROUP_LIST_ACK)]
public sealed class NKMPacket_LEADERBOARD_FIERCE_BOSSGROUP_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMLeaderBoardFierceData leaderBoardfierceData = new NKMLeaderBoardFierceData();

	public int userRank;

	public int fierceId;

	public int fierceBossGroupId;

	public bool isAll;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref leaderBoardfierceData);
		stream.PutOrGet(ref userRank);
		stream.PutOrGet(ref fierceId);
		stream.PutOrGet(ref fierceBossGroupId);
		stream.PutOrGet(ref isAll);
	}
}
