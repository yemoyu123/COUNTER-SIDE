using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DUNGEON_BOSS_PLAY_NOT)]
public sealed class NKMPacket_GUILD_DUNGEON_BOSS_PLAY_NOT : ISerializable
{
	public long userUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref userUid);
	}
}
