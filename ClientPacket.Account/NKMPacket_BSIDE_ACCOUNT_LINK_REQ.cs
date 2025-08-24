using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_BSIDE_ACCOUNT_LINK_REQ)]
public sealed class NKMPacket_BSIDE_ACCOUNT_LINK_REQ : ISerializable
{
	public long friendCode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref friendCode);
	}
}
