using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_SERVICE_TRANSFER_USER_VALIDATION_REQ)]
public sealed class NKMPacket_SERVICE_TRANSFER_USER_VALIDATION_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
