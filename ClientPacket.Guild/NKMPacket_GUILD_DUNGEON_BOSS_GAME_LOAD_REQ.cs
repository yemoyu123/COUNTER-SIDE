using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DUNGEON_BOSS_GAME_LOAD_REQ)]
public sealed class NKMPacket_GUILD_DUNGEON_BOSS_GAME_LOAD_REQ : ISerializable
{
	public byte selectDeckIndex;

	public int bossStageId;

	public bool isPractice;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref selectDeckIndex);
		stream.PutOrGet(ref bossStageId);
		stream.PutOrGet(ref isPractice);
	}
}
