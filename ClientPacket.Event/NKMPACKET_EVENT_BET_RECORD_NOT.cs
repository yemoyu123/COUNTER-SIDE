using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPACKET_EVENT_BET_RECORD_NOT)]
public sealed class NKMPACKET_EVENT_BET_RECORD_NOT : ISerializable
{
	public NKMEventBetRecord record = new NKMEventBetRecord();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref record);
	}
}
