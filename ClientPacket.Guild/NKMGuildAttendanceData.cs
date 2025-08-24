using System;
using Cs.Protocol;

namespace ClientPacket.Guild;

public sealed class NKMGuildAttendanceData : ISerializable
{
	public DateTime date;

	public int count;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref date);
		stream.PutOrGet(ref count);
	}
}
