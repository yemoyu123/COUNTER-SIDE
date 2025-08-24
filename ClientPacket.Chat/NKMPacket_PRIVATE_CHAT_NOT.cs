using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Chat;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_CHAT_NOT)]
public sealed class NKMPacket_PRIVATE_CHAT_NOT : ISerializable
{
	public NKMChatMessageData message = new NKMChatMessageData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref message);
	}
}
