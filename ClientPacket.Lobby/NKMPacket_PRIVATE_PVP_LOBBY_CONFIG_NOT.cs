using ClientPacket.Pvp;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Lobby;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_LOBBY_CONFIG_NOT)]
public sealed class NKMPacket_PRIVATE_PVP_LOBBY_CONFIG_NOT : ISerializable
{
	public NKMPrivateGameConfig lobbyConfig = new NKMPrivateGameConfig();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref lobbyConfig);
	}
}
