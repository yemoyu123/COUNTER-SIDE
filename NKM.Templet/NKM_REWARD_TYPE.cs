using Cs.GameLog.CountryDescription;

namespace NKM.Templet;

public enum NKM_REWARD_TYPE
{
	[CountryDescription("알수없음", CountryCode.KOR)]
	RT_NONE,
	[CountryDescription("유닛", CountryCode.KOR)]
	[CountryDescription("ユニット", CountryCode.JAPAN)]
	RT_UNIT,
	[CountryDescription("함선", CountryCode.KOR)]
	[CountryDescription("艦船", CountryCode.JAPAN)]
	RT_SHIP,
	[CountryDescription("일반아이템", CountryCode.KOR)]
	[CountryDescription("一般アイテム", CountryCode.JAPAN)]
	RT_MISC,
	[CountryDescription("유저경험치", CountryCode.KOR)]
	RT_USER_EXP,
	[CountryDescription("장비", CountryCode.KOR)]
	[CountryDescription("装備", CountryCode.JAPAN)]
	RT_EQUIP,
	[CountryDescription("금형", CountryCode.KOR)]
	[CountryDescription("金型", CountryCode.JAPAN)]
	RT_MOLD,
	[CountryDescription("스킨", CountryCode.KOR)]
	[CountryDescription("スキン", CountryCode.JAPAN)]
	RT_SKIN,
	[CountryDescription("버프아이템", CountryCode.KOR)]
	[CountryDescription("バフアイテム", CountryCode.JAPAN)]
	RT_BUFF,
	[CountryDescription("이모티콘", CountryCode.KOR)]
	[CountryDescription("スタンプ", CountryCode.JAPAN)]
	RT_EMOTICON,
	[CountryDescription("미션 포인트", CountryCode.KOR)]
	[CountryDescription("ミッションポイント", CountryCode.JAPAN)]
	RT_MISSION_POINT,
	[CountryDescription("빙고 타일", CountryCode.KOR)]
	[CountryDescription("ビンゴタイル", CountryCode.JAPAN)]
	RT_BINGO_TILE,
	[CountryDescription("이벤트 패스 경험치", CountryCode.KOR)]
	[CountryDescription("イベントパス経験値", CountryCode.JAPAN)]
	RT_PASS_EXP,
	[CountryDescription("오퍼레이터", CountryCode.KOR)]
	[CountryDescription("オペレーター", CountryCode.JAPAN)]
	RT_OPERATOR
}
