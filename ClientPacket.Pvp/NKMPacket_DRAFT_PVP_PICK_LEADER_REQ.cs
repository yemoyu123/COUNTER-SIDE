using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_DRAFT_PVP_PICK_LEADER_REQ)]
public sealed class NKMPacket_DRAFT_PVP_PICK_LEADER_REQ : ISerializable
{
	public int leaderIndex;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref leaderIndex);
	}
}
