using System;
using System.Collections.Generic;
using Cs.Core.Util;
using Cs.Logging;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public static class NKMUnitLimitBreakManager
{
	public enum UnitLimitBreakStatus
	{
		Invalid,
		NextTierLevelNotEnough,
		LevelNotEnough,
		CanLimitBreak,
		Max
	}

	public struct UnitLimitBreakStatusData
	{
		public readonly int Tier;

		public readonly UnitLimitBreakStatus Status;

		public UnitLimitBreakStatusData(int _tier, UnitLimitBreakStatus _status)
		{
			Tier = _tier;
			Status = _status;
		}

		public bool CurrentTierCompleted()
		{
			if (Status != UnitLimitBreakStatus.NextTierLevelNotEnough)
			{
				return Status == UnitLimitBreakStatus.Max;
			}
			return true;
		}
	}

	public static int LIMITBREAK_TIER_MAX;

	public static Dictionary<int, NKMLimitBreakTemplet> m_dicLimitBreakTemplet;

	public static Dictionary<int, NKMLimitBreakItemTemplet> m_dicLimitBreakItemTemplet;

	public static NKMLimitBreakTemplet GetLBInfo(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return null;
		}
		return GetLBInfo(unitData.m_LimitBreakLevel);
	}

	public static NKMLimitBreakTemplet GetLBInfo(int currentLimitBreakLevel)
	{
		if (m_dicLimitBreakTemplet.ContainsKey(currentLimitBreakLevel))
		{
			return m_dicLimitBreakTemplet[currentLimitBreakLevel];
		}
		return null;
	}

	public static int GetLBTier(int limitBreakLevel)
	{
		return GetLBInfo(limitBreakLevel)?.m_Tier ?? 0;
	}

	public static int GetLBTierShip(int shipID, int limitBreakLevel)
	{
		if (!NKMShipManager.IsMaxLimitBreak(shipID, limitBreakLevel))
		{
			return 0;
		}
		return 1;
	}

	public static int GetLBTier(NKMUnitData unitData)
	{
		if (NKMUnitManager.GetUnitTempletBase(unitData).IsShip())
		{
			if (unitData.m_LimitBreakLevel <= 0)
			{
				return 0;
			}
			return 1;
		}
		return GetLBInfo(unitData).m_Tier;
	}

	public static NKMLimitBreakItemTemplet GetLBSubstituteItemInfo(NKMUnitData targetUnit)
	{
		if (targetUnit == null)
		{
			return null;
		}
		NKMUnitTempletBase nKMUnitTempletBase = NKMUnitManager.GetUnitTempletBase(targetUnit);
		if (nKMUnitTempletBase == null)
		{
			return null;
		}
		if (nKMUnitTempletBase.BaseUnit != null)
		{
			nKMUnitTempletBase = nKMUnitTempletBase.BaseUnit;
		}
		return GetLBSubstituteItemInfo(nKMUnitTempletBase.m_NKM_UNIT_STYLE_TYPE, nKMUnitTempletBase.m_NKM_UNIT_GRADE, targetUnit.m_LimitBreakLevel + 1);
	}

	public static NKMLimitBreakItemTemplet GetLBSubstituteItemInfo(NKM_UNIT_STYLE_TYPE eUnitStyle, NKM_UNIT_GRADE eUnitGrade, int targetLBLevel)
	{
		if (m_dicLimitBreakTemplet.Count > targetLBLevel)
		{
			int key = MakeKey(eUnitStyle, eUnitGrade, targetLBLevel);
			if (m_dicLimitBreakItemTemplet.ContainsKey(key))
			{
				return m_dicLimitBreakItemTemplet[key];
			}
		}
		return null;
	}

	public static int GetMinLimitBreakLevelByUnitLevel(NKMUnitTempletBase unitTempletBase, int level)
	{
		if (unitTempletBase == null)
		{
			return 0;
		}
		NKM_UNIT_TYPE nKM_UNIT_TYPE = unitTempletBase.m_NKM_UNIT_TYPE;
		if (nKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL || nKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_SHIP)
		{
			NKMLimitBreakTemplet nKMLimitBreakTemplet = null;
			foreach (NKMLimitBreakTemplet value in m_dicLimitBreakTemplet.Values)
			{
				if ((nKMLimitBreakTemplet == null || nKMLimitBreakTemplet.m_iLBRank >= value.m_iLBRank) && value.m_iMaxLevel >= level)
				{
					nKMLimitBreakTemplet = value;
				}
			}
			return nKMLimitBreakTemplet?.m_iLBRank ?? 0;
		}
		return NKMShipLevelUpTemplet.GetShipMinLimitBreakLevel(level, unitTempletBase.m_NKM_UNIT_GRADE);
	}

	public static int GetMaxLimitBreakLevelByUnitLevel(NKMUnitTempletBase unitTempletBase, int level)
	{
		if (unitTempletBase == null)
		{
			return 0;
		}
		NKM_UNIT_TYPE nKM_UNIT_TYPE = unitTempletBase.m_NKM_UNIT_TYPE;
		if (nKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL || nKM_UNIT_TYPE != NKM_UNIT_TYPE.NUT_SHIP)
		{
			if (level <= 100)
			{
				return 3;
			}
			if (level <= 110)
			{
				return 8;
			}
			return 13;
		}
		return NKMShipLevelUpTemplet.GetShipMinLimitBreakLevel(level, unitTempletBase.m_NKM_UNIT_GRADE);
	}

	public static bool IsMaxLimitBreak(NKMUnitData unitData, int tier)
	{
		UnitLimitBreakStatusData unitLimitbreakStatus = GetUnitLimitbreakStatus(unitData);
		if (unitLimitbreakStatus.Tier == tier)
		{
			if (unitLimitbreakStatus.Status != UnitLimitBreakStatus.Max)
			{
				return unitLimitbreakStatus.Status == UnitLimitBreakStatus.NextTierLevelNotEnough;
			}
			return true;
		}
		return unitLimitbreakStatus.Tier > tier;
	}

	public static bool IsMaxLimitBreak(NKMUnitData unitData)
	{
		return IsMaxLimitBreak(unitData, LIMITBREAK_TIER_MAX);
	}

	public static bool IsMaxLimitBreak(UnitLimitBreakStatusData lbStatus)
	{
		return IsMaxLimitBreak(lbStatus, LIMITBREAK_TIER_MAX);
	}

	public static bool IsMaxLimitBreak(UnitLimitBreakStatusData lbStatus, int tier)
	{
		if (lbStatus.Tier == tier)
		{
			if (lbStatus.Status != UnitLimitBreakStatus.Max)
			{
				return lbStatus.Status == UnitLimitBreakStatus.NextTierLevelNotEnough;
			}
			return true;
		}
		return lbStatus.Tier > tier;
	}

	public static bool CanThisUnitLimitBreak(NKMUnitData unitData)
	{
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
		if (unitTempletBase != null && unitTempletBase.m_NKM_UNIT_STYLE_TYPE == NKM_UNIT_STYLE_TYPE.NUST_TRAINER)
		{
			return false;
		}
		return GetUnitLimitbreakStatus(unitData).Status == UnitLimitBreakStatus.CanLimitBreak;
	}

	public static int GetTranscendenceCount(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return 0;
		}
		return Math.Max(unitData.m_LimitBreakLevel - 3, 0);
	}

	public static bool IsTranscendenceUnit(NKMUnitData unitData)
	{
		if (NKMUnitManager.GetUnitTempletBase(unitData).m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_SHIP)
		{
			return unitData.m_LimitBreakLevel > 0;
		}
		return unitData.m_LimitBreakLevel > 3;
	}

	public static UnitLimitBreakStatusData GetUnitLimitbreakStatus(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return new UnitLimitBreakStatusData(0, UnitLimitBreakStatus.Invalid);
		}
		NKMLimitBreakTemplet lBInfo = GetLBInfo(unitData.m_LimitBreakLevel);
		NKMLimitBreakTemplet lBInfo2 = GetLBInfo(unitData.m_LimitBreakLevel + 1);
		NKMLimitBreakItemTemplet lBSubstituteItemInfo = GetLBSubstituteItemInfo(unitData);
		if (lBInfo2 == null || lBSubstituteItemInfo == null)
		{
			if (lBInfo != null)
			{
				return new UnitLimitBreakStatusData(lBInfo.m_Tier, UnitLimitBreakStatus.Max);
			}
			Log.Error($"Unit {unitData.GetUnitTempletBase().DebugName} have no limitBreakTemplet data. LB level = {unitData.m_LimitBreakLevel}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitLimitBreakManager.cs", 409);
			int tier = 0;
			if (unitData.m_LimitBreakLevel < 3)
			{
				tier = 0;
			}
			else if (unitData.m_LimitBreakLevel < 8)
			{
				tier = 1;
			}
			else if (unitData.m_LimitBreakLevel <= 13)
			{
				tier = 2;
			}
			return new UnitLimitBreakStatusData(tier, UnitLimitBreakStatus.Max);
		}
		if (lBInfo.m_Tier < lBInfo2.m_Tier)
		{
			if (unitData.m_UnitLevel < lBInfo2.m_iRequiredLevel)
			{
				return new UnitLimitBreakStatusData(lBInfo.m_Tier, UnitLimitBreakStatus.NextTierLevelNotEnough);
			}
			return new UnitLimitBreakStatusData(lBInfo2.m_Tier, UnitLimitBreakStatus.CanLimitBreak);
		}
		if (unitData.m_UnitLevel < lBInfo2.m_iRequiredLevel)
		{
			return new UnitLimitBreakStatusData(lBInfo2.m_Tier, UnitLimitBreakStatus.LevelNotEnough);
		}
		return new UnitLimitBreakStatusData(lBInfo2.m_Tier, UnitLimitBreakStatus.CanLimitBreak);
	}

	public static int CanThisUnitLimitBreakNow(NKMUnitData unitData, NKMUserData userData)
	{
		if (unitData == null || userData == null)
		{
			return -1;
		}
		if (!CanThisUnitLimitBreak(unitData))
		{
			return -1;
		}
		if (GetLBInfo(unitData.m_LimitBreakLevel + 1) == null)
		{
			return -1;
		}
		NKMLimitBreakItemTemplet lBSubstituteItemInfo = GetLBSubstituteItemInfo(unitData);
		if (lBSubstituteItemInfo == null)
		{
			return -1;
		}
		for (int i = 0; i < lBSubstituteItemInfo.m_lstRequiredItem.Count; i++)
		{
			int itemID = lBSubstituteItemInfo.m_lstRequiredItem[i].itemID;
			int count = lBSubstituteItemInfo.m_lstRequiredItem[i].count;
			if (userData.m_InventoryData.GetCountMiscItem(itemID) < count)
			{
				return -1;
			}
		}
		return 0;
	}

	public static NKM_ERROR_CODE CanLimitBreak(NKMUserData userData, NKMUnitData targetUnitData, out List<NKMItemMiscData> lstCost)
	{
		lstCost = new List<NKMItemMiscData>();
		_ = userData.m_ArmyData;
		if (targetUnitData == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_NOT_EXIST;
		}
		if (IsMaxLimitBreak(targetUnitData))
		{
			return NKM_ERROR_CODE.NEC_FAIL_LIMITBREAK_ALREADY_MAX_LEVEL;
		}
		NKMLimitBreakTemplet lBInfo = GetLBInfo(targetUnitData.m_LimitBreakLevel + 1);
		if (lBInfo == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_GET_UNIT_LIMIT_BREAK_TEMPLET_NULL;
		}
		if (targetUnitData.m_UnitLevel < lBInfo.m_iRequiredLevel)
		{
			return NKM_ERROR_CODE.NEC_FAIL_LIMITBREAK_LOW_LEVEL;
		}
		NKMLimitBreakItemTemplet lBSubstituteItemInfo = GetLBSubstituteItemInfo(targetUnitData);
		if (lBSubstituteItemInfo == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_GET_ITEM_LIMIT_BREAK_TEMPLET_NULL;
		}
		if (userData.GetCredit() < lBSubstituteItemInfo.m_CreditReq)
		{
			return NKM_ERROR_CODE.NEC_FAIL_INSUFFICIENT_CREDIT;
		}
		lstCost.Add(new NKMItemMiscData(1, lBSubstituteItemInfo.m_CreditReq, 0L));
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static float GetLimitBreakStatMultiplier(int LimitBreakLevel)
	{
		int num = Math.Min(LimitBreakLevel, 3);
		int num2 = (LimitBreakLevel - 3).Clamp(0, 5);
		int num3 = Math.Max(0, LimitBreakLevel - 8);
		return (1f + 0.1f * (float)num) * (1f + 0.02f * (float)num2) * (1f + 0.02f * (float)num3);
	}

	public static float GetLimitBreakStatMultiplierForShip(int lImitBreakLevel)
	{
		int num = Math.Max(0, lImitBreakLevel);
		return 1f + 0.02f * (float)num;
	}

	private static bool LoadFromLUA_LIMITBREAK_SUBSTITUTE_ITEM(string fileName)
	{
		m_dicLimitBreakItemTemplet = NKMTempletLoader.LoadDictionary("AB_SCRIPT", fileName, "m_LBSubstitute", NKMLimitBreakItemTemplet.LoadFromLUA);
		return m_dicLimitBreakItemTemplet != null;
	}

	private static bool LoadFromLUA_LUA_LIMITBREAK_INFO(string fileName)
	{
		m_dicLimitBreakTemplet = NKMTempletLoader.LoadDictionary("AB_SCRIPT", fileName, "m_LimitBreakInfo", NKMLimitBreakTemplet.LoadFromLUA);
		return m_dicLimitBreakTemplet != null;
	}

	public static bool LoadFromLua(string LBInfoFileName, string SubstituteItemFileName)
	{
		bool result = true;
		if (!LoadFromLUA_LUA_LIMITBREAK_INFO(LBInfoFileName))
		{
			Log.Error("LoadFromLUA_LUA_LIMITBREAK_INFO Fail", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitLimitBreakManager.cs", 586);
			result = false;
		}
		if (!LoadFromLUA_LIMITBREAK_SUBSTITUTE_ITEM(SubstituteItemFileName))
		{
			Log.Error("LoadFromLUA_LIMITBREAK_SUBSTITUTE_ITEM Fail", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitLimitBreakManager.cs", 593);
			result = false;
		}
		CheckValidation();
		return result;
	}

	public static int MakeKey(NKM_UNIT_STYLE_TYPE eUnitStyle, NKM_UNIT_GRADE eUnitGrade, int targetLBLevel)
	{
		return (int)eUnitStyle * 10000 + (int)eUnitGrade * 100 + targetLBLevel;
	}

	private static void CheckValidation()
	{
		foreach (KeyValuePair<int, NKMLimitBreakItemTemplet> item in m_dicLimitBreakItemTemplet)
		{
			foreach (NKMLimitBreakItemTemplet.ItemRequirement item2 in item.Value.m_lstRequiredItem)
			{
				int itemID = item2.itemID;
				int count = item2.count;
				if (NKMItemManager.GetItemMiscTempletByID(itemID) == null || count <= 0)
				{
					Log.ErrorAndExit($"[LimitBreakTemplet] 초월 아이템 정보가 존재하지 않음 eUnitStyle : {item.Value.m_NKM_UNIT_STYLE_TYPE}, eUnitGrade : {item.Value.m_NKM_UNIT_GRADE}, targetLBLevel : {item.Value.m_TargetLimitbreakLevel}, m_ItemMiscID : {itemID}, m_Count : {count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitLimitBreakManager.cs", 620);
				}
			}
		}
	}
}
