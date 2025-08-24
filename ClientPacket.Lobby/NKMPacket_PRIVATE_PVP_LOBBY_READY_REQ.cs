using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Lobby;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_LOBBY_READY_REQ)]
public sealed class NKMPacket_PRIVATE_PVP_LOBBY_READY_REQ : ISerializable
{
	public NKMDeckIndex deckIndex;

	public bool isReady;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref deckIndex);
		stream.PutOrGet(ref isReady);
	}
}
