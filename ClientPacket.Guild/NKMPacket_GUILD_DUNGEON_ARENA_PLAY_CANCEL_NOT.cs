using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DUNGEON_ARENA_PLAY_CANCEL_NOT)]
public sealed class NKMPacket_GUILD_DUNGEON_ARENA_PLAY_CANCEL_NOT : ISerializable
{
	public int arenaIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref arenaIndex);
	}
}
