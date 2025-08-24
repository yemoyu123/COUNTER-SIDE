using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_TOURNAMENT_PREDICTION_PRIVATE_INFO_ACK)]
public sealed class NKMPacket_TOURNAMENT_PREDICTION_PRIVATE_INFO_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMTournamentInfo> infos = new List<NKMTournamentInfo>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref infos);
	}
}
