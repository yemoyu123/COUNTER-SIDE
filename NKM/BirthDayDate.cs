using Cs.Protocol;

namespace NKM;

public struct BirthDayDate : ISerializable
{
	public int Month;

	public int Day;

	public BirthDayDate(int month, int day)
	{
		Month = month;
		Day = day;
	}

	public void Serialize(IPacketStream stream)
	{
		stream.PutOrGet(ref Month);
		stream.PutOrGet(ref Day);
	}
}
