using Cs.Logging;
using Cs.Math;

namespace NKM;

public class NKMMinMaxRGBA
{
	public float m_MinR;

	public float m_MaxR;

	public float m_MinG;

	public float m_MaxG;

	public float m_MinB;

	public float m_MaxB;

	public float m_MinA;

	public float m_MaxA;

	public void SetMinMax(float fMinR, float fMaxR, float fMinG, float fMaxG, float fMinB, float fMaxB, float fMinA, float fMaxA)
	{
		m_MinR = fMinR;
		m_MaxR = fMaxR;
		m_MinG = fMinG;
		m_MaxG = fMaxG;
		m_MinB = fMinB;
		m_MaxB = fMaxB;
		m_MinA = fMinA;
		m_MaxA = fMaxA;
	}

	public void DeepCopyFromSource(NKMMinMaxRGBA source)
	{
		m_MinR = source.m_MinR;
		m_MaxR = source.m_MaxR;
		m_MinG = source.m_MinG;
		m_MaxG = source.m_MaxG;
		m_MinB = source.m_MinB;
		m_MaxB = source.m_MaxB;
		m_MinA = source.m_MinA;
		m_MaxA = source.m_MaxA;
	}

	public NKMMinMaxRGBA(float fMinR = 1f, float fMaxR = 1f, float fMinG = 1f, float fMaxG = 1f, float fMinB = 1f, float fMaxB = 1f, float fMinA = 1f, float fMaxA = 1f)
	{
		m_MinR = fMinR;
		m_MaxR = fMaxR;
		m_MinG = fMinG;
		m_MaxG = fMaxG;
		m_MinB = fMinB;
		m_MaxB = fMaxB;
		m_MinA = fMinA;
		m_MaxA = fMaxA;
	}

	public float GetRandomR()
	{
		if (m_MinR.IsNearlyEqual(m_MaxR))
		{
			return m_MaxR;
		}
		return NKMRandom.Range(m_MinR, m_MaxR);
	}

	public float GetRandomG()
	{
		if (m_MinG.IsNearlyEqual(m_MaxG))
		{
			return m_MaxG;
		}
		return NKMRandom.Range(m_MinG, m_MaxG);
	}

	public float GetRandomB()
	{
		if (m_MinB.IsNearlyEqual(m_MaxB))
		{
			return m_MaxB;
		}
		return NKMRandom.Range(m_MinB, m_MaxB);
	}

	public float GetRandomA()
	{
		if (m_MinA.IsNearlyEqual(m_MaxA))
		{
			return m_MaxA;
		}
		return NKMRandom.Range(m_MinA, m_MaxA);
	}

