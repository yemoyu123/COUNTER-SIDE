using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_CANCEL_INVITE_ACK)]
public sealed class NKMPacket_GUILD_CANCEL_INVITE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long userUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref userUid);
	}
}
