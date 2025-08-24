using ClientPacket.Common;
using Cs.Protocol;

namespace ClientPacket.Chat;

public sealed class PrivateChatListData : ISerializable
{
	public NKMCommonProfile commonProfile = new NKMCommonProfile();

	public NKMChatMessageData lastMessage = new NKMChatMessageData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref commonProfile);
		stream.PutOrGet(ref lastMessage);
	}
}
