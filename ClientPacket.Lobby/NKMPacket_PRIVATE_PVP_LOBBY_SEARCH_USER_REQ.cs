using Cs.Protocol;
using Protocol;

namespace ClientPacket.Lobby;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_LOBBY_SEARCH_USER_REQ)]
public sealed class NKMPacket_PRIVATE_PVP_LOBBY_SEARCH_USER_REQ : ISerializable
{
	public string searchKeyword;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref searchKeyword);
	}
}
