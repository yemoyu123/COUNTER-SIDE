using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_SERVICE_TRANSFER_CODE_COPY_REWARD_REQ)]
public sealed class NKMPacket_SERVICE_TRANSFER_CODE_COPY_REWARD_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
