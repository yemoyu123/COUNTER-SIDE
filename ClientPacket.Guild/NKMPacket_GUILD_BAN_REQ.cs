using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_BAN_REQ)]
public sealed class NKMPacket_GUILD_BAN_REQ : ISerializable
{
	public long guildUid;

	public long targetUserUid;

	public int banReason;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref targetUserUid);
		stream.PutOrGet(ref banReason);
	}
}
