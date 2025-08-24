using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_UNIT_RETREAT_REQ)]
public sealed class NKMPacket_GAME_UNIT_RETREAT_REQ : ISerializable
{
	public long unitUID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUID);
	}
}
