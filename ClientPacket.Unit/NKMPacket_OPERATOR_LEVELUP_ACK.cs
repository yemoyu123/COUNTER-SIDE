using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_OPERATOR_LEVELUP_ACK)]
public sealed class NKMPacket_OPERATOR_LEVELUP_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMItemMiscData> costItemData = new List<NKMItemMiscData>();

	public NKMOperator operatorUnit;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref costItemData);
		stream.PutOrGet(ref operatorUnit);
	}
}
