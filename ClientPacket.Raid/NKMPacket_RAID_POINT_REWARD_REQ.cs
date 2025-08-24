using Cs.Protocol;
using Protocol;

namespace ClientPacket.Raid;

[PacketId(ClientPacketId.kNKMPacket_RAID_POINT_REWARD_REQ)]
public sealed class NKMPacket_RAID_POINT_REWARD_REQ : ISerializable
{
	public int raidPointReward;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref raidPointReward);
	}
}
