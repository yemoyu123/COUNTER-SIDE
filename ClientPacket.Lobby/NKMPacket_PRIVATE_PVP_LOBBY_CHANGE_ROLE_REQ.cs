using ClientPacket.Pvp;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Lobby;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_LOBBY_CHANGE_ROLE_REQ)]
public sealed class NKMPacket_PRIVATE_PVP_LOBBY_CHANGE_ROLE_REQ : ISerializable
{
	public long targetUserUid;

	public PvpPlayerRole changeRole;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref targetUserUid);
		stream.PutOrGetEnum(ref changeRole);
	}
}
