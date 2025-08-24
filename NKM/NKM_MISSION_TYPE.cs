using Cs.GameLog.CountryDescription;

namespace NKM;

public enum NKM_MISSION_TYPE
{
	NONE,
	[CountryDescription("Ʃ\ufffd丮\ufffd\ufffd", CountryCode.KOR)]
	TUTORIAL,
	[CountryDescription("\ufffd\ufffd\ufffd\ufffd", CountryCode.KOR)]
	ACHIEVE,
	[CountryDescription("\ufffd\ufffd\ufffd\ufffd \ufffdݺ\ufffd\ufffd\u033c\ufffd", CountryCode.KOR)]
	REPEAT_DAILY,
	[CountryDescription("\ufffd\u05b0\ufffd \ufffdݺ\ufffd\ufffd\u033c\ufffd", CountryCode.KOR)]
	REPEAT_WEEKLY,
	[CountryDescription("\ufffd\u033a\ufffdƮ", CountryCode.KOR)]
	EVENT,
	[CountryDescription("\ufffd\ufffd\ufffd\ufffd\u033c\ufffd", CountryCode.KOR)]
	GROWTH,
	[CountryDescription("\ufffd\ufffd\ufffd\ufffd\u033c\ufffd \ufffdǺ\ufffd\ufffd\ufffd", CountryCode.KOR)]
	GROWTH_COMPLETE,
	[CountryDescription("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd", CountryCode.KOR)]
	EMBLEM,
	[CountryDescription("\ufffd\ufffd\ufffd\ufffd \ufffd\ufffd\ufffd\ufffd\u033c\ufffd", CountryCode.KOR)]
	GROWTH_UNIT,
	[CountryDescription("\ufffd\ufffd\ufffd\ufffd\ufffd\u033c\ufffd", CountryCode.KOR)]
	BINGO,
	[CountryDescription("\ufffd\ufffd\ufffd丵", CountryCode.KOR)]
	MENTORING,
	[CountryDescription("\ufffd\ufffd\ufffd", CountryCode.KOR)]
	GUILD,
	[CountryDescription("\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd", CountryCode.KOR)]
	FIERCE,
	[CountryDescription("\ufffd\u033a\ufffdƮ \ufffdн\ufffd", CountryCode.KOR)]
	EVENT_PASS,
	[CountryDescription("\ufffd\ufffd\ufffd\ufffd\u033c\ufffd(\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd\ufffd)", CountryCode.KOR)]
	USER_LEVEL,
	[CountryDescription("\ufffd\ufffd\ufffd\ufffdƮ \ufffd\ufffdȯ", CountryCode.KOR)]
	POINT_EXCHANGE,
	[CountryDescription("\ufffdű\ufffd \ufffd\ufffd\ufffd\u0335\ufffd \ufffd\u033c\ufffd", CountryCode.KOR)]
	COMBINE_GUIDE_MISSION,
	[CountryDescription("Īȣ", CountryCode.KOR)]
	TITLE,
	[CountryDescription("Ʈ\ufffd\ufffd\ufffd\ufffd", CountryCode.KOR)]
	TROPHY,
	MAX
}
