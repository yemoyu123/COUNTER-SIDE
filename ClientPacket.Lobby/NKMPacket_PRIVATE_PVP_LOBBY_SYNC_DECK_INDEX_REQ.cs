using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Lobby;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_LOBBY_SYNC_DECK_INDEX_REQ)]
public sealed class NKMPacket_PRIVATE_PVP_LOBBY_SYNC_DECK_INDEX_REQ : ISerializable
{
	public NKMDeckIndex deckIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref deckIndex);
	}
}
