using System;
using Cs.Protocol;

namespace ClientPacket.Common;

public sealed class NKMReturningUserState : ISerializable
{
	public ReturningUserType type;

	public DateTime startDate;

	public DateTime endDate;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref type);
		stream.PutOrGet(ref startDate);
		stream.PutOrGet(ref endDate);
	}
}
