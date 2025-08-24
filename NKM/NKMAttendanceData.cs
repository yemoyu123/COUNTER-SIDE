using System;
using System.Collections.Generic;
using Cs.Protocol;

namespace NKM;

public class NKMAttendanceData : ISerializable
{
	public DateTime LastUpdateDate;

	public List<NKMAttendance> AttList = new List<NKMAttendance>();

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref LastUpdateDate);
		stream.PutOrGet(ref AttList);
	}
}
