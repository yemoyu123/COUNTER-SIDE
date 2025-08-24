using Cs.Protocol;
using Protocol;

namespace ClientPacket.Lobby;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_LOBBY_STATE_NOT)]
public sealed class NKMPacket_PRIVATE_PVP_LOBBY_STATE_NOT : ISerializable
{
	public NKMLobbyData lobbyData = new NKMLobbyData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref lobbyData);
	}
}
