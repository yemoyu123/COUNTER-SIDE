using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_TOURNAMENT_PREDICTION_REQ)]
public sealed class NKMPacket_TOURNAMENT_PREDICTION_REQ : ISerializable
{
	public int templetId;

	public NKMTournamentGroups groupIndex;

	public List<long> slotUserUid = new List<long>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref templetId);
		stream.PutOrGetEnum(ref groupIndex);
		stream.PutOrGet(ref slotUserUid);
	}
}
