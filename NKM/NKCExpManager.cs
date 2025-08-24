using Cs.Logging;
using NKM.Templet;
using NKM.Unit;

namespace NKM;

public static class NKCExpManager
{
	public static NKMUnitExpTemplet GetUnitExpTemplet(NKMUnitData cUnitData)
	{
		if (cUnitData == null)
		{
			Log.Error("NKMExpManager : UnitData Null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCExpManager.cs", 15);
			return null;
		}
		return cUnitData.GetExpTemplet();
	}

	public static NKMUserExpTemplet GetUserExpTemplet(NKMUserData cUserData)
	{
		if (cUserData == null)
		{
			Log.Error("NKMExpManager : UserData Null", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCExpManager.cs", 26);
			return null;
		}
		return NKMUserExpTemplet.Find(cUserData.UserLevel);
	}

	public static float GetUnitNextLevelExpProgress(NKMUnitData cUnitData)
	{
		NKMUnitExpTemplet unitExpTemplet = GetUnitExpTemplet(cUnitData);
		if (unitExpTemplet == null)
		{
			return 0f;
		}
		if (unitExpTemplet.m_iExpRequired == 0)
		{
			return 1f;
		}
		return (float)cUnitData.m_iUnitLevelEXP / (float)unitExpTemplet.m_iExpRequired;
	}

	public static float GetOperatorNextLevelExpProgress(NKMOperator operatorData)
	{
		if (operatorData == null)
		{
			return 0f;
		}
		return (float)operatorData.exp / (float)NKCOperatorUtil.GetRequiredExp(operatorData);
	}

	public static float GetUnitNextLevelExpProgress(int unitID, int level, int currentExp)
	{
		NKMUnitExpTemplet nKMUnitExpTemplet = NKMUnitExpTemplet.FindByUnitId(unitID, level);
		if (nKMUnitExpTemplet == null)
		{
			return 0f;
		}
		if (nKMUnitExpTemplet.m_iExpRequired == 0)
		{
			return 1f;
		}
		return (float)currentExp / (float)nKMUnitExpTemplet.m_iExpRequired;
	}

	public static long GetCurrentExp(NKMUnitData cUnitData)
	{
		if (GetUnitExpTemplet(cUnitData) == null)
		{
			return 0L;
		}
		return cUnitData.m_iUnitLevelEXP;
	}

	public static int GetRequiredExp(NKMUnitData cUnitData)
	{
		return GetUnitExpTemplet(cUnitData)?.m_iExpRequired ?? 0;
	}

	public static int GetRequiredUnitExp(int unitID, int lv)
	{
		return NKMUnitExpTemplet.FindByUnitId(unitID, lv)?.m_iExpRequired ?? 0;
	}

	public static int GetCumulatedTotalExp(NKMUnitData cUnitData)
	{
		NKMUnitExpTemplet unitExpTemplet = GetUnitExpTemplet(cUnitData);
		if (unitExpTemplet == null)
		{
			return cUnitData.m_iUnitLevelEXP;
		}
		return cUnitData.m_iUnitLevelEXP + unitExpTemplet.m_iExpCumulated;
	}

	public static float GetNextLevelExpProgress(NKMUserData cUserData)
	{
		NKMUserExpTemplet userExpTemplet = GetUserExpTemplet(cUserData);
		if (userExpTemplet == null)
		{
			return 0f;
		}
		if (userExpTemplet.m_lExpRequired == 0)
		{
			return 1f;
		}
		return (float)cUserData.UserLevelEXP / (float)userExpTemplet.m_lExpRequired;
	}

	public static long GetCurrentExp(NKMUserData cUserData)
	{
		if (GetUserExpTemplet(cUserData) == null)
		{
			return 0L;
		}
		return cUserData.UserLevelEXP;
	}

	public static int GetRequiredExp(NKMUserData cUserData)
	{
		return GetUserExpTemplet(cUserData)?.m_lExpRequired ?? 0;
	}

	public static int GetRequiredUserExp(int level)
	{
		return NKMUserExpTemplet.Find(level)?.m_lExpRequired ?? 0;
	}

	public static int GetFutureUserLevel(NKMUserData cUserData, int plusEXP)
	{
		if (cUserData == null)
		{
			return 0;
		}
		int num = cUserData.UserLevelEXP;
		int num2 = cUserData.UserLevel;
		int num3 = plusEXP;
		int num4 = GetRequiredExp(cUserData);
		if (num4 == 0)
		{
			return cUserData.UserLevel;
		}
		while (num4 <= num + num3)
		{
			num3 -= num4 - num;
			num2++;
			num = 0;
			num4 = GetRequiredUserExp(num2);
			if (num4 == 0)
			{
				return num2;
			}
		}
		return num2;
	}

	public static int GetFutureUserRemainEXP(NKMUserData cUserData, int plusEXP)
	{
		if (cUserData == null)
		{
			return 0;
		}
		int num = cUserData.UserLevelEXP;
		int num2 = cUserData.UserLevel;
		int num3 = plusEXP;
		int num4 = GetRequiredExp(cUserData);
		if (num4 == 0)
		{
			return 0;
		}
		bool flag = false;
		while (num4 <= num + num3)
		{
			num3 -= num4 - num;
			num2++;
			num = 0;
			num4 = GetRequiredUserExp(num2);
			flag = true;
			if (num4 == 0)
			{
				return num4;
			}
		}
		if (!flag)
		{
			num3 += num;
		}
		return num3;
	}

	public static void CalculateFutureUnitExpAndLevel(NKMUnitData cUnitData, int expGain, out int Level, out int Exp)
	{
		if (cUnitData == null)
		{
			Level = 0;
			Exp = 0;
			return;
		}
		Level = cUnitData.m_UnitLevel;
		Exp = cUnitData.m_iUnitLevelEXP + expGain;
		int unitMaxLevel = GetUnitMaxLevel(cUnitData);
		if (Level >= unitMaxLevel)
		{
			Exp = 0;
			return;
		}
		int requiredUnitExp = GetRequiredUnitExp(cUnitData.m_UnitID, Level);
		if (requiredUnitExp == 0)
		{
			return;
		}
		while (requiredUnitExp <= Exp)
		{
			Level++;
			if (Level >= unitMaxLevel)
			{
				Exp = 0;
				break;
			}
			Exp -= requiredUnitExp;
			requiredUnitExp = GetRequiredUnitExp(cUnitData.m_UnitID, Level);
			if (requiredUnitExp == 0)
			{
				break;
			}
		}
	}

	public static int CalculateNeedExpForUnitMaxLevel(NKMUnitData cUnitData)
	{
		if (cUnitData == null)
		{
			return 0;
		}
		int num = cUnitData.m_UnitLevel;
		int iUnitLevelEXP = cUnitData.m_iUnitLevelEXP;
		int unitMaxLevel = GetUnitMaxLevel(cUnitData);
		if (num == unitMaxLevel)
		{
			return 0;
		}
		int num2 = GetRequiredUnitExp(cUnitData.m_UnitID, num);
		if (num2 == 0)
		{
			return 0;
		}
		while (num < unitMaxLevel)
		{
			num++;
			if (num == unitMaxLevel)
			{
				break;
			}
			num2 += GetRequiredUnitExp(cUnitData.m_UnitID, num);
		}
		return num2 - iUnitLevelEXP;
	}

	public static (int level, int levelExp) CalcUnitTotalExp(NKMUnitTempletBase unitTempletBase, short limitBreakLevel, int totalUnitExp)
	{
		NKMUnitExpTable expTable = NKMUnitTempletBase.Find(unitTempletBase.m_UnitID).ExpTable;
		if (expTable == null)
		{
			Log.Error($"[ExpTable] invalid unitId:{unitTempletBase.m_UnitID}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCExpManager.cs", 322);
			return (level: 1, levelExp: 0);
		}
		int unitMaxLevel = GetUnitMaxLevel(unitTempletBase, limitBreakLevel);
		NKMUnitExpTemplet nKMUnitExpTemplet = null;
		foreach (NKMUnitExpTemplet level in expTable.Levels)
		{
			if (level.m_iLevel > unitMaxLevel || level.m_iExpCumulated > totalUnitExp)
			{
				break;
			}
			nKMUnitExpTemplet = level;
		}
		if (nKMUnitExpTemplet == null)
		{
			return (level: 1, levelExp: 0);
		}
		return (level: nKMUnitExpTemplet.m_iLevel, levelExp: totalUnitExp - nKMUnitExpTemplet.m_iExpCumulated);
	}

	public static int CalculateUnitExpGain(int unitID, int levelBefore, int expBefore, int levelAfter, int expAfter)
	{
		if (levelBefore > levelAfter)
		{
			return 0;
		}
		int num = 0;
		for (int i = levelBefore; i < levelAfter; i++)
		{
			NKMUnitExpTemplet nKMUnitExpTemplet = NKMUnitExpTemplet.FindByUnitId(unitID, i);
			num += nKMUnitExpTemplet.m_iExpRequired;
		}
		num += expAfter;
		return num - expBefore;
	}

	public static int CalculateUserExpGain(int levelBefore, int expBefore, int levelAfter, int expAfter)
	{
		if (levelBefore > levelAfter)
		{
			return 0;
		}
		if (levelBefore == levelAfter && expBefore > expAfter)
		{
			return 0;
		}
		int num = 0;
		for (int i = levelBefore; i < levelAfter; i++)
		{
			NKMUserExpTemplet nKMUserExpTemplet = NKMUserExpTemplet.Find(i);
			num += nKMUserExpTemplet.m_lExpRequired;
		}
		num += expAfter;
		return num - expBefore;
	}

	public static int GetUnitMaxLevel(NKMUnitData cUnitData)
	{
		if (cUnitData == null)
		{
			return 120;
		}
		return GetUnitMaxLevel(NKMUnitManager.GetUnitTempletBase(cUnitData.m_UnitID), cUnitData.m_LimitBreakLevel);
	}

	public static bool IsUnitMaxLevel(NKMUnitData unitData)
	{
		return GetUnitMaxLevel(unitData) == unitData.m_UnitLevel;
	}

	public static int GetUnitMaxLevel(NKMUnitTempletBase templetBase, int limitBreakLevel)
	{
		switch (templetBase.m_NKM_UNIT_TYPE)
		{
		case NKM_UNIT_TYPE.NUT_NORMAL:
		{
			NKMLimitBreakTemplet lBInfo = NKMUnitLimitBreakManager.GetLBInfo(limitBreakLevel);
			if (lBInfo == null)
			{
				Log.Error($"LimitBreakInfo not found! unit id : {templetBase.m_UnitID}, lbLevel {limitBreakLevel}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKC/NKCExpManager.cs", 424);
				return 120;
			}
			return lBInfo.m_iMaxLevel;
		}
		case NKM_UNIT_TYPE.NUT_SHIP:
			return NKMShipLevelUpTemplet.GetMaxLevel(templetBase.m_StarGradeMax, templetBase.m_NKM_UNIT_GRADE, limitBreakLevel);
		default:
			return 120;
		}
	}
}
