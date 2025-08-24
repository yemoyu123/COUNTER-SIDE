using Cs.Logging;

namespace NKM;

public class NKMMinMaxInt
{
	public int m_Min;

	public int m_Max;

	public void DeepCopyFromSource(NKMMinMaxInt source)
	{
		m_Min = source.m_Min;
		m_Max = source.m_Max;
	}

	public NKMMinMaxInt(int iMin = 0, int iMax = 0)
	{
		m_Min = iMin;
		m_Max = iMax;
	}

	public int GetRandom()
	{
		if (m_Min == m_Max)
		{
			return m_Max;
		}
		return NKMRandom.Range(m_Min, m_Max + 1);
	}

	public bool IsBetween(int value, bool negativeIsTrue)
	{
		if (negativeIsTrue)
		{
			if (m_Min >= 0 && value < m_Min)
			{
				return false;
			}
			if (m_Max >= 0 && value > m_Max)
			{
				return false;
			}
			return true;
		}
		if (m_Min <= value)
		{
			return value <= m_Max;
		}
		return false;
	}

	public bool LoadFromLua(NKMLua cNKMLua, string pKey)
	{
		if (cNKMLua.OpenTable(pKey))
		{
			cNKMLua.GetData(1, ref m_Min);
			cNKMLua.GetData(2, ref m_Max);
			cNKMLua.CloseTable();
		}
		else if (cNKMLua.GetData(pKey, ref m_Min))
		{
			m_Max = m_Min;
		}
		if (m_Min > m_Max)
		{
			Log.Error("m_Min > m_Max Key: " + pKey, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMinMax.cs", 203);
			m_Max = m_Min;
			return false;
		}
		return true;
	}

	public bool LoadFromLua(NKMLua cNKMLua, int index)
	{
		if (cNKMLua.OpenTable(index))
		{
			cNKMLua.GetData(1, ref m_Min);
			cNKMLua.GetData(2, ref m_Max);
			cNKMLua.CloseTable();
		}
		else if (cNKMLua.GetData(index, ref m_Min))
		{
			m_Max = m_Min;
		}
		if (m_Min > m_Max)
		{
			Log.Error("m_Min > m_Max Index: " + index, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMinMax.cs", 229);
			m_Max = m_Min;
			return false;
		}
		return true;
	}
}
