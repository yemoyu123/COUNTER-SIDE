using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_CREATE_REQ)]
public sealed class NKMPacket_GUILD_CREATE_REQ : ISerializable
{
	public string guildName;

	public GuildJoinType guildJoinType;

	public long badgeId;

	public string greeting;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildName);
		stream.PutOrGetEnum(ref guildJoinType);
		stream.PutOrGet(ref badgeId);
		stream.PutOrGet(ref greeting);
	}
}
