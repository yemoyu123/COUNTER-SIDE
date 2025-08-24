using System.Collections.Generic;
using ClientPacket.Raid;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Game;

[PacketId(ClientPacketId.kNKMPacket_RAID_SWEEP_ACK)]
public sealed class NKMPacket_RAID_SWEEP_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long raidUid;

	public NKMRaidBossResultData raidResultData = new NKMRaidBossResultData();

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	public NKMRaidDetailData raidDetailData = new NKMRaidDetailData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref raidUid);
		stream.PutOrGet(ref raidResultData);
		stream.PutOrGet(ref costItemDataList);
		stream.PutOrGet(ref raidDetailData);
	}
}
