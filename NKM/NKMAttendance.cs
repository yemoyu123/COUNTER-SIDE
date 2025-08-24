using System;
using Cs.Protocol;

namespace NKM;

public class NKMAttendance : ISerializable
{
	public int IDX;

	public int Count;

	public DateTime EventEndDate;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref IDX);
		stream.PutOrGet(ref Count);
		stream.PutOrGet(ref EventEndDate);
	}
}
