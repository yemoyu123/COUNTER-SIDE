using System;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_ATTENDANCE_ACK)]
public sealed class NKMPacket_GUILD_ATTENDANCE_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long guildUid;

	public DateTime lastAttendanceDate;

	public DateTime memberJoinDate;

	public NKMRewardData rewardData;

	public NKMAdditionalReward additionalReward = new NKMAdditionalReward();

	public int yesterdayAttendanceCount;

	public int todayAttendanceCount;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref lastAttendanceDate);
		stream.PutOrGet(ref memberJoinDate);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref additionalReward);
		stream.PutOrGet(ref yesterdayAttendanceCount);
		stream.PutOrGet(ref todayAttendanceCount);
	}
}
