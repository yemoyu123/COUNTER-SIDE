using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPACKET_EVENT_BET_BETTING_REQ)]
public sealed class NKMPACKET_EVENT_BET_BETTING_REQ : ISerializable
{
	public int betCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref betCount);
	}
}
