using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_CHAT_LIST_ACK)]
public sealed class NKMPacket_GUILD_CHAT_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long guildUid;

	public List<NKMChatMessageData> messages = new List<NKMChatMessageData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref messages);
	}
}
