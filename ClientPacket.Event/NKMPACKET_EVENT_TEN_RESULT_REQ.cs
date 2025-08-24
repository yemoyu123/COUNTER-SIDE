using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPACKET_EVENT_TEN_RESULT_REQ)]
public sealed class NKMPACKET_EVENT_TEN_RESULT_REQ : ISerializable
{
	public int newScore;

	public int newRemainTime;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref newScore);
		stream.PutOrGet(ref newRemainTime);
	}
}
