using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_PVP_GAME_MATCH_FAIL_NOT)]
public sealed class NKMPacket_PVP_GAME_MATCH_FAIL_NOT : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
	}
}
