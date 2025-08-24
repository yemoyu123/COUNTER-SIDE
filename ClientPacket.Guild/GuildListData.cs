using Cs.Protocol;

namespace ClientPacket.Guild;

public sealed class GuildListData : ISerializable
{
	public long guildUid;

	public string name;

	public long badgeId;

	public int guildLevel;

	public GuildJoinType guildJoinType;

	public string masterNickname;

	public int memberCount;

	public string greeting;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref name);
		stream.PutOrGet(ref badgeId);
		stream.PutOrGet(ref guildLevel);
		stream.PutOrGetEnum(ref guildJoinType);
		stream.PutOrGet(ref masterNickname);
		stream.PutOrGet(ref memberCount);
		stream.PutOrGet(ref greeting);
	}
}
