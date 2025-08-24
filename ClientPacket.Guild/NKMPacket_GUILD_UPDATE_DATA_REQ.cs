using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_UPDATE_DATA_REQ)]
public sealed class NKMPacket_GUILD_UPDATE_DATA_REQ : ISerializable
{
	public long guildUid;

	public string greeting;

	public GuildJoinType guildJoinType;

	public long badgeId;

	public GuildChatNoticeType chatNoticeType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref greeting);
		stream.PutOrGetEnum(ref guildJoinType);
		stream.PutOrGet(ref badgeId);
		stream.PutOrGetEnum(ref chatNoticeType);
	}
}
