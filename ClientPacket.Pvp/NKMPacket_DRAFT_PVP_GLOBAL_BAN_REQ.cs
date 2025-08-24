using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_DRAFT_PVP_GLOBAL_BAN_REQ)]
public sealed class NKMPacket_DRAFT_PVP_GLOBAL_BAN_REQ : ISerializable
{
	public int unitId;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitId);
	}
}
