using Cs.GameLog.CountryDescription;

namespace NKM;

public enum NKM_MISSION_RESET_INTERVAL
{
	[CountryDescription("\ufffdϷ\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd", CountryCode.KOR)]
	ALWAYS,
	[CountryDescription("\ufffd\ufffd\ufffd\ufffd \ufffdݺ\ufffd\ufffd\u033c\ufffd", CountryCode.KOR)]
	DAILY,
	[CountryDescription("\ufffd\u05b0\ufffd \ufffdݺ\ufffd\ufffd\u033c\ufffd", CountryCode.KOR)]
	WEEKLY,
	[CountryDescription("\ufffd\ufffd\ufffd\ufffd \ufffdݺ\ufffd\ufffd\u033c\ufffd", CountryCode.KOR)]
	MONTHLY,
	[CountryDescription("\ufffdϷ\ufffd üũ \ufffd\ufffd", CountryCode.KOR)]
	ON_COMPLETE,
	[CountryDescription("\ufffdݺ\ufffd\ufffd\u05b1\ufffd \ufffd\ufffd\ufffd\ufffd", CountryCode.KOR)]
	NONE
}
