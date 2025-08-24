using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_TOURNAMENT_REPLAY_LINK_ACK)]
public sealed class NKMPacket_TOURNAMENT_REPLAY_LINK_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMTournamentReplayLink replayLink = new NKMTournamentReplayLink();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref replayLink);
	}
}
