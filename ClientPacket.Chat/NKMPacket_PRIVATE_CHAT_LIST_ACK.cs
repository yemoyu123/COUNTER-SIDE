using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Chat;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_CHAT_LIST_ACK)]
public sealed class NKMPacket_PRIVATE_CHAT_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long userUid;

	public List<NKMChatMessageData> messages = new List<NKMChatMessageData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref userUid);
		stream.PutOrGet(ref messages);
	}
}
