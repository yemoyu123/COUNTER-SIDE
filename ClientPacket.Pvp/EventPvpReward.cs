using Cs.Protocol;

namespace ClientPacket.Pvp;

public sealed class EventPvpReward : ISerializable
{
	public int seasonId;

	public int groupId;

	public int rewardId;

	public int step;

	public int playCount;

	public bool isReward;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref seasonId);
		stream.PutOrGet(ref groupId);
		stream.PutOrGet(ref rewardId);
		stream.PutOrGet(ref step);
		stream.PutOrGet(ref playCount);
		stream.PutOrGet(ref isReward);
	}
}
