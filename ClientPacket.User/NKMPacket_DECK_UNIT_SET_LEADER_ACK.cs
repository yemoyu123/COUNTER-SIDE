using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_DECK_UNIT_SET_LEADER_ACK)]
public sealed class NKMPacket_DECK_UNIT_SET_LEADER_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMDeckIndex deckIndex;

	public sbyte leaderSlotIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref deckIndex);
		stream.PutOrGet(ref leaderSlotIndex);
	}
}
