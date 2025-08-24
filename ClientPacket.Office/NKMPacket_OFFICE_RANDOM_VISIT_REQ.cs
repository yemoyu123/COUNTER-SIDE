using Cs.Protocol;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_RANDOM_VISIT_REQ)]
public sealed class NKMPacket_OFFICE_RANDOM_VISIT_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
