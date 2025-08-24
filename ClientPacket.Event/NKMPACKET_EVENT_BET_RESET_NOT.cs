using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPACKET_EVENT_BET_RESET_NOT)]
public sealed class NKMPACKET_EVENT_BET_RESET_NOT : ISerializable
{
	public int eventId;

	public int eventIndex;

	public NKMEventBetSummary summary = new NKMEventBetSummary();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref eventId);
		stream.PutOrGet(ref eventIndex);
		stream.PutOrGet(ref summary);
	}
}
