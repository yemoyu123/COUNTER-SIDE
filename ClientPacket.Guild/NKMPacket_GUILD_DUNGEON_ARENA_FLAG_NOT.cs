using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DUNGEON_ARENA_FLAG_NOT)]
public sealed class NKMPacket_GUILD_DUNGEON_ARENA_FLAG_NOT : ISerializable
{
	public int arenaIndex;

	public int flagIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref arenaIndex);
		stream.PutOrGet(ref flagIndex);
	}
}
