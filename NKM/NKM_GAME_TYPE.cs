using Cs.GameLog.CountryDescription;

namespace NKM;

public enum NKM_GAME_TYPE : byte
{
	[CountryDescription("알수없음", CountryCode.KOR)]
	[CountryDescription("沒有內容", CountryCode.TWN)]
	NGT_INVALID,
	[CountryDescription("개발용", CountryCode.KOR)]
	[CountryDescription("研發用", CountryCode.TWN)]
	NGT_DEV,
	[CountryDescription("연습모드", CountryCode.KOR)]
	[CountryDescription("練習模式", CountryCode.TWN)]
	NGT_PRACTICE,
	[CountryDescription("일반던전", CountryCode.KOR)]
	[CountryDescription("一般副本", CountryCode.TWN)]
	NGT_DUNGEON,
	[CountryDescription("전역", CountryCode.KOR)]
	[CountryDescription("戰役", CountryCode.TWN)]
	NGT_WARFARE,
	[CountryDescription("다이브", CountryCode.KOR)]
	[CountryDescription("躍入", CountryCode.TWN)]
	NGT_DIVE,
	[CountryDescription("랭크전", CountryCode.KOR)]
	[CountryDescription("排位賽", CountryCode.TWN)]
	NGT_PVP_RANK,
	[CountryDescription("튜토리얼", CountryCode.KOR)]
	[CountryDescription("新手教學", CountryCode.TWN)]
	NGT_TUTORIAL,
	[CountryDescription("레이드", CountryCode.KOR)]
	[CountryDescription("團體副本", CountryCode.TWN)]
	NGT_RAID,
	[CountryDescription("컷신", CountryCode.KOR)]
	[CountryDescription("過場動畫", CountryCode.TWN)]
	NGT_CUTSCENE,
	[CountryDescription("월드맵", CountryCode.KOR)]
	[CountryDescription("世界地圖", CountryCode.TWN)]
	NGT_WORLDMAP,
	[CountryDescription("전략전", CountryCode.KOR)]
	[CountryDescription("戰略對抗戰", CountryCode.TWN)]
	NGT_ASYNC_PVP,
	[CountryDescription("솔로 레이드", CountryCode.KOR)]
	[CountryDescription("單人團體副本", CountryCode.TWN)]
	NGT_RAID_SOLO,
	[CountryDescription("그림자 전당", CountryCode.KOR)]
	[CountryDescription("暗影殿堂", CountryCode.TWN)]
	NGT_SHADOW_PALACE,
	[CountryDescription("격전 지원", CountryCode.KOR)]
	[CountryDescription("激戰支援", CountryCode.TWN)]
	NGT_FIERCE,
	[CountryDescription("페이즈", CountryCode.KOR)]
	[CountryDescription("階段", CountryCode.TWN)]
	NGT_PHASE,
	[CountryDescription("길드 협력전 아레나", CountryCode.KOR)]
	[CountryDescription("(TWN)길드 협력전 아레나", CountryCode.TWN)]
	NGT_GUILD_DUNGEON_ARENA,
	[CountryDescription("길드 협력전 보스", CountryCode.KOR)]
	[CountryDescription("(TWN)길드 협력전 보스", CountryCode.TWN)]
	NGT_GUILD_DUNGEON_BOSS,
	[CountryDescription("친선전", CountryCode.KOR)]
	[CountryDescription("(TWN)친선전", CountryCode.TWN)]
	NGT_PVP_PRIVATE,
	[CountryDescription("리그전", CountryCode.KOR)]
	[CountryDescription("(TWN)리그전", CountryCode.TWN)]
	NGT_PVP_LEAGUE,
	[CountryDescription("전략전 개편", CountryCode.KOR)]
	[CountryDescription("(TWN)전략전 개편", CountryCode.TWN)]
	NGT_PVP_STRATEGY,
	[CountryDescription("전략전 리벤지", CountryCode.KOR)]
	[CountryDescription("(TWN)전략전 리벤지", CountryCode.TWN)]
	NGT_PVP_STRATEGY_REVENGE,
	[CountryDescription("전략전 NPC", CountryCode.KOR)]
	[CountryDescription("(TWN)전략전 NPC", CountryCode.TWN)]
	NGT_PVP_STRATEGY_NPC,
	[CountryDescription("디멘션 트리밍", CountryCode.KOR)]
	[CountryDescription("(TWN)디멘션 트리밍", CountryCode.TWN)]
	NGT_TRIM,
	[CountryDescription("이벤트전", CountryCode.KOR)]
	[CountryDescription("(TWN)이벤트전", CountryCode.TWN)]
	NGT_PVP_EVENT,
	[CountryDescription("길드 협력전 보스 연습전", CountryCode.KOR)]
	[CountryDescription("(TWN)길드 협력전 보스 연습전", CountryCode.TWN)]
	NGT_GUILD_DUNGEON_BOSS_PRACTICE,
	[CountryDescription("무한 디펜스 던전", CountryCode.KOR)]
	[CountryDescription("(TWN)무한 디펜스 던전", CountryCode.TWN)]
	NGT_PVE_DEFENCE,
	[CountryDescription("시뮬레이션 게임", CountryCode.KOR)]
	[CountryDescription("(TWN)시뮬레이션 게임", CountryCode.TWN)]
	NGT_PVE_SIMULATED,
	[CountryDescription("언리미티드 랭크전", CountryCode.KOR)]
	[CountryDescription("(TWN)언리미티드 랭크전", CountryCode.TWN)]
	NGT_PVP_UNLIMITED
}
