using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_DECK_UNLOCK_ACK)]
public sealed class NKMPacket_DECK_UNLOCK_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKM_DECK_TYPE deckType;

	public byte unlockedDeckSize;

	public NKMItemMiscData costItemData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGetEnum(ref deckType);
		stream.PutOrGet(ref unlockedDeckSize);
		stream.PutOrGet(ref costItemData);
	}
}
