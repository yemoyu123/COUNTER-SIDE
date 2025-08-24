using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_BSIDE_ACCOUNT_LINK_SUCCESS_NOT)]
public sealed class NKMPacket_BSIDE_ACCOUNT_LINK_SUCCESS_NOT : ISerializable
{
	public NKMAccountLinkUserProfile selectedUserProfile = new NKMAccountLinkUserProfile();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref selectedUserProfile);
	}
}
