using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_UPDATE_MEMBER_GREETING_ACK)]
public sealed class NKMPacket_GUILD_UPDATE_MEMBER_GREETING_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public string greeting;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref greeting);
	}
}
