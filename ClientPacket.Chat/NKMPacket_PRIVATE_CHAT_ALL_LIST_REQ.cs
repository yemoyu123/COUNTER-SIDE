using Cs.Protocol;
using Protocol;

namespace ClientPacket.Chat;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_CHAT_ALL_LIST_REQ)]
public sealed class NKMPacket_PRIVATE_CHAT_ALL_LIST_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
