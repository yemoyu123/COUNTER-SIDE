using Cs.GameLog.CountryDescription;

namespace NKM.Templet;

public enum NKM_DIVE_STAGE_TYPE
{
	[CountryDescription("보통", CountryCode.KOR)]
	NDST_NORMAL,
	[CountryDescription("어려움", CountryCode.KOR)]
	NDST_HARD,
	[CountryDescription("스캐빈저", CountryCode.KOR)]
	NDST_SCAVENGER
}
