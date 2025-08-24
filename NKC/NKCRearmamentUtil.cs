using System.Collections.Generic;
using System.Linq;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;
using UnityEngine;

namespace NKC;

public static class NKCRearmamentUtil
{
	public static void Init()
	{
		NKMUnitExtractBonuseTemplet.LoadFromLua();
		NKMTempletContainer<NKMUnitRearmamentTemplet>.Load("AB_SCRIPT", "LUA_REARMAMENT_TEMPLET", "REARMAMENT_TEMPLET", NKMUnitRearmamentTemplet.LoadFromLua);
		foreach (NKMUnitRearmamentTemplet value in NKMTempletContainer<NKMUnitRearmamentTemplet>.Values)
		{
			value.Join();
		}
	}

	public static int GetFromUnitID(int rearmUnitID)
	{
		foreach (NKMUnitRearmamentTemplet value in NKMTempletContainer<NKMUnitRearmamentTemplet>.Values)
		{
			if (value.EnableByTag && value.Key == rearmUnitID)
			{
				return value.FromUnitTemplet.m_UnitID;
			}
		}
		return 0;
	}

	public static bool IsCanRearmamentUnit(long unitUID)
	{
		NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(unitUID);
		if (unitFromUID == null)
		{
			return false;
		}
		if (unitFromUID.m_UnitLevel < 110)
		{
			return false;
		}
		if (!NKMUnitLimitBreakManager.IsMaxLimitBreak(unitFromUID, 1))
		{
			return false;
		}
		return IsCanRearmamentUnit(NKMUnitManager.GetUnitTempletBase(unitFromUID.m_UnitID));
	}

	public static bool IsCanRearmamentUnit(NKMUnitTempletBase unitTempletBase)
	{
		if (unitTempletBase == null)
		{
			return false;
		}
		bool flag = false;
		foreach (NKMUnitRearmamentTemplet value in NKMTempletContainer<NKMUnitRearmamentTemplet>.Values)
		{
			if (value.EnableByTag && value.FromUnitTemplet.m_UnitID == unitTempletBase.m_UnitID)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return false;
		}
		return true;
	}

	public static bool IsHasLeaderSkill(long unitUID)
	{
		NKMUnitData unitFromUID = NKCScenManager.CurrentUserData().m_ArmyData.GetUnitFromUID(unitUID);
		if (unitFromUID == null)
		{
			return false;
		}
		return IsHasLeaderSkill(unitFromUID);
	}

	public static bool IsHasLeaderSkill(NKMUnitData unitData)
	{
		bool result = false;
		foreach (NKMUnitSkillTemplet unitAllSkillTemplet in NKMUnitSkillManager.GetUnitAllSkillTempletList(unitData))
		{
			if (unitAllSkillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_LEADER)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public static List<int> GetSameBaseUnitIDList(NKMUnitData unitData)
	{
		List<int> list = new List<int>();
		NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitData);
		if (unitTempletBase == null)
		{
			return list;
		}
		if (unitTempletBase.IsRearmUnit)
		{
			list.Add(unitTempletBase.m_BaseUnitID);
			NKMUnitRearmamentTemplet rearmamentTemplet = GetRearmamentTemplet(unitData.m_UnitID);
			if (rearmamentTemplet != null)
			{
				list.AddRange(GetSameBaseUnitIDList(rearmamentTemplet.FromUnitTemplet));
			}
		}
		else
		{
			list = GetSameBaseUnitIDList(unitTempletBase);
		}
		return list;
	}

	private static List<int> GetSameBaseUnitIDList(NKMUnitTempletBase targetBaseUnitTemplet)
	{
		List<int> list = new List<int>();
		if (IsCanRearmamentUnit(targetBaseUnitTemplet))
		{
			foreach (NKMUnitRearmamentTemplet rearmamentTargetTemplet in GetRearmamentTargetTemplets(targetBaseUnitTemplet))
			{
				if (rearmamentTargetTemplet.EnableByTag)
				{
					list.Add(rearmamentTargetTemplet.ToUnitTemplet.m_UnitID);
				}
			}
		}
		return list;
	}

	public static List<NKMUnitRearmamentTemplet> GetRearmamentTargetTemplets(NKMUnitTempletBase rearmTargetUnitTemplet)
	{
		if (rearmTargetUnitTemplet == null)
		{
			return null;
		}
		return GetRearmamentTargetTemplets(rearmTargetUnitTemplet.m_UnitID);
	}

	public static List<NKMUnitRearmamentTemplet> GetRearmamentTargetTemplets(int unitID)
	{
		List<NKMUnitRearmamentTemplet> list = new List<NKMUnitRearmamentTemplet>();
		foreach (NKMUnitRearmamentTemplet value in NKMTempletContainer<NKMUnitRearmamentTemplet>.Values)
		{
			if (value.EnableByTag && value.FromUnitTemplet.m_UnitID == unitID)
			{
				list.Add(value);
			}
		}
		list.OrderBy((NKMUnitRearmamentTemplet x) => x.Key);
		return list;
	}

	public static NKMUnitRearmamentTemplet GetRearmamentTemplet(int unitID)
	{
		foreach (NKMUnitRearmamentTemplet value in NKMTempletContainer<NKMUnitRearmamentTemplet>.Values)
		{
			if (value.EnableByTag && value.Key == unitID)
			{
				return value;
			}
		}
		return null;
	}

	public static bool IsCanUseContent()
	{
		return NKMContentsVersionManager.HasTag("REARMAMENT_BASE");
	}

	public static bool CanUseExtract()
	{
		return NKMContentsVersionManager.HasTag("REARMAMENT_EXTRACT");
	}

	public static int GetSynergyIncreasePercentage(List<long> lstUnits)
	{
		int num = 0;
		NKMArmyData armyData = NKCScenManager.CurrentUserData().m_ArmyData;
		foreach (long lstUnit in lstUnits)
		{
			NKMUnitData unitFromUID = armyData.GetUnitFromUID(lstUnit);
			if (unitFromUID != null)
			{
				NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(unitFromUID.m_UnitID);
				if (unitTempletBase.m_bAwaken)
				{
					num += NKMCommonConst.ExtractBonusRatePercent_Awaken;
				}
				else if (unitTempletBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SSR)
				{
					num += NKMCommonConst.ExtractBonusRatePercent_SSR;
				}
				else if (unitTempletBase.m_NKM_UNIT_GRADE == NKM_UNIT_GRADE.NUG_SR)
				{
					num += NKMCommonConst.ExtractBonusRatePercent_SR;
				}
			}
		}
		return Mathf.Min(num, 100);
	}
}
