using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_LIMIT_BREAK_SHIP_ACK)]
public sealed class NKMPacket_LIMIT_BREAK_SHIP_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMUnitData shipData;

	public long consumeUnitUid;

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref shipData);
		stream.PutOrGet(ref consumeUnitUid);
		stream.PutOrGet(ref costItemDataList);
	}
}
