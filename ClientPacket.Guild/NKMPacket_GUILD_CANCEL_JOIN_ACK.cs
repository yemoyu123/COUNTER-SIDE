using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_CANCEL_JOIN_ACK)]
public sealed class NKMPacket_GUILD_CANCEL_JOIN_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long guildUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref guildUid);
	}
}
