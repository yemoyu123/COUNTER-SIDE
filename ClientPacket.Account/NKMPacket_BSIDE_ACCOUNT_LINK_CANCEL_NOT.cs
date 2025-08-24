using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_BSIDE_ACCOUNT_LINK_CANCEL_NOT)]
public sealed class NKMPacket_BSIDE_ACCOUNT_LINK_CANCEL_NOT : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
