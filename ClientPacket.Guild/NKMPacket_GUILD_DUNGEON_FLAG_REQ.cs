using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DUNGEON_FLAG_REQ)]
public sealed class NKMPacket_GUILD_DUNGEON_FLAG_REQ : ISerializable
{
	public long guildUid;

	public int arenaIndex;

	public int flagIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref arenaIndex);
		stream.PutOrGet(ref flagIndex);
	}
}
