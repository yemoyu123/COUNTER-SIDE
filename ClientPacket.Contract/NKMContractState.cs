using System;
using System.Collections.Generic;
using Cs.Protocol;

namespace ClientPacket.Contract;

public sealed class NKMContractState : ISerializable
{
	public int contractId;

	public int remainFreeChance;

	public DateTime nextResetDate;

	public bool isActive;

	public int totalUseCount;

	public int dailyUseCount;

	public List<int> bonusCandidate = new List<int>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref contractId);
		stream.PutOrGet(ref remainFreeChance);
		stream.PutOrGet(ref nextResetDate);
		stream.PutOrGet(ref isActive);
		stream.PutOrGet(ref totalUseCount);
		stream.PutOrGet(ref dailyUseCount);
		stream.PutOrGet(ref bonusCandidate);
	}
}
