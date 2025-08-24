using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_AUTO_RESPAWN_REQ)]
public sealed class NKMPacket_GAME_AUTO_RESPAWN_REQ : ISerializable
{
	public bool isAutoRespawn;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref isAutoRespawn);
	}
}
