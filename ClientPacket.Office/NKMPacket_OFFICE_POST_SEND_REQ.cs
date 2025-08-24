using Cs.Protocol;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_POST_SEND_REQ)]
public sealed class NKMPacket_OFFICE_POST_SEND_REQ : ISerializable
{
	public long receiverUserUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref receiverUserUid);
	}
}
