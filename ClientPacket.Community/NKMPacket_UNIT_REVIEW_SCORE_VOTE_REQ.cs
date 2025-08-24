using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_UNIT_REVIEW_SCORE_VOTE_REQ)]
public sealed class NKMPacket_UNIT_REVIEW_SCORE_VOTE_REQ : ISerializable
{
	public int unitID;

	public int score;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitID);
		stream.PutOrGet(ref score);
	}
}
