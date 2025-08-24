using Cs.GameLog.CountryDescription;

namespace NKM.Templet;

public enum NKM_UNIT_ROLE_TYPE : short
{
	[CountryDescription("미분류", CountryCode.KOR)]
	NURT_INVALID,
	[CountryDescription("스트라이커", CountryCode.KOR)]
	NURT_STRIKER,
	[CountryDescription("레인저", CountryCode.KOR)]
	NURT_RANGER,
	[CountryDescription("디펜더", CountryCode.KOR)]
	NURT_DEFENDER,
	[CountryDescription("스나이퍼", CountryCode.KOR)]
	NURT_SNIPER,
	[CountryDescription("서포터", CountryCode.KOR)]
	NURT_SUPPORTER,
	[CountryDescription("시즈", CountryCode.KOR)]
	NURT_SIEGE,
	[CountryDescription("타워", CountryCode.KOR)]
	NURT_TOWER
}
