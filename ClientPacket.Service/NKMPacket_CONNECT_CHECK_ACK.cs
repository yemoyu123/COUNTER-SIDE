using Cs.Protocol;
using Protocol;

namespace ClientPacket.Service;

[PacketId(ClientPacketId.kNKMPacket_CONNECT_CHECK_ACK)]
public sealed class NKMPacket_CONNECT_CHECK_ACK : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
