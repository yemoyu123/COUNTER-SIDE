using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_LIST_REQ)]
public sealed class NKMPacket_GUILD_LIST_REQ : ISerializable
{
	public GuildListType guildListType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref guildListType);
	}
}
