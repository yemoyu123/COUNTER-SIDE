using Cs.GameLog.CountryDescription;

namespace NKM;

public enum NKM_GAME_STATE : byte
{
	NGS_INVALID,
	[CountryDescription("던전 미참여", CountryCode.KOR)]
	NGS_STOP,
	[CountryDescription("던전 플레이 시작", CountryCode.KOR)]
	NGS_START,
	[CountryDescription("던전 플레이 중", CountryCode.KOR)]
	NGS_PLAY,
	[CountryDescription("던전 플레이 종료", CountryCode.KOR)]
	NGS_FINISH,
	[CountryDescription("던전 종료", CountryCode.KOR)]
	NGS_END,
	[CountryDescription("방생성-파티매칭", CountryCode.KOR)]
	NGS_LOBBY_MATCHING,
	[CountryDescription("방생성-게임편성", CountryCode.KOR)]
	NGS_LOBBY_GAME_SETTING
}
