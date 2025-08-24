using Cs.Protocol;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_ADD_FURNITURE_REQ)]
public sealed class NKMPacket_OFFICE_ADD_FURNITURE_REQ : ISerializable
{
	public int roomId;

	public int itemId;

	public OfficePlaneType planeType;

	public int positionX;

	public int positionY;

	public bool inverted;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref roomId);
		stream.PutOrGet(ref itemId);
		stream.PutOrGetEnum(ref planeType);
		stream.PutOrGet(ref positionX);
		stream.PutOrGet(ref positionY);
		stream.PutOrGet(ref inverted);
	}
}
