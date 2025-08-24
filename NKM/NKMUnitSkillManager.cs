using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM;

public static class NKMUnitSkillManager
{
	private static Dictionary<int, NKMUnitSkillTempletContainer> m_dicSkillTempletContainer = new Dictionary<int, NKMUnitSkillTempletContainer>();

	private static Dictionary<string, NKMUnitSkillTempletContainer> m_dicSkillTempletContainerByStrID = new Dictionary<string, NKMUnitSkillTempletContainer>();

	public static IEnumerable<NKMUnitSkillTemplet> AllSkillTempletEnumerator()
	{
		foreach (NKMUnitSkillTempletContainer value in m_dicSkillTempletContainer.Values)
		{
			foreach (NKMUnitSkillTemplet value2 in value.dicTemplets.Values)
			{
				yield return value2;
			}
		}
	}

	public static NKMUnitSkillTempletContainer GetSkillTempletContainer(int skillID)
	{
		if (m_dicSkillTempletContainer.TryGetValue(skillID, out var value))
		{
			return value;
		}
		return null;
	}

	public static NKMUnitSkillTempletContainer GetSkillTempletContainer(string skillStrID)
	{
		if (m_dicSkillTempletContainerByStrID.TryGetValue(skillStrID, out var value))
		{
			return value;
		}
		return null;
	}

