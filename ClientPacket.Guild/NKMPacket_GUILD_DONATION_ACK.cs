using System;
using System.Collections.Generic;
using ClientPacket.Common;
using Cs.Protocol;
using NKM;
using NKM.Templet;
using Protocol;

namespace ClientPacket.Guild;

[PacketId(ClientPacketId.kNKMPacket_GUILD_DONATION_ACK)]
public sealed class NKMPacket_GUILD_DONATION_ACK : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public int donationId;

	public List<NKMItemMiscData> costItemDataList = new List<NKMItemMiscData>();

	public NKMRewardData rewardData;

	public NKMAdditionalReward additionalReward = new NKMAdditionalReward();

	public int donationCount;

	public DateTime lastDailyResetDate;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref donationId);
		stream.PutOrGet(ref costItemDataList);
		stream.PutOrGet(ref rewardData);
		stream.PutOrGet(ref additionalReward);
		stream.PutOrGet(ref donationCount);
		stream.PutOrGet(ref lastDailyResetDate);
	}
}
