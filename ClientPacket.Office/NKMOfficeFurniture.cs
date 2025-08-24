using Cs.Protocol;

namespace ClientPacket.Office;

public sealed class NKMOfficeFurniture : ISerializable
{
	public long uid;

	public int itemId;

	public OfficePlaneType planeType;

	public int positionX;

	public int positionY;

	public bool inverted;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref uid);
		stream.PutOrGet(ref itemId);
		stream.PutOrGetEnum(ref planeType);
		stream.PutOrGet(ref positionX);
		stream.PutOrGet(ref positionY);
		stream.PutOrGet(ref inverted);
	}
}
