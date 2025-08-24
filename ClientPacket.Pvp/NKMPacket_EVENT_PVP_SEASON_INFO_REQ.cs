using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_EVENT_PVP_SEASON_INFO_REQ)]
public sealed class NKMPacket_EVENT_PVP_SEASON_INFO_REQ : ISerializable
{
	public int seasonId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref seasonId);
	}
}
