using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_MENTORING_DATA_REQ)]
public sealed class NKMPacket_MENTORING_DATA_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
