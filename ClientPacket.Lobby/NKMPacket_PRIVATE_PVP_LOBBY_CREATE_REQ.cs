using ClientPacket.Pvp;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Lobby;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_LOBBY_CREATE_REQ)]
public sealed class NKMPacket_PRIVATE_PVP_LOBBY_CREATE_REQ : ISerializable
{
	public bool isObserverMode;

	public long inviteFriendCode;

	public NKMPrivateGameConfig config = new NKMPrivateGameConfig();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref isObserverMode);
		stream.PutOrGet(ref inviteFriendCode);
		stream.PutOrGet(ref config);
	}
}
