using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_OPERATOR_REMOVE_REQ)]
public sealed class NKMPacket_OPERATOR_REMOVE_REQ : ISerializable
{
	public List<long> removeUnitUIDList = new List<long>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref removeUnitUIDList);
	}
}
