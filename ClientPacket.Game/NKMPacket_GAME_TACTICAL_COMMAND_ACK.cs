using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_GAME_TACTICAL_COMMAND_ACK)]
public sealed class NKMPacket_GAME_TACTICAL_COMMAND_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMTacticalCommandData cTacticalCommandData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref cTacticalCommandData);
	}
}
