using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_UNIT_TACTIC_UPDATE_REQ)]
public sealed class NKMPacket_UNIT_TACTIC_UPDATE_REQ : ISerializable
{
	public long unitUid;

	public List<long> consumeUnitUids = new List<long>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUid);
		stream.PutOrGet(ref consumeUnitUids);
	}
}
