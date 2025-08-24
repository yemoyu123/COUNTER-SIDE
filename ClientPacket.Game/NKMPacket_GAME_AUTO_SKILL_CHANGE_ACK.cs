using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_AUTO_SKILL_CHANGE_ACK)]
public sealed class NKMPacket_GAME_AUTO_SKILL_CHANGE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKM_GAME_AUTO_SKILL_TYPE gameAutoSkillType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGetEnum(ref gameAutoSkillType);
	}
}
