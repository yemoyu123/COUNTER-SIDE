using Cs.GameLog.CountryDescription;

namespace NKM.Templet;

public enum NKM_UNIT_GRADE : short
{
	[CountryDescription("N등급", CountryCode.KOR)]
	NUG_N,
	[CountryDescription("R등급", CountryCode.KOR)]
	NUG_R,
	[CountryDescription("SR등급", CountryCode.KOR)]
	NUG_SR,
	[CountryDescription("SSR등급", CountryCode.KOR)]
	NUG_SSR,
	NUG_COUNT
}
