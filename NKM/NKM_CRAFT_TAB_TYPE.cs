using Cs.GameLog.CountryDescription;

namespace NKM;

public enum NKM_CRAFT_TAB_TYPE
{
	[CountryDescription("장비", CountryCode.KOR)]
	MT_EQUIP,
	[CountryDescription("초월 재료", CountryCode.KOR)]
	MT_LIMITBREAK,
	[CountryDescription("이벤트", CountryCode.KOR)]
	MT_MISC_EVENT_1,
	[CountryDescription("이벤트", CountryCode.KOR)]
	MT_MISC_EVENT_2,
	[CountryDescription("이벤트", CountryCode.KOR)]
	MT_MISC_EVENT_3,
	[CountryDescription("비품", CountryCode.KOR)]
	MT_MISC_1,
	[CountryDescription("비품", CountryCode.KOR)]
	MT_MISC_2,
	[CountryDescription("비품", CountryCode.KOR)]
	MT_MISC_3,
	[CountryDescription("전용장비", CountryCode.KOR)]
	MT_EQUIP_PRIVATE,
	[CountryDescription("렐릭장비", CountryCode.KOR)]
	MT_EQUIP_RELIC,
	[CountryDescription("레이드장비", CountryCode.KOR)]
	MT_EQUIP_RAID,
	[CountryDescription("레이드장비", CountryCode.KOR)]
	MT_EQUIP_RAID_2,
	[CountryDescription("함선", CountryCode.KOR)]
	MT_SHIP
}
