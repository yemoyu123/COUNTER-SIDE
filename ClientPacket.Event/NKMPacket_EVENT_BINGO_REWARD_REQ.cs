using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_BINGO_REWARD_REQ)]
public sealed class NKMPacket_EVENT_BINGO_REWARD_REQ : ISerializable
{
	public int eventId;

	public int rewardIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref eventId);
		stream.PutOrGet(ref rewardIndex);
	}
}
