using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_SERVICE_TRANSFER_CODE_VALIDATION_ACK)]
public sealed class NKMPacket_SERVICE_TRANSFER_CODE_VALIDATION_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMAccountLinkUserProfile userProfile = new NKMAccountLinkUserProfile();

	public int failCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref userProfile);
		stream.PutOrGet(ref failCount);
	}
}
