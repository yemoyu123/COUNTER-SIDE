using System.Collections.Generic;
using Cs.Logging;
using NKM.Templet;

namespace NKM;

public class NKMRandomGradeTemplet
{
	public int m_RandomGradeID;

	public string m_RandomGradeStrID = "";

	public List<NKM_UNIT_GRADE> m_listGrade = new List<NKM_UNIT_GRADE>();

	public int m_iMaxSalaryLevel;

	public Dictionary<int, RatioData> m_dicRatioList = new Dictionary<int, RatioData>();

	public void LoadFromLUA(NKMLua cNKMLua)
	{
		cNKMLua.GetData("m_RandomGradeID", ref m_RandomGradeID);
		cNKMLua.GetData("m_RandomGradeStrID", ref m_RandomGradeStrID);
		int rValue = 0;
		cNKMLua.GetData("Ratio_SSR", ref rValue);
		for (int i = 0; i < rValue; i++)
		{
			m_listGrade.Add(NKM_UNIT_GRADE.NUG_SSR);
		}
		int rValue2 = 0;
		cNKMLua.GetData("Ratio_SR", ref rValue2);
		for (int j = 0; j < rValue2; j++)
		{
			m_listGrade.Add(NKM_UNIT_GRADE.NUG_SR);
		}
		int rValue3 = 0;
		cNKMLua.GetData("Ratio_R", ref rValue3);
		for (int k = 0; k < rValue3; k++)
		{
			m_listGrade.Add(NKM_UNIT_GRADE.NUG_R);
		}
		while (m_listGrade.Count < 1000)
		{
			m_listGrade.Add(NKM_UNIT_GRADE.NUG_N);
		}
		int num = 0;
		num = 10000 - (rValue + rValue2 + rValue3);
		cNKMLua.GetData("m_SalaryLevel", ref m_iMaxSalaryLevel);
		m_dicRatioList.Add(m_iMaxSalaryLevel, new RatioData((float)rValue * 0.01f, (float)rValue2 * 0.01f, (float)rValue3 * 0.01f, (float)num * 0.01f));
	}

	public RatioData GetLastData()
	{
		return m_dicRatioList[m_iMaxSalaryLevel];
	}

	public void MergeData(int salaryLv, RatioData data)
	{
		if (!m_dicRatioList.ContainsKey(salaryLv))
		{
			m_dicRatioList.Add(salaryLv, data);
			m_iMaxSalaryLevel = salaryLv;
		}
	}

	public NKM_UNIT_GRADE GetRandomGrade()
	{
		if (m_listGrade.Count != 100)
		{
			Log.Error($"NKMRandomGrade m_listGrade count is not 100 m_RandomGradeStrID: {m_RandomGradeStrID}, count: {m_listGrade.Count}", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMRandomGradeManager.cs", 98);
			return NKM_UNIT_GRADE.NUG_N;
		}
		return m_listGrade[NKMRandom.Range(0, m_listGrade.Count)];
	}
}
