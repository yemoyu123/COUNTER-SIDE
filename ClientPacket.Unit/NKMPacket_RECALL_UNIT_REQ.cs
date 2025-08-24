using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_RECALL_UNIT_REQ)]
public sealed class NKMPacket_RECALL_UNIT_REQ : ISerializable
{
	public long recallUnitUid;

	public Dictionary<int, int> exchangeUnitList = new Dictionary<int, int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref recallUnitUid);
		stream.PutOrGet(ref exchangeUnitList);
	}
}
