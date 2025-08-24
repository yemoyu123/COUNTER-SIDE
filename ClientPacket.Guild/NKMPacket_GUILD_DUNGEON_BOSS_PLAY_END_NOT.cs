using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DUNGEON_BOSS_PLAY_END_NOT)]
public sealed class NKMPacket_GUILD_DUNGEON_BOSS_PLAY_END_NOT : ISerializable
{
	public long playedUserUid;

	public int bossStageId;

	public float damage;

	public float remainHp;

	public int totalPoint;

	public int extraPoint;

	public int point;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref playedUserUid);
		stream.PutOrGet(ref bossStageId);
		stream.PutOrGet(ref damage);
		stream.PutOrGet(ref remainHp);
		stream.PutOrGet(ref totalPoint);
		stream.PutOrGet(ref extraPoint);
		stream.PutOrGet(ref point);
	}
}
