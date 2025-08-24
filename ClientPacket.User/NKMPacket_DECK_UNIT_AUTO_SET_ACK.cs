using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_DECK_UNIT_AUTO_SET_ACK)]
public sealed class NKMPacket_DECK_UNIT_AUTO_SET_ACK : ISerializable
{
	public NKMDeckIndex deckIndex;

	public NKM_ERROR_CODE errorCode;

	public NKMDeckData deckData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref deckIndex);
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref deckData);
	}
}
