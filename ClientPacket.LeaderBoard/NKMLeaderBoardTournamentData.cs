using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.LeaderBoard;

public sealed class NKMLeaderBoardTournamentData : ISerializable
{
	public List<long> userUids = new List<long>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref userUids);
	}
}
