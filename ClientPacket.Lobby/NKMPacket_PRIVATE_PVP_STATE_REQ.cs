using ClientPacket.Pvp;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Lobby;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_STATE_REQ)]
public sealed class NKMPacket_PRIVATE_PVP_STATE_REQ : ISerializable
{
	public LobbyPlayerState changeState;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref changeState);
	}
}
