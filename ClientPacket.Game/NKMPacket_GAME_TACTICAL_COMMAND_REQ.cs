using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_TACTICAL_COMMAND_REQ)]
public sealed class NKMPacket_GAME_TACTICAL_COMMAND_REQ : ISerializable
{
	public int TCID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref TCID);
	}
}
