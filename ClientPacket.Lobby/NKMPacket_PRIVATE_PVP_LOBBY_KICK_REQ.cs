using Cs.Protocol;
using Protocol;

namespace ClientPacket.Lobby;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_LOBBY_KICK_REQ)]
public sealed class NKMPacket_PRIVATE_PVP_LOBBY_KICK_REQ : ISerializable
{
	public long targetUserUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref targetUserUid);
	}
}
