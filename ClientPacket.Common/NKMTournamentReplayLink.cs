using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMTournamentReplayLink : ISerializable
{
	public int tournamentId;

	public NKMTournamentGroups groupIndex;

	public int slotIndex;

	public NKMReplayLink replayLink = new NKMReplayLink();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref tournamentId);
		stream.PutOrGetEnum(ref groupIndex);
		stream.PutOrGet(ref slotIndex);
		stream.PutOrGet(ref replayLink);
	}
}
