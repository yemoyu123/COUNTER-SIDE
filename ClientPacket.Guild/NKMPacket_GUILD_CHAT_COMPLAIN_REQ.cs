using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_CHAT_COMPLAIN_REQ)]
public sealed class NKMPacket_GUILD_CHAT_COMPLAIN_REQ : ISerializable
{
	public long guildUid;

	public long messageUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref messageUid);
	}
}
