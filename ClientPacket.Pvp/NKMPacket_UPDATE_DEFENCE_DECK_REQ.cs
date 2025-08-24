using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_UPDATE_DEFENCE_DECK_REQ)]
public sealed class NKMPacket_UPDATE_DEFENCE_DECK_REQ : ISerializable
{
	public NKMDeckData deckData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref deckData);
	}
}
