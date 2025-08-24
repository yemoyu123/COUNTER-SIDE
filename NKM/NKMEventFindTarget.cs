using NKM.Unit;

namespace NKM;

public class NKMEventFindTarget : NKMUnitStateEventOneTime, INKMUnitStateEventRollback, INKMUnitStateEvent, IEventConditionOwner
{
	public bool m_bSubTarget;

	public float m_fDuration = 5f;

	public NKMFindTargetData m_FindTargetData = new NKMFindTargetData();

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Server;

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_bSubTarget", ref m_bSubTarget);
		cNKMLua.GetData("m_fDuration", ref m_fDuration);
		m_FindTargetData.LoadFromLUA(cNKMLua);
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		if (m_FindTargetData.m_FindTargetType != NKM_FIND_TARGET_TYPE.NFTT_INVALID)
		{
			NKMUnit targetUnit = cNKMGame.FindTarget(cNKMUnit, cNKMUnit.GetSortUnitListByNearDist(m_FindTargetData.m_bUseUnitSize), m_FindTargetData, cNKMUnit.GetUnitDataGame().m_NKM_TEAM_TYPE, cNKMUnit.GetUnitSyncData().m_PosX, cNKMUnit.GetUnitTemplet().m_UnitSizeX, cNKMUnit.GetUnitSyncData().m_bRight);
			cNKMUnit.SetTargetUnit(targetUnit, m_fDuration, m_bSubTarget);
		}
		else if (m_bSubTarget)
		{
			cNKMUnit.GetUnitFrameData().m_fFindSubTargetTime = m_fDuration;
		}
		else
		{
			cNKMUnit.GetUnitFrameData().m_fFindTargetTime = m_fDuration;
		}
		if (!m_bSubTarget)
		{
			cNKMUnit.SeeTarget();
		}
	}

	public void DeepCopyFromSource(NKMEventFindTarget source)
	{
		DeepCopy(source);
		m_bSubTarget = source.m_bSubTarget;
		m_fDuration = source.m_fDuration;
		m_FindTargetData.DeepCopyFrom(source.m_FindTargetData);
	}
}
