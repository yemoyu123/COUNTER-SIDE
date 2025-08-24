using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.LeaderBoard;

public sealed class NKMLeaderBoardFierceData : ISerializable
{
	public List<NKMFierceData> fierceData = new List<NKMFierceData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref fierceData);
	}
}
