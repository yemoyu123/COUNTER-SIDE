using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_BUY_BUFF_REQ)]
public sealed class NKMPacket_GUILD_BUY_BUFF_REQ : ISerializable
{
	public long guildUid;

	public int welfareId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref welfareId);
	}
}
