using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPACKET_EVENT_BET_RESULT_REQ)]
public sealed class NKMPACKET_EVENT_BET_RESULT_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
