using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_DRAFT_PVP_PICK_OPERATOR_REQ)]
public sealed class NKMPacket_DRAFT_PVP_PICK_OPERATOR_REQ : ISerializable
{
	public long operatorUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref operatorUid);
	}
}
