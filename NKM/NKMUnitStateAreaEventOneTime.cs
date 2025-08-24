using System.Collections.Generic;
using NKM.Game;

namespace NKM;

public abstract class NKMUnitStateAreaEventOneTime : NKMUnitStateEventOneTime
{
	public NKMMinMaxFloat m_Range = new NKMMinMaxFloat();

	public NKMEventConditionV2 m_ConditionTarget;

	public bool m_bUseTriggerTargetRange;

	public int m_MaxCount;

	protected void DeepCopy(NKMUnitStateAreaEventOneTime source)
	{
		DeepCopy((NKMUnitStateEventOneTime)source);
		m_Range.DeepCopyFromSource(source.m_Range);
		m_ConditionTarget = NKMEventConditionV2.Clone(source.m_ConditionTarget);
		m_bUseTriggerTargetRange = source.m_bUseTriggerTargetRange;
		m_MaxCount = source.m_MaxCount;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		bool result = base.LoadFromLUA(cNKMLua);
		m_Range.LoadFromLua(cNKMLua, "m_Range");
		m_ConditionTarget = NKMEventConditionV2.LoadFromLUA(cNKMLua, "m_ConditionTarget");
		cNKMLua.GetData("m_bUseTriggerTargetRange ", ref m_bUseTriggerTargetRange);
		cNKMLua.GetData("m_MaxCount", ref m_MaxCount);
		return result;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		if (!m_Range.HasValue())
		{
			NKMUnit triggerTargetUnit = cNKMUnit.GetTriggerTargetUnit(m_bUseTriggerTargetRange);
			OnAreaEventToTarget(cNKMGame, cNKMUnit, triggerTargetUnit);
			return;
		}
		NKMUnit triggerTargetUnit2 = cNKMUnit.GetTriggerTargetUnit(m_bUseTriggerTargetRange);
		List<NKMUnit> sortUnitListByNearDistAndUnitSize = triggerTargetUnit2.GetSortUnitListByNearDistAndUnitSize();
		int num = 0;
		for (int i = 0; i < sortUnitListByNearDistAndUnitSize.Count; i++)
		{
			NKMUnit nKMUnit = sortUnitListByNearDistAndUnitSize[i];
			if (nKMUnit.GetUnitSyncData().m_GameUnitUID == cNKMUnit.GetUnitSyncData().m_GameUnitUID || !triggerTargetUnit2.IsInRange(nKMUnit, m_Range.m_Min, m_Range.m_Max, bUseUnitSize: true) || !nKMUnit.WillInteractWithGameUnits() || nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DYING || nKMUnit.GetUnitSyncData().m_NKM_UNIT_PLAY_STATE == NKM_UNIT_PLAY_STATE.NUPS_DIE || !nKMUnit.CheckEventCondition(m_ConditionTarget, cNKMUnit))
			{
				continue;
			}
			OnAreaEventToTarget(cNKMGame, cNKMUnit, nKMUnit);
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

	public abstract void OnAreaEventToTarget(NKMGame cNKMGame, NKMUnit eventOwner, NKMUnit target);
}
