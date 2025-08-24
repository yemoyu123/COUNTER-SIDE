using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Defence;

[PacketId(ClientPacketId.kNKMPacket_DEFENCE_GAME_GIVE_UP_ACK)]
public sealed class NKMPacket_DEFENCE_GAME_GIVE_UP_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
	}
}
