using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_TOURNAMENT_PREDICTION_ACK)]
public sealed class NKMPacket_TOURNAMENT_PREDICTION_ACK : ISerializable
{
	public NKMTournamentInfo info = new NKMTournamentInfo();

	public NKMRewardData rewardData;

	public NKM_ERROR_CODE errorCode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref info);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGetEnum(ref errorCode);
	}
}
