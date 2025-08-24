using NKM.Unit;

namespace NKM;

public class NKMCommonStateData : IEventConditionOwner
{
	public NKMEventCondition m_Condition = new NKMEventCondition();

	public string m_StateName = "";

	public float m_fUseHPRateOver;

	public float m_fUseHPRateUnder;

	public int m_PhaseOver = -1;

	public int m_PhaseLess = -1;

	public int m_Ratio = 1;

	public NKMEventCondition Condition => m_Condition;

	public bool CanUseState(float fHPRate, int phaseNow)
	{
		if (m_StateName.Length <= 1)
		{
			return false;
		}
		if (m_fUseHPRateOver > 0f && m_fUseHPRateOver > fHPRate)
		{
			return false;
		}
		if (m_fUseHPRateUnder > 0f && m_fUseHPRateUnder < fHPRate)
		{
			return false;
		}
		if (m_PhaseOver > -1 && m_PhaseOver > phaseNow)
		{
			return false;
		}
		if (m_PhaseLess > -1 && m_PhaseLess <= phaseNow)
		{
			return false;
		}
		return true;
	}

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_Condition.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_StateName", ref m_StateName);
		cNKMLua.GetData("m_fUseHPRateOver", ref m_fUseHPRateOver);
		cNKMLua.GetData("m_fUseHPRateUnder", ref m_fUseHPRateUnder);
		cNKMLua.GetData("m_PhaseOver", ref m_PhaseOver);
		cNKMLua.GetData("m_PhaseLess", ref m_PhaseLess);
		cNKMLua.GetData("m_Ratio", ref m_Ratio);
		return true;
	}

	public void DeepCopyFromSource(NKMCommonStateData source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_StateName = source.m_StateName;
		m_fUseHPRateOver = source.m_fUseHPRateOver;
		m_fUseHPRateUnder = source.m_fUseHPRateUnder;
		m_PhaseOver = source.m_PhaseOver;
		m_PhaseLess = source.m_PhaseLess;
		m_Ratio = source.m_Ratio;
	}
}
