using NKM.Unit;

namespace NKM;

public class NKMEventCatchEnd : IEventConditionOwner, INKMUnitStateEvent
{
	public NKMEventCondition m_Condition = new NKMEventCondition();

	public bool m_bAnimTime = true;

	public float m_fEventTime;

	public bool m_bStateEndTime;

	public NKMEventCondition Condition => m_Condition;

	public float EventStartTime => m_fEventTime;

	public EventRollbackType RollbackType => EventRollbackType.Prohibited;

	public bool bAnimTime => m_bAnimTime;

	public bool bStateEnd => m_bStateEndTime;

	public void DeepCopyFromSource(NKMEventCatchEnd source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_bAnimTime = source.m_bAnimTime;
		m_fEventTime = source.m_fEventTime;
		m_bStateEndTime = source.m_bStateEndTime;
	}

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_Condition.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_bAnimTime", ref m_bAnimTime);
		cNKMLua.GetData("m_fEventTime", ref m_fEventTime);
		cNKMLua.GetData("m_bStateEndTime", ref m_bStateEndTime);
		return true;
	}
}
