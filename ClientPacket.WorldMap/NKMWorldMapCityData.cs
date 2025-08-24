using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.WorldMap;

public sealed class NKMWorldMapCityData : ISerializable
{
	public int cityID;

	public long leaderUnitUID;

	public int exp;

	public int level;

	public NKMWorldMapMission worldMapMission = new NKMWorldMapMission();

	public NKMWorldMapEventGroup worldMapEventGroup = new NKMWorldMapEventGroup();

	public Dictionary<int, NKMWorldmapCityBuildingData> worldMapCityBuildingDataMap = new Dictionary<int, NKMWorldmapCityBuildingData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref cityID);
		stream.PutOrGet(ref leaderUnitUID);
		stream.PutOrGet(ref exp);
		stream.PutOrGet(ref level);
		stream.PutOrGet(ref worldMapMission);
		stream.PutOrGet(ref worldMapEventGroup);
		stream.PutOrGet(ref worldMapCityBuildingDataMap);
	}
}
