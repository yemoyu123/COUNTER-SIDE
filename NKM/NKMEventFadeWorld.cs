using NKM.Unit;

namespace NKM;

public class NKMEventFadeWorld : IEventConditionOwner, INKMUnitStateEvent
{
	public NKMEventCondition m_Condition = new NKMEventCondition();

	public bool m_bAnimTime = true;

	public float m_fEventTimeMin;

	public float m_fEventTimeMax;

	public float m_fColorR = 1f;

	public float m_fColorG = 1f;

	public float m_fColorB = 1f;

	public float m_fMapColorKeepTime;

	public float m_fMapColorReturnTime;

	public bool m_bHideMapObject;

	public NKMEventCondition Condition => m_Condition;

	public float EventStartTime => m_fEventTimeMin;

	public EventRollbackType RollbackType => EventRollbackType.Allowed;

	public bool bAnimTime => m_bAnimTime;

	public bool bStateEnd => false;

	public void DeepCopyFromSource(NKMEventFadeWorld source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_bAnimTime = source.m_bAnimTime;
		m_fEventTimeMin = source.m_fEventTimeMin;
		m_fEventTimeMax = source.m_fEventTimeMax;
		m_fColorR = source.m_fColorR;
		m_fColorG = source.m_fColorG;
		m_fColorB = source.m_fColorB;
		m_bHideMapObject = source.m_bHideMapObject;
		m_fMapColorKeepTime = source.m_fMapColorKeepTime;
		m_fMapColorReturnTime = source.m_fMapColorReturnTime;
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
		cNKMLua.GetData("m_bHideMapObject", ref m_bHideMapObject);
		cNKMLua.GetData("m_fMapColorKeepTime", ref m_fMapColorKeepTime);
		cNKMLua.GetData("m_fMapColorReturnTime", ref m_fMapColorReturnTime);
		return true;
	}
}
