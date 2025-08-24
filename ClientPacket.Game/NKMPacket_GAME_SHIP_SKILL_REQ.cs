using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_SHIP_SKILL_REQ)]
public sealed class NKMPacket_GAME_SHIP_SKILL_REQ : ISerializable
{
	public short gameUnitUID;

	public int shipSkillID;

	public float skillPosX;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref gameUnitUID);
		stream.PutOrGet(ref shipSkillID);
		stream.PutOrGet(ref skillPosX);
	}
}
