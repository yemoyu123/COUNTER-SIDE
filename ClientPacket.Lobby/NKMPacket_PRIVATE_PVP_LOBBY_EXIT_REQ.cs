using Cs.Protocol;
using Protocol;

namespace ClientPacket.Lobby;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_LOBBY_EXIT_REQ)]
public sealed class NKMPacket_PRIVATE_PVP_LOBBY_EXIT_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
