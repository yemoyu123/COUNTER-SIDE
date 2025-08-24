using Cs.GameLog.CountryDescription;

namespace NKM;

public enum NKM_ATTENDANCE_EVENT_TYPE
{
	[CountryDescription("\ufffdϹ\ufffd", CountryCode.KOR)]
	NORMAL,
	[CountryDescription("\ufffd\ufffd\ufffd\ufffd", CountryCode.KOR)]
	RETURN,
	[CountryDescription("\ufffdű\ufffd", CountryCode.KOR)]
	NEW
}
