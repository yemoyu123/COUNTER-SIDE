using Cs.Protocol;

namespace ClientPacket.Guild;

public sealed class GuildDungeonBoss : ISerializable
{
	public int stageId;

	public int playCount;

	public float remainHp;

	public int totalPoint;

	public int extraPoint;

	public long playUserUid;

	public short orderIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref stageId);
		stream.PutOrGet(ref playCount);
		stream.PutOrGet(ref remainHp);
		stream.PutOrGet(ref totalPoint);
		stream.PutOrGet(ref extraPoint);
		stream.PutOrGet(ref playUserUid);
		stream.PutOrGet(ref orderIndex);
	}
}
