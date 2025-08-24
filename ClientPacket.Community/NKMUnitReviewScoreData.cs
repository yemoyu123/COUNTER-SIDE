using Cs.Protocol;

namespace ClientPacket.Community;

public sealed class NKMUnitReviewScoreData : ISerializable
{
	public float avgScore;

	public int votedCount;

	public byte myScore;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref avgScore);
		stream.PutOrGet(ref votedCount);
		stream.PutOrGet(ref myScore);
	}
}
