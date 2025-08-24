using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_CHAT_NOT)]
public sealed class NKMPacket_GUILD_CHAT_NOT : ISerializable
{
	public NKMChatMessageData message = new NKMChatMessageData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref message);
	}
}
