using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_AUTO_SKILL_CHANGE_REQ)]
public sealed class NKMPacket_GAME_AUTO_SKILL_CHANGE_REQ : ISerializable
{
	public NKM_GAME_AUTO_SKILL_TYPE gameAutoSkillType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref gameAutoSkillType);
	}
}
