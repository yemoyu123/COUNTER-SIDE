using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.WorldMap;

[PacketId(ClientPacketId.kNKMPacket_WORLDMAP_CITY_MISSION_ACK)]
public sealed class NKMPacket_WORLDMAP_CITY_MISSION_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int cityID;

	public int missionID;

	public long completeTime;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref cityID);
		stream.PutOrGet(ref missionID);
		stream.PutOrGet(ref completeTime);
	}
}
