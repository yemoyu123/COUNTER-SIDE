using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPACKET_RACE_START_REQ)]
public sealed class NKMPACKET_RACE_START_REQ : ISerializable
{
	public int selectLine;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref selectLine);
	}
}
