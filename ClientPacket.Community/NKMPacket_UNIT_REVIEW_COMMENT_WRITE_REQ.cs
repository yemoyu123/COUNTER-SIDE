using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_UNIT_REVIEW_COMMENT_WRITE_REQ)]
public sealed class NKMPacket_UNIT_REVIEW_COMMENT_WRITE_REQ : ISerializable
{
	public int unitID;

	public string content;

	public bool isRewrite;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitID);
		stream.PutOrGet(ref content);
		stream.PutOrGet(ref isRewrite);
	}
}
