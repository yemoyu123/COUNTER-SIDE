using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_DECK_OPERATOR_SET_REQ)]
public sealed class NKMPacket_DECK_OPERATOR_SET_REQ : ISerializable
{
	public NKMDeckIndex deckIndex;

	public long operatorUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref deckIndex);
		stream.PutOrGet(ref operatorUid);
	}
}
