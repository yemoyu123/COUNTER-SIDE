using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Lobby;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_LOBBY_CREATE_ACK)]
public sealed class NKMPacket_PRIVATE_PVP_LOBBY_CREATE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMLobbyData lobbyState = new NKMLobbyData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref lobbyState);
	}
}
