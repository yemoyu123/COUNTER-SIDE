using NKM.Unit;

namespace NKM;

public class NKMHitBuff : IEventConditionOwner
{
	public NKMEventCondition m_Condition = new NKMEventCondition();

	public string m_HitBuff = "";

	public byte m_HitBuffStatBaseLevel = 1;

	public byte m_HitBuffStatAddLVBySkillLV;

	public byte m_HitBuffTimeBaseLevel = 1;

	public byte m_HitBuffTimeAddLVBySkillLV;

	public int m_HitBuffOverlap = 1;

	public bool m_bRemove;

	public NKMEventCondition Condition => m_Condition;

	public bool Validate()
	{
		if (!string.IsNullOrEmpty(m_HitBuff) && NKMBuffManager.GetBuffTempletByStrID(m_HitBuff) == null)
		{
			return false;
		}
		return true;
	}

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_Condition.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_HitBuff", ref m_HitBuff);
		byte rValue = 0;
		if (cNKMLua.GetData("m_HitBuffBaseLevel", ref rValue))
		{
			m_HitBuffStatBaseLevel = rValue;
			m_HitBuffTimeBaseLevel = rValue;
		}
		byte rValue2 = 0;
		if (cNKMLua.GetData("m_HitBuffAddLVBySkillLV", ref rValue2))
		{
			m_HitBuffStatAddLVBySkillLV = rValue2;
			m_HitBuffTimeAddLVBySkillLV = rValue2;
		}
		cNKMLua.GetData("m_HitBuffStatBaseLevel", ref m_HitBuffStatBaseLevel);
		cNKMLua.GetData("m_HitBuffStatAddLVBySkillLV", ref m_HitBuffStatAddLVBySkillLV);
		cNKMLua.GetData("m_HitBuffTimeBaseLevel", ref m_HitBuffTimeBaseLevel);
		cNKMLua.GetData("m_HitBuffTimeAddLVBySkillLV", ref m_HitBuffTimeAddLVBySkillLV);
		cNKMLua.GetData("m_bRemove", ref m_bRemove);
		cNKMLua.GetData("m_HitBuffOverlap", ref m_HitBuffOverlap);
		return true;
	}

	public void DeepCopyFromSource(NKMHitBuff source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_HitBuff = source.m_HitBuff;
		m_HitBuffStatBaseLevel = source.m_HitBuffStatBaseLevel;
		m_HitBuffStatAddLVBySkillLV = source.m_HitBuffStatAddLVBySkillLV;
		m_HitBuffTimeBaseLevel = source.m_HitBuffTimeBaseLevel;
		m_HitBuffTimeAddLVBySkillLV = source.m_HitBuffTimeAddLVBySkillLV;
		m_bRemove = source.m_bRemove;
		m_HitBuffOverlap = source.m_HitBuffOverlap;
	}
}
