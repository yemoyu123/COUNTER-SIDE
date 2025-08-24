using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_INVITE_REQ)]
public sealed class NKMPacket_GUILD_INVITE_REQ : ISerializable
{
	public long guildUid;

	public long userUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref userUid);
	}
}
