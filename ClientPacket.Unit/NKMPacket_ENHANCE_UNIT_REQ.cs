using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_ENHANCE_UNIT_REQ)]
public sealed class NKMPacket_ENHANCE_UNIT_REQ : ISerializable
{
	public long unitUID;

	public List<long> consumeUnitUIDList = new List<long>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUID);
		stream.PutOrGet(ref consumeUnitUIDList);
	}
}
