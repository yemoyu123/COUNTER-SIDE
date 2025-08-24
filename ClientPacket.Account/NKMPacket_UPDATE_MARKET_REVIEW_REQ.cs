using Cs.Protocol;
using Protocol;

namespace ClientPacket.Account;

[PacketId(ClientPacketId.kNKMPacket_UPDATE_MARKET_REVIEW_REQ)]
public sealed class NKMPacket_UPDATE_MARKET_REVIEW_REQ : ISerializable
{
	public string description;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref description);
	}
}
