using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPACKET_EVENT_TEN_REWARD_REQ)]
public sealed class NKMPACKET_EVENT_TEN_REWARD_REQ : ISerializable
{
	public int rewardId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref rewardId);
	}
}
