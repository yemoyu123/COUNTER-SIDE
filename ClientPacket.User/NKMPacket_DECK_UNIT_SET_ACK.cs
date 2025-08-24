using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_DECK_UNIT_SET_ACK)]
public sealed class NKMPacket_DECK_UNIT_SET_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMDeckIndex deckIndex;

	public byte slotIndex;

	public long slotUnitUID;

	public NKMDeckIndex oldDeckIndex;

	public sbyte oldSlotIndex;

	public sbyte leaderSlotIndex;

	public sbyte oldLeaderSlotIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref deckIndex);
		stream.PutOrGet(ref slotIndex);
		stream.PutOrGet(ref slotUnitUID);
		stream.PutOrGet(ref oldDeckIndex);
		stream.PutOrGet(ref oldSlotIndex);
		stream.PutOrGet(ref leaderSlotIndex);
		stream.PutOrGet(ref oldLeaderSlotIndex);
	}
}
