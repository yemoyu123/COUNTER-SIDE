using Cs.Protocol;
using Protocol;

namespace ClientPacket.Unit;

[PacketId(ClientPacketId.kNKMPacket_OPERATOR_ENHANCE_REQ)]
public sealed class NKMPacket_OPERATOR_ENHANCE_REQ : ISerializable
{
	public long targetUnitUid;

	public long sourceUnitUid;

	public int tokenItemId;

	public bool transSkill;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref targetUnitUid);
		stream.PutOrGet(ref sourceUnitUid);
		stream.PutOrGet(ref tokenItemId);
		stream.PutOrGet(ref transSkill);
	}
}
