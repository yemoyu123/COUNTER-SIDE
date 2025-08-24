using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_OPERATOR_ENHANCE_ACK)]
public sealed class NKMPacket_OPERATOR_ENHANCE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMOperator operatorUnit;

	public List<NKMItemMiscData> costItemDatas = new List<NKMItemMiscData>();

	public long sourceUnitUid;

	public bool transSkill;

	public int tokenItemId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref operatorUnit);
		stream.PutOrGet(ref costItemDatas);
		stream.PutOrGet(ref sourceUnitUid);
		stream.PutOrGet(ref transSkill);
		stream.PutOrGet(ref tokenItemId);
	}
}
