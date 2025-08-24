using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_TOURNAMENT_REPLAY_LINK_REQ)]
public sealed class NKMPacket_TOURNAMENT_REPLAY_LINK_REQ : ISerializable
{
	public int tournamentId;

	public NKMTournamentGroups groupIndex;

	public int slotIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref tournamentId);
		stream.PutOrGetEnum(ref groupIndex);
		stream.PutOrGet(ref slotIndex);
	}
}
