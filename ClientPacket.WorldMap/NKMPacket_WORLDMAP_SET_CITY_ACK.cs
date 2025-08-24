using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.WorldMap;

[PacketId(ClientPacketId.kNKMPacket_WORLDMAP_SET_CITY_ACK)]
public sealed class NKMPacket_WORLDMAP_SET_CITY_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMWorldMapCityData worldMapCityData = new NKMWorldMapCityData();

	public NKMItemMiscData costItemData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref worldMapCityData);
		stream.PutOrGet(ref costItemData);
	}
}
