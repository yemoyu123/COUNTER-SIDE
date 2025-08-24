using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Raid;

[PacketId(ClientPacketId.kNKMPacket_RAID_DETAIL_INFO_ACK)]
public sealed class NKMPacket_RAID_DETAIL_INFO_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public NKMRaidDetailData raidDetailData = new NKMRaidDetailData();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref raidDetailData);
	}
}
