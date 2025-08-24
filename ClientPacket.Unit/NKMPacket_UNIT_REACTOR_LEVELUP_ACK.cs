using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_UNIT_REACTOR_LEVELUP_ACK)]
public sealed class NKMPacket_UNIT_REACTOR_LEVELUP_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMUnitData unitData;

	public List<NKMItemMiscData> costItemDatas = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref unitData);
		stream.PutOrGet(ref costItemDatas);
	}
}
