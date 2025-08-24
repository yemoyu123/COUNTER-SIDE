using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_DECK_UNIT_SWAP_ACK)]
public sealed class NKMPacket_DECK_UNIT_SWAP_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMDeckIndex deckIndex;

	public sbyte leaderSlotIndex = -1;

	public byte slotIndexFrom;

	public byte slotIndexTo;

	public long slotUnitUIDFrom;

	public long slotUnitUIDTo;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref deckIndex);
		stream.PutOrGet(ref leaderSlotIndex);
		stream.PutOrGet(ref slotIndexFrom);
		stream.PutOrGet(ref slotIndexTo);
		stream.PutOrGet(ref slotUnitUIDFrom);
		stream.PutOrGet(ref slotUnitUIDTo);
	}
}
