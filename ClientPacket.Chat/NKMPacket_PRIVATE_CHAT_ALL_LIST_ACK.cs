using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Chat;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_CHAT_ALL_LIST_ACK)]
public sealed class NKMPacket_PRIVATE_CHAT_ALL_LIST_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<PrivateChatListData> friends = new List<PrivateChatListData>();

	public List<PrivateChatListData> guildMembers = new List<PrivateChatListData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref friends);
		stream.PutOrGet(ref guildMembers);
	}
}
