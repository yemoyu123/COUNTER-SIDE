using ClientPacket.Common;
using ClientPacket.Pvp;
using Cs.Protocol;
using Protocol;

namespace ClientPacket.Lobby;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_LOBBY_INVITE_NOT)]
public sealed class NKMPacket_PRIVATE_PVP_LOBBY_INVITE_NOT : ISerializable
{
	public NKMUserProfileData senderProfile = new NKMUserProfileData();

	public int timeoutDurationSec;

	public NKMPrivateGameConfig config = new NKMPrivateGameConfig();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref senderProfile);
		stream.PutOrGet(ref timeoutDurationSec);
		stream.PutOrGet(ref config);
	}
}
