using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_DECK_UNIT_SET_REQ)]
public sealed class NKMPacket_DECK_UNIT_SET_REQ : ISerializable
{
	public NKMDeckIndex deckIndex;

	public byte slotIndex;

	public long unitUID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref deckIndex);
		stream.PutOrGet(ref slotIndex);
		stream.PutOrGet(ref unitUID);
	}
}
