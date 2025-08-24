using Cs.Protocol;
using Protocol;

namespace ClientPacket.Negotiation;

[PacketId(ClientPacketId.kNKMPacket_NEGOTIATE_NOT_USED_02)]
public sealed class NKMPacket_NEGOTIATE_NOT_USED_02 : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
