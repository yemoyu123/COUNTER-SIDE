using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_RESPAWN_REQ)]
public sealed class NKMPacket_GAME_RESPAWN_REQ : ISerializable
{
	public long unitUID;

	public bool assistUnit;

	public float respawnPosX;

	public float gameTime;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUID);
		stream.PutOrGet(ref assistUnit);
		stream.PutOrGet(ref respawnPosX);
		stream.PutOrGet(ref gameTime);
	}
}
