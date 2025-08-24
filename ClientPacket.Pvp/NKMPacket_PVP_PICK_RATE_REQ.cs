using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_PVP_PICK_RATE_REQ)]
public sealed class NKMPacket_PVP_PICK_RATE_REQ : ISerializable
{
	public NKM_GAME_TYPE gameType;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref gameType);
	}
}
