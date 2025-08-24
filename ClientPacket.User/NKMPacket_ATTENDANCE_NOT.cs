using System.Collections.Generic;
using Cs.Protocol;
using NKM;
using Protocol;

namespace ClientPacket.User;

[PacketId(ClientPacketId.kNKMPacket_ATTENDANCE_NOT)]
public sealed class NKMPacket_ATTENDANCE_NOT : ISerializable
{
	public NKM_ERROR_CODE errorCode;

	public long lastUpdateDate;

	public List<NKMAttendance> attendanceData = new List<NKMAttendance>();

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGetEnum(ref errorCode);
		stream.PutOrGet(ref lastUpdateDate);
		stream.PutOrGet(ref attendanceData);
	}
}
