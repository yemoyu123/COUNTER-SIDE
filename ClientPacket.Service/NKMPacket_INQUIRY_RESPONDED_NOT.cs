using Cs.Protocol;
using Protocol;

namespace ClientPacket.Service;

[PacketId(ClientPacketId.kNKMPacket_INQUIRY_RESPONDED_NOT)]
public sealed class NKMPacket_INQUIRY_RESPONDED_NOT : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
