using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.LeaderBoard;

public sealed class NKMLeaderBoardTimeAttackData : ISerializable
{
	public List<NKMTimeAttackData> timeAttackData = new List<NKMTimeAttackData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref timeAttackData);
	}
}
