using Cs.Math;
using NKM.Unit;

namespace NKM;

public class NKMEventSpeedX : IEventConditionOwner, INKMUnitStateEventRollback, INKMUnitStateEvent
{
	public NKMEventCondition m_Condition = new NKMEventCondition();

	public bool m_bFade;

	public bool m_bAnimTime = true;

	public float m_fEventTimeMin;

	public float m_fEventTimeMax;

	public bool m_bStateEndTime;

	public bool m_bAdd;

	public bool m_bMultiply;

	public float m_SpeedX;

	public NKMEventCondition Condition => m_Condition;

	public float EventStartTime => m_fEventTimeMin;

	public EventRollbackType RollbackType
	{
		get
		{
			if (m_bAdd || m_bMultiply || m_bFade)
			{
				return EventRollbackType.Prohibited;
			}
			if (m_fEventTimeMax < NKMCommonConst.SUMMON_UNIT_NOEVENT_TIME)
			{
				return EventRollbackType.Warning;
			}
			return EventRollbackType.Allowed;
		}
	}

	public bool bAnimTime => m_bAnimTime;

	public bool bStateEnd => m_bStateEndTime;

	public void DeepCopyFromSource(NKMEventSpeedX source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_bFade = source.m_bFade;
		m_bAnimTime = source.m_bAnimTime;
		m_fEventTimeMin = source.m_fEventTimeMin;
		m_fEventTimeMax = source.m_fEventTimeMax;
		m_bStateEndTime = source.m_bStateEndTime;
		m_bAdd = source.m_bAdd;
		m_bMultiply = source.m_bMultiply;
		m_SpeedX = source.m_SpeedX;
	}

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_Condition.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_bFade", ref m_bFade);
		cNKMLua.GetData("m_bAnimTime", ref m_bAnimTime);
		cNKMLua.GetData("m_fEventTimeMin", ref m_fEventTimeMin);
		cNKMLua.GetData("m_fEventTimeMax", ref m_fEventTimeMax);
		cNKMLua.GetData("m_bStateEndTime", ref m_bStateEndTime);
		cNKMLua.GetData("m_bAdd", ref m_bAdd);
		cNKMLua.GetData("m_bMultiply", ref m_bMultiply);
		cNKMLua.GetData("m_SpeedX", ref m_SpeedX);
		return true;
	}

	public float GetSpeed(float fTime, float fSpeedNow)
	{
		if (!m_bFade || m_fEventTimeMin.IsNearlyEqual(m_fEventTimeMax))
		{
			return m_SpeedX;
		}
		float num = (fTime - m_fEventTimeMin) / (m_fEventTimeMax - m_fEventTimeMin);
		return fSpeedNow + (m_SpeedX - fSpeedNow) * num;
	}

	public void ApplyEventRollback(NKMGame cNKMGame, NKMUnit cNKMUnit, float rollbackTime)
	{
		if (!m_bAnimTime || cNKMUnit.GetUnitFrameData().m_fAnimSpeed != 0f)
		{
			float num = ((!m_bAnimTime) ? m_fEventTimeMin : (m_fEventTimeMin / cNKMUnit.GetUnitFrameData().m_fAnimSpeed));
			float num2 = ((!cNKMUnit.RollbackEventTimer(m_bAnimTime, m_fEventTimeMax)) ? rollbackTime : ((!m_bAnimTime) ? m_fEventTimeMax : (m_fEventTimeMax / cNKMUnit.GetUnitFrameData().m_fAnimSpeed)));
			float num3 = m_SpeedX * (num2 - num);
			cNKMUnit.GetUnitFrameData().m_fSpeedX = m_SpeedX;
			if (cNKMUnit.GetUnitSyncData().m_bRight)
			{
				cNKMUnit.GetUnitFrameData().m_PosXCalc += num3;
			}
			else
			{
				cNKMUnit.GetUnitFrameData().m_PosXCalc -= num3;
			}
		}
	}
}
