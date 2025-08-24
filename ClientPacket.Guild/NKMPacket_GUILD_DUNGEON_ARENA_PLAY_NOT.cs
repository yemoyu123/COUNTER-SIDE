using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DUNGEON_ARENA_PLAY_NOT)]
public sealed class NKMPacket_GUILD_DUNGEON_ARENA_PLAY_NOT : ISerializable
{
	public int arenaId;

	public long userUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref arenaId);
		stream.PutOrGet(ref userUid);
	}
}
