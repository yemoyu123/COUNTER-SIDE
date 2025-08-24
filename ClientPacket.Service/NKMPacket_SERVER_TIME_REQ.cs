using Cs.Protocol;
using Protocol;

namespace ClientPacket.Service;

[PacketId(ClientPacketId.kNKMPacket_SERVER_TIME_REQ)]
public sealed class NKMPacket_SERVER_TIME_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
