using Cs.GameLog.CountryDescription;

namespace NKM;

public enum NKM_INVENTORY_EXPAND_TYPE
{
	[CountryDescription("일반인벤", CountryCode.KOR)]
	NIET_NONE,
	[CountryDescription("장비인벤", CountryCode.KOR)]
	NIET_EQUIP,
	[CountryDescription("유닛인벤", CountryCode.KOR)]
	NIET_UNIT,
	[CountryDescription("함선인벤", CountryCode.KOR)]
	NIET_SHIP,
	[CountryDescription("오퍼레이터인벤", CountryCode.KOR)]
	NIET_OPERATOR,
	[CountryDescription("트로피인벤", CountryCode.KOR)]
	NIET_TROPHY,
	[CountryDescription("알수없음", CountryCode.KOR)]
	NEIT_MAX
}
