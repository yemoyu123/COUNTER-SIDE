using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_CHAT_LIST_REQ)]
public sealed class NKMPacket_GUILD_CHAT_LIST_REQ : ISerializable
{
	public long guildUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
	}
}
