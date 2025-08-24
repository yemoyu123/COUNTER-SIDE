using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_SET_ROOM_NAME_ACK)]
public sealed class NKMPacket_OFFICE_SET_ROOM_NAME_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMOfficeRoom room = new NKMOfficeRoom();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref room);
	}
}
