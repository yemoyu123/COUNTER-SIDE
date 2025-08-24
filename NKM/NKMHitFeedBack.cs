using NKM.Unit;

namespace NKM;

public class NKMHitFeedBack : IEventConditionOwner
{
	public NKMEventCondition m_Condition = new NKMEventCondition();

	public bool m_bStartAnyTime;

	public byte m_Count;

	public string m_StateName = "";

	public string m_BuffStrID = "";

	public byte m_BuffStatLevel = 1;

	public byte m_BuffTimeLevel = 1;

	public string m_TargetTrigger = string.Empty;

	public NKMEventCondition Condition => m_Condition;

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_Condition.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_bStartAnyTime", ref m_bStartAnyTime);
		cNKMLua.GetData("m_Count", ref m_Count);
		byte rValue = 0;
		if (cNKMLua.GetData("m_HitCount", ref rValue))
		{
			m_Count = rValue;
		}
		cNKMLua.GetData("m_StateName", ref m_StateName);
		cNKMLua.GetData("m_BuffStrID", ref m_BuffStrID);
		byte rValue2 = 0;
		if (cNKMLua.GetData("m_BuffLevel", ref rValue2))
		{
			m_BuffStatLevel = rValue2;
			m_BuffTimeLevel = rValue2;
		}
		cNKMLua.GetData("m_BuffStatLevel", ref m_BuffStatLevel);
		cNKMLua.GetData("m_BuffTimeLevel", ref m_BuffTimeLevel);
		cNKMLua.GetData("m_TargetTrigger", ref m_TargetTrigger);
		return true;
	}

	public void DeepCopyFromSource(NKMHitFeedBack source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_Count = source.m_Count;
		m_StateName = source.m_StateName;
		m_BuffStrID = source.m_BuffStrID;
		m_BuffStatLevel = source.m_BuffStatLevel;
		m_BuffTimeLevel = source.m_BuffTimeLevel;
		m_TargetTrigger = source.m_TargetTrigger;
	}

	public bool Validate()
	{
		if (!string.IsNullOrEmpty(m_BuffStrID) && NKMBuffManager.GetBuffTempletByStrID(m_BuffStrID) == null)
		{
			return false;
		}
		return true;
	}
}
