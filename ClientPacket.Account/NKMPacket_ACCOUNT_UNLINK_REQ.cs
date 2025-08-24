using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_ACCOUNT_UNLINK_REQ)]
public sealed class NKMPacket_ACCOUNT_UNLINK_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
