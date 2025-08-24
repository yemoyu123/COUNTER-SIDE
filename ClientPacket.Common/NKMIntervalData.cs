using System;
using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMIntervalData : ISerializable
{
	public int key;

	public string strKey;

	public DateTime startDate;

	public DateTime endDate;

	public int repeatStartDate;

	public int repeatEndDate;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref key);
		stream.PutOrGet(ref strKey);
		stream.PutOrGet(ref startDate);
		stream.PutOrGet(ref endDate);
		stream.PutOrGet(ref repeatStartDate);
		stream.PutOrGet(ref repeatEndDate);
	}
}
