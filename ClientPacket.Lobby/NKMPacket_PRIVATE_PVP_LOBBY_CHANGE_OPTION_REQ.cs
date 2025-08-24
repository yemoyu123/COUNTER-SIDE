using ClientPacket.Pvp;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Lobby;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_LOBBY_CHANGE_OPTION_REQ)]
public sealed class NKMPacket_PRIVATE_PVP_LOBBY_CHANGE_OPTION_REQ : ISerializable
{
	public NKMPrivateGameConfig newConfig = new NKMPrivateGameConfig();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref newConfig);
	}
}
