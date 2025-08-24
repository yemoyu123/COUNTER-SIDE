namespace NKM;

public abstract class NKMUnitEffectStateEventOneTime : NKMUnitStateEventOneTime
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

	public abstract void ApplyEvent(NKMGame cNKMGame, NKMDamageEffect cNKMDamageEffect);

	public virtual void ProcessEvent(NKMGame cNKMGame, NKMDamageEffect cNKMDamageEffect, bool bStateEnd)
	{
		if (CheckEventCondition(cNKMDamageEffect, bStateEnd))
		{
			ApplyEvent(cNKMGame, cNKMDamageEffect);
		}
	}
}
