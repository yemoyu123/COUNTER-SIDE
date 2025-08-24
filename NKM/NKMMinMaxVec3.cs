using Cs.Logging;
using Cs.Math;

namespace NKM;

public class NKMMinMaxVec3
{
	public float m_MinX;

	public float m_MaxX;

	public float m_MinY;

	public float m_MaxY;

	public float m_MinZ;

	public float m_MaxZ;

	public void SetMinMax(float fMinX, float fMaxX, float fMinY, float fMaxY, float fMinZ, float fMaxZ)
	{
		m_MinX = fMinX;
		m_MaxX = fMaxX;
		m_MinY = fMinY;
		m_MaxY = fMaxY;
		m_MinZ = fMinZ;
		m_MaxZ = fMaxZ;
	}

	public void DeepCopyFromSource(NKMMinMaxVec3 source)
	{
		m_MinX = source.m_MinX;
		m_MaxX = source.m_MaxX;
		m_MinY = source.m_MinY;
		m_MaxY = source.m_MaxY;
		m_MinZ = source.m_MinZ;
		m_MaxZ = source.m_MaxZ;
	}

	public NKMMinMaxVec3(float fMinX = 0f, float fMaxX = 0f, float fMinY = 0f, float fMaxY = 0f, float fMinZ = 0f, float fMaxZ = 0f)
	{
		m_MinX = fMinX;
		m_MaxX = fMaxX;
		m_MinY = fMinY;
		m_MaxY = fMaxY;
		m_MinZ = fMinZ;
		m_MaxZ = fMaxZ;
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

	public float GetRandomZ()
	{
		if (m_MinZ.IsNearlyEqual(m_MaxZ))
		{
			return m_MaxZ;
		}
		return NKMRandom.Range(m_MinZ, m_MaxZ);
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
			if (cNKMLua.OpenTable(3))
			{
				cNKMLua.GetData(1, ref m_MinZ);
				cNKMLua.GetData(2, ref m_MaxZ);
				cNKMLua.CloseTable();
			}
			else
			{
				float rValue3 = m_MinZ;
				cNKMLua.GetData(3, ref rValue3);
				m_MinZ = rValue3;
				m_MaxZ = rValue3;
			}
			cNKMLua.CloseTable();
		}
		if (m_MinX > m_MaxX)
		{
			Log.Error("m_MinX > m_MaxX Key: " + pKey, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMinMax.cs", 532);
			m_MaxX = m_MinX;
			return false;
		}
		if (m_MinY > m_MaxY)
		{
			Log.Error("m_MinY > m_MaxY Key: " + pKey, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMinMax.cs", 538);
			m_MaxY = m_MinY;
			return false;
		}
		if (m_MinZ > m_MaxZ)
		{
			Log.Error("m_MinZ > m_MaxZ Key: " + pKey, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMinMax.cs", 544);
			m_MaxZ = m_MinZ;
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
			if (cNKMLua.OpenTable(3))
			{
				cNKMLua.GetData(1, ref m_MinZ);
				cNKMLua.GetData(2, ref m_MaxZ);
				cNKMLua.CloseTable();
			}
			else
			{
				float rValue3 = m_MinZ;
				cNKMLua.GetData(3, ref rValue3);
				m_MinZ = rValue3;
				m_MaxZ = rValue3;
			}
			cNKMLua.CloseTable();
		}
		if (m_MinX > m_MaxX)
		{
			Log.Error("m_MinX > m_MaxX index: " + index, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMinMax.cs", 607);
			m_MaxX = m_MinX;
			return false;
		}
		if (m_MinY > m_MaxY)
		{
			Log.Error("m_MinY > m_MaxY index: " + index, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMinMax.cs", 613);
			m_MaxY = m_MinY;
			return false;
		}
		if (m_MinZ > m_MaxZ)
		{
			Log.Error("m_MinZ > m_MaxZ index: " + index, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMMinMax.cs", 619);
			m_MaxZ = m_MinZ;
			return false;
		}
		return true;
	}
}
