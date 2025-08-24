using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_REARMAMENT_UNIT_ACK)]
public sealed class NKMPacket_REARMAMENT_UNIT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMUnitData rearmamentUnitData;

	public List<NKMItemMiscData> costItems = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref rearmamentUnitData);
		stream.PutOrGet(ref costItems);
	}
}
