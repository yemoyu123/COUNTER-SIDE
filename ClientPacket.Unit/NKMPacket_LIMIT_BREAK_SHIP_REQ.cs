using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_LIMIT_BREAK_SHIP_REQ)]
public sealed class NKMPacket_LIMIT_BREAK_SHIP_REQ : ISerializable
{
	public long shipUid;

	public long consumeShipUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref shipUid);
		stream.PutOrGet(ref consumeShipUid);
	}
}
