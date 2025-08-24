using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_DRAFT_PVP_SELECT_UNIT_REQ)]
public sealed class NKMPacket_DRAFT_PVP_SELECT_UNIT_REQ : ISerializable
{
	public long unitUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUid);
	}
}
