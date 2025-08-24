using Cs.Protocol;
using Protocol;

namespace ClientPacket.Mode;

[PacketId(ClientPacketId.kNKMPacket_DIVE_GIVE_UP_REQ)]
public sealed class NKMPacket_DIVE_GIVE_UP_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
