using System.Collections.Generic;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_SET_ROOM_UNIT_REQ)]
public sealed class NKMPacket_OFFICE_SET_ROOM_UNIT_REQ : ISerializable
{
	public int roomId;

	public List<long> unitUids = new List<long>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref roomId);
		stream.PutOrGet(ref unitUids);
	}
}
