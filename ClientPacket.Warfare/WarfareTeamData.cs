using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.Warfare;

public sealed class WarfareTeamData : ISerializable
{
	public int flagShipWarfareUnitUID;

	public Dictionary<int, WarfareUnitData> warfareUnitDataByUIDMap = new Dictionary<int, WarfareUnitData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref flagShipWarfareUnitUID);
		stream.PutOrGet(ref warfareUnitDataByUIDMap);
	}
}
