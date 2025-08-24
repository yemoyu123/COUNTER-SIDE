using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_PAUSE_ACK)]
public sealed class NKMPacket_GAME_PAUSE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public bool isPause;

	public bool isPauseEvent;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref isPause);
		stream.PutOrGet(ref isPauseEvent);
	}
}
