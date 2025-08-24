using NKM.Unit;

namespace NKM;

public class NKMEventDEStateChange : NKMUnitStateEventOneTime, INKMUnitStateEventRollback, INKMUnitStateEvent, IEventConditionOwner
{
	public string m_DamageEffectID = "";

	public string m_ChangeState = "";

	public override EventRollbackType RollbackType => EventRollbackType.Allowed;

	public override EventHostType HostType => EventHostType.Both;

	public void DeepCopyFromSource(NKMEventDEStateChange source)
	{
		DeepCopy(source);
		m_DamageEffectID = source.m_DamageEffectID;
		m_ChangeState = source.m_ChangeState;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_DamageEffectID", ref m_DamageEffectID);
		cNKMLua.GetData("m_ChangeState", ref m_ChangeState);
		return true;
	}

	public override void ApplyEventRollback(NKMGame cNKMGame, NKMUnit cNKMUnit, float rollbackTime)
	{
		foreach (NKMDamageEffect item in cNKMUnit.llDamageEffect)
		{
			if (item != null && item.GetTemplet() != null && item.GetTemplet().m_DamageEffectID.Equals(m_DamageEffectID))
			{
				item.StateChangeByUnitState(m_ChangeState);
			}
		}
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		cNKMUnit.ApplyEventDEStateChange(this);
	}
}
