using System.Collections.Generic;

namespace NKM;

public abstract class NKMUnitEffectStateAreaEventOneTime : NKMUnitStateAreaEventOneTime
{
	public virtual bool CheckEventCondition(NKMDamageEffect cNKMDamageEffect, bool bStateEnd)
	{
		if (!cNKMDamageEffect.CheckEventCondition(m_Condition))
		{
			return false;
		}
		if (bStateEnd)
		{
			if (!m_bStateEndTime)
			{
				return false;
			}
		}
		else
		{
			if (m_bStateEndTime)
			{
				return false;
			}
			if (!cNKMDamageEffect.EventTimer(m_bAnimTime, m_fEventTime, bOneTime: true))
			{
				return false;
			}
		}
		return true;
	}

	public void ApplyEvent(NKMGame cNKMGame, NKMDamageEffect cNKMDamageEffect)
	{
		NKMUnit masterUnit = cNKMDamageEffect.GetMasterUnit();
		float posX = cNKMDamageEffect.GetDEData().m_PosX;
		bool bRight = cNKMDamageEffect.GetDEData().m_bRight;
		if (!m_Range.HasValue())
		{
			OnAreaEventToTarget(cNKMGame, masterUnit, masterUnit);
			return;
		}
		List<NKMUnit> sortUnitListByNearDistBySize = cNKMDamageEffect.GetSortUnitListByNearDistBySize();
		int num = 0;
		for (int i = 0; i < sortUnitListByNearDistBySize.Count && (m_MaxCount <= 0 || i < m_MaxCount); i++)
		{
			NKMUnit nKMUnit = sortUnitListByNearDistBySize[i];
			if (nKMUnit.GetUnitSyncData().m_GameUnitUID == masterUnit.GetUnitSyncData().m_GameUnitUID || !nKMUnit.WillInteractWithGameUnits() || nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DYING || nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DIE || !nKMUnit.IsInRange(posX, m_Range.m_Min, m_Range.m_Max, bUseUnitSize: true, bRight) || !nKMUnit.CheckEventCondition(m_ConditionTarget, masterUnit))
			{
				continue;
			}
			OnAreaEventToTarget(cNKMGame, masterUnit, nKMUnit);
			if (m_MaxCount > 0)
			{
				num++;
				if (num >= m_MaxCount)
				{
					break;
				}
			}
		}
	}

	public virtual void ProcessEvent(NKMGame cNKMGame, NKMDamageEffect cNKMDamageEffect, bool bStateEnd)
	{
		if (CheckEventCondition(cNKMDamageEffect, bStateEnd))
		{
			ApplyEvent(cNKMGame, cNKMDamageEffect);
		}
	}
}
