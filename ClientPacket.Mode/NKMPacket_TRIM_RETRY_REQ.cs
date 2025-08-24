using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_TRIM_RETRY_REQ)]
public sealed class NKMPacket_TRIM_RETRY_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
