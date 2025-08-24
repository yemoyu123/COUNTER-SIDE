using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_UNIT_REVIEW_TAG_VOTE_REQ)]
public sealed class NKMPacket_UNIT_REVIEW_TAG_VOTE_REQ : ISerializable
{
	public int unitID;

	public short tagType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitID);
		stream.PutOrGet(ref tagType);
	}
}
