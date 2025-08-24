using Cs.Protocol;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_STATE_REQ)]
public sealed class NKMPacket_OFFICE_STATE_REQ : ISerializable
{
	public long userUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref userUid);
	}
}
