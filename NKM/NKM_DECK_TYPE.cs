using Cs.GameLog.CountryDescription;

namespace NKM;

public enum NKM_DECK_TYPE : byte
{
	[CountryDescription("소대타입 없음", CountryCode.KOR)]
	NDT_NONE,
	[CountryDescription("전역 소대", CountryCode.KOR)]
	NDT_NORMAL,
	[CountryDescription("경쟁전 소대", CountryCode.KOR)]
	NDT_PVP,
	[CountryDescription("전투 소대", CountryCode.KOR)]
	NDT_DAILY,
	[CountryDescription("레이드 소대", CountryCode.KOR)]
	NDT_RAID,
	[CountryDescription("친구 소대", CountryCode.KOR)]
	NDT_FRIEND,
	[CountryDescription("전략전 방어소대", CountryCode.KOR)]
	NDT_PVP_DEFENCE,
	[CountryDescription("트리밍 던전 소대", CountryCode.KOR)]
	NDT_TRIM,
	[CountryDescription("다이브 소대", CountryCode.KOR)]
	NDT_DIVE,
	[CountryDescription("토너먼트 소대", CountryCode.KOR)]
	NDT_TOURNAMENT
}
