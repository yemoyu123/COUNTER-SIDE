using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_DECK_UNIT_SWAP_REQ)]
public sealed class NKMPacket_DECK_UNIT_SWAP_REQ : ISerializable
{
	public NKMDeckIndex deckIndex;

	public byte slotIndexFrom;

	public byte slotIndexTo;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref deckIndex);
		stream.PutOrGet(ref slotIndexFrom);
		stream.PutOrGet(ref slotIndexTo);
	}
}
