using Cs.Protocol;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_SET_ROOM_WALL_REQ)]
public sealed class NKMPacket_OFFICE_SET_ROOM_WALL_REQ : ISerializable
{
	public int roomId;

	public int wallInteriorId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref roomId);
		stream.PutOrGet(ref wallInteriorId);
	}
}
