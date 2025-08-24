using Cs.Protocol;
using NKM.Templet;

namespace ClientPacket.User;

public sealed class NKMBackgroundUnitInfo : ISerializable
{
	public long unitUid;

	public NKM_UNIT_TYPE unitType;

	public float unitSize;

	public int unitFace;

	public float unitPosX;

	public float unitPosY;

	public bool backImage;

	public int skinOption;

	public float rotation;

	public bool flip;

	public float animTime;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUid);
		stream.PutOrGetEnum(ref unitType);
		stream.PutOrGet(ref unitSize);
		stream.PutOrGet(ref unitFace);
		stream.PutOrGet(ref unitPosX);
		stream.PutOrGet(ref unitPosY);
		stream.PutOrGet(ref backImage);
		stream.PutOrGet(ref skinOption);
		stream.PutOrGet(ref rotation);
		stream.PutOrGet(ref flip);
		stream.PutOrGet(ref animTime);
	}
}
