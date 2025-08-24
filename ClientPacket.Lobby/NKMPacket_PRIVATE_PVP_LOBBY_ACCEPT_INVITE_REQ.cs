using Cs.Protocol;
using Protocol;

namespace ClientPacket.Lobby;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_INVITE_REQ)]
public sealed class NKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_INVITE_REQ : ISerializable
{
	public long targetUserUid;

	public bool accept;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref targetUserUid);
		stream.PutOrGet(ref accept);
	}
}
