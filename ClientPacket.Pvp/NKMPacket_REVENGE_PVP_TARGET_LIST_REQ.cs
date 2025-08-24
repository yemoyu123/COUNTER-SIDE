using Cs.Protocol;
using Protocol;

namespace ClientPacket.Pvp;

[PacketId(ClientPacketId.kNKMPacket_REVENGE_PVP_TARGET_LIST_REQ)]
public sealed class NKMPacket_REVENGE_PVP_TARGET_LIST_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
