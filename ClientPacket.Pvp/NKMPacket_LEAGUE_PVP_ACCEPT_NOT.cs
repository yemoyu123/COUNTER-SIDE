using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_LEAGUE_PVP_ACCEPT_NOT)]
public sealed class NKMPacket_LEAGUE_PVP_ACCEPT_NOT : ISerializable
{
	public DraftPvpRoomData roomData = new DraftPvpRoomData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref roomData);
	}
}
