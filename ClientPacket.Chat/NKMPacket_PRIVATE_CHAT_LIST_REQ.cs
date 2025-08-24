using Cs.Protocol;
using Protocol;

namespace ClientPacket.Chat;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_CHAT_LIST_REQ)]
public sealed class NKMPacket_PRIVATE_CHAT_LIST_REQ : ISerializable
{
	public long userUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref userUid);
	}
}
