using System.Collections.Generic;
using Cs.Math;
using NKM.Game;
using NKM.Unit;

namespace NKM;

public class NKMEventStatus : NKMUnitEffectStateEventOneTime, INKMUnitStateEventRollback, INKMUnitStateEvent, IEventConditionOwner
{
	public NKM_UNIT_STATUS_EFFECT m_StatusType;

	public bool m_bRemove;

	public float m_fStatusTime;

	public float m_fRange;

	public bool m_bUseUnitSize;

	public bool m_bUseTriggerTargetRange;

	public bool m_bMyTeam;

	public bool m_bEnemy;

	public int m_ApplyCount;

	public float m_fMinTargetHP;

	public float m_fMaxTargetHP;

	public NKMEventConditionV2 m_ConditionTarget;

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Server;

	public bool Validate()
	{
		if (m_StatusType == NKM_UNIT_STATUS_EFFECT.NUSE_NONE)
		{
			return false;
		}
		if (m_fStatusTime <= 0f)
		{
			return false;
		}
		return true;
	}

	public void DeepCopyFromSource(NKMEventStatus source)
	{
		DeepCopy(source);
		m_StatusType = source.m_StatusType;
		m_bRemove = source.m_bRemove;
		m_fStatusTime = source.m_fStatusTime;
		m_fRange = source.m_fRange;
		m_bUseUnitSize = source.m_bUseUnitSize;
		m_bMyTeam = source.m_bMyTeam;
		m_bEnemy = source.m_bEnemy;
		m_ApplyCount = source.m_ApplyCount;
		m_fMinTargetHP = source.m_fMinTargetHP;
		m_fMaxTargetHP = source.m_fMaxTargetHP;
		m_ConditionTarget = NKMEventConditionV2.Clone(source.m_ConditionTarget);
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_StatusType", ref m_StatusType);
		cNKMLua.GetData("m_bRemove", ref m_bRemove);
		cNKMLua.GetData("m_fStatusTime", ref m_fStatusTime);
		cNKMLua.GetData("m_fRange", ref m_fRange);
		cNKMLua.GetData("m_bUseUnitSize", ref m_bUseUnitSize);
		cNKMLua.GetData("m_bUseTriggerTargetRange", ref m_bUseTriggerTargetRange);
		cNKMLua.GetData("m_bMyTeam", ref m_bMyTeam);
		cNKMLua.GetData("m_bEnemy", ref m_bEnemy);
		cNKMLua.GetData("m_ApplyCount", ref m_ApplyCount);
		cNKMLua.GetData("m_fMinTargetHP", ref m_fMinTargetHP);
		cNKMLua.GetData("m_fMaxTargetHP", ref m_fMaxTargetHP);
		m_ConditionTarget = NKMEventConditionV2.LoadFromLUA(cNKMLua, "m_ConditionTarget");
		return true;
	}

	private void ProcessRangeStatus(NKMGame cNKMGame, NKMUnit cNKMUnit, float posX, float range, List<NKMUnit> cSortedUnitList)
	{
		int num = 0;
		for (int i = 0; i < cSortedUnitList.Count; i++)
		{
			NKMUnit nKMUnit = cSortedUnitList[i];
			if (nKMUnit == null)
			{
				continue;
			}
			if (!nKMUnit.IsInRange(posX, range, m_bUseUnitSize))
			{
				break;
			}
			if (nKMUnit.GetUnitSyncData().m_GameUnitUID != cNKMUnit.GetUnitSyncData().m_GameUnitUID && nKMUnit.WillInteractWithGameUnits() && nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DYING && nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE != NKM_UNIT_PLAY_STATE.NUPS_DIE && (m_bMyTeam || cNKMGame.IsEnemy(cNKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE, nKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE)) && (m_bEnemy || !cNKMGame.IsEnemy(cNKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE, nKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE)) && nKMUnit.CheckEventCondition(m_ConditionTarget, cNKMUnit) && !(m_fMinTargetHP > nKMUnit.GetHPRate()) && (!(m_fMaxTargetHP > 0f) || !(m_fMaxTargetHP <= nKMUnit.GetHPRate())))
			{
				num++;
				if (m_bRemove)
				{
					nKMUnit.RemoveStatus(m_StatusType);
				}
				else
				{
					nKMUnit.ApplyStatusTime(m_StatusType, m_fStatusTime, cNKMUnit);
				}
				if (m_ApplyCount > 0 && num >= m_ApplyCount)
				{
					break;
				}
			}
		}
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		if (m_fRange.IsNearlyZero())
		{
			if (!(m_fMinTargetHP > cNKMUnit.GetHPRate()) && (!(m_fMaxTargetHP > 0f) || !(m_fMaxTargetHP <= cNKMUnit.GetHPRate())))
			{
				NKMUnit triggerTargetUnit = cNKMUnit.GetTriggerTargetUnit(m_bUseTriggerTargetRange);
				if (m_bRemove)
				{
					triggerTargetUnit.RemoveStatus(m_StatusType);
				}
				else
				{
					triggerTargetUnit.ApplyStatusTime(m_StatusType, m_fStatusTime, cNKMUnit);
				}
			}
		}
		else
		{
			NKMUnit triggerTargetUnit2 = cNKMUnit.GetTriggerTargetUnit(m_bUseTriggerTargetRange);
			List<NKMUnit> sortUnitListByNearDist = triggerTargetUnit2.GetSortUnitListByNearDist(m_bUseUnitSize);
			float num = (m_bUseUnitSize ? (triggerTargetUnit2.GetUnitTemplet().m_UnitSizeX * 0.5f) : 0f);
			ProcessRangeStatus(cNKMGame, cNKMUnit, triggerTargetUnit2.GetUnitSyncData().m_PosX, m_fRange + num, sortUnitListByNearDist);
		}
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMDamageEffect cNKMDamageEffect)
	{
		List<NKMUnit> sortUnitListByNearDist = cNKMDamageEffect.GetSortUnitListByNearDist(m_bUseUnitSize);
		NKMUnit masterUnit = cNKMDamageEffect.GetMasterUnit();
		ProcessRangeStatus(cNKMGame, masterUnit, cNKMDamageEffect.GetDEData().m_PosX, m_fRange, sortUnitListByNearDist);
	}
}
