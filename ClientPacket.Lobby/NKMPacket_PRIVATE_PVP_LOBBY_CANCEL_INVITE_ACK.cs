using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Lobby;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_LOBBY_CANCEL_INVITE_ACK)]
public sealed class NKMPacket_PRIVATE_PVP_LOBBY_CANCEL_INVITE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long targetUserUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref targetUserUid);
	}
}
