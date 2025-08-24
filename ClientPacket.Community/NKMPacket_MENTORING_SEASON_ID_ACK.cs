using Cs.Protocol;
using Protocol;

namespace ClientPacket.Community;

[PacketId(ClientPacketId.kNKMPacket_MENTORING_SEASON_ID_ACK)]
public sealed class NKMPacket_MENTORING_SEASON_ID_ACK : ISerializable
{
	public int seasonId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref seasonId);
	}
}
