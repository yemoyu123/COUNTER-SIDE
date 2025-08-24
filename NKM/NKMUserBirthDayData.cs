using System;
using Cs.Protocol;

namespace NKM;

public class NKMUserBirthDayData : ISerializable
{
	public BirthDayDate BirthDay;

	public int Years;

	public DateTime NextRewardResetDate;

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref BirthDay);
		stream.PutOrGet(ref Years);
	}

	public bool IsLeapDay()
	{
		if (BirthDay.Month == 2 && BirthDay.Day == 29)
		{
			return true;
		}
		return false;
	}
}
