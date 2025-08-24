using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.LeaderBoard;

[PacketId(ClientPacketId.kNKMPacket_LEADERBOARD_SHADOWPALACE_LIST_ACK)]
public sealed class NKMPacket_LEADERBOARD_SHADOWPALACE_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMLeaderBoardShadowPalaceData leaderBoardShadowPalaceData = new NKMLeaderBoardShadowPalaceData();

	public int userRank;

	public int actId;

	public bool isAll;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref leaderBoardShadowPalaceData);
		stream.PutOrGet(ref userRank);
		stream.PutOrGet(ref actId);
		stream.PutOrGet(ref isAll);
	}
}
