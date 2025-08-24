using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_DEV_RESPAWN_REQ)]
public sealed class NKMPacket_GAME_DEV_RESPAWN_REQ : ISerializable
{
	public int unitID;

	public int unitLevel;

	public float respawnPosX;

	public NKM_TEAM_TYPE teamType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitID);
		stream.PutOrGet(ref unitLevel);
		stream.PutOrGet(ref respawnPosX);
		stream.PutOrGetEnum(ref teamType);
	}
}
