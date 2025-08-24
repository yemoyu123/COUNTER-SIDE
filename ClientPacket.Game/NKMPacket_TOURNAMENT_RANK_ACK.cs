using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_TOURNAMENT_RANK_ACK)]
public sealed class NKMPacket_TOURNAMENT_RANK_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMTournamentRankInfo> rankInfos = new List<NKMTournamentRankInfo>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref rankInfos);
	}
}
