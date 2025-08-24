using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_BAN_NOT)]
public sealed class NKMPacket_GUILD_BAN_NOT : ISerializable
{
	public long guildUid;

	public int banReason;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref banReason);
	}
}
