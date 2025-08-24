using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_FIERCE_DATA_REQ)]
public sealed class NKMPacket_FIERCE_DATA_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
