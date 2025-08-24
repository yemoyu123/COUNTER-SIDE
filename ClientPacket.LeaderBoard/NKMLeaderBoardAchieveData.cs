using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.LeaderBoard;

public sealed class NKMLeaderBoardAchieveData : ISerializable
{
	public List<NKMAchieveData> achieveData = new List<NKMAchieveData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref achieveData);
	}
}
