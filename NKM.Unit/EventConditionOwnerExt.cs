using System.Collections.Generic;
using System.Linq;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKM.Unit;

public static class EventConditionOwnerExt
{
	public static bool Validate(this IEventConditionOwner condOwner, NKMUnitTemplet unitTemplet, NKMUnitTempletBase masterTemplet)
	{
		if (condOwner.Condition == null)
		{
			return true;
		}
		if (condOwner.Condition.m_EventConditionV2 != null)
		{
			return condOwner.Condition.m_EventConditionV2.Validate(unitTemplet, masterTemplet);
		}
		return true;
	}

	public static void ValidateSkillId(this IEventConditionOwner condOwner, NKMUnitTempletBase templet)
	{
		string name = condOwner.GetType().Name;
		if (string.IsNullOrEmpty(condOwner.Condition.m_SkillStrID) || templet.m_bMonster || condOwner.Condition.m_SkillStrID.Equals("-1"))
		{
			return;
		}
		if (templet.IsShip())
		{
			IEnumerable<NKMUnitTempletBase> enumerable = NKMUnitTempletBase.Values.Where((NKMUnitTempletBase e) => e.m_ShipGroupID == templet.m_ShipGroupID);
			bool flag = false;
			foreach (NKMUnitTempletBase item in enumerable)
			{
				if (item.GetSkillIndex(condOwner.Condition.m_SkillStrID) >= 0)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				NKMTempletError.Add("[EventCondition] " + templet.DebugName + " 함선의 " + name + "에있는 스킬아이디 (" + condOwner.Condition.m_SkillStrID + ")가 잘못되었습니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Unit/EventConditionOwner.cs", 53);
			}
		}
		else if (templet.GetSkillIndex(condOwner.Condition.m_SkillStrID) < 0)
		{
			NKMTempletError.Add("[EventCondition] " + templet.DebugName + " 유닛의 " + name + "에있는 스킬아이디 (" + condOwner.Condition.m_SkillStrID + ")가 잘못되었습니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Unit/EventConditionOwner.cs", 61);
		}
	}

	public static void ValidateMasterSkillId(this IEventConditionOwner condOwner, NKMUnitTempletBase ownerTemplet, NKMUnitTempletBase templet)
	{
		string masterSkillStrID = condOwner.Condition.m_MasterSkillStrID;
		if (string.IsNullOrEmpty(masterSkillStrID))
		{
			return;
		}
		if (ownerTemplet.IsShip())
		{
			IEnumerable<NKMUnitTempletBase> enumerable = NKMUnitTempletBase.Values.Where((NKMUnitTempletBase e) => e.m_ShipGroupID == ownerTemplet.m_ShipGroupID);
			bool flag = false;
			foreach (NKMUnitTempletBase item in enumerable)
			{
				if (item.GetSkillIndex(masterSkillStrID) >= 0)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				NKMTempletError.Add("[EventCondition] " + ownerTemplet.DebugName + " 함선이 소환하는 " + templet.DebugName + "의 마스터 스킬아이디 (" + masterSkillStrID + ")가 잘못되었습니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Unit/EventConditionOwner.cs", 90);
			}
		}
		else if (ownerTemplet.GetSkillIndex(condOwner.Condition.m_MasterSkillStrID) < 0)
		{
			NKMTempletError.Add("[EventCondition] " + ownerTemplet.DebugName + " 유닛의 소환하는 " + templet.DebugName + "의 마스터 스킬아이디 (" + masterSkillStrID + ")가 잘못되었습니다.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Unit/EventConditionOwner.cs", 97);
		}
	}
}
