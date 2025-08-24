using Cs.Protocol;
using Protocol;

namespace ClientPacket.Raid;

[PacketId(ClientPacketId.kNKMPacket_RAID_POINT_EXTRA_REWARD_REQ)]
public sealed class NKMPacket_RAID_POINT_EXTRA_REWARD_REQ : ISerializable
{
	void ISerializable.Serialize(IPacketStream stream)
	{
	}
}
