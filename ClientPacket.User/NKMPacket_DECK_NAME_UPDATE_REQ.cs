using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_DECK_NAME_UPDATE_REQ)]
public sealed class NKMPacket_DECK_NAME_UPDATE_REQ : ISerializable
{
	public NKMDeckIndex deckIndex;

	public string name;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref deckIndex);
		stream.PutOrGet(ref name);
	}
}
