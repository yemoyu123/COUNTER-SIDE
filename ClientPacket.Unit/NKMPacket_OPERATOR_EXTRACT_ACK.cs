using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_OPERATOR_EXTRACT_ACK)]
public sealed class NKMPacket_OPERATOR_EXTRACT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<long> extractUnitUids = new List<long>();

	public List<NKMItemMiscData> costItemDatas = new List<NKMItemMiscData>();

	public List<NKMItemMiscData> rewardItemDatas = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref extractUnitUids);
		stream.PutOrGet(ref costItemDatas);
		stream.PutOrGet(ref rewardItemDatas);
	}
}
