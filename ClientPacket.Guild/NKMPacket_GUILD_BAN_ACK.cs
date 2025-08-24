using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_BAN_ACK)]
public sealed class NKMPacket_GUILD_BAN_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long guildUid;

	public long targetUserUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref targetUserUid);
	}
}
