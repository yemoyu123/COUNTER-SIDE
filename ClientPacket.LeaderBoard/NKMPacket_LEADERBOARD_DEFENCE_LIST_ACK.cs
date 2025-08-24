using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.LeaderBoard;

[PacketId(ClientPacketId.kNKMPacket_LEADERBOARD_DEFENCE_LIST_ACK)]
public sealed class NKMPacket_LEADERBOARD_DEFENCE_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMLeaderBoardDefenceData leaderBoardDefenceData = new NKMLeaderBoardDefenceData();

	public int userRank;

	public int defenceTempletId;

	public bool isAll;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref leaderBoardDefenceData);
		stream.PutOrGet(ref userRank);
		stream.PutOrGet(ref defenceTempletId);
		stream.PutOrGet(ref isAll);
	}
}
