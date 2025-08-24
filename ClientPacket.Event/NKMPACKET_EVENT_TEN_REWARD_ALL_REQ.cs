using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPACKET_EVENT_TEN_REWARD_ALL_REQ)]
public sealed class NKMPACKET_EVENT_TEN_REWARD_ALL_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
