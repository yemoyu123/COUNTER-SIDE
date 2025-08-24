using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_ACCEPT_INVITE_ACK)]
public sealed class NKMPacket_GUILD_ACCEPT_INVITE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public bool isAllow;

	public long guildUid;

	public PrivateGuildData privateGuildData = new PrivateGuildData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref isAllow);
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref privateGuildData);
	}
}
