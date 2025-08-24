using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_ACCOUNT_LINK_REQ)]
public sealed class NKMPacket_ACCOUNT_LINK_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
