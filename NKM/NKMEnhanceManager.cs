using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet;

namespace NKM;

public static class NKMEnhanceManager
{
	public static readonly List<NKM_STAT_TYPE> s_lstEnhancebleStat = new List<NKM_STAT_TYPE>
	{
		NKM_STAT_TYPE.NST_HP,
		NKM_STAT_TYPE.NST_ATK,
		NKM_STAT_TYPE.NST_DEF,
		NKM_STAT_TYPE.NST_CRITICAL,
		NKM_STAT_TYPE.NST_HIT,
		NKM_STAT_TYPE.NST_EVADE
	};

	public static NKM_ERROR_CODE CanEnhance(NKMUserData userData, NKMUnitData targetEnhancedUnit, List<long> lstConsumedUnit)
	{
		if (lstConsumedUnit == null || lstConsumedUnit.Count <= 0)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_NOT_EXIST;
		}
		NKMArmyData armyData = userData.m_ArmyData;
		if (targetEnhancedUnit == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_NOT_EXIST;
		}
		if (targetEnhancedUnit.IsSeized)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_IS_SEIZED;
		}
		switch (armyData.GetUnitDeckState(targetEnhancedUnit.m_UnitUID))
		{
		case NKM_DECK_STATE.DECK_STATE_WARFARE:
			return NKM_ERROR_CODE.NEC_FAIL_WARFARE_DOING;
		case NKM_DECK_STATE.DECK_STATE_DIVE:
			return NKM_ERROR_CODE.NEC_FAIL_DIVE_DOING;
		default:
		{
			foreach (long item in lstConsumedUnit)
			{
				if (item == targetEnhancedUnit.m_UnitUID)
				{
					return NKM_ERROR_CODE.NEC_FAIL_UNIT_INVALID_UNIT_UID;
				}
				NKMUnitData unitOrTrophyFromUID = armyData.GetUnitOrTrophyFromUID(item);
				if (unitOrTrophyFromUID == null)
				{
					return NKM_ERROR_CODE.NEC_FAIL_UNIT_NOT_EXIST;
				}
				if (unitOrTrophyFromUID.IsSeized)
				{
					return NKM_ERROR_CODE.NEC_FAIL_MATERIAL_UNIT_IS_SEIZED;
				}
				if (armyData.IsUnitInAnyDeck(item))
				{
					return NKM_ERROR_CODE.NEC_FAIL_ENHANCE_UNIT_IN_ANY_DECK;
				}
				NKM_ERROR_CODE canDeleteUnit = NKMUnitManager.GetCanDeleteUnit(unitOrTrophyFromUID, userData);
				if (canDeleteUnit != NKM_ERROR_CODE.NEC_OK)
				{
					return canDeleteUnit;
				}
				if (!NKMUnitManager.GetUnitStatTemplet(unitOrTrophyFromUID.m_UnitID).m_StatData.HasEnchantFeedExp)
				{
					return NKM_ERROR_CODE.NEC_FAIL_UNIT_ENHANCE_INVALID_FEED_EXP;
				}
			}
			if (CheckUnitFullEnhance(targetEnhancedUnit))
			{
				return NKM_ERROR_CODE.NEC_FAIL_ENHANCE_UNIT_ENHANCE_MAX;
			}
			int num = CalculateCreditCost(lstConsumedUnit);
			if (userData.GetCredit() < num)
			{
				return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_CREDIT;
			}
			return NKM_ERROR_CODE.NEC_OK;
		}
		}
	}

	public static int CalculateCreditCost(List<long> lstComsumedUnitUID)
	{
		float num = NKMCommonConst.ENHANCE_CREDIT_COST_PER_UNIT * lstComsumedUnitUID.Count;
		return (int)(num + num * ((float)lstComsumedUnitUID.Count * NKMCommonConst.ENHANCE_CREDIT_COST_FACTOR));
	}

	public static bool CheckUnitFullEnhance(NKMUnitData unitData)
	{
		for (int i = 0; i <= 5; i++)
		{
			int num = (int)NKMUnitStatManager.CalculateStat((NKM_STAT_TYPE)i, unitData);
			int num2 = (int)NKMUnitStatManager.GetMaxStat((NKM_STAT_TYPE)i, unitData);
			if (num < num2)
			{
				return false;
			}
		}
		return true;
	}

	public static Dictionary<NKM_STAT_TYPE, int> CalculateExpGain(NKMArmyData armyData, List<long> lstConsumedUnit, NKM_UNIT_ROLE_TYPE bonusRoleType)
	{
		float[] array = new float[s_lstEnhancebleStat.Count];
		for (int i = 0; i < lstConsumedUnit.Count; i++)
		{
			long uID = lstConsumedUnit[i];
			NKMUnitData unitOrTrophyFromUID = armyData.GetUnitOrTrophyFromUID(uID);
			if (unitOrTrophyFromUID == null)
			{
				continue;
			}
			float num = ((NKMUnitManager.GetUnitTempletBase(unitOrTrophyFromUID).m_NKM_UNIT_ROLE_TYPE == bonusRoleType) ? 1.5f : 1f);
			NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(unitOrTrophyFromUID.m_UnitID);
			if (unitStatTemplet != null)
			{
				for (int j = 0; j < s_lstEnhancebleStat.Count; j++)
				{
					int statType = (int)s_lstEnhancebleStat[j];
					array[j] += unitStatTemplet.m_StatData.GetStatEnhanceFeedEXP(statType) * num;
				}
			}
		}
		Dictionary<NKM_STAT_TYPE, int> dictionary = new Dictionary<NKM_STAT_TYPE, int>();
		for (int k = 0; k < array.Length; k++)
		{
			float num2 = array[k] * (1f + (float)lstConsumedUnit.Count * NKMCommonConst.ENHANCE_EXP_BONUS_FACTOR);
			dictionary.Add(s_lstEnhancebleStat[k], (int)num2);
		}
		return dictionary;
	}

	public static int CalculateMaxEXP(NKMUnitData _target_unit, NKM_STAT_TYPE targetStat)
	{
		NKMUnitStatTemplet unitStatTemplet = NKMUnitManager.GetUnitStatTemplet(_target_unit.m_UnitID);
		if (unitStatTemplet == null)
		{
			Log.Error($"Can not found UnitStatTemplet. userUid:{_target_unit.m_UserUID}, unitId:{_target_unit.m_UnitID}, unitUid:{_target_unit.m_UnitUID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMEnhanceManager.cs", 183);
			return 0;
		}
		NKMStatData statData = unitStatTemplet.m_StatData;
		return NKMMathf.Ceiling(statData.GetStatMaxPerLevel(targetStat) * (float)(_target_unit.m_UnitLevel - 1) * statData.GetStatEXP(targetStat));
	}
}
