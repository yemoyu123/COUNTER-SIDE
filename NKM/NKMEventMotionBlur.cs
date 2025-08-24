using NKM.Unit;

namespace NKM;

public class NKMEventMotionBlur : IEventConditionOwner, INKMUnitStateEvent
{
	public NKMEventCondition m_Condition = new NKMEventCondition();

	public bool m_bAnimTime = true;

	public float m_fEventTimeMin;

	public float m_fEventTimeMax;

	public float m_fColorR = 0.5f;

	public float m_fColorG = 0.5f;

	public float m_fColorB = 1f;

	public float m_fColorA = 1f;

	public int m_maxImageCount = 10;

	public float m_fGapTime = 0.1f;

	public float m_fFadeSpeed = 1.5f;

	public float m_fLifeTime = 2f;

	public NKMEventCondition Condition => m_Condition;

	public float EventStartTime => m_fEventTimeMin;

	public EventRollbackType RollbackType => EventRollbackType.Allowed;

	public bool bAnimTime => m_bAnimTime;

	public bool bStateEnd => false;

	public void DeepCopyFromSource(NKMEventMotionBlur source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_bAnimTime = source.m_bAnimTime;
		m_fEventTimeMin = source.m_fEventTimeMin;
		m_fEventTimeMax = source.m_fEventTimeMax;
		m_fColorR = source.m_fColorR;
		m_fColorG = source.m_fColorG;
		m_fColorB = source.m_fColorB;
		m_fColorA = source.m_fColorA;
		m_maxImageCount = source.m_maxImageCount;
		m_fGapTime = source.m_fGapTime;
		m_fFadeSpeed = source.m_fFadeSpeed;
		m_fLifeTime = source.m_fLifeTime;
	}

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_Condition.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_bAnimTime", ref m_bAnimTime);
		cNKMLua.GetData("m_fEventTimeMin", ref m_fEventTimeMin);
		cNKMLua.GetData("m_fEventTimeMax", ref m_fEventTimeMax);
		cNKMLua.GetData("m_fColorR", ref m_fColorR);
		cNKMLua.GetData("m_fColorG", ref m_fColorG);
		cNKMLua.GetData("m_fColorB", ref m_fColorB);
		cNKMLua.GetData("m_fColorA", ref m_fColorA);
		cNKMLua.GetData("m_maxImageCount", ref m_maxImageCount);
		cNKMLua.GetData("m_fGapTime", ref m_fGapTime);
		cNKMLua.GetData("m_fFadeSpeed", ref m_fFadeSpeed);
		cNKMLua.GetData("m_fLifeTime", ref m_fLifeTime);
		return true;
	}
}
