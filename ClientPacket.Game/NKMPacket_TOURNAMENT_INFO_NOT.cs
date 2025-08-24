using Cs.Protocol;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_TOURNAMENT_INFO_NOT)]
public sealed class NKMPacket_TOURNAMENT_INFO_NOT : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
