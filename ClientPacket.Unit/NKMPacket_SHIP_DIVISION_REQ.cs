using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_SHIP_DIVISION_REQ)]
public sealed class NKMPacket_SHIP_DIVISION_REQ : ISerializable
{
	public List<long> removeShipUIDList = new List<long>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref removeShipUIDList);
	}
}
