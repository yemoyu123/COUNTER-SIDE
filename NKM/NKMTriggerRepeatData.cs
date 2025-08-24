using NKM.Unit;

namespace NKM;

public class NKMTriggerRepeatData : IEventConditionOwner
{
	public NKMEventCondition m_Condition = new NKMEventCondition();

	public string m_TriggerName;

	public float m_fRepeatTime;

	public NKMEventCondition Condition => m_Condition;

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_Condition.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_TriggerName", ref m_TriggerName);
		cNKMLua.GetData("m_fRepeatTime", ref m_fRepeatTime);
		return true;
	}

	public void DeepCopyFromSource(NKMTriggerRepeatData source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_TriggerName = source.m_TriggerName;
		m_fRepeatTime = source.m_fRepeatTime;
	}
}
