using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_DUPLICATED_CONNECTED_NOT)]
public sealed class NKMPacket_DUPLICATED_CONNECTED_NOT : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
