using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Negotiation;

[PacketId(ClientPacketId.kNKMPacket_NEGOTIATE_ACK)]
public sealed class NKMPacket_NEGOTIATE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NEGOTIATE_RESULT negotiateResult;

	public int finalSalary;

	public long targetUnitUid;

	public int targetUnitLevel;

	public int targetUnitLoyalty;

	public int targetUnitExp;

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGetEnum(ref negotiateResult);
		stream.PutOrGet(ref finalSalary);
		stream.PutOrGet(ref targetUnitUid);
		stream.PutOrGet(ref targetUnitLevel);
		stream.PutOrGet(ref targetUnitLoyalty);
		stream.PutOrGet(ref targetUnitExp);
		stream.PutOrGet(ref costItemDataList);
	}
}
