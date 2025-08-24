using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_DECK_OPERATOR_SET_ACK)]
public sealed class NKMPacket_DECK_OPERATOR_SET_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMDeckIndex deckIndex;

	public long operatorUid;

	public NKMDeckIndex oldDeckIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref deckIndex);
		stream.PutOrGet(ref operatorUid);
		stream.PutOrGet(ref oldDeckIndex);
	}
}
