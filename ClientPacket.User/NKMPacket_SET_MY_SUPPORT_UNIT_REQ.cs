using Cs.Protocol;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_SET_MY_SUPPORT_UNIT_REQ)]
public sealed class NKMPacket_SET_MY_SUPPORT_UNIT_REQ : ISerializable
{
	public long unitUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUid);
	}
}
