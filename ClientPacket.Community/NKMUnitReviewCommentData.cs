using Cs.Protocol;

namespace ClientPacket.Community;

public sealed class NKMUnitReviewCommentData : ISerializable
{
	public long commentUID;

	public long userUID;

	public string nickName;

	public int level;

	public string content;

	public int votedCount;

	public bool isVoted;

	public long regDate;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref commentUID);
		stream.PutOrGet(ref userUID);
		stream.PutOrGet(ref nickName);
		stream.PutOrGet(ref level);
		stream.PutOrGet(ref content);
		stream.PutOrGet(ref votedCount);
		stream.PutOrGet(ref isVoted);
		stream.PutOrGet(ref regDate);
	}
}
