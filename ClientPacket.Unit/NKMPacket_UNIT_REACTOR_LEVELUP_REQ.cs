using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_UNIT_REACTOR_LEVELUP_REQ)]
public sealed class NKMPacket_UNIT_REACTOR_LEVELUP_REQ : ISerializable
{
	public long unitUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUid);
	}
}
