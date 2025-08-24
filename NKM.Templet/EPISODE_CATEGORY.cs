using Cs.GameLog.CountryDescription;

namespace NKM.Templet;

public enum EPISODE_CATEGORY
{
	[CountryDescription("메인스트림", CountryCode.KOR)]
	EC_MAINSTREAM,
	[CountryDescription("모의작전", CountryCode.KOR)]
	EC_DAILY,
	[CountryDescription("카운터케이스", CountryCode.KOR)]
	EC_COUNTERCASE,
	[CountryDescription("외전", CountryCode.KOR)]
	EC_SIDESTORY,
	[CountryDescription("자유 계약", CountryCode.KOR)]
	EC_FIELD,
	[CountryDescription("이벤트 에피소드", CountryCode.KOR)]
	EC_EVENT,
	[CountryDescription("보급작전", CountryCode.KOR)]
	EC_SUPPLY,
	[CountryDescription("챌린지", CountryCode.KOR)]
	EC_CHALLENGE,
	[CountryDescription("타임어택", CountryCode.KOR)]
	EC_TIMEATTACK,
	[CountryDescription("디멘션 트리밍(작전 구분용)", CountryCode.KOR)]
	EC_TRIM,
	[CountryDescription("격전지원(작전 구분용)", CountryCode.KOR)]
	EC_FIERCE,
	[CountryDescription("그림자전당(작전 구분용)", CountryCode.KOR)]
	EC_SHADOW,
	[CountryDescription("시즈널", CountryCode.KOR)]
	EC_SEASONAL,
	[CountryDescription("EC_COUNT", CountryCode.KOR)]
	EC_COUNT
}
