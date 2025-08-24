using NKM.Unit;

namespace NKM;

public class NKMEventTargetDirSpeed : INKMUnitStateEvent, IEventConditionOwner
{
	public bool m_bAnimTime = true;

	public float m_fEventTime;

	public float m_fChangeTime;

	public float m_fTargetDirSpeed;

	public float EventStartTime => m_fEventTime;

	public EventRollbackType RollbackType => EventRollbackType.DEOnly;

	public bool bAnimTime => m_bAnimTime;

	public bool bStateEnd => false;

	public NKMEventCondition Condition => null;

	public void DeepCopyFromSource(NKMEventTargetDirSpeed source)
	{
		m_bAnimTime = source.m_bAnimTime;
		m_fEventTime = source.m_fEventTime;
		m_fChangeTime = source.m_fChangeTime;
		m_fTargetDirSpeed = source.m_fTargetDirSpeed;
	}

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		cNKMLua.GetData("m_bAnimTime", ref m_bAnimTime);
		cNKMLua.GetData("m_fEventTime", ref m_fEventTime);
		cNKMLua.GetData("m_fChangeTime", ref m_fChangeTime);
		cNKMLua.GetData("m_fTargetDirSpeed", ref m_fTargetDirSpeed);
		return true;
	}
}
