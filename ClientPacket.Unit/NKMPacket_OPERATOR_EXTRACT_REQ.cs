using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_OPERATOR_EXTRACT_REQ)]
public sealed class NKMPacket_OPERATOR_EXTRACT_REQ : ISerializable
{
	public List<long> extractUnitUids = new List<long>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref extractUnitUids);
	}
}
