using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_CHAT_ACK)]
public sealed class NKMPacket_OFFICE_CHAT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long messageUid;

	public List<NKMOfficeChatMessageData> messages = new List<NKMOfficeChatMessageData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref messageUid);
		stream.PutOrGet(ref messages);
	}
}
