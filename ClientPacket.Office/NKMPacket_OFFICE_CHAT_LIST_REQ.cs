using Cs.Protocol;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_CHAT_LIST_REQ)]
public sealed class NKMPacket_OFFICE_CHAT_LIST_REQ : ISerializable
{
	public long userUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref userUid);
	}
}
