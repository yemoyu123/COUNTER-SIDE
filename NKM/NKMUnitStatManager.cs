using System;
using System.Collections.Generic;
using System.Linq;
using Cs.Logging;
using Cs.Math;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public class NKMUnitStatManager
{
	public static float m_fConstEvade = 1000f;

	public static float m_fConstHit = 1000f;

	public static float m_fConstCritical = 2000f;

	public static float m_fConstDef = 1000f;

	public static float m_fConstEvadeDamage = 0.75f;

	public static float m_fLONG_RANGE = 500f;

	public static float m_fPercentPerBanLevel = 0.2f;

	public static float m_fMaxPercentPerBanLevel = 0.8f;

	public static float m_fPercentPerUpLevel = 0.1f;

	public static float m_fMaxPercentPerUpLevel = 0.2f;

	public static float ROLE_TYPE_BONUS_FACTOR = 0.3f;

	public static float ROLE_TYPE_REDUCE_FACTOR = 0.3f;

	public static float m_fDEFENDER_PROTECT_RATE = 0.05f;

	public static float m_fDEFENDER_PROTECT_RATE_MAX = 0.5f;

	public static byte m_OperatorSkillLevelPerBanLevel = 2;

	public static byte m_MinOperatorSkillLevelPerBanLevel = 1;

	public static float m_OperatorTacticalCommandPerBanLevel = 3f;

	public static float m_MaxOperatorTacticalCommandPerBanLevel = 12f;

	public static float m_fConstSourceAttack = 4000f;

	public static float m_fSourceAttackStatMax = 5000f;

	public static float m_fConstSourceDefend = 2000f;

	public const float FIXED_DAMAGE_ATTACK_STAT = 10000f;

	public const float FIXED_DAMAGE_DEFENDER_HP = 100000f;

	public static void LoadFromLua()
	{
		NKMTempletContainer<NKMGameStatRateTemplet>.Load("AB_SCRIPT", "LUA_GAME_STAT_RATE", "m_GameStatRate", NKMGameStatRateTemplet.LoadFromLua, (NKMGameStatRateTemplet e) => e.m_strID);
	}

	public static NKMStatData MakeFinalStat(NKMUnitData unitData, NKMInventoryData inventoryData, NKMOperator cNKMOperator)
	{
		NKMStatData nKMStatData = new NKMStatData();
		nKMStatData.Init();
		NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitData.m_UnitID);
		if (unitStatTemplet != null)
		{
			nKMStatData.MakeBaseStat(null, bPvP: false, unitData, unitStatTemplet.m_StatData, bPure: false, 0, cNKMOperator);
		}
		nKMStatData.MakeBaseBonusFactor(unitData, inventoryData?.EquipItems, null, null);
		nKMStatData.UpdateFinalStat(null, null, null);
		return nKMStatData;
	}

	public static NKMStatData MakeFinalStat(NKMGame cNKMGame, NKMUnitData unitData, NKM_TEAM_TYPE teamType, NKMUnit cNKMUnit, NKMOperator cNKMOperator)
	{
		NKMStatData nKMStatData = new NKMStatData();
		nKMStatData.Init();
		NKMGameData nKMGameData = cNKMGame?.GetGameData();
		bool bPvP = nKMGameData?.IsPVP() ?? false;
		NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitData.m_UnitID);
		if (unitStatTemplet != null)
		{
			nKMStatData.MakeBaseStat(nKMGameData, bPvP, unitData, unitStatTemplet.m_StatData, bPure: false, 0, cNKMOperator);
		}
		Dictionary<long, NKMEquipItemData> dicEquipItemData = (teamType.IsAteam() ? nKMGameData.m_NKMGameTeamDataA.m_ItemEquipData : nKMGameData.m_NKMGameTeamDataB.m_ItemEquipData);
		nKMStatData.MakeBaseBonusFactor(unitData, dicEquipItemData, (teamType.IsAteam() ? nKMGameData.m_NKMGameTeamDataA : nKMGameData.m_NKMGameTeamDataB)?.m_MainShip?.ShipCommandModule, nKMGameData?.GameStatRate);
		nKMStatData.UpdateFinalStat(cNKMGame, nKMGameData.GameStatRate, cNKMUnit);
		return nKMStatData;
	}

	public static float CalculateStat(NKM_STAT_TYPE eNKM_STAT_TYPE, NKMUnitData unitData, NKMGameStatRate cGameStatRate = null)
	{
		if (unitData == null)
		{
			return 0f;
		}
		NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitData.m_UnitID);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
		return CalculateStat(eNKM_STAT_TYPE, unitStatTemplet.m_StatData, unitData.m_UnitLevel, unitData.m_LimitBreakLevel, unitData.GetMultiplierByPermanentContract(), cGameStatRate, null, 0, unitTempletBase.m_NKM_UNIT_TYPE);
	}

	public static float CalculateStat(NKM_STAT_TYPE eNKM_STAT_TYPE, NKMStatData unitBaseStatData, List<int> lstStatExp, int unitLevel, int limitBreakLevel, float permanentContractMultiplier, NKMGameStatRate cGameStatRate, NKMOperator cNKMOperator, int operatorBanLevel, NKM_UNIT_TYPE eNKM_UNIT_TYPE)
	{
		return CalculateStat(eNKM_STAT_TYPE, unitBaseStatData, unitLevel, limitBreakLevel, permanentContractMultiplier, cGameStatRate, cNKMOperator, operatorBanLevel, eNKM_UNIT_TYPE);
	}

	public static float CalculateStat(NKM_STAT_TYPE eNKM_STAT_TYPE, NKMStatData unitBaseStatData, int unitLevel, int limitBreakLevel, float permanentContractMultiplier, NKMGameStatRate cGameStatRate, NKMOperator cNKMOperator, int operatorBanLevel, NKM_UNIT_TYPE eNKM_UNIT_TYPE)
	{
		float num = 1f;
		num = ((eNKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_SHIP) ? NKMUnitLimitBreakManager.GetLimitBreakStatMultiplier(limitBreakLevel) : NKMUnitLimitBreakManager.GetLimitBreakStatMultiplierForShip(limitBreakLevel));
		float num2 = cGameStatRate?.GetStatValueRate(eNKM_STAT_TYPE) ?? 1f;
		if ((uint)eNKM_STAT_TYPE <= 5u)
		{
			float statBase = unitBaseStatData.GetStatBase(eNKM_STAT_TYPE);
			statBase += unitBaseStatData.GetStatPerLevel(eNKM_STAT_TYPE) * (float)(unitLevel - 1) * num2;
			statBase = GetFinalStatValueByOperator(statBase, eNKM_STAT_TYPE, cNKMOperator, operatorBanLevel);
			statBase *= num;
			if (permanentContractMultiplier > 0f)
			{
				statBase += statBase * permanentContractMultiplier;
			}
			float maxStat = GetMaxStat(eNKM_STAT_TYPE, unitBaseStatData, unitLevel, limitBreakLevel, permanentContractMultiplier, cGameStatRate, cNKMOperator, eNKM_UNIT_TYPE);
			if (statBase > maxStat)
			{
				statBase = maxStat;
			}
			return statBase;
		}
		Log.Error("잘못된 스탯 계산 시도", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStatManager.cs", 1742);
		return 0f;
	}

	public static float GetMaxStat(NKM_STAT_TYPE eNKM_STAT_TYPE, NKMUnitData unitData, NKMGameStatRate cGameStatRate = null, NKMOperator cNKMOperator = null)
	{
		if (unitData == null)
		{
			return 0f;
		}
		NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitData.m_UnitID);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(cNKMOperator.id);
		return GetMaxStat(eNKM_STAT_TYPE, unitStatTemplet.m_StatData, unitData.m_UnitLevel, unitData.m_LimitBreakLevel, unitData.GetMultiplierByPermanentContract(), cGameStatRate, cNKMOperator, unitTempletBase.m_NKM_UNIT_TYPE);
	}

	private static float GetFinalStatValueByOperator(float inputValue, NKM_STAT_TYPE eNKM_STAT_TYPE, NKMOperator cNKMOperator, int operatorBanLevel)
	{
		float num = inputValue;
		if (cNKMOperator != null)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(cNKMOperator.id);
			if (unitTempletBase != null && unitTempletBase.StatTemplet != null)
			{
				float statBase = unitTempletBase.StatTemplet.m_StatData.GetStatBase(eNKM_STAT_TYPE);
				statBase += unitTempletBase.StatTemplet.m_StatData.GetStatPerLevel(eNKM_STAT_TYPE) * (float)(cNKMOperator.level - 1);
				if (operatorBanLevel != 0 && (eNKM_STAT_TYPE == NKM_STAT_TYPE.NST_HP || eNKM_STAT_TYPE == NKM_STAT_TYPE.NST_ATK || eNKM_STAT_TYPE == NKM_STAT_TYPE.NST_DEF))
				{
					float num2 = statBase * m_fPercentPerBanLevel * (float)operatorBanLevel;
					if (num2 < statBase * m_fMaxPercentPerBanLevel)
					{
						num2 = statBase * m_fMaxPercentPerBanLevel;
					}
					statBase -= num2;
				}
				num += num * (statBase / 10000f);
			}
		}
		return num;
	}

	public static float GetMaxStat(NKM_STAT_TYPE eNKM_STAT_TYPE, NKMStatData unitBaseStatData, int unitLevel, int limitBreakLevel, float permanentContractMultiplier, NKMGameStatRate cGameStatRate = null, NKMOperator cNKMOperator = null, NKM_UNIT_TYPE eNKM_UNIT_TYPE = NKM_UNIT_TYPE.NUT_NORMAL)
	{
		float num = 1f;
		num = ((eNKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_SHIP) ? NKMUnitLimitBreakManager.GetLimitBreakStatMultiplier(limitBreakLevel) : NKMUnitLimitBreakManager.GetLimitBreakStatMultiplierForShip(limitBreakLevel));
		float num2 = cGameStatRate?.GetStatValueRate(eNKM_STAT_TYPE) ?? 1f;
		if ((uint)eNKM_STAT_TYPE <= 5u)
		{
			float statBase = unitBaseStatData.GetStatBase(eNKM_STAT_TYPE);
			statBase += unitBaseStatData.GetStatPerLevel(eNKM_STAT_TYPE) * (float)(unitLevel - 1) * num2;
			statBase = GetFinalStatValueByOperator(statBase, eNKM_STAT_TYPE, cNKMOperator, 0);
			statBase *= num;
			statBase += unitBaseStatData.GetStatMaxPerLevel(eNKM_STAT_TYPE) * (float)(unitLevel - 1) * num2;
			if (permanentContractMultiplier > 0f)
			{
				float num3 = statBase * permanentContractMultiplier;
				statBase += num3;
			}
			return statBase;
		}
		Log.Error("잘못된 스탯 계산 시도", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStatManager.cs", 1849);
		return 0f;
	}

	public static bool IsMainStat(NKM_STAT_TYPE eStat)
	{
		if ((uint)eStat <= 5u)
		{
			return true;
		}
		return false;
	}

	public static bool IsMainFactorStat(NKM_STAT_TYPE eStat)
	{
		if ((uint)(eStat - 10000) <= 5u)
		{
			return true;
		}
		return false;
	}

	public static bool IsPercentStat(NKM_STAT_TYPE eNKM_STAT_TYPE)
	{
		if ((uint)eNKM_STAT_TYPE <= 5u || (uint)(eNKM_STAT_TYPE - 4000) <= 5u)
		{
			return false;
		}
		return true;
	}

	public static NKM_STAT_TYPE GetFactorStat(NKM_STAT_TYPE eType)
	{
		return eType switch
		{
			NKM_STAT_TYPE.NST_HP => NKM_STAT_TYPE.NST_HP_FACTOR, 
			NKM_STAT_TYPE.NST_ATK => NKM_STAT_TYPE.NST_ATK_FACTOR, 
			NKM_STAT_TYPE.NST_DEF => NKM_STAT_TYPE.NST_DEF_FACTOR, 
			NKM_STAT_TYPE.NST_CRITICAL => NKM_STAT_TYPE.NST_CRITICAL_FACTOR, 
			NKM_STAT_TYPE.NST_HIT => NKM_STAT_TYPE.NST_HIT_FACTOR, 
			NKM_STAT_TYPE.NST_EVADE => NKM_STAT_TYPE.NST_EVADE_FACTOR, 
			_ => NKM_STAT_TYPE.NST_END, 
		};
	}

	private static float GetDamageAdjustFactor(NKMUnit unitAtk, NKMUnit unitDef, NKMStatData statAtk, NKMStatData statDef, NKMUnitTempletBase atkTempletBase, NKMUnitTempletBase defTempletBase, float distance, bool isPvP)
	{
		if (statAtk == null || statDef == null)
		{
			return 1f;
		}
		if (atkTempletBase == null || defTempletBase == null)
		{
			return 1f;
		}
		bool bDefAir = defTempletBase.m_bAirUnit;
		if (unitDef != null)
		{
			bDefAir = unitDef.IsAirUnit();
		}
		float num = 0f;
		float num2 = 0f;
		if (unitAtk != null && unitDef != null && !statDef.GetStatFinal(NKM_STAT_TYPE.NST_DAMAGE_RESIST).IsNearlyZero())
		{
			float value = statDef.GetStatFinal(NKM_STAT_TYPE.NST_DAMAGE_RESIST) * (float)unitDef.GetDamageResistCount(unitAtk.GetUnitSyncData().m_GameUnitUID);
			value = value.Clamp(-0.8f, 0.8f);
			unitDef.AddDamageResistCount(unitAtk.GetUnitSyncData().m_GameUnitUID);
			if (value < 0f)
			{
				num2 = 0f - value;
			}
			else
			{
				num = value;
			}
		}
		GetDistanceBonusRate(statAtk, statDef, distance, out var atkRangeDamageBonus, out var defRangeDamageReduce);
		float num3 = GetAttackerBonusFromDefUnitStyle(statAtk, defTempletBase.m_NKM_UNIT_STYLE_TYPE) + GetAttackerBonusFromDefUnitStyle(statAtk, defTempletBase.m_NKM_UNIT_STYLE_TYPE_SUB) + GetAttackerBonusFromDefUnitRoleStat(statAtk, defTempletBase.m_NKM_UNIT_ROLE_TYPE) + GetAttackerBonusFromDefUnitAir(statAtk, bDefAir) + atkRangeDamageBonus + num2;
		if (unitDef != null)
		{
			if (!isPvP && unitDef.IsBoss())
			{
				num3 += statAtk.GetStatFinal(NKM_STAT_TYPE.NST_ATTACK_VS_BOSS_DAMAGE_MODIFY_G1);
			}
			if (unitDef.IsSummonUnit())
			{
				num3 += statAtk.GetStatFinal(NKM_STAT_TYPE.NST_ATTACK_VS_SUMMON_DAMAGE_MODIFY_G1);
			}
		}
		num3 -= num3 * (statDef.GetStatFinal(NKM_STAT_TYPE.NST_DAMAGE_INCREASE_DEFENCE) + statAtk.GetStatFinal(NKM_STAT_TYPE.NST_DAMAGE_INCREASE_REDUCE));
		bool bAtkAir = atkTempletBase.m_bAirUnit;
		if (unitAtk != null)
		{
			bAtkAir = unitAtk.IsAirUnit();
		}
		float num4 = GetAttackReduceRateFromDefUnitStyle(statDef, atkTempletBase.m_NKM_UNIT_STYLE_TYPE) + GetAttackReduceRateFromDefUnitStyle(statDef, atkTempletBase.m_NKM_UNIT_STYLE_TYPE_SUB) + GetAttackReduceRateFromAtkUnitRoleStat(statDef, atkTempletBase.m_NKM_UNIT_ROLE_TYPE) + GetAttackReduceFromAtkUnitAir(statDef, bAtkAir) + defRangeDamageReduce + num;
		if (unitAtk != null)
		{
			if (!isPvP && unitAtk.IsBoss())
			{
				num4 += statDef.GetStatFinal(NKM_STAT_TYPE.NST_DEFEND_VS_BOSS_DAMAGE_MODIFY_G1);
			}
			if (unitAtk.IsSummonUnit())
			{
				num4 += statDef.GetStatFinal(NKM_STAT_TYPE.NST_DEFEND_VS_SUMMON_DAMAGE_MODIFY_G1);
			}
		}
		num4 -= num4 * (statAtk.GetStatFinal(NKM_STAT_TYPE.NST_DAMAGE_REDUCE_PENETRATE) + statDef.GetStatFinal(NKM_STAT_TYPE.NST_DAMAGE_REDUCE_REDUCE));
		float num5 = 1f + num3 - num4;
		if (num5 < 0f)
		{
			num5 = 0f;
		}
		return num5;
	}

	private static float Get2ndDamageAdjustFactor(NKMStatData statAtk, NKMStatData statDef, NKMUnitSkillTemplet AttackerSkillTemplet, bool bSplashHit)
	{
		if (statAtk == null || statDef == null)
		{
			return 1f;
		}
		float num = 1f;
		if (AttackerSkillTemplet != null)
		{
			switch (AttackerSkillTemplet.m_NKM_SKILL_TYPE)
			{
			case NKM_SKILL_TYPE.NST_SKILL:
				num += statAtk.GetStatFinal(NKM_STAT_TYPE.NST_SKILL_DAMAGE_RATE);
				num -= statDef.GetStatFinal(NKM_STAT_TYPE.NST_SKILL_DAMAGE_REDUCE_RATE);
				break;
			case NKM_SKILL_TYPE.NST_HYPER:
				num += statAtk.GetStatFinal(NKM_STAT_TYPE.NST_HYPER_SKILL_DAMAGE_RATE);
				num -= statDef.GetStatFinal(NKM_STAT_TYPE.NST_HYPER_SKILL_DAMAGE_REDUCE_RATE);
				break;
			case NKM_SKILL_TYPE.NST_ATTACK:
				num += statAtk.GetStatFinal(NKM_STAT_TYPE.NST_ATTACK_ATTACK_DAMAGE_MODIFY_G2);
				num -= statDef.GetStatFinal(NKM_STAT_TYPE.NST_DEFEND_ATTACK_DAMAGE_MODIFY_G2);
				break;
			}
		}
		if (bSplashHit)
		{
			num -= statDef.GetStatFinal(NKM_STAT_TYPE.NST_SPLASH_DAMAGE_REDUCE_RATE);
		}
		num -= statDef.GetStatFinal(NKM_STAT_TYPE.NST_DAMAGE_REDUCE_RATE);
		num += statAtk.GetStatFinal(NKM_STAT_TYPE.NST_ATTACK_DAMAGE_MODIFY_G2);
		if (num < 0.5f)
		{
			num = 0.5f;
		}
		return num;
	}

	private static void GetDistanceBonusRate(NKMStatData statAtk, NKMStatData statDef, float distance, out float atkRangeDamageBonus, out float defRangeDamageReduce)
	{
		float statFinal = statAtk.GetStatFinal(NKM_STAT_TYPE.NST_LONG_RANGE_DAMAGE_RATE);
		float statFinal2 = statAtk.GetStatFinal(NKM_STAT_TYPE.NST_SHORT_RANGE_DAMAGE_RATE);
		float statFinal3 = statDef.GetStatFinal(NKM_STAT_TYPE.NST_LONG_RANGE_DAMAGE_REDUCE_RATE);
		float statFinal4 = statDef.GetStatFinal(NKM_STAT_TYPE.NST_SHORT_RANGE_DAMAGE_REDUCE_RATE);
		float num = 1f - distance / m_fLONG_RANGE;
		float value = 1f - 4f * num * num;
		float num2;
		float num3;
		if (distance < m_fLONG_RANGE)
		{
			num2 = 1f;
			num3 = value.Clamp(0f, 1f);
		}
		else
		{
			num2 = value.Clamp(0f, 1f);
			num3 = 1f;
		}
		atkRangeDamageBonus = Math.Max(statFinal * num3, statFinal2 * num2);
		defRangeDamageReduce = Math.Max(statFinal3 * num3, statFinal4 * num2);
	}

	private static float GetRoleDamageAdjustFactor(NKMStatData statAtk, NKMStatData statDef, NKMUnitTempletBase atkTempletBase, NKMUnitTempletBase defTempletBase, ref NKM_DAMAGE_RESULT_TYPE eNKM_DAMAGE_RESULT_TYPE)
	{
		if (statAtk == null || statDef == null)
		{
			return 1f;
		}
		if (atkTempletBase == null || defTempletBase == null)
		{
			return 1f;
		}
		float attackerBonusFromDefUnitRole = GetAttackerBonusFromDefUnitRole(atkTempletBase.m_NKM_UNIT_ROLE_TYPE, defTempletBase.m_NKM_UNIT_ROLE_TYPE);
		float num = statAtk.GetStatFinal(NKM_STAT_TYPE.NST_ROLE_TYPE_DAMAGE_RATE) - statDef.GetStatFinal(NKM_STAT_TYPE.NST_ROLE_TYPE_DAMAGE_REDUCE_RATE);
		if (num < 0f)
		{
			num = 0f;
		}
		attackerBonusFromDefUnitRole *= 1f + num;
		float attackReduceRateFromAtkUnitRole = GetAttackReduceRateFromAtkUnitRole(atkTempletBase.m_NKM_UNIT_ROLE_TYPE, defTempletBase.m_NKM_UNIT_ROLE_TYPE);
		float num2 = 1f + attackerBonusFromDefUnitRole - attackReduceRateFromAtkUnitRole;
		if (num2 > 1f)
		{
			eNKM_DAMAGE_RESULT_TYPE = NKM_DAMAGE_RESULT_TYPE.NDRT_WEAK;
		}
		return num2;
	}

	private static float Get4thDamageAdjustFactor(NKMStatData statAtk, NKMStatData statDef, NKMUnit atkUnit, NKMUnit defUnit)
	{
		float num = 1f;
		if (atkUnit == null || defUnit == null)
		{
			return num;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(atkUnit.GetUnitData());
		NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(defUnit.GetUnitData());
		if (unitTempletBase == null || unitTempletBase2 == null)
		{
			return num;
		}
		if (atkUnit.IsMonster() == defUnit.IsMonster())
		{
			return num;
		}
		float num2 = 0f;
		if (!atkUnit.IsMonster())
		{
			List<NKM_STAT_TYPE> sourceTypeStatList = GetSourceTypeStatList(defUnit.GetUnitFrameData().m_UnitSourceType, defUnit.GetUnitFrameData().m_UnitSourceTypeSub, bIsAttack: true);
			for (int i = 0; i < sourceTypeStatList.Count; i++)
			{
				if (sourceTypeStatList[i] != NKM_STAT_TYPE.NST_RANDOM)
				{
					num2 = statAtk.GetStatFinal(sourceTypeStatList[i]);
					if (num2 > 0f)
					{
						num += num2 / (m_fConstSourceAttack - num2 * num2 / (m_fConstSourceAttack + num2));
						break;
					}
				}
			}
		}
		else
		{
			List<NKM_STAT_TYPE> sourceTypeStatList2 = GetSourceTypeStatList(atkUnit.GetUnitFrameData().m_UnitSourceType, atkUnit.GetUnitFrameData().m_UnitSourceTypeSub, bIsAttack: false);
			for (int j = 0; j < sourceTypeStatList2.Count; j++)
			{
				if (sourceTypeStatList2[j] != NKM_STAT_TYPE.NST_RANDOM)
				{
					num2 = statDef.GetStatFinal(sourceTypeStatList2[j]);
					if (num2 > 0f)
					{
						num -= num2 / (m_fConstSourceDefend + num2);
						break;
					}
				}
			}
		}
		return num;
	}

	private static List<NKM_STAT_TYPE> GetSourceTypeStatList(NKM_UNIT_SOURCE_TYPE sourceType, NKM_UNIT_SOURCE_TYPE sourceTypeSub, bool bIsAttack)
	{
		List<NKM_STAT_TYPE> list = new List<NKM_STAT_TYPE>();
		if (sourceType != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
		{
			NKM_STAT_TYPE sourceTypeStat = GetSourceTypeStat(sourceType, bIsAttack);
			if (sourceTypeStat != NKM_STAT_TYPE.NST_RANDOM)
			{
				list.Add(sourceTypeStat);
			}
		}
		if (sourceTypeSub != NKM_UNIT_SOURCE_TYPE.NUST_NONE)
		{
			NKM_STAT_TYPE sourceTypeStat2 = GetSourceTypeStat(sourceTypeSub, bIsAttack);
			if (sourceTypeStat2 != NKM_STAT_TYPE.NST_RANDOM)
			{
				list.Add(sourceTypeStat2);
			}
		}
		return list;
	}

	private static NKM_STAT_TYPE GetSourceTypeStat(NKM_UNIT_SOURCE_TYPE sourceType, bool bIsAttack)
	{
		switch (sourceType)
		{
		case NKM_UNIT_SOURCE_TYPE.NUST_CONFLICT:
			if (bIsAttack)
			{
				return NKM_STAT_TYPE.NST_ATTACK_VS_SOURCE_CONFLICT_G4;
			}
			return NKM_STAT_TYPE.NST_DEFEND_VS_SOURCE_CONFLICT_G4;
		case NKM_UNIT_SOURCE_TYPE.NUST_STABLE:
			if (bIsAttack)
			{
				return NKM_STAT_TYPE.NST_ATTACK_VS_SOURCE_STABLE_G4;
			}
			return NKM_STAT_TYPE.NST_DEFEND_VS_SOURCE_STABLE_G4;
		case NKM_UNIT_SOURCE_TYPE.NUST_LIBERAL:
			if (bIsAttack)
			{
				return NKM_STAT_TYPE.NST_ATTACK_VS_SOURCE_LIBERAL_G4;
			}
			return NKM_STAT_TYPE.NST_DEFEND_VS_SOURCE_LIBERAL_G4;
		default:
			return NKM_STAT_TYPE.NST_RANDOM;
		}
	}

	private static float GetExtraDamageAdjustFactor(NKMUnit unitAtk, NKMUnit unitDef, NKMStatData statAtk, NKMStatData statDef, bool bCritical)
	{
		float num = 1f;
		if (unitDef != null && unitDef.HasBarrierBuff() && statAtk != null && statAtk.GetStatFinal(NKM_STAT_TYPE.NST_DAMAGE_REDUCE_RATE_AGAINST_BARRIER) > 0f)
		{
			float statFinal = statAtk.GetStatFinal(NKM_STAT_TYPE.NST_DAMAGE_REDUCE_RATE_AGAINST_BARRIER);
			statFinal = statFinal.Clamp(0f, 0.99f);
			num -= statFinal;
		}
		if (!bCritical && statDef != null)
		{
			float num2 = statDef.GetStatFinal(NKM_STAT_TYPE.NST_NON_CRITICAL_DAMAGE_TAKE_RATE);
			if (num2 < -0.99f)
			{
				num2 = -0.99f;
			}
			num += num2;
		}
		if (statAtk != null)
		{
			num += statAtk.GetStatFinal(NKM_STAT_TYPE.NST_EXTRA_ADJUST_DAMAGE_DEALT);
		}
		if (statDef != null)
		{
			num += statDef.GetStatFinal(NKM_STAT_TYPE.NST_EXTRA_ADJUST_DAMAGE_RECEIVE);
		}
		if (num < 0.01f)
		{
			num = 0.01f;
		}
		return num;
	}

	private static float GetAttackerBonusFromDefUnitStyle(NKMStatData statAtk, NKM_UNIT_STYLE_TYPE defStyle)
	{
		return defStyle switch
		{
			NKM_UNIT_STYLE_TYPE.NUST_COUNTER => statAtk.GetStatFinal(NKM_STAT_TYPE.NST_UNIT_TYPE_COUNTER_DAMAGE_RATE), 
			NKM_UNIT_STYLE_TYPE.NUST_MECHANIC => statAtk.GetStatFinal(NKM_STAT_TYPE.NST_UNIT_TYPE_MECHANIC_DAMAGE_RATE), 
			NKM_UNIT_STYLE_TYPE.NUST_SOLDIER => statAtk.GetStatFinal(NKM_STAT_TYPE.NST_UNIT_TYPE_SOLDIER_DAMAGE_RATE), 
			NKM_UNIT_STYLE_TYPE.NUST_CORRUPTED => statAtk.GetStatFinal(NKM_STAT_TYPE.NST_UNIT_TYPE_CORRUPTED_DAMAGE_RATE), 
			NKM_UNIT_STYLE_TYPE.NUST_REPLACER => statAtk.GetStatFinal(NKM_STAT_TYPE.NST_UNIT_TYPE_REPLACER_DAMAGE_RATE), 
			_ => 0f, 
		};
	}

	private static float GetAttackReduceRateFromDefUnitStyle(NKMStatData statDef, NKM_UNIT_STYLE_TYPE atkRole)
	{
		return atkRole switch
		{
			NKM_UNIT_STYLE_TYPE.NUST_COUNTER => statDef.GetStatFinal(NKM_STAT_TYPE.NST_UNIT_TYPE_COUNTER_DAMAGE_REDUCE_RATE), 
			NKM_UNIT_STYLE_TYPE.NUST_MECHANIC => statDef.GetStatFinal(NKM_STAT_TYPE.NST_UNIT_TYPE_MECHANIC_DAMAGE_REDUCE_RATE), 
			NKM_UNIT_STYLE_TYPE.NUST_SOLDIER => statDef.GetStatFinal(NKM_STAT_TYPE.NST_UNIT_TYPE_SOLDIER_DAMAGE_REDUCE_RATE), 
			NKM_UNIT_STYLE_TYPE.NUST_CORRUPTED => statDef.GetStatFinal(NKM_STAT_TYPE.NST_UNIT_TYPE_CORRUPTED_DAMAGE_REDUCE_RATE), 
			NKM_UNIT_STYLE_TYPE.NUST_REPLACER => statDef.GetStatFinal(NKM_STAT_TYPE.NST_UNIT_TYPE_REPLACER_DAMAGE_REDUCE_RATE), 
			_ => 0f, 
		};
	}

	private static float GetAttackerBonusFromDefUnitRole(NKM_UNIT_ROLE_TYPE atkRole, NKM_UNIT_ROLE_TYPE defRole)
	{
		switch (atkRole)
		{
		case NKM_UNIT_ROLE_TYPE.NURT_STRIKER:
			if (defRole != NKM_UNIT_ROLE_TYPE.NURT_RANGER)
			{
				return 0f;
			}
			return ROLE_TYPE_BONUS_FACTOR;
		case NKM_UNIT_ROLE_TYPE.NURT_RANGER:
			if (defRole != NKM_UNIT_ROLE_TYPE.NURT_DEFENDER)
			{
				return 0f;
			}
			return ROLE_TYPE_BONUS_FACTOR;
		case NKM_UNIT_ROLE_TYPE.NURT_DEFENDER:
			if (defRole != NKM_UNIT_ROLE_TYPE.NURT_SNIPER)
			{
				return 0f;
			}
			return ROLE_TYPE_BONUS_FACTOR;
		case NKM_UNIT_ROLE_TYPE.NURT_SNIPER:
			if (defRole != NKM_UNIT_ROLE_TYPE.NURT_STRIKER)
			{
				return 0f;
			}
			return ROLE_TYPE_BONUS_FACTOR;
		case NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER:
		case NKM_UNIT_ROLE_TYPE.NURT_SIEGE:
		case NKM_UNIT_ROLE_TYPE.NURT_TOWER:
			return 0f;
		default:
			return 0f;
		}
	}

	private static float GetAttackReduceRateFromAtkUnitRole(NKM_UNIT_ROLE_TYPE atkRole, NKM_UNIT_ROLE_TYPE defRole)
	{
		switch (defRole)
		{
		case NKM_UNIT_ROLE_TYPE.NURT_STRIKER:
			if (atkRole != NKM_UNIT_ROLE_TYPE.NURT_RANGER)
			{
				return 0f;
			}
			return ROLE_TYPE_REDUCE_FACTOR;
		case NKM_UNIT_ROLE_TYPE.NURT_RANGER:
			if (atkRole != NKM_UNIT_ROLE_TYPE.NURT_DEFENDER)
			{
				return 0f;
			}
			return ROLE_TYPE_REDUCE_FACTOR;
		case NKM_UNIT_ROLE_TYPE.NURT_DEFENDER:
			if (atkRole != NKM_UNIT_ROLE_TYPE.NURT_SNIPER)
			{
				return 0f;
			}
			return ROLE_TYPE_REDUCE_FACTOR;
		case NKM_UNIT_ROLE_TYPE.NURT_SNIPER:
			if (atkRole != NKM_UNIT_ROLE_TYPE.NURT_STRIKER)
			{
				return 0f;
			}
			return ROLE_TYPE_REDUCE_FACTOR;
		case NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER:
		case NKM_UNIT_ROLE_TYPE.NURT_SIEGE:
		case NKM_UNIT_ROLE_TYPE.NURT_TOWER:
			return 0f;
		default:
			return 0f;
		}
	}

	private static float GetAttackerBonusFromDefUnitRoleStat(NKMStatData statAtk, NKM_UNIT_ROLE_TYPE defRole)
	{
		return defRole switch
		{
			NKM_UNIT_ROLE_TYPE.NURT_STRIKER => statAtk.GetStatFinal(NKM_STAT_TYPE.NST_ROLE_TYPE_STRIKER_DAMAGE_RATE), 
			NKM_UNIT_ROLE_TYPE.NURT_RANGER => statAtk.GetStatFinal(NKM_STAT_TYPE.NST_ROLE_TYPE_RANGER_DAMAGE_RATE), 
			NKM_UNIT_ROLE_TYPE.NURT_DEFENDER => statAtk.GetStatFinal(NKM_STAT_TYPE.NST_ROLE_TYPE_DEFFENDER_DAMAGE_RATE), 
			NKM_UNIT_ROLE_TYPE.NURT_SNIPER => statAtk.GetStatFinal(NKM_STAT_TYPE.NST_ROLE_TYPE_SNIPER_DAMAGE_RATE), 
			NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER => statAtk.GetStatFinal(NKM_STAT_TYPE.NST_ROLE_TYPE_SUPPOERTER_DAMAGE_RATE), 
			NKM_UNIT_ROLE_TYPE.NURT_SIEGE => statAtk.GetStatFinal(NKM_STAT_TYPE.NST_ROLE_TYPE_SIEGE_DAMAGE_RATE), 
			NKM_UNIT_ROLE_TYPE.NURT_TOWER => statAtk.GetStatFinal(NKM_STAT_TYPE.NST_ROLE_TYPE_TOWER_DAMAGE_RATE), 
			_ => 0f, 
		};
	}

	private static float GetAttackReduceRateFromAtkUnitRoleStat(NKMStatData statDef, NKM_UNIT_ROLE_TYPE atkRole)
	{
		return atkRole switch
		{
			NKM_UNIT_ROLE_TYPE.NURT_STRIKER => statDef.GetStatFinal(NKM_STAT_TYPE.NST_ROLE_TYPE_STRIKER_DAMAGE_REDUCE_RATE), 
			NKM_UNIT_ROLE_TYPE.NURT_RANGER => statDef.GetStatFinal(NKM_STAT_TYPE.NST_ROLE_TYPE_RANGER_DAMAGE_REDUCE_RATE), 
			NKM_UNIT_ROLE_TYPE.NURT_DEFENDER => statDef.GetStatFinal(NKM_STAT_TYPE.NST_ROLE_TYPE_DEFFENDER_DAMAGE_REDUCE_RATE), 
			NKM_UNIT_ROLE_TYPE.NURT_SNIPER => statDef.GetStatFinal(NKM_STAT_TYPE.NST_ROLE_TYPE_SNIPER_DAMAGE_REDUCE_RATE), 
			NKM_UNIT_ROLE_TYPE.NURT_SUPPORTER => statDef.GetStatFinal(NKM_STAT_TYPE.NST_ROLE_TYPE_SUPPOERTER_DAMAGE_REDUCE_RATE), 
			NKM_UNIT_ROLE_TYPE.NURT_SIEGE => statDef.GetStatFinal(NKM_STAT_TYPE.NST_ROLE_TYPE_SIEGE_DAMAGE_REDUCE_RATE), 
			NKM_UNIT_ROLE_TYPE.NURT_TOWER => statDef.GetStatFinal(NKM_STAT_TYPE.NST_ROLE_TYPE_TOWER_DAMAGE_REDUCE_RATE), 
			_ => 0f, 
		};
	}

	private static float GetAttackerBonusFromDefUnitAir(NKMStatData statAtk, bool bDefAir)
	{
		if (bDefAir)
		{
			return statAtk.GetStatFinal(NKM_STAT_TYPE.NST_MOVE_TYPE_AIR_DAMAGE_RATE);
		}
		return statAtk.GetStatFinal(NKM_STAT_TYPE.NST_MOVE_TYPE_LAND_DAMAGE_RATE);
	}

	private static float GetAttackReduceFromAtkUnitAir(NKMStatData statDef, bool bAtkAir)
	{
		if (bAtkAir)
		{
			return statDef.GetStatFinal(NKM_STAT_TYPE.NST_MOVE_TYPE_AIR_DAMAGE_REDUCE_RATE);
		}
		return statDef.GetStatFinal(NKM_STAT_TYPE.NST_MOVE_TYPE_LAND_DAMAGE_REDUCE_RATE);
	}

	public static bool GetEvade(NKMUnit atkUnit, NKMUnit defUnit, bool bBuffDamage, float fDefHPRate, NKMEventAttack cNKMEventAttack = null)
	{
		if (bBuffDamage)
		{
			return false;
		}
		bool flag = cNKMEventAttack.m_bCleanHit || atkUnit.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_FORCE_HIT) || defUnit.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_NO_EVADE);
		bool flag2 = atkUnit.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_FORCE_MISS) || defUnit.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_FORCE_EVADE);
		if (flag2 && !flag)
		{
			return true;
		}
		if (!flag2 && flag)
		{
			return false;
		}
		NKMStatData statData = atkUnit.GetUnitFrameData().m_StatData;
		NKMStatData statData2 = defUnit.GetUnitFrameData().m_StatData;
		float statFinal = statData2.GetStatFinal(NKM_STAT_TYPE.NST_EVADE);
		float statFinal2 = statData2.GetStatFinal(NKM_STAT_TYPE.NST_HP_GROWN_EVADE_RATE);
		float num = Math.Max(-1f, statFinal2 * (1f - fDefHPRate));
		statFinal2 = statFinal + statFinal * num;
		float num2 = statFinal2 / (statFinal2 + m_fConstEvade);
		float num3 = statData.GetStatFinal(NKM_STAT_TYPE.NST_HIT) / (statData.GetStatFinal(NKM_STAT_TYPE.NST_HIT) + m_fConstHit);
		if (num2 < num3)
		{
			return false;
		}
		int num4 = (int)(num2 * 10000f);
		return NKMRandom.Range(0, 10000) <= num4;
	}

	public static float GetAttackFactorDamage(NKMDamageTempletBase damageTempletBase, NKMUnitSkillTemplet m_AttackerUnitSkillTemplet, bool bBoss)
	{
		float num = m_AttackerUnitSkillTemplet?.m_fEmpowerFactor ?? 1f;
		float fAtkFactor = damageTempletBase.m_fAtkFactor;
		float num2 = num * fAtkFactor * 10000f;
		if (!bBoss)
		{
			float num3 = 100000f * damageTempletBase.m_fAtkMaxHPRateFactor;
			float num4 = 100000f * damageTempletBase.m_fAtkHPRateFactor;
			num2 = num2 + num3 + num4;
		}
		return num2;
	}

	public static float GetFinalDamage(bool bPVP, NKMStatData statAtk, NKMStatData statDef, NKMUnitData unitdataAtk, NKMUnit unitAtk, NKMUnit unitDef, NKMDamageTemplet damageTemplet, NKMUnitSkillTemplet AttackerSkillTemplet, bool bAttackCountOver, bool bBuffDamage, bool bEvade, out NKM_DAMAGE_RESULT_TYPE eNKM_DAMAGE_RESULT_TYPE, float fDefenderDamageReduce, float distance, bool bBoss, float fAtkHPRate, bool bSplashHit, NKMDamageAttribute damageAttribute)
	{
		return GetFinalDamage(bPVP, statAtk, statDef, unitdataAtk, unitAtk, unitDef, damageTemplet, AttackerSkillTemplet, bAttackCountOver, bBuffDamage, bEvade, out eNKM_DAMAGE_RESULT_TYPE, fDefenderDamageReduce, distance, bBoss, fAtkHPRate, damageAttribute?.m_bTrueDamage ?? false, bSplashHit, damageAttribute?.m_bForceCritical ?? false, damageAttribute?.m_bNoCritical ?? false);
	}

	public static float GetFinalDamage(bool bPVP, NKMStatData statAtk, NKMStatData statDef, NKMUnitData unitdataAtk, NKMUnit unitAtk, NKMUnit unitDef, NKMDamageTemplet damageTemplet, NKMUnitSkillTemplet AttackerSkillTemplet, bool bAttackCountOver, bool bBuffDamage, bool bEvade, out NKM_DAMAGE_RESULT_TYPE eNKM_DAMAGE_RESULT_TYPE, float fDefenderDamageReduce, float distance, bool bBoss, float fAtkHPRate, bool bTrueDamage, bool bSplashHit, bool bForceCritical, bool bNoCritical)
	{
		if (damageTemplet.IsZeroDamage(bPVP))
		{
			eNKM_DAMAGE_RESULT_TYPE = NKM_DAMAGE_RESULT_TYPE.NDRT_NO_MARK;
			return 0f;
		}
		if (unitDef.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_IRONWALL))
		{
			if (statAtk == null)
			{
				bEvade = false;
			}
			if (bEvade)
			{
				eNKM_DAMAGE_RESULT_TYPE = NKM_DAMAGE_RESULT_TYPE.NDRT_MISS;
				return 0f;
			}
			eNKM_DAMAGE_RESULT_TYPE = NKM_DAMAGE_RESULT_TYPE.NDRT_NORMAL;
			return 1f;
		}
		eNKM_DAMAGE_RESULT_TYPE = NKM_DAMAGE_RESULT_TYPE.NDRT_NORMAL;
		float num = AttackerSkillTemplet?.m_fEmpowerFactor ?? 1f;
		float num2 = damageTemplet.m_DamageTempletBase.m_fAtkFactor;
		if (bPVP && !damageTemplet.m_DamageTempletBase.m_fAtkFactorPVP.IsNearlyZero())
		{
			num2 = damageTemplet.m_DamageTempletBase.m_fAtkFactorPVP;
		}
		float num3 = 1f;
		if (statAtk != null)
		{
			num3 = statAtk.GetRuntimeStatFinal(damageTemplet.m_DamageTempletBase.m_AtkFactorStat, fAtkHPRate) * num2 * num;
		}
		if (!bBoss && !bAttackCountOver && !unitDef.HasStatus(NKM_UNIT_STATUS_EFFECT.NUSE_IMMUNE_HP_RATE_DAMAGE))
		{
			float num4 = statDef.GetStatFinal(NKM_STAT_TYPE.NST_HP) * damageTemplet.m_DamageTempletBase.m_fAtkMaxHPRateFactor;
			if (bPVP && !damageTemplet.m_DamageTempletBase.m_fAtkMaxHPRateFactorPVP.IsNearlyZero())
			{
				num4 = statDef.GetStatFinal(NKM_STAT_TYPE.NST_HP) * damageTemplet.m_DamageTempletBase.m_fAtkMaxHPRateFactorPVP;
			}
			float num5 = unitDef.GetUnitSyncData().GetHP() * damageTemplet.m_DamageTempletBase.m_fAtkHPRateFactor;
			if (bPVP && !damageTemplet.m_DamageTempletBase.m_fAtkHPRateFactorPVP.IsNearlyZero())
			{
				num5 = unitDef.GetUnitSyncData().GetHP() * damageTemplet.m_DamageTempletBase.m_fAtkHPRateFactorPVP;
			}
			num3 = num3 + num4 + num5;
		}
		float statFinal = statDef.GetStatFinal(NKM_STAT_TYPE.NST_DEF);
		float statFinal2 = statDef.GetStatFinal(NKM_STAT_TYPE.NST_HP_GROWN_DEF_RATE);
		float num6 = Math.Max(-1f, statFinal2 * (1f - unitDef.GetHPRate()));
		statFinal2 = statFinal + statFinal * num6;
		statFinal = ((statAtk == null) ? statFinal2 : (statFinal2 * (1f - statAtk.GetStatFinal(NKM_STAT_TYPE.NST_DEF_PENETRATE_RATE))));
		float num7 = 1f - statFinal / (statFinal + m_fConstDef);
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitdataAtk);
		NKMUnitTempletBase unitTempletBase2 = NKMUnitManager.GetUnitTempletBase(unitDef.GetUnitData());
		if (unitdataAtk != null && unitTempletBase == null)
		{
			Log.Error($"Can not found UnitTempletBase. UnitId:{unitdataAtk.m_UnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStatManager.cs", 2717);
			return 1f;
		}
		if (unitDef.GetUnitData() != null && unitTempletBase2 == null)
		{
			Log.Error($"Can not found UnitTempletBase. UnitId:{unitDef.GetUnitData().m_UnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStatManager.cs", 2723);
			return 1f;
		}
		float num8 = GetDamageAdjustFactor(unitAtk, unitDef, statAtk, statDef, unitTempletBase, unitTempletBase2, distance, bPVP);
		if (bTrueDamage)
		{
			if (num7 < 1f)
			{
				num7 = 1f;
			}
			if (num8 < 1f)
			{
				num8 = 1f;
			}
		}
		float num9 = num7 * num8;
		if (num9 < 0.2f)
		{
			num9 = 0.2f;
		}
		float num10 = Get2ndDamageAdjustFactor(statAtk, statDef, AttackerSkillTemplet, bSplashHit);
		float roleDamageAdjustFactor = GetRoleDamageAdjustFactor(statAtk, statDef, unitTempletBase, unitTempletBase2, ref eNKM_DAMAGE_RESULT_TYPE);
		float num11 = Get4thDamageAdjustFactor(statAtk, statDef, unitAtk, unitDef);
		float num12 = num3 * num9 * num10 * roleDamageAdjustFactor * num11;
		if (statAtk == null)
		{
			bEvade = false;
		}
		float num13 = 1f;
		if (bEvade && statAtk != null)
		{
			num13 = 1f - (m_fConstEvadeDamage - statAtk.GetStatFinal(NKM_STAT_TYPE.NST_HIT) / (statAtk.GetStatFinal(NKM_STAT_TYPE.NST_HIT) + m_fConstHit));
			eNKM_DAMAGE_RESULT_TYPE = NKM_DAMAGE_RESULT_TYPE.NDRT_MISS;
		}
		bool flag = false;
		float num14 = 0f;
		if (bForceCritical)
		{
			flag = true;
		}
		else if (!bEvade && !bAttackCountOver && statAtk != null && statDef != null)
		{
			int num15 = (int)(((statAtk.GetStatFinal(NKM_STAT_TYPE.NST_CRITICAL) - statDef.GetStatFinal(NKM_STAT_TYPE.NST_CRITICAL_RESIST)) / m_fConstCritical).Clamp(0f, 0.85f) * 10000f);
			if (NKMRandom.Range(0, 10000) <= num15)
			{
				flag = true;
			}
		}
		if (bNoCritical)
		{
			flag = false;
		}
		if (bBuffDamage || statAtk == null)
		{
			flag = false;
		}
		if (flag && statAtk != null)
		{
			num14 = statAtk.GetStatFinal(NKM_STAT_TYPE.NST_CRITICAL_DAMAGE_RATE) - statDef.GetStatFinal(NKM_STAT_TYPE.NST_CRITICAL_DAMAGE_RESIST_RATE);
			num14 = num14.Clamp(0f, 5f);
			eNKM_DAMAGE_RESULT_TYPE = NKM_DAMAGE_RESULT_TYPE.NDRT_CRITICAL;
		}
		float num16 = num12 * num14;
		float num17 = num12 * num13 + num16;
		float min = num17 * 0.95f;
		float max = num17 * 1.05f;
		num17 = NKMRandom.Range(min, max);
		if (fDefenderDamageReduce > 0f && unitDef.GetUnitTemplet().m_UnitTempletBase.m_NKM_UNIT_ROLE_TYPE != NKM_UNIT_ROLE_TYPE.NURT_DEFENDER)
		{
			num17 -= num17 * fDefenderDamageReduce;
			if (eNKM_DAMAGE_RESULT_TYPE == NKM_DAMAGE_RESULT_TYPE.NDRT_NORMAL || eNKM_DAMAGE_RESULT_TYPE == NKM_DAMAGE_RESULT_TYPE.NDRT_WEAK)
			{
				eNKM_DAMAGE_RESULT_TYPE = NKM_DAMAGE_RESULT_TYPE.NDRT_PROTECT;
			}
		}
		if (bAttackCountOver)
		{
			num17 *= 0.3f;
		}
		float extraDamageAdjustFactor = GetExtraDamageAdjustFactor(unitAtk, unitDef, statAtk, statDef, flag);
		num17 *= extraDamageAdjustFactor;
		if (statDef.GetStatFinal(NKM_STAT_TYPE.NST_DAMAGE_LIMIT_RATE_BY_HP) > 0f)
		{
			float num18 = statDef.GetStatFinal(NKM_STAT_TYPE.NST_DAMAGE_LIMIT_RATE_BY_HP) * statDef.GetStatFinal(NKM_STAT_TYPE.NST_HP);
			if (num17 > num18)
			{
				num17 -= num18;
				num17 *= 0.04f;
				num17 += num18;
			}
		}
		if (num17 < 1f)
		{
			if (bEvade)
			{
				eNKM_DAMAGE_RESULT_TYPE = NKM_DAMAGE_RESULT_TYPE.NDRT_MISS;
				return 0f;
			}
			num17 = 1f;
		}
		return num17;
	}

	public static decimal GetFinalStatForUIOutput(NKM_STAT_TYPE eType, NKMStatData statData)
	{
		bool num = IsPercentStat(eType);
		float statBase = statData.GetStatBase(eType);
		float baseBonusStat = statData.GetBaseBonusStat(eType);
		decimal num2 = new decimal(baseBonusStat);
		if (num)
		{
			num2 = Math.Round(num2 * 1000m);
			num2 /= 1000m;
		}
		if (num)
		{
			return Math.Round(decimal.Add(new decimal(statBase), num2) * 1000m) / 1000m;
		}
		return new decimal(statBase + baseBonusStat);
	}

	public static int GetNerfPercentByShipBanLevel(int banLevel)
	{
		if (banLevel == 0)
		{
			return 0;
		}
		float num = (float)banLevel * m_fPercentPerBanLevel;
		if (num <= 0f)
		{
			num = 0f;
		}
		if (num >= m_fMaxPercentPerBanLevel)
		{
			num = m_fMaxPercentPerBanLevel;
		}
		return (int)(num * 100f);
	}

	public static HashSet<NKM_STAT_TYPE> GetUnitBonusStatList(NKMStatData statData, bool bIncludeFactor = false)
	{
		HashSet<NKM_STAT_TYPE> hashSet = new HashSet<NKM_STAT_TYPE>();
		hashSet.Union(statData.dicStatBase.Keys);
		hashSet.Union(statData.dicStatBonus.Keys);
		return hashSet;
	}

	public static void LoadStat(NKMLua cNKMLua, string valueName, string factorName, ref NKM_STAT_TYPE statType, ref float statValue)
	{
		float rValue = 0f;
		float rValue2 = 0f;
		cNKMLua.GetData(valueName, ref rValue);
		cNKMLua.GetData(factorName, ref rValue2);
		if (rValue2 > 0f)
		{
			NKM_STAT_TYPE factorStat = GetFactorStat(statType);
			if (factorStat == NKM_STAT_TYPE.NST_END)
			{
				Log.ErrorAndExit($"Non-Factor stat {statType} have factor value!", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitStatManager.cs", 2977);
				return;
			}
			statType = factorStat;
			statValue = rValue2;
		}
		else
		{
			statValue = rValue;
		}
	}
}
