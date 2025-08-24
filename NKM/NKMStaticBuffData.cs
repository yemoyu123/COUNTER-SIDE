using NKM.Unit;

namespace NKM;

public class NKMStaticBuffData : IEventConditionOwner
{
	public NKMEventCondition m_Condition = new NKMEventCondition();

	public string m_BuffStrID = "";

	public byte m_BuffStatLevel = 1;

	public byte m_BuffTimeLevel = 1;

	public float m_fRebuffTime = -1f;

	public float m_fRange;

	public bool m_bMyTeam;

	public bool m_bEnemy;

	public bool m_bApplyOnSummon = true;

	public bool m_bApplyToNewUnits;

	public NKMEventCondition Condition => m_Condition;

	public NKMStaticBuffData Clone()
	{
		return MemberwiseClone() as NKMStaticBuffData;
	}

	public bool Validate()
	{
		if (!string.IsNullOrEmpty(m_BuffStrID) && NKMBuffManager.GetBuffTempletByStrID(m_BuffStrID) == null)
		{
			return false;
		}
		return true;
	}

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_Condition.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_BuffStrID", ref m_BuffStrID);
		byte rValue = 0;
		if (cNKMLua.GetData("m_BuffLevel", ref rValue))
		{
			m_BuffStatLevel = rValue;
			m_BuffTimeLevel = rValue;
		}
		cNKMLua.GetData("m_BuffStatLevel", ref m_BuffStatLevel);
		cNKMLua.GetData("m_BuffTimeLevel", ref m_BuffTimeLevel);
		cNKMLua.GetData("m_fRebuffTime", ref m_fRebuffTime);
		cNKMLua.GetData("m_fRange", ref m_fRange);
		cNKMLua.GetData("m_bMyTeam", ref m_bMyTeam);
		cNKMLua.GetData("m_bEnemy", ref m_bEnemy);
		cNKMLua.GetData("m_bApplyOnSummon", ref m_bApplyOnSummon);
		cNKMLua.GetData("m_bApplyToNewUnits", ref m_bApplyToNewUnits);
		return true;
	}

	public void DeepCopyFromSource(NKMStaticBuffData source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_BuffStrID = source.m_BuffStrID;
		m_BuffStatLevel = source.m_BuffStatLevel;
		m_BuffTimeLevel = source.m_BuffTimeLevel;
		m_fRebuffTime = source.m_fRebuffTime;
		m_fRange = source.m_fRange;
		m_bMyTeam = source.m_bMyTeam;
		m_bEnemy = source.m_bEnemy;
		m_bApplyOnSummon = source.m_bApplyOnSummon;
		m_bApplyToNewUnits = source.m_bApplyToNewUnits;
	}
}
