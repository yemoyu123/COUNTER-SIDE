using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_EXCHANGE_PIECE_TO_UNIT_REQ)]
public sealed class NKMPacket_EXCHANGE_PIECE_TO_UNIT_REQ : ISerializable
{
	public int itemId;

	public int count;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref itemId);
		stream.PutOrGet(ref count);
	}
}
