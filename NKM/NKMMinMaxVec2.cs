using Cs.Logging;
using Cs.Math;

namespace NKM;

public class NKMMinMaxVec2
{
	public float m_MinX;

	public float m_MaxX;

	public float m_MinY;

	public float m_MaxY;

	public void SetMinMax(float fMinX, float fMaxX, float fMinY, float fMaxY)
	{
		m_MinX = fMinX;
		m_MaxX = fMaxX;
		m_MinY = fMinY;
		m_MaxY = fMaxY;
	}

	public void DeepCopyFromSource(NKMMinMaxVec2 source)
	{
		m_MinX = source.m_MinX;
		m_MaxX = source.m_MaxX;
		m_MinY = source.m_MinY;
		m_MaxY = source.m_MaxY;
	}

	public NKMMinMaxVec2(float fMinX = 0f, float fMaxX = 0f, float fMinY = 0f, float fMaxY = 0f)
	{
		m_MinX = fMinX;
		m_MaxX = fMaxX;
		m_MinY = fMinY;
		m_MaxY = fMaxY;
	}

	public float GetRandomX()
	{
		if (m_MinX.IsNearlyEqual(m_MaxX))
		{
			return m_MaxX;
		}
		return NKMRandom.Range(m_MinX, m_MaxX);
	}

	public float GetRandomY()
	{
		if (m_MinY.IsNearlyEqual(m_MaxY))
		{
			return m_MaxY;
		}
		return NKMRandom.Range(m_MinY, m_MaxY);
	}

	public bool LoadFromLua(NKMLua cNKMLua, string pKey)
	{
		if (cNKMLua.OpenTable(pKey))
		{
			if (cNKMLua.OpenTable(1))
			{
				cNKMLua.GetData(1, ref m_MinX);
				cNKMLua.GetData(2, ref m_MaxX);
				cNKMLua.CloseTable();
			}
			else
			{
				float rValue = m_MinX;
				cNKMLua.GetData(1, ref rValue);
				m_MinX = rValue;
				m_MaxX = rValue;
			}
			if (cNKMLua.OpenTable(2))
			{
				cNKMLua.GetData(1, ref m_MinY);
				cNKMLua.GetData(2, ref m_MaxY);
				cNKMLua.CloseTable();
			}
			else
			{
				float rValue2 = m_MinY;
				cNKMLua.GetData(2, ref rValue2);
				m_MinY = rValue2;
				m_MaxY = rValue2;
			}
			cNKMLua.CloseTable();
		}
		if (m_MinX > m_MaxX)
		{
			Log.Error("m_MinX > m_MaxX Key: " + pKey, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMinMax.cs", 332);
			m_MaxX = m_MinX;
			return false;
		}
		if (m_MinY > m_MaxY)
		{
			Log.Error("m_MinY > m_MaxY Key: " + pKey, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMinMax.cs", 338);
			m_MaxY = m_MinY;
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
				cNKMLua.GetData(1, ref m_MinX);
				cNKMLua.GetData(2, ref m_MaxX);
				cNKMLua.CloseTable();
			}
			else
			{
				float rValue = m_MinX;
				cNKMLua.GetData(1, ref rValue);
				m_MinX = rValue;
				m_MaxX = rValue;
			}
			if (cNKMLua.OpenTable(2))
			{
				cNKMLua.GetData(1, ref m_MinY);
				cNKMLua.GetData(2, ref m_MaxY);
				cNKMLua.CloseTable();
			}
			else
			{
				float rValue2 = m_MinY;
				cNKMLua.GetData(2, ref rValue2);
				m_MinY = rValue2;
				m_MaxY = rValue2;
			}
			cNKMLua.CloseTable();
		}
		if (m_MinX > m_MaxX)
		{
			Log.Error("m_MinX > m_MaxX index: " + index, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMinMax.cs", 387);
			m_MaxX = m_MinX;
			return false;
		}
		if (m_MinY > m_MaxY)
		{
			Log.Error("m_MinY > m_MaxY index: " + index, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMinMax.cs", 393);
			m_MaxY = m_MinY;
			return false;
		}
		return true;
	}
}
