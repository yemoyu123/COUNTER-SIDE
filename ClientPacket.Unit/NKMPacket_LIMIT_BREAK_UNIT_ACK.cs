using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_LIMIT_BREAK_UNIT_ACK)]
public sealed class NKMPacket_LIMIT_BREAK_UNIT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMUnitData unitData;

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref unitData);
		stream.PutOrGet(ref costItemDataList);
	}
}
