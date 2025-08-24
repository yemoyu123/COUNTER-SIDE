using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_FAVORITES_STAGE_REQ)]
public sealed class NKMPacket_FAVORITES_STAGE_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
