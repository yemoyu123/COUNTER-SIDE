using Cs.Protocol;

namespace ClientPacket.Event;

public sealed class NKMRaceSummary : ISerializable
{
	public NKMRaceResult raceResult = new NKMRaceResult();

	public NKMRacePrivate racePrivate = new NKMRacePrivate();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref raceResult);
		stream.PutOrGet(ref racePrivate);
	}
}
