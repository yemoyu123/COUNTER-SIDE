using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_MASTER_SPECIFIED_MIGRATION_REQ)]
public sealed class NKMPacket_GUILD_MASTER_SPECIFIED_MIGRATION_REQ : ISerializable
{
	public long guildUid;

	public long targetUserUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref targetUserUid);
	}
}
