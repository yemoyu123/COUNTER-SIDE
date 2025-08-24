using Cs.GameLog.CountryDescription;

namespace NKM;

public enum NKM_ITEM_PAYMENT_TYPE : byte
{
	[CountryDescription("무료", CountryCode.KOR)]
	NIPT_FREE,
	[CountryDescription("유료", CountryCode.KOR)]
	NIPT_PAID,
	[CountryDescription("무료,유료", CountryCode.KOR)]
	NIPT_BOTH
}
