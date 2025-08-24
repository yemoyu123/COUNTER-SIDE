using Cs.Protocol;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_UPDATE_FURNITURE_REQ)]
public sealed class NKMPacket_OFFICE_UPDATE_FURNITURE_REQ : ISerializable
{
	public int roomId;

	public long furnitureUid;

	public OfficePlaneType planeType;

	public int positionX;

	public int positionY;

	public bool inverted;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref roomId);
		stream.PutOrGet(ref furnitureUid);
		stream.PutOrGetEnum(ref planeType);
		stream.PutOrGet(ref positionX);
		stream.PutOrGet(ref positionY);
		stream.PutOrGet(ref inverted);
	}
}
