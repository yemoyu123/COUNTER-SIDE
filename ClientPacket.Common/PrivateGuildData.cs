using System;
using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class PrivateGuildData : ISerializable
{
	public long guildUid;

	public int donationCount;

	public DateTime lastDailyResetDate;

	public DateTime guildJoinDisableTime;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref guildUid);
		stream.PutOrGet(ref donationCount);
		stream.PutOrGet(ref lastDailyResetDate);
		stream.PutOrGet(ref guildJoinDisableTime);
	}
}
