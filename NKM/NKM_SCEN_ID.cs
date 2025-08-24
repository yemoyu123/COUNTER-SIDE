using Cs.GameLog.CountryDescription;

namespace NKM;

public enum NKM_SCEN_ID
{
	[CountryDescription("알수없음", CountryCode.KOR)]
	NSI_INVALID,
	[CountryDescription("로그인", CountryCode.KOR)]
	NSI_LOGIN,
	[CountryDescription("로비", CountryCode.KOR)]
	NSI_HOME,
	[CountryDescription("게임", CountryCode.KOR)]
	NSI_GAME,
	[CountryDescription("편성", CountryCode.KOR)]
	NSI_TEAM,
	[CountryDescription("본부", CountryCode.KOR)]
	NSI_BASE,
	[CountryDescription("채용", CountryCode.KOR)]
	NSI_CONTRACT,
	[CountryDescription("관리부", CountryCode.KOR)]
	NSI_INVENTORY,
	[CountryDescription("컷신 시뮬레이터", CountryCode.KOR)]
	NSI_CUTSCENE_SIM,
	[CountryDescription("작전", CountryCode.KOR)]
	NSI_OPERATION,
	[CountryDescription("에피소드", CountryCode.KOR)]
	NSI_EPISODE,
	[CountryDescription("전투대기", CountryCode.KOR)]
	NSI_DUNGEON_ATK_READY,
	[CountryDescription("유닛(함선) 리스트", CountryCode.KOR)]
	NSI_UNIT_LIST,
	[CountryDescription("도감", CountryCode.KOR)]
	NSI_COLLECTION,
	[CountryDescription("전역전투", CountryCode.KOR)]
	NSI_WARFARE_GAME,
	[CountryDescription("상점", CountryCode.KOR)]
	NSI_SHOP,
	[CountryDescription("친구", CountryCode.KOR)]
	NSI_FRIEND,
	[CountryDescription("월드맵", CountryCode.KOR)]
	NSI_WORLDMAP,
	[CountryDescription("컷신던전", CountryCode.KOR)]
	NSI_CUTSCENE_DUNGEON,
	[CountryDescription("게임결과", CountryCode.KOR)]
	NSI_GAME_RESULT,
	[CountryDescription("다이브준비", CountryCode.KOR)]
	NSI_DIVE_READY,
	[CountryDescription("다이브", CountryCode.KOR)]
	NSI_DIVE,
	[CountryDescription("다이브결과", CountryCode.KOR)]
	NSI_DIVE_RESULT,
	[CountryDescription("PVP 인트로", CountryCode.KOR)]
	NSI_GAUNTLET_INTRO,
	[CountryDescription("PVP 로비", CountryCode.KOR)]
	NSI_GAUNTLET_LOBBY,
	[CountryDescription("PVP 매치대기", CountryCode.KOR)]
	NSI_GAUNTLET_MATCH_READY,
	[CountryDescription("PVP 매치", CountryCode.KOR)]
	NSI_GAUNTLET_MATCH,
	[CountryDescription("PVP 전략전 준비", CountryCode.KOR)]
	NSI_GAUNTLET_ASYNC_READY,
	[CountryDescription("레이드", CountryCode.KOR)]
	NSI_RAID,
	[CountryDescription("레이드 준비", CountryCode.KOR)]
	NSI_RAID_READY,
	[CountryDescription("보이스리스트", CountryCode.KOR)]
	NSI_VOICE_LIST,
	[CountryDescription("그림자 전당", CountryCode.KOR)]
	NSI_SHADOW_PALACE,
	[CountryDescription("그림자 전당 미션", CountryCode.KOR)]
	NSI_SHADOW_BATTLE,
	[CountryDescription("그림자 전당 결과", CountryCode.KOR)]
	NSI_SHADOW_RESULT,
	[CountryDescription("컨소시움 인트로", CountryCode.KOR)]
	NSI_GUILD_INTRO,
	[CountryDescription("컨소시움 로비", CountryCode.KOR)]
	NSI_GUILD_LOBBY,
	[CountryDescription("격전 지원", CountryCode.KOR)]
	NSI_FIERCE_BATTLE_SUPPORT,
	[CountryDescription("길드 협력전", CountryCode.KOR)]
	NSI_GUILD_COOP,
	[CountryDescription("사옥", CountryCode.KOR)]
	NSI_OFFICE,
	[CountryDescription("PVP 리그전 룸", CountryCode.KOR)]
	NSI_GAUNTLET_LEAGUE_ROOM,
	[CountryDescription("PVP 친선전 룸", CountryCode.KOR)]
	NSI_GAUNTLET_PRIVATE_ROOM,
	[CountryDescription("디멘션 트리밍", CountryCode.KOR)]
	NSI_TRIM,
	[CountryDescription("디멘션 트리밍 결과", CountryCode.KOR)]
	NSI_TRIM_RESULT,
	[CountryDescription("던전 결과", CountryCode.KOR)]
	NSI_DUNGEON_RESULT,
	[CountryDescription("PVP 이벤트전 준비", CountryCode.KOR)]
	NSI_GAUNTLET_EVENT_READY
}
