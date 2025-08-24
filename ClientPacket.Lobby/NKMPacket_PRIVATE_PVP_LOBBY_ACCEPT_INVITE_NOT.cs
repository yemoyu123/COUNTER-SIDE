using Cs.Protocol;
using Protocol;

namespace ClientPacket.Lobby;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_INVITE_NOT)]
public sealed class NKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_INVITE_NOT : ISerializable
{
	public NKMLobbyData lobbyState = new NKMLobbyData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref lobbyState);
	}
}
