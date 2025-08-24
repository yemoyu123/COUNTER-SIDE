using Cs.Protocol;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_OPEN_ROOM_REQ)]
public sealed class NKMPacket_OFFICE_OPEN_ROOM_REQ : ISerializable
{
	public int roomId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref roomId);
	}
}
