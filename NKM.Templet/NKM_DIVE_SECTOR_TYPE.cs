using Cs.GameLog.CountryDescription;

namespace NKM.Templet;

public enum NKM_DIVE_SECTOR_TYPE
{
	[CountryDescription("알수없음", CountryCode.KOR)]
	NDST_SECTOR_NONE,
	[CountryDescription("시작섹터", CountryCode.KOR)]
	NDST_SECTOR_START,
	[CountryDescription("침식코어", CountryCode.KOR)]
	NDST_SECTOR_BOSS,
	[CountryDescription("침식코어(어려움)", CountryCode.KOR)]
	NDST_SECTOR_BOSS_HARD,
	[CountryDescription("푸엥카레", CountryCode.KOR)]
	NDST_SECTOR_POINCARE,
	[CountryDescription("푸엥카레(어려움)", CountryCode.KOR)]
	NDST_SECTOR_POINCARE_HARD,
	[CountryDescription("리만", CountryCode.KOR)]
	NDST_SECTOR_REIMANN,
	[CountryDescription("리만(어려움)", CountryCode.KOR)]
	NDST_SECTOR_REIMANN_HARD,
	[CountryDescription("건틀렛", CountryCode.KOR)]
	NDST_SECTOR_GAUNTLET,
	[CountryDescription("건틀렛(어려움)", CountryCode.KOR)]
	NDST_SECTOR_GAUNTLET_HARD,
	[CountryDescription("유클리드", CountryCode.KOR)]
	NDST_SECTOR_EUCLID,
	[CountryDescription("유클리드(어려움)", CountryCode.KOR)]
	NDST_SECTOR_EUCLID_HARD
}
