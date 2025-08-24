using Cs.Protocol;
using Protocol;

namespace ClientPacket.Lobby;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_CODE_REQ)]
public sealed class NKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_CODE_REQ : ISerializable
{
	public string code;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref code);
	}
}
