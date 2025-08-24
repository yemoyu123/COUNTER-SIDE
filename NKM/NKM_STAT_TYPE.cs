using Cs.GameLog.CountryDescription;

namespace NKM;

public enum NKM_STAT_TYPE : short
{
	[CountryDescription("없음", CountryCode.KOR)]
	NST_RANDOM = -1,
	[CountryDescription("체력", CountryCode.KOR)]
	NST_HP = 0,
	[CountryDescription("공격력", CountryCode.KOR)]
	NST_ATK = 1,
	[CountryDescription("방어력", CountryCode.KOR)]
	NST_DEF = 2,
	[CountryDescription("치명타율", CountryCode.KOR)]
	NST_CRITICAL = 3,
	[CountryDescription("명중율", CountryCode.KOR)]
	NST_HIT = 4,
	[CountryDescription("회피율", CountryCode.KOR)]
	NST_EVADE = 5,
	[CountryDescription("체력 회복율", CountryCode.KOR)]
	NST_HP_REGEN_RATE = 6,
	[CountryDescription("치명타 피해 증가율", CountryCode.KOR)]
	NST_CRITICAL_DAMAGE_RATE = 7,
	[CountryDescription("치명타 저항율", CountryCode.KOR)]
	NST_CRITICAL_RESIST = 8,
	[CountryDescription("치명타 피해 증가 저항율", CountryCode.KOR)]
	NST_CRITICAL_DAMAGE_RESIST_RATE = 9,
	[CountryDescription("피해 감소율", CountryCode.KOR)]
	NST_DAMAGE_REDUCE_RATE = 10,
	[CountryDescription("이동속도 증가율", CountryCode.KOR)]
	NST_MOVE_SPEED_RATE = 11,
	[CountryDescription("공격속도 증가율", CountryCode.KOR)]
	NST_ATTACK_SPEED_RATE = 12,
	[CountryDescription("스킬 쿨타임 감소율", CountryCode.KOR)]
	NST_SKILL_COOL_TIME_REDUCE_RATE = 13,
	[CountryDescription("상태이상 저항율", CountryCode.KOR)]
	NST_CC_RESIST_RATE = 14,
	[CountryDescription("vs카운터에게 주는 피해 증가", CountryCode.KOR)]
	NST_UNIT_TYPE_COUNTER_DAMAGE_RATE = 15,
	[CountryDescription("vs카운터에게 받는 피해 감소", CountryCode.KOR)]
	NST_UNIT_TYPE_COUNTER_DAMAGE_REDUCE_RATE = 16,
	[CountryDescription("vs솔저에게 주는 피해 증가", CountryCode.KOR)]
	NST_UNIT_TYPE_SOLDIER_DAMAGE_RATE = 17,
	[CountryDescription("vs솔저에게 받는 피해 감소", CountryCode.KOR)]
	NST_UNIT_TYPE_SOLDIER_DAMAGE_REDUCE_RATE = 18,
	[CountryDescription("vs메카닉에게 주는 피해 증가", CountryCode.KOR)]
	NST_UNIT_TYPE_MECHANIC_DAMAGE_RATE = 19,
	[CountryDescription("vs메카닉에게 받는 피해 감소", CountryCode.KOR)]
	NST_UNIT_TYPE_MECHANIC_DAMAGE_REDUCE_RATE = 20,
	[CountryDescription("vs스트라이커에게 주는 피해 증가", CountryCode.KOR)]
	NST_ROLE_TYPE_STRIKER_DAMAGE_RATE = 21,
	[CountryDescription("vs스트라이커에게 받는 피해 감소", CountryCode.KOR)]
	NST_ROLE_TYPE_STRIKER_DAMAGE_REDUCE_RATE = 22,
	[CountryDescription("vs레인저에게 주는 피해 증가", CountryCode.KOR)]
	NST_ROLE_TYPE_RANGER_DAMAGE_RATE = 23,
	[CountryDescription("vs레인저에게 받는 피해 감소", CountryCode.KOR)]
	NST_ROLE_TYPE_RANGER_DAMAGE_REDUCE_RATE = 24,
	[CountryDescription("vs스나이퍼에게 주는 피해 증가", CountryCode.KOR)]
	NST_ROLE_TYPE_SNIPER_DAMAGE_RATE = 25,
	[CountryDescription("vs스나이퍼에게 받는 피해 감소", CountryCode.KOR)]
	NST_ROLE_TYPE_SNIPER_DAMAGE_REDUCE_RATE = 26,
	[CountryDescription("vs디펜더에게 주는 피해 증가", CountryCode.KOR)]
	NST_ROLE_TYPE_DEFFENDER_DAMAGE_RATE = 27,
	[CountryDescription("vs디펜더에게 받는 피해 감소", CountryCode.KOR)]
	NST_ROLE_TYPE_DEFFENDER_DAMAGE_REDUCE_RATE = 28,
	[CountryDescription("vs서포터에게 주는 피해 증가", CountryCode.KOR)]
	NST_ROLE_TYPE_SUPPOERTER_DAMAGE_RATE = 29,
	[CountryDescription("vs서포터에게 받는 피해 감소", CountryCode.KOR)]
	NST_ROLE_TYPE_SUPPOERTER_DAMAGE_REDUCE_RATE = 30,
	[CountryDescription("vs시즈에게 주는 피해 증가", CountryCode.KOR)]
	NST_ROLE_TYPE_SIEGE_DAMAGE_RATE = 31,
	[CountryDescription("vs시즈에게 받는 피해 감소", CountryCode.KOR)]
	NST_ROLE_TYPE_SIEGE_DAMAGE_REDUCE_RATE = 32,
	[CountryDescription("vs타워에게 주는 피해 증가", CountryCode.KOR)]
	NST_ROLE_TYPE_TOWER_DAMAGE_RATE = 33,
	[CountryDescription("vs타워에게 받는 피해 감소", CountryCode.KOR)]
	NST_ROLE_TYPE_TOWER_DAMAGE_REDUCE_RATE = 34,
	[CountryDescription("vs지상유닛에게 주는 피해 증가", CountryCode.KOR)]
	NST_MOVE_TYPE_LAND_DAMAGE_RATE = 35,
	[CountryDescription("vs지상유닛에게 받는 피해 감소", CountryCode.KOR)]
	NST_MOVE_TYPE_LAND_DAMAGE_REDUCE_RATE = 36,
	[CountryDescription("vs공중유닛에게 주는 피해 증가", CountryCode.KOR)]
	NST_MOVE_TYPE_AIR_DAMAGE_RATE = 37,
	[CountryDescription("vs공중유닛에게 받는 피해 감소", CountryCode.KOR)]
	NST_MOVE_TYPE_AIR_DAMAGE_REDUCE_RATE = 38,
	[CountryDescription("장거리 공격으로 받는 피해 감소", CountryCode.KOR)]
	NST_LONG_RANGE_DAMAGE_REDUCE_RATE = 39,
	[CountryDescription("장거리 공격으로 주는 피해 추가", CountryCode.KOR)]
	NST_LONG_RANGE_DAMAGE_RATE = 40,
	[CountryDescription("단거리 공격으로 받는 피해 감소", CountryCode.KOR)]
	NST_SHORT_RANGE_DAMAGE_REDUCE_RATE = 41,
	[CountryDescription("단거리 공격으로 주는 피해 추가", CountryCode.KOR)]
	NST_SHORT_RANGE_DAMAGE_RATE = 42,
	[CountryDescription("힐 감소율", CountryCode.KOR)]
	NST_HEAL_REDUCE_RATE = 43,
	[CountryDescription("방어 관통율", CountryCode.KOR)]
	NST_DEF_PENETRATE_RATE = 44,
	[CountryDescription("방어막 강화율", CountryCode.KOR)]
	NST_BARRIER_REINFORCE_RATE = 45,
	[CountryDescription("스킬 데미지 강화율", CountryCode.KOR)]
	NST_SKILL_DAMAGE_RATE = 46,
	[CountryDescription("스킬 데미지 감소율", CountryCode.KOR)]
	NST_SKILL_DAMAGE_REDUCE_RATE = 47,
	[CountryDescription("궁극기 데미지 강화율", CountryCode.KOR)]
	NST_HYPER_SKILL_DAMAGE_RATE = 48,
	[CountryDescription("궁극기 데미지 감소율", CountryCode.KOR)]
	NST_HYPER_SKILL_DAMAGE_REDUCE_RATE = 49,
	[CountryDescription("침식체에게 주는 피해 증가", CountryCode.KOR)]
	NST_UNIT_TYPE_CORRUPTED_DAMAGE_RATE = 50,
	[CountryDescription("침식체에게 받는 피해 감소", CountryCode.KOR)]
	NST_UNIT_TYPE_CORRUPTED_DAMAGE_REDUCE_RATE = 51,
	[CountryDescription("리플레이서에게 주는 피해 증가", CountryCode.KOR)]
	NST_UNIT_TYPE_REPLACER_DAMAGE_RATE = 52,
	[CountryDescription("리플레이서에게 받는 피해 감소", CountryCode.KOR)]
	NST_UNIT_TYPE_REPLACER_DAMAGE_REDUCE_RATE = 53,
	[CountryDescription("상성으로 주는 피해 증가", CountryCode.KOR)]
	NST_ROLE_TYPE_DAMAGE_RATE = 54,
	[CountryDescription("상성으로 받는 피해 감소", CountryCode.KOR)]
	NST_ROLE_TYPE_DAMAGE_REDUCE_RATE = 55,
	[CountryDescription("성장형 공격력 증가율", CountryCode.KOR)]
	NST_HP_GROWN_ATK_RATE = 56,
	[CountryDescription("성장형 방어력 증가율", CountryCode.KOR)]
	NST_HP_GROWN_DEF_RATE = 57,
	[CountryDescription("성장형 회피 증가율", CountryCode.KOR)]
	NST_HP_GROWN_EVADE_RATE = 58,
	[CountryDescription("광역 피해 감소율", CountryCode.KOR)]
	NST_SPLASH_DAMAGE_REDUCE_RATE = 59,
	[CountryDescription("디펜더 프로텍트 증가", CountryCode.KOR)]
	NST_DEFENDER_PROTECT_RATE = 60,
	[CountryDescription("추가 피해 감소율", CountryCode.KOR)]
	NST_DAMAGE_INCREASE_DEFENCE = 61,
	[CountryDescription("피해 감소 관통율", CountryCode.KOR)]
	NST_DAMAGE_REDUCE_PENETRATE = 62,
	[CountryDescription("자기 추가 피해 감소율", CountryCode.KOR)]
	NST_DAMAGE_INCREASE_REDUCE = 63,
	[CountryDescription("자기 피해 감소 관통율", CountryCode.KOR)]
	NST_DAMAGE_REDUCE_REDUCE = 64,
	[CountryDescription("최대 피해 제한", CountryCode.KOR)]
	NST_DAMAGE_LIMIT_RATE_BY_HP = 65,
	[CountryDescription("유효 타격 감소", CountryCode.KOR)]
	NST_ATTACK_COUNT_REDUCE = 66,
	[CountryDescription("피해 내성", CountryCode.KOR)]
	NST_DAMAGE_RESIST = 67,
	[CountryDescription("배리어가 있는 적에게 공격 피해 감소", CountryCode.KOR)]
	NST_DAMAGE_REDUCE_RATE_AGAINST_BARRIER = 68,
	[CountryDescription("크리가 아닌 공격 데미지 비율", CountryCode.KOR)]
	NST_NON_CRITICAL_DAMAGE_TAKE_RATE = 69,
	[CountryDescription("주는 회복량 증가", CountryCode.KOR)]
	NST_HEAL_RATE = 70,
	[CountryDescription("넉백 저항", CountryCode.KOR)]
	NST_DAMAGE_BACK_RESIST = 71,
	[CountryDescription("전체 능력치 증가/감소", CountryCode.KOR)]
	NST_MAIN_STAT_RATE = 72,
	[CountryDescription("주는 피해 증감_Extra", CountryCode.KOR)]
	NST_EXTRA_ADJUST_DAMAGE_DEALT = 73,
	[CountryDescription("받는 피해 증감_Extra", CountryCode.KOR)]
	NST_EXTRA_ADJUST_DAMAGE_RECEIVE = 74,
	[CountryDescription("주는 피해 증감", CountryCode.KOR)]
	NST_ATTACK_DAMAGE_MODIFY_G2 = 75,
	[CountryDescription("코스트 반환", CountryCode.KOR)]
	NST_COST_RETURN_RATE = 76,
	[CountryDescription("보스에게 주는 피해", CountryCode.KOR)]
	NST_ATTACK_VS_BOSS_DAMAGE_MODIFY_G1 = 1000,
	[CountryDescription("보스에게 받는 피해 감소", CountryCode.KOR)]
	NST_DEFEND_VS_BOSS_DAMAGE_MODIFY_G1 = 1001,
	[CountryDescription("소환수에게 주는 피해", CountryCode.KOR)]
	NST_ATTACK_VS_SUMMON_DAMAGE_MODIFY_G1 = 1010,
	[CountryDescription("소환수에게 받는 피해 감소", CountryCode.KOR)]
	NST_DEFEND_VS_SUMMON_DAMAGE_MODIFY_G1 = 1011,
	[CountryDescription("공격자 기본기 피해 증폭/감쇄", CountryCode.KOR)]
	NST_ATTACK_ATTACK_DAMAGE_MODIFY_G2 = 2000,
	[CountryDescription("방어자 기본기 피해 증폭/감쇄", CountryCode.KOR)]
	NST_DEFEND_ATTACK_DAMAGE_MODIFY_G2 = 2001,
	[CountryDescription("투쟁 몬스터에게 주는 피해 증가", CountryCode.KOR)]
	NST_ATTACK_VS_SOURCE_CONFLICT_G4 = 4000,
	[CountryDescription("투쟁 몬스터에게 받는 피해 감소", CountryCode.KOR)]
	NST_DEFEND_VS_SOURCE_CONFLICT_G4 = 4001,
	[CountryDescription("안정 몬스터에게 주는 피해 증가", CountryCode.KOR)]
	NST_ATTACK_VS_SOURCE_STABLE_G4 = 4002,
	[CountryDescription("안정 몬스터에게 받는 피해 감소", CountryCode.KOR)]
	NST_DEFEND_VS_SOURCE_STABLE_G4 = 4003,
	[CountryDescription("변화 몬스터에게 주는 피해 증가", CountryCode.KOR)]
	NST_ATTACK_VS_SOURCE_LIBERAL_G4 = 4004,
	[CountryDescription("변화 몬스터에게 받는 피해 감소", CountryCode.KOR)]
	NST_DEFEND_VS_SOURCE_LIBERAL_G4 = 4005,
	[CountryDescription("감응으로 인한 효과 증폭(곱연산)", CountryCode.KOR)]
	NST_SOURCE_ALL_RATE_G4 = 4100,
	[CountryDescription("체력%", CountryCode.KOR)]
	NST_HP_FACTOR = 10000,
	[CountryDescription("공격력%", CountryCode.KOR)]
	NST_ATK_FACTOR = 10001,
	[CountryDescription("방어력%", CountryCode.KOR)]
	NST_DEF_FACTOR = 10002,
	[CountryDescription("치명타율%", CountryCode.KOR)]
	NST_CRITICAL_FACTOR = 10003,
	[CountryDescription("명중율%", CountryCode.KOR)]
	NST_HIT_FACTOR = 10004,
	[CountryDescription("회피율%", CountryCode.KOR)]
	NST_EVADE_FACTOR = 10005,
	NST_RESERVE01 = 30000,
	NST_RESERVE02 = 30001,
	NST_RESERVE03 = 30002,
	NST_RESERVE04 = 30003,
	NST_RESERVE05 = 30004,
	NST_END = 30005
}
