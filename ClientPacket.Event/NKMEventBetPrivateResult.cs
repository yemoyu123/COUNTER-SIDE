using Cs.Protocol;

namespace ClientPacket.Event;

public sealed class NKMEventBetPrivateResult : ISerializable
{
	public int eventIndex;

	public EventBetTeam selectTeam;

	public bool receiveReward;

	public bool isWin;

	public float dividentRate;

	public long betCount;

	public long rewardCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref eventIndex);
		stream.PutOrGetEnum(ref selectTeam);
		stream.PutOrGet(ref receiveReward);
		stream.PutOrGet(ref isWin);
		stream.PutOrGet(ref dividentRate);
		stream.PutOrGet(ref betCount);
		stream.PutOrGet(ref rewardCount);
	}
}
