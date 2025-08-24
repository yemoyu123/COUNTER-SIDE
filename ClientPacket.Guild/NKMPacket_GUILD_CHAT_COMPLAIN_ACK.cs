using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_CHAT_COMPLAIN_ACK)]
public sealed class NKMPacket_GUILD_CHAT_COMPLAIN_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long guildUid;

	public long messageUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref messageUid);
	}
}
