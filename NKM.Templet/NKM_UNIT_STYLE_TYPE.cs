using Cs.GameLog.CountryDescription;

namespace NKM.Templet;

public enum NKM_UNIT_STYLE_TYPE : short
{
	[CountryDescription("알수없음", CountryCode.KOR)]
	NUST_INVALID,
	[CountryDescription("카운터", CountryCode.KOR)]
	NUST_COUNTER,
	[CountryDescription("솔져", CountryCode.KOR)]
	NUST_SOLDIER,
	[CountryDescription("메카닉", CountryCode.KOR)]
	NUST_MECHANIC,
	[CountryDescription("침식체", CountryCode.KOR)]
	NUST_CORRUPTED,
	[CountryDescription("리플레이서", CountryCode.KOR)]
	NUST_REPLACER,
	[CountryDescription("재료전용유닛", CountryCode.KOR)]
	NUST_TRAINER,
	[CountryDescription("강습함", CountryCode.KOR)]
	NUST_SHIP_ASSAULT,
	[CountryDescription("중장함", CountryCode.KOR)]
	NUST_SHIP_HEAVY,
	[CountryDescription("순양함", CountryCode.KOR)]
	NUST_SHIP_CRUISER,
	[CountryDescription("특무함", CountryCode.KOR)]
	NUST_SHIP_SPECIAL,
	[CountryDescription("초계기", CountryCode.KOR)]
	NUST_SHIP_PATROL,
	[CountryDescription("미분류", CountryCode.KOR)]
	NUST_SHIP_ETC,
	[CountryDescription("환경유닛", CountryCode.KOR)]
	NUST_ENV,
	[CountryDescription("기타", CountryCode.KOR)]
	NUST_ETC,
	[CountryDescription("강화 모듈", CountryCode.KOR)]
	NUST_ENCHANT,
	[CountryDescription("오퍼레이터", CountryCode.KOR)]
	NUST_OPERATOR
}
