using Cs.Protocol;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_TAKE_HEART_REQ)]
public sealed class NKMPacket_OFFICE_TAKE_HEART_REQ : ISerializable
{
	public long unitUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUid);
	}
}
