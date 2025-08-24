using Cs.GameLog.CountryDescription;

namespace NKM.Templet;

public enum NKM_UNIT_SOURCE_TYPE : short
{
	[CountryDescription("없음", CountryCode.KOR)]
	NUST_NONE,
	[CountryDescription("투쟁", CountryCode.KOR)]
	NUST_CONFLICT,
	[CountryDescription("안정", CountryCode.KOR)]
	NUST_STABLE,
	[CountryDescription("변화", CountryCode.KOR)]
	NUST_LIBERAL
}
