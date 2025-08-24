using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.LeaderBoard;

[PacketId(ClientPacketId.kNKMPacket_LEADERBOARD_FIERCE_LIST_ACK)]
public sealed class NKMPacket_LEADERBOARD_FIERCE_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMLeaderBoardFierceData leaderBoardfierceData = new NKMLeaderBoardFierceData();

	public int userRankNumber;

	public int userRankPercent;

	public int fierceId;

	public List<NKMFierceBoss> bossList = new List<NKMFierceBoss>();

	public bool isAll;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref leaderBoardfierceData);
		stream.PutOrGet(ref userRankNumber);
		stream.PutOrGet(ref userRankPercent);
		stream.PutOrGet(ref fierceId);
		stream.PutOrGet(ref bossList);
		stream.PutOrGet(ref isAll);
	}
}
