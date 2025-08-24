using Cs.Protocol;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_CHAT_NOT)]
public sealed class NKMPacket_OFFICE_CHAT_NOT : ISerializable
{
	public NKMOfficeChatMessageData message = new NKMOfficeChatMessageData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref message);
	}
}
