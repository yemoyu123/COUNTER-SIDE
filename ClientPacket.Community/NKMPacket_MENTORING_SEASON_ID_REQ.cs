using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_MENTORING_SEASON_ID_REQ)]
public sealed class NKMPacket_MENTORING_SEASON_ID_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
