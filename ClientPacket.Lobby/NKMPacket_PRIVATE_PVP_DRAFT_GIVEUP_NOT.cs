using Cs.Protocol;
using Protocol;

namespace ClientPacket.Lobby;

[PacketId(ClientPacketId.kNKMPacket_PRIVATE_PVP_DRAFT_GIVEUP_NOT)]
public sealed class NKMPacket_PRIVATE_PVP_DRAFT_GIVEUP_NOT : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
