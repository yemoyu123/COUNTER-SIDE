using Cs.Protocol;

namespace ClientPacket.Community;

public sealed class NKMUnitReviewTagData : ISerializable
{
	public short tagType;

	public int votedCount;

	public bool isVoted;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref tagType);
		stream.PutOrGet(ref votedCount);
		stream.PutOrGet(ref isVoted);
	}
}
