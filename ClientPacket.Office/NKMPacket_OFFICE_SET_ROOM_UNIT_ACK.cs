using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_SET_ROOM_UNIT_ACK)]
public sealed class NKMPacket_OFFICE_SET_ROOM_UNIT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMUnitData> units = new List<NKMUnitData>();

	public List<NKMOfficeRoom> rooms = new List<NKMOfficeRoom>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref units);
		stream.PutOrGet(ref rooms);
	}
}
