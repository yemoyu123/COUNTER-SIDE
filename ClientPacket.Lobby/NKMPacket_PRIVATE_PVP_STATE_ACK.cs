using ClientPacket.Pvp;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Lobby;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_STATE_ACK)]
public sealed class NKMPacket_PRIVATE_PVP_STATE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public LobbyPlayerState playerState;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGetEnum(ref playerState);
	}
}
