using System;
using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class RecallHistoryInfo : ISerializable
{
	public int unitId;

	public DateTime lastUpdateDate;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitId);
		stream.PutOrGet(ref lastUpdateDate);
	}
}
