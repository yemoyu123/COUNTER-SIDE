using Cs.Protocol;

namespace ClientPacket.Event;

public sealed class NKMRacePrivate : ISerializable
{
	public int RaceId;

	public int RaceIndex;

	public RaceTeam SelectTeam;

	public int racePlayCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref RaceId);
		stream.PutOrGet(ref RaceIndex);
		stream.PutOrGetEnum(ref SelectTeam);
		stream.PutOrGet(ref racePlayCount);
	}
}
