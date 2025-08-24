using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_SHIP_BUILD_ACK)]
public sealed class NKMPacket_SHIP_BUILD_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMUnitData shipData;

	public List<NKMItemMiscData> costItemList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref shipData);
		stream.PutOrGet(ref costItemList);
	}
}
