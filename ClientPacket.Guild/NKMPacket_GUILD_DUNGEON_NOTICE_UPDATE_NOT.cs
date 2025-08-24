using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DUNGEON_NOTICE_UPDATE_NOT)]
public sealed class NKMPacket_GUILD_DUNGEON_NOTICE_UPDATE_NOT : ISerializable
{
	public long guildUid;

	public string notice;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref notice);
	}
}
