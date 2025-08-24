using Cs.Protocol;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_SET_ROOM_NAME_REQ)]
public sealed class NKMPacket_OFFICE_SET_ROOM_NAME_REQ : ISerializable
{
	public int roomId;

	public string roomName;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref roomId);
		stream.PutOrGet(ref roomName);
	}
}
