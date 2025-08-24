using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_DEV_COOL_TIME_RESET_REQ)]
public sealed class NKMPacket_GAME_DEV_COOL_TIME_RESET_REQ : ISerializable
{
	public NKM_TEAM_TYPE teamType;

	public bool isSkill;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref teamType);
		stream.PutOrGet(ref isSkill);
	}
}
