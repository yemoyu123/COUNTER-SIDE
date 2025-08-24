using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_DECK_UNIT_SET_LEADER_REQ)]
public sealed class NKMPacket_DECK_UNIT_SET_LEADER_REQ : ISerializable
{
	public NKMDeckIndex deckIndex;

	public sbyte leaderSlotIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref deckIndex);
		stream.PutOrGet(ref leaderSlotIndex);
	}
}
