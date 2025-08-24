using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_NPC_PVP_TARGET_LIST_REQ)]
public sealed class NKMPacket_NPC_PVP_TARGET_LIST_REQ : ISerializable
{
	public int targetTier;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref targetTier);
	}
}
