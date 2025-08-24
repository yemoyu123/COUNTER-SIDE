using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_CHAT_LIST_ACK)]
public sealed class NKMPacket_OFFICE_CHAT_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long userUid;

	public List<NKMOfficeChatMessageData> messages = new List<NKMOfficeChatMessageData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref userUid);
		stream.PutOrGet(ref messages);
	}
}
