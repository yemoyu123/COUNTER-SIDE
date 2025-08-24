using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_SHIP_SKILL_ACK)]
public sealed class NKMPacket_GAME_SHIP_SKILL_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public short gameUnitUID;

	public int shipSkillID;

	public float skillPosX;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref gameUnitUID);
		stream.PutOrGet(ref shipSkillID);
		stream.PutOrGet(ref skillPosX);
	}
}
