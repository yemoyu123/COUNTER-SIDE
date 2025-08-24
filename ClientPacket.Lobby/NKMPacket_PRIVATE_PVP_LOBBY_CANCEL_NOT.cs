using ClientPacket.Pvp;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Lobby;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_LOBBY_CANCEL_NOT)]
public sealed class NKMPacket_PRIVATE_PVP_LOBBY_CANCEL_NOT : ISerializable
{
	public long targetUserUid;

	public PrivatePvpCancelType cancelType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref targetUserUid);
		stream.PutOrGetEnum(ref cancelType);
	}
}
