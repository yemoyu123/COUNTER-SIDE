using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_SET_MEMBER_GRADE_REQ)]
public sealed class NKMPacket_GUILD_SET_MEMBER_GRADE_REQ : ISerializable
{
	public long guildUid;

	public long targetUserUid;

	public GuildMemberGrade grade;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref targetUserUid);
		stream.PutOrGetEnum(ref grade);
	}
}
