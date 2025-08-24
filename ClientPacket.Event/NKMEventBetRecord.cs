using System;
using Cs.Protocol;

namespace ClientPacket.Event;

public sealed class NKMEventBetRecord : ISerializable
{
	public int eventIndex;

	public DateTime updatedTime;

	public int winRateTeamA;

	public int winRateTeamB;

	public float dividentRateTeamA;

	public float dividentRateTeamB;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref eventIndex);
		stream.PutOrGet(ref updatedTime);
		stream.PutOrGet(ref winRateTeamA);
		stream.PutOrGet(ref winRateTeamB);
		stream.PutOrGet(ref dividentRateTeamA);
		stream.PutOrGet(ref dividentRateTeamB);
	}
}
