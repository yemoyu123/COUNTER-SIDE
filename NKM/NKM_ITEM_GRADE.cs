using Cs.GameLog.CountryDescription;

namespace NKM;

public enum NKM_ITEM_GRADE : short
{
	[CountryDescription("N등급", CountryCode.KOR)]
	NIG_N,
	[CountryDescription("R등급", CountryCode.KOR)]
	NIG_R,
	[CountryDescription("SR등급", CountryCode.KOR)]
	NIG_SR,
	[CountryDescription("SSR등급", CountryCode.KOR)]
	NIG_SSR,
	NIG_COUNT
}
