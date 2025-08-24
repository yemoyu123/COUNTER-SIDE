using Cs.Protocol;
using Protocol;

namespace ClientPacket.Raid;

[PacketId(ClientPacketId.kNKMPacket_RAID_COOP_LIST_REQ)]
public sealed class NKMPacket_RAID_COOP_LIST_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
