using Cs.Protocol;
using Protocol;

namespace ClientPacket.Event;

[PacketId(ClientPacketId.kNKMPacket_EVENT_PASS_NOT)]
public sealed class NKMPacket_EVENT_PASS_NOT : ISerializable
{
	public int eventPassId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref eventPassId);
	}
}
