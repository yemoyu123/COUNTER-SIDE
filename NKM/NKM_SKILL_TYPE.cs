using Cs.GameLog.CountryDescription;

namespace NKM;

public enum NKM_SKILL_TYPE : short
{
	[CountryDescription("알수없음", CountryCode.KOR)]
	NST_INVALID,
	[CountryDescription("패시브", CountryCode.KOR)]
	NST_PASSIVE,
	[CountryDescription("공격스킬", CountryCode.KOR)]
	NST_ATTACK,
	[CountryDescription("액티브", CountryCode.KOR)]
	NST_SKILL,
	[CountryDescription("궁극기", CountryCode.KOR)]
	NST_HYPER,
	[CountryDescription("함선스킬", CountryCode.KOR)]
	NST_SHIP_ACTIVE,
	[CountryDescription("리더스킬", CountryCode.KOR)]
	NST_LEADER
}
