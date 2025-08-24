using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_PAUSE_REQ)]
public sealed class NKMPacket_GAME_PAUSE_REQ : ISerializable
{
	public bool isPause;

	public bool isPauseEvent;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref isPause);
		stream.PutOrGet(ref isPauseEvent);
	}
}
