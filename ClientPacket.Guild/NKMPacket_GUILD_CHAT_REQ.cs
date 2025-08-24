using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_CHAT_REQ)]
public sealed class NKMPacket_GUILD_CHAT_REQ : ISerializable
{
	public long guildUid;

	public ChatMessageType messageType;

	public int emotionId;

	public string message;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGetEnum(ref messageType);
		stream.PutOrGet(ref emotionId);
		stream.PutOrGet(ref message);
	}
}