	public static IEnumerable<NKMUnitSkillTemplet> GetUnitAllSkillTemplets(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			yield break;
		}
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
		if (unitTempletBase == null)
		{
			yield break;
		}
		int iSkillCnt = 0;
		while (iSkillCnt < unitTempletBase.GetSkillCount())
		{
			string skillStrID = unitTempletBase.GetSkillStrID(iSkillCnt);
			if (!string.IsNullOrEmpty(skillStrID))
			{
				NKMUnitSkillTempletContainer skillTempletContainer = GetSkillTempletContainer(skillStrID);
				if (skillTempletContainer != null)
				{
					NKMUnitSkillTemplet skillTemplet = skillTempletContainer.GetSkillTemplet(unitData.GetSkillLevel(skillStrID));
					if (skillTemplet != null)
					{
						yield return skillTemplet;
					}
				}
			}
			int num = iSkillCnt + 1;
			iSkillCnt = num;
		}
	}

	public static List<NKMUnitSkillTemplet> GetUnitAllSkillTempletList(NKMUnitData unitData)
	{
		List<NKMUnitSkillTemplet> list = new List<NKMUnitSkillTemplet>();
		if (unitData != null)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData.m_UnitID);
			if (unitTempletBase != null)
			{
				for (int i = 0; i < unitTempletBase.GetSkillCount(); i++)
				{
					string skillStrID = unitTempletBase.GetSkillStrID(i);
					if (string.IsNullOrEmpty(skillStrID))
					{
						continue;
					}
					NKMUnitSkillTempletContainer skillTempletContainer = GetSkillTempletContainer(skillStrID);
					if (skillTempletContainer != null)
					{
						NKMUnitSkillTemplet skillTemplet = skillTempletContainer.GetSkillTemplet(unitData.GetSkillLevel(skillStrID));
						if (skillTemplet != null)
						{
							list.Add(skillTemplet);
						}
					}
				}
			}
		}
		return list;
	}

	public static NKM_ERROR_CODE CanTrainSkill(NKMUserData userData, NKMUnitData targetUnit, int skillID)
	{
		if (targetUnit == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_NOT_EXIST;
		}
		if (targetUnit.GetUnitSkillIndex(skillID) < 0)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_SKILL_NOT_EXIST;
		}
		int unitSkillLevel = targetUnit.GetUnitSkillLevel(skillID);
		NKMUnitSkillTemplet skillTemplet = GetSkillTemplet(skillID, unitSkillLevel);
		NKMUnitSkillTemplet skillTemplet2 = GetSkillTemplet(skillID, unitSkillLevel + 1);
		if (skillTemplet == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_SKILL_NOT_EXIST;
		}
		if (skillTemplet2 == null)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_SKILL_ALREADY_MAX;
		}
		if (IsLockedSkill(skillTemplet.m_ID, targetUnit.m_LimitBreakLevel))
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_SKILL_NEED_LIMIT_BREAK;
		}
		int maxSkillLevelFromLimitBreakLevel = GetMaxSkillLevelFromLimitBreakLevel(skillID, targetUnit.m_LimitBreakLevel);
		if (skillTemplet2.m_Level > maxSkillLevelFromLimitBreakLevel)
		{
			return NKM_ERROR_CODE.NEC_FAIL_UNIT_SKILL_NEED_LIMIT_BREAK;
		}
		foreach (NKMUnitSkillTemplet.NKMUpgradeReqItem item in skillTemplet2.m_lstUpgradeReqItem)
		{
			if (userData.m_InventoryData.GetCountMiscItem(item.ItemID) < item.ItemCount)
			{
				return NKM_ERROR_CODE.NEC_FAIL_UNIT_SKILL_NOT_ENOUGH_ITEM;
			}
		}
		return NKM_ERROR_CODE.NEC_OK;
	}

	public static int GetMaxSkillLevel(string skillStrID)
	{
		NKMUnitSkillTempletContainer skillTempletContainer = GetSkillTempletContainer(skillStrID);
		if (skillTempletContainer != null && skillTempletContainer.dicTemplets != null)
		{
			return skillTempletContainer.dicTemplets.Count;
		}
		return 0;
	}

	public static int GetMaxSkillLevel(int skillID)
	{
		NKMUnitSkillTempletContainer skillTempletContainer = GetSkillTempletContainer(skillID);
		if (skillTempletContainer != null && skillTempletContainer.dicTemplets != null)
		{
			return skillTempletContainer.dicTemplets.Count;
		}
		return 0;
	}

	public static int GetMaxSkillLevelFromLimitBreakLevel(int skillID, int LBLevel)
	{
		NKMUnitSkillTempletContainer skillTempletContainer = GetSkillTempletContainer(skillID);
		if (skillTempletContainer != null && skillTempletContainer.dicTemplets != null)
		{
			foreach (KeyValuePair<int, NKMUnitSkillTemplet> dicTemplet in skillTempletContainer.dicTemplets)
			{
				NKMUnitSkillTemplet value = dicTemplet.Value;
				if (value != null && value.m_UnlockReqUpgrade == LBLevel + 1)
				{
					return value.m_Level - 1;
				}
			}
			return skillTempletContainer.dicTemplets.Count;
		}
		return 0;
	}

	public static bool IsLockedSkill(int skillId, int unitLimitBreakLevel)
	{
		return GetUnlockReqUpgradeFromSkillId(skillId) > unitLimitBreakLevel;
	}

	public static int GetUnlockReqUpgradeFromSkillId(int skillId)
	{
		return GetSkillTemplet(skillId, 1)?.m_UnlockReqUpgrade ?? 0;
	}

	public static bool CheckHaveUpgradableSkill(NKMUnitData unitData)
	{
		if (unitData == null)
		{
			return false;
		}
		if (NKMUnitManager.GetUnitTempletBase(unitData) == null)
		{
			return false;
		}
		int unitSkillCount = unitData.GetUnitSkillCount();
		for (int i = 0; i < unitSkillCount; i++)
		{
			NKMUnitSkillTemplet unitSkillTempletByIndex = unitData.GetUnitSkillTempletByIndex(i);
			if (unitSkillTempletByIndex == null)
			{
				Log.Error("Unit has skill but can't find templet. unitID : " + unitData.m_UnitID, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitSkillManager.cs", 388);
			}
			else
			{
				if (IsLockedSkill(unitSkillTempletByIndex.m_ID, unitData.m_LimitBreakLevel))
				{
					continue;
				}
				string strID = unitSkillTempletByIndex.m_strID;
				int level = unitSkillTempletByIndex.m_Level;
				NKMUnitSkillTemplet skillTemplet = GetSkillTemplet(strID, level + 1);
				if (skillTemplet != null)
				{
					int maxSkillLevelFromLimitBreakLevel = GetMaxSkillLevelFromLimitBreakLevel(unitSkillTempletByIndex.m_ID, unitData.m_LimitBreakLevel);
					if (skillTemplet.m_Level <= maxSkillLevelFromLimitBreakLevel)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static NKMUnitSkillTemplet GetUnitSkillTemplet(string strID, NKMUnitData unitData)
	{
		if (string.IsNullOrEmpty(strID))
		{
			return null;
		}
		int skillLevel = unitData.GetSkillLevel(strID);
		return GetSkillTemplet(strID, skillLevel);
	}

	public static NKMUnitSkillTemplet GetUnitSkillTemplet(string strID, int skillLevel)
	{
		if (string.IsNullOrEmpty(strID))
		{
			return null;
		}
		return GetSkillTemplet(strID, skillLevel);
	}

	public static NKMUnitSkillTemplet GetUnitSkillTemplet(int skillID, NKMUnitData unitData)
	{
		return GetUnitSkillTemplet(GetSkillStrID(skillID), unitData);
	}

	public static string GetSkillStrID(int ID)
	{
		NKMUnitSkillTempletContainer skillTempletContainer = GetSkillTempletContainer(ID);
		if (skillTempletContainer != null)
		{
			return skillTempletContainer.SkillStrID;
		}
		Log.Error("Skill Templet for ID " + ID + " Not Found", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitSkillManager.cs", 445);
		return string.Empty;
	}

	public static int GetSkillID(string strID)
	{
		return GetSkillTempletContainer(strID)?.SkillID ?? (-1);
	}

	public static NKMUnitSkillTemplet GetSkillTemplet(int ID, int level)
	{
		return GetSkillTempletContainer(ID)?.GetSkillTemplet(level);
	}

	public static NKMUnitSkillTemplet GetSkillTemplet(string strID, int level)
	{
		if (string.IsNullOrEmpty(strID))
		{
			return null;
		}
		return GetSkillTempletContainer(strID)?.GetSkillTemplet(level);
	}

	public static bool LoadFromLUA(string filename)
	{
		m_dicSkillTempletContainer?.Clear();
		m_dicSkillTempletContainerByStrID?.Clear();
		IEnumerable<NKMUnitSkillTemplet> enumerable = NKMTempletLoader.LoadCommonPath("AB_SCRIPT_UNIT_DATA", filename, "m_UnitSkillTemplet", NKMUnitSkillTemplet.LoadFromLUA);
		if (enumerable == null)
		{
			Log.ErrorAndExit("Cannot found " + filename + "-m_UnitSkillTemplet", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitSkillManager.cs", 492);
			return false;
		}
		foreach (NKMUnitSkillTemplet item in enumerable)
		{
			if (!m_dicSkillTempletContainer.TryGetValue(item.m_ID, out var value))
			{
				value = new NKMUnitSkillTempletContainer(item.m_ID, item.m_strID);
				m_dicSkillTempletContainer.Add(item.m_ID, value);
				m_dicSkillTempletContainerByStrID.Add(item.m_strID, value);
			}
			if (value.SkillStrID != item.m_strID)
			{
				Log.ErrorAndExit($"Skill ID and SkillStrID Mismatch : ID {item.m_ID}, StrID {item.m_strID}, level {item.m_Level}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMUnitSkillManager.cs", 508);
			}
			value.AddSkillTemplet(item);
		}
		return true;
	}
}
