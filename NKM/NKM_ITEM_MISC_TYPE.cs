using Cs.GameLog.CountryDescription;

namespace NKM;

public enum NKM_ITEM_MISC_TYPE : short
{
	[CountryDescription("일반아이템", CountryCode.KOR)]
	IMT_MISC,
	[CountryDescription("패키지아이템", CountryCode.KOR)]
	IMT_PACKAGE,
	[CountryDescription("랜덤박스아이템", CountryCode.KOR)]
	IMT_RANDOMBOX,
	[CountryDescription("자원아이템", CountryCode.KOR)]
	IMT_RESOURCE,
	[CountryDescription("엠블렘", CountryCode.KOR)]
	IMT_EMBLEM,
	[CountryDescription("랭킹엠블렘", CountryCode.KOR)]
	IMT_EMBLEM_RANK,
	[CountryDescription("뷰 전용 아이템", CountryCode.KOR)]
	IMT_VIEW,
	[CountryDescription("유닛 선택권", CountryCode.KOR)]
	IMT_CHOICE_UNIT,
	[CountryDescription("함선 선택권", CountryCode.KOR)]
	IMT_CHOICE_SHIP,
	[CountryDescription("장비 선택권", CountryCode.KOR)]
	IMT_CHOICE_EQUIP,
	[CountryDescription("비품 선택권", CountryCode.KOR)]
	IMT_CHOICE_MISC,
	[CountryDescription("장비 금형 선택권", CountryCode.KOR)]
	IMT_CHOICE_MOLD,
	[CountryDescription("오퍼레이터 선택권", CountryCode.KOR)]
	IMT_CHOICE_OPERATOR,
	[CountryDescription("유닛 조각", CountryCode.KOR)]
	IMT_PIECE,
	[CountryDescription("로비 배경화면", CountryCode.KOR)]
	IMT_BACKGROUND,
	[CountryDescription("프로필 테두리", CountryCode.KOR)]
	IMT_SELFIE_FRAME,
	[CountryDescription("커스텀 패키지", CountryCode.KOR)]
	IMT_CUSTOM_PACKAGE,
	[CountryDescription("채용 아이템", CountryCode.KOR)]
	IMT_CONTRACT,
	[CountryDescription("가구", CountryCode.KOR)]
	IMT_INTERIOR,
	[CountryDescription("가구 선택권", CountryCode.KOR)]
	IMT_CHOICE_FURNITURE,
	[CountryDescription("스킨 선택권", CountryCode.KOR)]
	IMT_CHOICE_SKIN,
	[CountryDescription("칭호", CountryCode.KOR)]
	IMT_TITLE
}
