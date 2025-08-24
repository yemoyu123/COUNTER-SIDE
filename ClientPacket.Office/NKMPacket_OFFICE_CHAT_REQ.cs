using Cs.Protocol;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_CHAT_REQ)]
public sealed class NKMPacket_OFFICE_CHAT_REQ : ISerializable
{
	public long userUid;

	public int emotionId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref userUid);
		stream.PutOrGet(ref emotionId);
	}
}
