using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_BSIDE_ACCOUNT_LINK_NOT)]
public sealed class NKMPacket_BSIDE_ACCOUNT_LINK_NOT : ISerializable
{
	public string linkCode;

	public NKMAccountLinkUserProfile requestUserProfile = new NKMAccountLinkUserProfile();

	public float remainingTime;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref linkCode);
		stream.PutOrGet(ref requestUserProfile);
		stream.PutOrGet(ref remainingTime);
	}
}
