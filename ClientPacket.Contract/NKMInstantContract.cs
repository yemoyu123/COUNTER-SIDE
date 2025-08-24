using System;
using Cs.Protocol;

namespace ClientPacket.Contract;

public sealed class NKMInstantContract : ISerializable
{
	public int contractId;

	public DateTime endDate;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref contractId);
		stream.PutOrGet(ref endDate);
	}
}
