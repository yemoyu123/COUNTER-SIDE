using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_SHIP_UPGRADE_ACK)]
public sealed class NKMPacket_SHIP_UPGRADE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMUnitData shipUnitData;

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref shipUnitData);
		stream.PutOrGet(ref costItemDataList);
	}
}
