using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_AUTO_RESPAWN_ACK)]
public sealed class NKMPacket_GAME_AUTO_RESPAWN_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public bool isAutoRespawn = true;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref isAutoRespawn);
	}
}
