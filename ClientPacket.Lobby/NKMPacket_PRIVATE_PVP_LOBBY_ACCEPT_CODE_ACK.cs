using ClientPacket.Pvp;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Lobby;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_CODE_ACK)]
public sealed class NKMPacket_PRIVATE_PVP_LOBBY_ACCEPT_CODE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public PrivatePvpCancelType cancelType;

	public string serverIp;

	public int port;

	public string accessToken;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGetEnum(ref cancelType);
		stream.PutOrGet(ref serverIp);
		stream.PutOrGet(ref port);
		stream.PutOrGet(ref accessToken);
	}
}
