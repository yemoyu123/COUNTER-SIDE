using System;
using Cs.Protocol;
using NKM.Templet.Office;

namespace ClientPacket.Common;

public sealed class UnitLoyaltyUpdateData : ISerializable
{
	public long unitUid;

	public int loyalty;

	public int officeRoomId;

	public OfficeGrade officeGrade;

	public DateTime heartGaugeStartTime;

	void ISerializable.Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref unitUid);
		stream.PutOrGet(ref loyalty);
		stream.PutOrGet(ref officeRoomId);
		stream.PutOrGetEnum(ref officeGrade);
		stream.PutOrGet(ref heartGaugeStartTime);
	}
}
