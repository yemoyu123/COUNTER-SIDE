using Cs.Protocol;
using Protocol;

namespace ClientPacket.Service;

[PacketId(ClientPacketId.kNKMPacket_SERVER_TIME_ACK)]
public sealed class NKMPacket_SERVER_TIME_ACK : ISerializable
{
	public long utcServerTimeTicks;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref utcServerTimeTicks);
	}
}
