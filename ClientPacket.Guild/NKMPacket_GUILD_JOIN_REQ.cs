using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_JOIN_REQ)]
public sealed class NKMPacket_GUILD_JOIN_REQ : ISerializable
{
	public long guildUid;

	public GuildJoinType guildJoinType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGetEnum(ref guildJoinType);
	}
}
