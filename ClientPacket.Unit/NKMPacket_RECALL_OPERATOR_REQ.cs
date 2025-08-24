using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_RECALL_OPERATOR_REQ)]
public sealed class NKMPacket_RECALL_OPERATOR_REQ : ISerializable
{
	public long recallOperatorUid;

	public int exchangeOperatorId;

	public int exchangeSubSkillId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref recallOperatorUid);
		stream.PutOrGet(ref exchangeOperatorId);
		stream.PutOrGet(ref exchangeSubSkillId);
	}
}
