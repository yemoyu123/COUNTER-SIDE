using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_BSIDE_ACCOUNT_LINK_SELECT_USERDATA_REQ)]
public sealed class NKMPacket_BSIDE_ACCOUNT_LINK_SELECT_USERDATA_REQ : ISerializable
{
	public long selectedUserUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref selectedUserUid);
	}
}
