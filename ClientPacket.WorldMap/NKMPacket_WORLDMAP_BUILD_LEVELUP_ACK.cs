using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.WorldMap;

[PacketId(ClientPacketId.kNKMPacket_WORLDMAP_BUILD_LEVELUP_ACK)]
public sealed class NKMPacket_WORLDMAP_BUILD_LEVELUP_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int cityID;

	public NKMWorldmapCityBuildingData worldMapCityBuildingData = new NKMWorldmapCityBuildingData();

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref cityID);
		stream.PutOrGet(ref worldMapCityBuildingData);
		stream.PutOrGet(ref costItemDataList);
	}
}
