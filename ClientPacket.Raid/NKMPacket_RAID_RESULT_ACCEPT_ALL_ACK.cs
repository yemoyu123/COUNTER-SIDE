using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Raid;

[PacketId(ClientPacketId.kNKMPacket_RAID_RESULT_ACCEPT_ALL_ACK)]
public sealed class NKMPacket_RAID_RESULT_ACCEPT_ALL_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public List<long> raidUids = new List<long>();

	public NKMRewardData rewardData;

	public int rewardRaidPoint;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref raidUids);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref rewardRaidPoint);
	}
}
