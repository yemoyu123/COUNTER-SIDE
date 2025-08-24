using Cs.Protocol;
using Protocol;

namespace ClientPacket.Service;

[PacketId(ClientPacketId.kNKMPacket_ACCOUNT_KICK_NOT)]
public sealed class NKMPacket_ACCOUNT_KICK_NOT : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
