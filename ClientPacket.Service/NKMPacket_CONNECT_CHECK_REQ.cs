using Cs.Protocol;
using Protocol;

namespace ClientPacket.Service;

[PacketId(ClientPacketId.kNKMPacket_CONNECT_CHECK_REQ)]
public sealed class NKMPacket_CONNECT_CHECK_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
