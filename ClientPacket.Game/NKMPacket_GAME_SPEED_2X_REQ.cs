using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_SPEED_2X_REQ)]
public sealed class NKMPacket_GAME_SPEED_2X_REQ : ISerializable
{
	public NKM_GAME_SPEED_TYPE gameSpeedType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref gameSpeedType);
	}
}
