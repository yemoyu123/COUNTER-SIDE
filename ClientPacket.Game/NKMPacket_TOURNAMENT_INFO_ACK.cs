using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_TOURNAMENT_INFO_ACK)]
public sealed class NKMPacket_TOURNAMENT_INFO_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int tournamentId;

	public NKMTournamentState state;

	public List<NKMTournamentPlayInfo> history = new List<NKMTournamentPlayInfo>();

	public List<NKMTournamentInfo> infos = new List<NKMTournamentInfo>();

	public bool canRecvReward;

	public NKMAsyncDeckData deck = new NKMAsyncDeckData();

	public long userCount;

	public PvpCastingVoteData pvpCastingVoteData = new PvpCastingVoteData();

	public Dictionary<int, NKMTournamentBanResult> tournamentBanResult = new Dictionary<int, NKMTournamentBanResult>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref tournamentId);
		stream.PutOrGetEnum(ref state);
		stream.PutOrGet(ref history);
		stream.PutOrGet(ref infos);
		stream.PutOrGet(ref canRecvReward);
		stream.PutOrGet(ref deck);
		stream.PutOrGet(ref userCount);
		stream.PutOrGet(ref pvpCastingVoteData);
		stream.PutOrGet(ref tournamentBanResult);
	}
}
