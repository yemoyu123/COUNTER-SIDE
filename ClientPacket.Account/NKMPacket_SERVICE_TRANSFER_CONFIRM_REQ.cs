using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_SERVICE_TRANSFER_CONFIRM_REQ)]
public sealed class NKMPacket_SERVICE_TRANSFER_CONFIRM_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
