using Cs.GameLog.CountryDescription;

namespace NKM;

public enum NKM_DUNGEON_END_TYPE
{
	[CountryDescription("정상종료", CountryCode.KOR)]
	NORMAL,
	[CountryDescription("포기", CountryCode.KOR)]
	GIVE_UP,
	[CountryDescription("타임아웃", CountryCode.KOR)]
	TIME_OUT,
	[CountryDescription("연결종료", CountryCode.KOR)]
	DISCONNECT
}
