using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.LeaderBoard;

public sealed class NKMLeaderBoardGuildData : ISerializable
{
	public List<NKMGuildRankData> rankDatas = new List<NKMGuildRankData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref rankDatas);
	}
}
