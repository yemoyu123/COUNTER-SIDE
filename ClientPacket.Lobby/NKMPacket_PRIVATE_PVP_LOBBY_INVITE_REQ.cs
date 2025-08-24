using Cs.Protocol;
using Protocol;

namespace ClientPacket.Lobby;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_LOBBY_INVITE_REQ)]
public sealed class NKMPacket_PRIVATE_PVP_LOBBY_INVITE_REQ : ISerializable
{
	public long friendCode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref friendCode);
	}
}
