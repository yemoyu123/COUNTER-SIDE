using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_UPDATE_DEFENCE_DECK_ACK)]
public sealed class NKMPacket_UPDATE_DEFENCE_DECK_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMDeckData deckData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref deckData);
	}
}
