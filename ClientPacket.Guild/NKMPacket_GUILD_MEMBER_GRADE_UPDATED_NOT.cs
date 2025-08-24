using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_MEMBER_GRADE_UPDATED_NOT)]
public sealed class NKMPacket_GUILD_MEMBER_GRADE_UPDATED_NOT : ISerializable
{
	public long guildUid;

	public GuildMemberGrade gradeBefore;

	public GuildMemberGrade gradeAfter;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGetEnum(ref gradeBefore);
		stream.PutOrGetEnum(ref gradeAfter);
	}
}
