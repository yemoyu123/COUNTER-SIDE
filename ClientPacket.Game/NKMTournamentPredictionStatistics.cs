using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;

namespace ClientPacket.Game;

public sealed class NKMTournamentPredictionStatistics : ISerializable
{
	public Dictionary<long, float> predictionStatistics = new Dictionary<long, float>();

	public List<NKMTournamentProfileData> userInfo = new List<NKMTournamentProfileData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref predictionStatistics);
		stream.PutOrGet(ref userInfo);
	}
}
