using NKC;
using NKM.Unit;

namespace NKM;

public abstract class NKMUnitStateEventOneTime : INKMUnitStateEventRollback, INKMUnitStateEvent, IEventConditionOwner
{
	public NKMEventCondition m_Condition = new NKMEventCondition();

	public bool m_bAnimTime = true;

	public bool m_bStateEndTime;

	public float m_fEventTime;

	public NKMEventCondition Condition => m_Condition;

	public bool bAnimTime => m_bAnimTime;

	public bool bStateEnd => m_bStateEndTime;

	public float EventStartTime => m_fEventTime;

	public abstract EventHostType HostType { get; }

	public abstract EventRollbackType RollbackType { get; }

	public virtual void ApplyEventClient(NKCGameClient cNKMGame, NKCUnitClient cNKMUnit)
	{
		ApplyEvent(cNKMGame, cNKMUnit);
	}

	public void ProcessEventClient(NKCGameClient cNKMGame, NKCUnitClient cNKMUnit, bool bStateEnd, bool bIgnoreTimer = false)
	{
		if (CheckEventCondition(cNKMUnit, bStateEnd, bIgnoreTimer))
		{
			ApplyEventClient(cNKMGame, cNKMUnit);
		}
	}

	public bool LoadFromLua(NKMLua cNKMLua, string tableName)
	{
		if (cNKMLua.OpenTable(tableName))
		{
			LoadFromLUA(cNKMLua);
			cNKMLua.CloseTable();
		}
		return true;
	}

	public virtual bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_Condition.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_bAnimTime", ref m_bAnimTime);
		cNKMLua.GetData("m_fEventTime", ref m_fEventTime);
		cNKMLua.GetData("m_bStateEndTime", ref m_bStateEndTime);
		return true;
	}

	protected void DeepCopy(NKMUnitStateEventOneTime source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_bAnimTime = source.m_bAnimTime;
		m_fEventTime = source.m_fEventTime;
		m_bStateEndTime = source.m_bStateEndTime;
	}

	public virtual bool CheckEventCondition(NKMUnit cNKMUnit, bool bStateEnd, bool bIgnoreTimer = false)
	{
		if (!bIgnoreTimer)
		{
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
				if (!cNKMUnit.EventTimer(m_bAnimTime, m_fEventTime, bOneTime: true))
				{
					return false;
				}
			}
		}
		if (!cNKMUnit.CheckEventCondition(m_Condition))
		{
			return false;
		}
		return true;
	}

	public abstract void ApplyEvent(NKMGame cNKMGame, NKMUnit cNKMUnit);

	public void ProcessEvent(NKMGame cNKMGame, NKMUnit cNKMUnit, bool bStateEnd, bool bIgnoreTimer = false)
	{
		if (CheckEventCondition(cNKMUnit, bStateEnd, bIgnoreTimer))
		{
			ApplyEvent(cNKMGame, cNKMUnit);
		}
	}

	public virtual void ApplyEventRollback(NKMGame cNKMGame, NKMUnit cNKMUnit, float rollbackTime)
	{
		ApplyEvent(cNKMGame, cNKMUnit);
	}
}
