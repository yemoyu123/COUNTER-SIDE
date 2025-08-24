using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DUNGEON_BOSS_PLAY_CANCEL_NOT)]
public sealed class NKMPacket_GUILD_DUNGEON_BOSS_PLAY_CANCEL_NOT : ISerializable
{
	public long playUserUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref playUserUid);
	}
}
