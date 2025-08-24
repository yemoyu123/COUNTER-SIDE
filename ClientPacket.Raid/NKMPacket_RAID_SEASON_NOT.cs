using Cs.Protocol;
using Protocol;

namespace ClientPacket.Raid;

[PacketId(ClientPacketId.kNKMPacket_RAID_SEASON_NOT)]
public sealed class NKMPacket_RAID_SEASON_NOT : ISerializable
{
	public NKMRaidSeason raidSeason = new NKMRaidSeason();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref raidSeason);
	}
}
