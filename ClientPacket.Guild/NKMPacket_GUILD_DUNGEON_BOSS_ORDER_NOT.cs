using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DUNGEON_BOSS_ORDER_NOT)]
public sealed class NKMPacket_GUILD_DUNGEON_BOSS_ORDER_NOT : ISerializable
{
	public short orderIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref orderIndex);
	}
}
