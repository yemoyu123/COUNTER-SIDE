using Cs.Protocol;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_REMOVE_FURNITURE_REQ)]
public sealed class NKMPacket_OFFICE_REMOVE_FURNITURE_REQ : ISerializable
{
	public int roomId;

	public long furnitureUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref roomId);
		stream.PutOrGet(ref furnitureUid);
	}
}
