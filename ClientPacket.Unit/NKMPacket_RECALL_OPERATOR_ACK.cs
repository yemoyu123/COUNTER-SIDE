using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_RECALL_OPERATOR_ACK)]
public sealed class NKMPacket_RECALL_OPERATOR_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long removeUnitUid;

	public NKMOperator exchangeOperatorData;

	public RecallHistoryInfo historyInfo;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref removeUnitUid);
		stream.PutOrGet(ref exchangeOperatorData);
		stream.PutOrGet(ref historyInfo);
	}
}
