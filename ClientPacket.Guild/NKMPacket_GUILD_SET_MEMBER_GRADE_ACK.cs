using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_SET_MEMBER_GRADE_ACK)]
public sealed class NKMPacket_GUILD_SET_MEMBER_GRADE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long guildUid;

	public long targetUserUid;

	public GuildMemberGrade grade;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref targetUserUid);
		stream.PutOrGetEnum(ref grade);
	}
}
