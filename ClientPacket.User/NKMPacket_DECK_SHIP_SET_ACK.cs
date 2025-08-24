using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_DECK_SHIP_SET_ACK)]
public sealed class NKMPacket_DECK_SHIP_SET_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMDeckIndex deckIndex;

	public NKMDeckIndex oldDeckIndex;

	public long shipUID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref deckIndex);
		stream.PutOrGet(ref oldDeckIndex);
		stream.PutOrGet(ref shipUID);
	}
}
