using Cs.GameLog.CountryDescription;

namespace NKM.Templet;

public enum WARFARE_GAME_CONDITION
{
	[CountryDescription("알수없음", CountryCode.KOR)]
	WFC_NONE,
	[CountryDescription("보스처치", CountryCode.KOR)]
	WFC_KILL_BOSS,
	[CountryDescription("모두제거", CountryCode.KOR)]
	WFC_KILL_ALL,
	[CountryDescription("타겟제거", CountryCode.KOR)]
	WFC_KILL_TARGET,
	[CountryDescription("제거횟수", CountryCode.KOR)]
	WFC_KILL_COUNT,
	[CountryDescription("타일도착", CountryCode.KOR)]
	WFC_TILE_ENTER,
	[CountryDescription("페이즈제한", CountryCode.KOR)]
	WFC_PHASE,
	[CountryDescription("타일점유", CountryCode.KOR)]
	WFC_PHASE_TILE_HOLD,
	[CountryDescription("", CountryCode.KOR)]
	WFC_ON_DEATH
}
