using NKM.Unit;

namespace NKM;

public class NKMEventInvincibleGlobal : NKMUnitStateEventOneTime, INKMUnitStateEventRollback, INKMUnitStateEvent, IEventConditionOwner
{
	public float m_InvincibleTime;

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Server;

	public void DeepCopyFromSource(NKMEventInvincibleGlobal source)
	{
		DeepCopy(source);
		m_InvincibleTime = source.m_InvincibleTime;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_InvincibleTime", ref m_InvincibleTime);
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		cNKMUnit.ApplyStatusTime(NKM_UNIT_STATUS_EFFECT.NUSE_INVINCIBLE, m_InvincibleTime, cNKMUnit, bForceOverwrite: false, bServerOnly: false, bImmediate: true);
	}

	public override void ApplyEventRollback(NKMGame cNKMGame, NKMUnit cNKMUnit, float rollbackTime)
	{
		float eventStateTime = cNKMUnit.GetEventStateTime(m_bAnimTime, m_fEventTime);
		float time = m_InvincibleTime - (rollbackTime - eventStateTime);
		cNKMUnit.ApplyStatusTime(NKM_UNIT_STATUS_EFFECT.NUSE_INVINCIBLE, time, cNKMUnit, bForceOverwrite: false, bServerOnly: false, bImmediate: true);
	}
}
