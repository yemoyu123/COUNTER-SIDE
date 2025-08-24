using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_OPEN_ROOM_ACK)]
public sealed class NKMPacket_OFFICE_OPEN_ROOM_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMItemMiscData> costItems = new List<NKMItemMiscData>();

	public NKMOfficeRoom room = new NKMOfficeRoom();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref costItems);
		stream.PutOrGet(ref room);
	}
}
