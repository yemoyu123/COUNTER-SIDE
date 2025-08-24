using Cs.Protocol;
using Protocol;

namespace ClientPacket.WorldMap;

[PacketId(ClientPacketId.kNKMPacket_WORLDMAP_CITY_MISSION_CANCEL_REQ)]
public sealed class NKMPacket_WORLDMAP_CITY_MISSION_CANCEL_REQ : ISerializable
{
	public int cityID;

	public int missionID;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref cityID);
		stream.PutOrGet(ref missionID);
	}
}
