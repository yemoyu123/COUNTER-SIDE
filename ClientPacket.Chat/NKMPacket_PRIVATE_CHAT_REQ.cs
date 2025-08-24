using Cs.Protocol;
using Protocol;

namespace ClientPacket.Chat;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_CHAT_REQ)]
public sealed class NKMPacket_PRIVATE_CHAT_REQ : ISerializable
{
	public long userUid;

	public int emotionId;

	public string message;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref userUid);
		stream.PutOrGet(ref emotionId);
		stream.PutOrGet(ref message);
	}
}
