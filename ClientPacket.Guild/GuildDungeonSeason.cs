using Cs.Protocol;

namespace ClientPacket.Guild;

public sealed class GuildDungeonSeason : ISerializable
{
	public int seasonId;

	public long score;

	public int joinCount;

	public int lastRewardScore;

	public int lastRewardJoin;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref seasonId);
		stream.PutOrGet(ref score);
		stream.PutOrGet(ref joinCount);
		stream.PutOrGet(ref lastRewardScore);
		stream.PutOrGet(ref lastRewardJoin);
	}
}
