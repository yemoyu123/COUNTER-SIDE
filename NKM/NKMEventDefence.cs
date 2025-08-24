using NKM.Unit;

namespace NKM;

public class NKMEventDefence : IEventConditionOwner, INKMUnitStateEvent
{
	public NKMEventCondition m_Condition = new NKMEventCondition();

	public bool m_bAnimTime = true;

	public float m_fEventTimeMin;

	public float m_fEventTimeMax;

	public bool m_bDefenceFront = true;

	public bool m_bDefenceBack = true;

	public float m_fDamageReduceRate;

	public float m_fDamageReducePerSkillLevel;

	public NKMEventCondition Condition => m_Condition;

	public float EventStartTime => m_fEventTimeMin;

	public EventRollbackType RollbackType => EventRollbackType.Allowed;

	public bool bAnimTime => m_bAnimTime;

	public bool bStateEnd => false;

	public void DeepCopyFromSource(NKMEventDefence source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_bAnimTime = source.m_bAnimTime;
		m_fEventTimeMin = source.m_fEventTimeMin;
		m_fEventTimeMax = source.m_fEventTimeMax;
		m_bDefenceFront = source.m_bDefenceFront;
		m_bDefenceBack = source.m_bDefenceBack;
		m_fDamageReduceRate = source.m_fDamageReduceRate;
		m_fDamageReducePerSkillLevel = source.m_fDamageReducePerSkillLevel;
	}

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_Condition.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_bAnimTime", ref m_bAnimTime);
		cNKMLua.GetData("m_fEventTimeMin", ref m_fEventTimeMin);
		cNKMLua.GetData("m_fEventTimeMax", ref m_fEventTimeMax);
		cNKMLua.GetData("m_bDefenceFront", ref m_bDefenceFront);
		cNKMLua.GetData("m_bDefenceBack", ref m_bDefenceBack);
		cNKMLua.GetData("m_fDamageReduceRate", ref m_fDamageReduceRate);
		cNKMLua.GetData("m_fDamageReducePerSkillLevel", ref m_fDamageReducePerSkillLevel);
		return true;
	}
}
