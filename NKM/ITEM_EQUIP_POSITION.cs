using Cs.GameLog.CountryDescription;

namespace NKM;

public enum ITEM_EQUIP_POSITION : short
{
	[CountryDescription("무기", CountryCode.KOR)]
	IEP_WEAPON,
	[CountryDescription("방어구", CountryCode.KOR)]
	IEP_DEFENCE,
	[CountryDescription("보조 장비", CountryCode.KOR)]
	IEP_ACC,
	[CountryDescription("보조 장비2", CountryCode.KOR)]
	IEP_ACC2,
	IEP_MAX,
	[CountryDescription("강화 모듈", CountryCode.KOR)]
	IEP_ENCHANT,
	[CountryDescription("알수없음", CountryCode.KOR)]
	IEP_NONE
}