	public bool LoadFromLua(NKMLua cNKMLua, string pKey)
	{
		if (cNKMLua.OpenTable(pKey))
		{
			if (cNKMLua.OpenTable(1))
			{
				cNKMLua.GetData(1, ref m_MinR);
				cNKMLua.GetData(2, ref m_MaxR);
				cNKMLua.CloseTable();
			}
			else
			{
				float rValue = m_MinR;
				cNKMLua.GetData(1, ref rValue);
				m_MinR = rValue;
				m_MaxR = rValue;
			}
			if (cNKMLua.OpenTable(2))
			{
				cNKMLua.GetData(1, ref m_MinG);
				cNKMLua.GetData(2, ref m_MaxG);
				cNKMLua.CloseTable();
			}
			else
			{
				float rValue2 = m_MinG;
				cNKMLua.GetData(2, ref rValue2);
				m_MinG = rValue2;
				m_MaxG = rValue2;
			}
			if (cNKMLua.OpenTable(3))
			{
				cNKMLua.GetData(1, ref m_MinB);
				cNKMLua.GetData(2, ref m_MaxB);
				cNKMLua.CloseTable();
			}
			else
			{
				float rValue3 = m_MinB;
				cNKMLua.GetData(3, ref rValue3);
				m_MinB = rValue3;
				m_MaxB = rValue3;
			}
			if (cNKMLua.OpenTable(4))
			{
				cNKMLua.GetData(1, ref m_MinA);
				cNKMLua.GetData(2, ref m_MaxA);
				cNKMLua.CloseTable();
			}
			else
			{
				float rValue4 = m_MinA;
				cNKMLua.GetData(4, ref rValue4);
				m_MinA = rValue4;
				m_MaxA = rValue4;
			}
			cNKMLua.CloseTable();
		}
		if (m_MinR > m_MaxR)
		{
			Log.Error("m_MinR > m_MaxR Key: " + pKey, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMinMax.cs", 794);
			m_MaxR = m_MinR;
			return false;
		}
		if (m_MinG > m_MaxG)
		{
			Log.Error("m_MinG > m_MaxG Key: " + pKey, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMinMax.cs", 800);
			m_MaxG = m_MinG;
			return false;
		}
		if (m_MinB > m_MaxB)
		{
			Log.Error("m_MinB > m_MaxB Key: " + pKey, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMinMax.cs", 806);
			m_MaxB = m_MinB;
			return false;
		}
		if (m_MinA > m_MaxA)
		{
			Log.Error("m_MinA > m_MaxA Key: " + pKey, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMinMax.cs", 812);
			m_MaxA = m_MinA;
			return false;
		}
		return true;
	}

	public bool LoadFromLua(NKMLua cNKMLua, int index)
	{
		if (cNKMLua.OpenTable(index))
		{
			if (cNKMLua.OpenTable(1))
			{
				cNKMLua.GetData(1, ref m_MinR);
				cNKMLua.GetData(2, ref m_MaxR);
				cNKMLua.CloseTable();
			}
			else
			{
				float rValue = m_MinR;
				cNKMLua.GetData(1, ref rValue);
				m_MinR = rValue;
				m_MaxR = rValue;
			}
			if (cNKMLua.OpenTable(2))
			{
				cNKMLua.GetData(1, ref m_MinG);
				cNKMLua.GetData(2, ref m_MaxG);
				cNKMLua.CloseTable();
			}
			else
			{
				float rValue2 = m_MinG;
				cNKMLua.GetData(2, ref rValue2);
				m_MinG = rValue2;
				m_MaxG = rValue2;
			}
			if (cNKMLua.OpenTable(3))
			{
				cNKMLua.GetData(1, ref m_MinB);
				cNKMLua.GetData(2, ref m_MaxB);
				cNKMLua.CloseTable();
			}
			else
			{
				float rValue3 = m_MinB;
				cNKMLua.GetData(3, ref rValue3);
				m_MinB = rValue3;
				m_MaxB = rValue3;
			}
			if (cNKMLua.OpenTable(4))
			{
				cNKMLua.GetData(1, ref m_MinA);
				cNKMLua.GetData(2, ref m_MaxA);
				cNKMLua.CloseTable();
			}
			else
			{
				float rValue4 = m_MinA;
				cNKMLua.GetData(4, ref rValue4);
				m_MinA = rValue4;
				m_MaxA = rValue4;
			}
			cNKMLua.CloseTable();
		}
		if (m_MinR > m_MaxR)
		{
			Log.Error("m_MinR > m_MaxR index: " + index, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMinMax.cs", 889);
			m_MaxR = m_MinR;
			return false;
		}
		if (m_MinG > m_MaxG)
		{
			Log.Error("m_MinG > m_MaxG index: " + index, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMinMax.cs", 895);
			m_MaxG = m_MinG;
			return false;
		}
		if (m_MinB > m_MaxB)
		{
			Log.Error("m_MinB > m_MaxB index: " + index, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMinMax.cs", 901);
			m_MaxB = m_MinB;
			return false;
		}
		if (m_MinA > m_MaxA)
		{
			Log.Error("m_MinA > m_MaxA index: " + index, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMinMax.cs", 907);
			m_MaxA = m_MinA;
			return false;
		}
		return true;
	}
}
