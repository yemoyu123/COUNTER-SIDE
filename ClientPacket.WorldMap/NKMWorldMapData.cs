using System;
using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.WorldMap;

public sealed class NKMWorldMapData : ISerializable
{
	public Dictionary<int, NKMWorldMapCityData> worldMapCityDataMap = new Dictionary<int, NKMWorldMapCityData>();

	public DateTime collectLastResetDate;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref worldMapCityDataMap);
		stream.PutOrGet(ref collectLastResetDate);
	}
}
