using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_TOURNAMENT_CASTING_VOTE_SHIP_REQ)]
public sealed class NKMPacket_TOURNAMENT_CASTING_VOTE_SHIP_REQ : ISerializable
{
	public int tournamentId;

	public List<int> shipGroupIdList = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref tournamentId);
		stream.PutOrGet(ref shipGroupIdList);
	}
}
