using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_CHAT_LIST_NOT)]
public sealed class NKMPacket_GUILD_CHAT_LIST_NOT : ISerializable
{
	public long guildUid;

	public List<NKMChatMessageData> messages = new List<NKMChatMessageData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref messages);
	}
}
