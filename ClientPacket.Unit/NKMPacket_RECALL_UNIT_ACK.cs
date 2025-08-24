using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_RECALL_UNIT_ACK)]
public sealed class NKMPacket_RECALL_UNIT_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long removeUnitUid;

	public List<NKMUnitData> exchangeUnitDatas = new List<NKMUnitData>();

	public RecallHistoryInfo historyInfo;

	public List<NKMItemMiscData> rewardList = new List<NKMItemMiscData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref removeUnitUid);
		stream.PutOrGet(ref exchangeUnitDatas);
		stream.PutOrGet(ref historyInfo);
		stream.PutOrGet(ref rewardList);
	}
}
