using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_CANCEL_JOIN_REQ)]
public sealed class NKMPacket_GUILD_CANCEL_JOIN_REQ : ISerializable
{
	public long guildUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
	}
}
