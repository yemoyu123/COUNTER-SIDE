using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.WorldMap;

[PacketId(ClientPacketId.kNKMPacket_WORLDMAP_MISSION_REFRESH_ACK)]
public sealed class NKMPacket_WORLDMAP_MISSION_REFRESH_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int cityID;

	public List<int> stMissionIDList = new List<int>();

	public NKMItemMiscData costItemData;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref cityID);
		stream.PutOrGet(ref stMissionIDList);
		stream.PutOrGet(ref costItemData);
	}
}
