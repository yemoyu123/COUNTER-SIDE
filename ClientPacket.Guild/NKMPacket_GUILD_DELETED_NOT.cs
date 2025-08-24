using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DELETED_NOT)]
public sealed class NKMPacket_GUILD_DELETED_NOT : ISerializable
{
	public long guildUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
	}
}
