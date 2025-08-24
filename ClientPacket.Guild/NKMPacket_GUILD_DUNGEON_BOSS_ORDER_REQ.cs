using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DUNGEON_BOSS_ORDER_REQ)]
public sealed class NKMPacket_GUILD_DUNGEON_BOSS_ORDER_REQ : ISerializable
{
	public long guildUid;

	public short orderIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref orderIndex);
	}
}
