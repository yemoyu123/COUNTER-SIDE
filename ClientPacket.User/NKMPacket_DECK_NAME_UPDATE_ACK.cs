using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_DECK_NAME_UPDATE_ACK)]
public sealed class NKMPacket_DECK_NAME_UPDATE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMDeckIndex deckIndex;

	public string name;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref deckIndex);
		stream.PutOrGet(ref name);
	}
}
