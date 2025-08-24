using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_CLOSE_REQ)]
public sealed class NKMPacket_GUILD_CLOSE_REQ : ISerializable
{
	public long guildUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
	}
}
