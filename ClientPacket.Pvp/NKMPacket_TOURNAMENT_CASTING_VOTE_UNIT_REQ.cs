using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_TOURNAMENT_CASTING_VOTE_UNIT_REQ)]
public sealed class NKMPacket_TOURNAMENT_CASTING_VOTE_UNIT_REQ : ISerializable
{
	public int tournamentId;

	public List<int> unitIdList = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref tournamentId);
		stream.PutOrGet(ref unitIdList);
	}
}
