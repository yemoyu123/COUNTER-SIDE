using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_UPDATE_NOTICE_REQ)]
public sealed class NKMPacket_GUILD_UPDATE_NOTICE_REQ : ISerializable
{
	public long guildUid;

	public string notice;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref notice);
	}
}
