using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_DRAFT_PVP_PICK_SHIP_REQ)]
public sealed class NKMPacket_DRAFT_PVP_PICK_SHIP_REQ : ISerializable
{
	public long shipUid;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref shipUid);
	}
}
