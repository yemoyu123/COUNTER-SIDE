using Cs.Protocol;
using Protocol;

namespace ClientPacket.Office;

[PacketId(ClientPacketId.kNKMPacket_OFFICE_POST_RECV_REQ)]
public sealed class NKMPacket_OFFICE_POST_RECV_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
