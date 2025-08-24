using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_SERVICE_TRANSFER_USER_VALIDATION_ACK)]
public sealed class NKMPacket_SERVICE_TRANSFER_USER_VALIDATION_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
	}
}
