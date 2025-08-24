using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_DECK_UNLOCK_REQ)]
public sealed class NKMPacket_DECK_UNLOCK_REQ : ISerializable
{
	public NKM_DECK_TYPE deckType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref deckType);
	}
}
