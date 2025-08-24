using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_BSIDE_ACCOUNT_LINK_ACK)]
public sealed class NKMPacket_BSIDE_ACCOUNT_LINK_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMAccountLinkUserProfile targetUserProfile = new NKMAccountLinkUserProfile();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref targetUserProfile);
	}
}
