using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Raid;

[PacketId(ClientPacketId.kNKMPacket_RAID_POINT_EXTRA_REWARD_ACK)]
public sealed class NKMPacket_RAID_POINT_EXTRA_REWARD_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMRewardData rewardData;

	public NKMRaidSeason raidSeason = new NKMRaidSeason();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref raidSeason);
	}
}
