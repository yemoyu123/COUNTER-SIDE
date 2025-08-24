using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Raid;

[PacketId(ClientPacketId.kNKMPacket_RAID_SET_COOP_ALL_ACK)]
public sealed class NKMPacket_RAID_SET_COOP_ALL_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<NKMRaidDetailData> raidDetailDataList = new List<NKMRaidDetailData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref raidDetailDataList);
	}
}
