using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_TOURNAMENT_PREDICTION_STATISTICS_ACK)]
public sealed class NKMPacket_TOURNAMENT_PREDICTION_STATISTICS_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int templetId;

	public NKMTournamentGroups groupIndex;

	public NKMTournamentPredictionStatistics predicitionStatistics = new NKMTournamentPredictionStatistics();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref templetId);
		stream.PutOrGetEnum(ref groupIndex);
		stream.PutOrGet(ref predicitionStatistics);
	}
}
