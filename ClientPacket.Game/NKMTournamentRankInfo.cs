using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;

namespace ClientPacket.Game;

public sealed class NKMTournamentRankInfo : ISerializable
{
	public int tournamentId;

	public List<long> ranks = new List<long>();

	public List<NKMTournamentProfileData> profiles = new List<NKMTournamentProfileData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref tournamentId);
		stream.PutOrGet(ref ranks);
		stream.PutOrGet(ref profiles);
	}
}
