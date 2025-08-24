using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.LeaderBoard;

public sealed class NKMLeaderBoardDefenceData : ISerializable
{
	public List<NKMDefenceRankData> rankDatas = new List<NKMDefenceRankData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref rankDatas);
	}
}
