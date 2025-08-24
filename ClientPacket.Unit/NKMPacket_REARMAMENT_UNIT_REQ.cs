using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_REARMAMENT_UNIT_REQ)]
public sealed class NKMPacket_REARMAMENT_UNIT_REQ : ISerializable
{
	public long unitUid;

	public int rearmamentId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUid);
		stream.PutOrGet(ref rearmamentId);
	}
}
