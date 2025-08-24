using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.Raid;

[PacketId(ClientPacketId.kNKMPacket_RAID_SET_COOP_ACK)]
public sealed class NKMPacket_RAID_SET_COOP_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long raidUID;

	public List<NKMRaidJoinData> raidJoinDataList = new List<NKMRaidJoinData>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref raidUID);
		stream.PutOrGet(ref raidJoinDataList);
	}
}
