using System;
using Cs.Protocol;

namespace ClientPacket.Raid;

public sealed class NKMRaidSeason : ISerializable
{
	public int seasonId;

	public int monthlyPoint;

	public int tryAssistCount;

	public int recvRewardRaidPoint;

	public float highestDamage;

	public DateTime latestUpdateTime;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref seasonId);
		stream.PutOrGet(ref monthlyPoint);
		stream.PutOrGet(ref tryAssistCount);
		stream.PutOrGet(ref recvRewardRaidPoint);
		stream.PutOrGet(ref highestDamage);
		stream.PutOrGet(ref latestUpdateTime);
	}
}
