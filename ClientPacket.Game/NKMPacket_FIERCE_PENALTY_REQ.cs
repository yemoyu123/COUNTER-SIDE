using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_FIERCE_PENALTY_REQ)]
public sealed class NKMPacket_FIERCE_PENALTY_REQ : ISerializable
{
	public int fierceBossId;

	public List<int> penaltyIds = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref fierceBossId);
		stream.PutOrGet(ref penaltyIds);
	}
}
