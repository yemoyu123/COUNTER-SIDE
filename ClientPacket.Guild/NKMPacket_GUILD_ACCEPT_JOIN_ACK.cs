using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_ACCEPT_JOIN_ACK)]
public sealed class NKMPacket_GUILD_ACCEPT_JOIN_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public bool isAllow;

	public long guildUid;

	public long joinUserUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref isAllow);
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref joinUserUid);
	}
}
