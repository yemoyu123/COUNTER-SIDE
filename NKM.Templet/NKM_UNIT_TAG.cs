using Cs.GameLog.CountryDescription;

namespace NKM.Templet;

public enum NKM_UNIT_TAG : short
{
	[CountryDescription("패트롤", CountryCode.KOR)]
	NUT_PATROL,
	[CountryDescription("스윙바이", CountryCode.KOR)]
	NUT_SWINGBY,
	[CountryDescription("전진출격", CountryCode.KOR)]
	NUT_RESPAWN_FREE_POS,
	[CountryDescription("반격", CountryCode.KOR)]
	NUT_REVENGE,
	[CountryDescription("퓨리", CountryCode.KOR)]
	NUT_FURY,
	[CountryDescription("다중 유닛", CountryCode.KOR)]
	NUT_MULTI_UNIT,
	[CountryDescription("일회성", CountryCode.KOR)]
	NUT_LIMIT_1,
	NUT_LIMIT_2,
	NUT_LIMIT_3
}
