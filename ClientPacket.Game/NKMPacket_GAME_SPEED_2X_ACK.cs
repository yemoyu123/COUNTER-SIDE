using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_SPEED_2X_ACK)]
public sealed class NKMPacket_GAME_SPEED_2X_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKM_GAME_SPEED_TYPE gameSpeedType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGetEnum(ref gameSpeedType);
	}
}
