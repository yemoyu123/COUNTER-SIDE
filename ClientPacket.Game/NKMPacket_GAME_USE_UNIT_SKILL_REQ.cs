using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_USE_UNIT_SKILL_REQ)]
public sealed class NKMPacket_GAME_USE_UNIT_SKILL_REQ : ISerializable
{
	public short gameUnitUID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref gameUnitUID);
	}
}
