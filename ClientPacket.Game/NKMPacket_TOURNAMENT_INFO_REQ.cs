using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_TOURNAMENT_INFO_REQ)]
public sealed class NKMPacket_TOURNAMENT_INFO_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
