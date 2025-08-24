using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT)]
public sealed class NKMPacket_BSIDE_ACCOUNT_LINK_CODE_NOT : ISerializable
{
	public NKMAccountLinkUserProfile requestUserProfile = new NKMAccountLinkUserProfile();

	public NKMAccountLinkUserProfile targetUserProfile = new NKMAccountLinkUserProfile();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref requestUserProfile);
		stream.PutOrGet(ref targetUserProfile);
	}
}
