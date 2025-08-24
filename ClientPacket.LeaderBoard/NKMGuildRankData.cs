using Cs.Protocol;

namespace ClientPacket.LeaderBoard;

public sealed class NKMGuildRankData : ISerializable
{
	public long guildUid;

	public long badgeId;

	public string guildName;

	public string masterNickname;

	public int guildLevel;

	public int memberCount;

	public long rankValue;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref badgeId);
		stream.PutOrGet(ref guildName);
		stream.PutOrGet(ref masterNickname);
		stream.PutOrGet(ref guildLevel);
		stream.PutOrGet(ref memberCount);
		stream.PutOrGet(ref rankValue);
	}
}
