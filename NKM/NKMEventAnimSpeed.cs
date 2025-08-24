namespace NKM;

public class NKMEventAnimSpeed : NKMUnitStateEventOneTime
{
	public float m_fAnimSpeed = 1f;

	public override EventRollbackType RollbackType
	{
		get
		{
			if (m_fEventTime != 0f)
			{
				return EventRollbackType.Prohibited;
			}
			return EventRollbackType.Warning;
		}
	}

	public override EventHostType HostType => EventHostType.Both;

	public NKMEventAnimSpeed()
	{
		m_bStateEndTime = false;
	}

	public void DeepCopyFromSource(NKMEventAnimSpeed source)
	{
		DeepCopy(source);
		m_fAnimSpeed = source.m_fAnimSpeed;
	}

	public override bool LoadFromLUA(NKMLua cNKMLua)
	{
		base.LoadFromLUA(cNKMLua);
		m_bStateEndTime = false;
		cNKMLua.GetData("m_fAnimSpeed", ref m_fAnimSpeed);
		return true;
	}

	public override void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit)
	{
		cNKMUnit.ChangeAnimSpeed(m_fAnimSpeed);
	}
}
