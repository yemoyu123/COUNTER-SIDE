using System;
using Cs.Protocol;

namespace ClientPacket.Event;

public sealed class NKMADItemRewardInfo : ISerializable
{
	public int adItemId;

	public int remainDailyLimit;

	public DateTime latestRewardTime;

	public DateTime latestDailyTime;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref adItemId);
		stream.PutOrGet(ref remainDailyLimit);
		stream.PutOrGet(ref latestRewardTime);
		stream.PutOrGet(ref latestDailyTime);
	}
}
