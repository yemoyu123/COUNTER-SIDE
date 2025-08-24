using NKM.Unit;

namespace NKM;

public class NKMKillFeedBack : IEventConditionOwner
{
	public NKMEventCondition m_Condition = new NKMEventCondition();

	public float m_fSkillCoolTime;

	public float m_fHyperSkillCoolTime;

	public float m_fSkillCoolTimeAdd;

	public float m_fHyperSkillCoolTimeAdd;

	public float m_fHPRate;

	public string m_BuffName = "";

	public byte m_BuffStatLevel = 1;

	public byte m_BuffTimeLevel = 1;

	public string m_TargetTrigger = string.Empty;

	public NKMEventCondition Condition => m_Condition;

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_Condition.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_fSkillCoolTime", ref m_fSkillCoolTime);
		cNKMLua.GetData("m_fHyperSkillCoolTime", ref m_fHyperSkillCoolTime);
		cNKMLua.GetData("m_fSkillCoolTimeAdd", ref m_fSkillCoolTimeAdd);
		cNKMLua.GetData("m_fHyperSkillCoolTimeAdd", ref m_fHyperSkillCoolTimeAdd);
		cNKMLua.GetData("m_fHPRate", ref m_fHPRate);
		cNKMLua.GetData("m_BuffName", ref m_BuffName);
		byte rValue = 0;
		if (cNKMLua.GetData("m_BuffLevel", ref rValue))
		{
			m_BuffStatLevel = rValue;
			m_BuffTimeLevel = rValue;
		}
		cNKMLua.GetData("m_BuffStatLevel", ref m_BuffStatLevel);
		cNKMLua.GetData("m_BuffTimeLevel", ref m_BuffTimeLevel);
		cNKMLua.GetData("m_TargetTrigger", ref m_TargetTrigger);
		return true;
	}

	public void DeepCopyFromSource(NKMKillFeedBack source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_fSkillCoolTime = source.m_fSkillCoolTime;
		m_fHyperSkillCoolTime = source.m_fHyperSkillCoolTime;
		m_fSkillCoolTimeAdd = source.m_fSkillCoolTimeAdd;
		m_fHyperSkillCoolTimeAdd = source.m_fHyperSkillCoolTimeAdd;
		m_fHPRate = source.m_fHPRate;
		m_BuffName = source.m_BuffName;
		m_BuffStatLevel = source.m_BuffStatLevel;
		m_BuffTimeLevel = source.m_BuffTimeLevel;
		m_TargetTrigger = source.m_TargetTrigger;
	}

	public bool Validate()
	{
		if (!string.IsNullOrEmpty(m_BuffName) && NKMBuffManager.GetBuffTempletByStrID(m_BuffName) == null)
		{
			return false;
		}
		return true;
	}
}
