using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_BSIDE_ACCOUNT_LINK_CODE_REQ)]
public sealed class NKMPacket_BSIDE_ACCOUNT_LINK_CODE_REQ : ISerializable
{
	public string linkCode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref linkCode);
	}
}
