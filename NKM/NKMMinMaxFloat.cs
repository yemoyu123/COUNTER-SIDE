using Cs.Logging;
using Cs.Math;

namespace NKM;

public class NKMMinMaxFloat
{
	public float m_Min;

	public float m_Max;

	public void SetMinMax(float fMin, float fMax)
	{
		m_Min = fMin;
		m_Max = fMax;
	}

	public void DeepCopyFromSource(NKMMinMaxFloat source)
	{
		m_Min = source.m_Min;
		m_Max = source.m_Max;
	}

	public static NKMMinMaxFloat Clone(NKMMinMaxFloat source)
	{
		if (source == null)
		{
			return null;
		}
		return new NKMMinMaxFloat
		{
			m_Max = source.m_Max,
			m_Min = source.m_Min
		};
	}

	public NKMMinMaxFloat(float fMin = 0f, float fMax = 0f)
	{
		m_Min = fMin;
		m_Max = fMax;
	}

	public float GetRandom()
	{
		if (m_Min.IsNearlyEqual(m_Max))
		{
			return m_Max;
		}
		return NKMRandom.Range(m_Min, m_Max);
	}

	public bool IsBetween(float value, bool NegativeIsOpen)
	{
		if (NegativeIsOpen)
		{
			if (m_Min >= 0f && value < m_Min)
			{
				return false;
			}
			if (m_Max >= 0f && value > m_Max)
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

	public static bool IsOverlaps(float minA, float maxA, float minB, float maxB)
	{
		if (minA <= maxB)
		{
			return minB <= maxA;
		}
		return false;
	}

	public bool IsOverlaps(float min, float max)
	{
		if (m_Max >= min)
		{
			return max >= m_Min;
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
			Log.Error("m_Min > m_Max Key: " + pKey, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMinMax.cs", 100);
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
			Log.Error("m_Min > m_Max Index: " + index, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMinMax.cs", 126);
			m_Max = m_Min;
			return false;
		}
		return true;
	}

	public bool HasValue()
	{
		if (m_Min != 0f)
		{
			return m_Max != 0f;
		}
		return false;
	}
}
