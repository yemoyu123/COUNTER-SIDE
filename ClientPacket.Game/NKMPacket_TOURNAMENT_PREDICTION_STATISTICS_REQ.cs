using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_TOURNAMENT_PREDICTION_STATISTICS_REQ)]
public sealed class NKMPacket_TOURNAMENT_PREDICTION_STATISTICS_REQ : ISerializable
{
	public int templetId;

	public NKMTournamentGroups groupIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref templetId);
		stream.PutOrGetEnum(ref groupIndex);
	}
}
