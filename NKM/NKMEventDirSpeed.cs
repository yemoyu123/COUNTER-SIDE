using Cs.Math;
using NKM.Unit;

namespace NKM;

public class NKMEventDirSpeed : IEventConditionOwner, INKMUnitStateEvent
{
	public NKMEventCondition m_Condition = new NKMEventCondition();

	public bool m_bFade;

	public bool m_bAnimTime = true;

	public float m_fEventTimeMin;

	public float m_fEventTimeMax;

	public bool m_bStateEndTime;

	public float m_fDirSpeed;

	public NKMEventCondition Condition => m_Condition;

	public float EventStartTime => m_fEventTimeMin;

	public EventRollbackType RollbackType => EventRollbackType.DEOnly;

	public bool bAnimTime => m_bAnimTime;

	public bool bStateEnd => m_bStateEndTime;

	public void DeepCopyFromSource(NKMEventDirSpeed source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_bFade = source.m_bFade;
		m_bAnimTime = source.m_bAnimTime;
		m_fEventTimeMin = source.m_fEventTimeMin;
		m_fEventTimeMax = source.m_fEventTimeMax;
		m_bStateEndTime = source.m_bStateEndTime;
		m_fDirSpeed = source.m_fDirSpeed;
	}

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_Condition.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_bFade", ref m_bFade);
		cNKMLua.GetData("m_bAnimTime", ref m_bAnimTime);
		cNKMLua.GetData("m_fEventTimeMin", ref m_fEventTimeMin);
		cNKMLua.GetData("m_fEventTimeMax", ref m_fEventTimeMax);
		cNKMLua.GetData("m_bStateEndTime", ref m_bStateEndTime);
		cNKMLua.GetData("m_fDirSpeed", ref m_fDirSpeed);
		return true;
	}

	public float GetSpeed(float fTime, float fSpeedNow)
	{
		if (!m_bFade || m_fEventTimeMin.IsNearlyEqual(m_fEventTimeMax))
		{
			return m_fDirSpeed;
		}
		float num = (fTime - m_fEventTimeMin) / (m_fEventTimeMax - m_fEventTimeMin);
		return fSpeedNow + (m_fDirSpeed - fSpeedNow) * num;
	}
}
