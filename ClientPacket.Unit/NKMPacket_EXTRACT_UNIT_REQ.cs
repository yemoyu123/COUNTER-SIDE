using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_EXTRACT_UNIT_REQ)]
public sealed class NKMPacket_EXTRACT_UNIT_REQ : ISerializable
{
	public List<long> extractUnitUidList = new List<long>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref extractUnitUidList);
	}
}
