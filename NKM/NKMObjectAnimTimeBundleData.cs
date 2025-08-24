using System;
using System.Collections.Generic;
using Cs.Logging;

namespace NKM;

public class NKMObjectAnimTimeBundleData
{
	public Dictionary<string, NKMObjectAnimTimeData> m_dicObjectAnimBundleData = new Dictionary<string, NKMObjectAnimTimeData>(StringComparer.OrdinalIgnoreCase);

	public Dictionary<string, NKMObjectAnimTimeData> GetAnimBundleData => m_dicObjectAnimBundleData;

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		int num = 1;
		string rValue = "";
		while (cNKMLua.OpenTable(num))
		{
			cNKMLua.GetData("bundleName", ref rValue);
			GetBundleData(rValue).LoadFromLUA(cNKMLua);
			num++;
			cNKMLua.CloseTable();
		}
		return true;
	}

	public NKMObjectAnimTimeData GetBundleData(string bundleName)
	{
		if (m_dicObjectAnimBundleData.ContainsKey(bundleName))
		{
			return m_dicObjectAnimBundleData[bundleName];
		}
		NKMObjectAnimTimeData nKMObjectAnimTimeData = new NKMObjectAnimTimeData();
		m_dicObjectAnimBundleData.Add(bundleName, nKMObjectAnimTimeData);
		return nKMObjectAnimTimeData;
	}

	public float GetAnimTimeMax(string bundleName, string objectName, string animName)
	{
		if (!m_dicObjectAnimBundleData.ContainsKey(bundleName))
		{
			Log.Error("NKMAnimDataManager No Exist ANIM Bundle: " + bundleName + " : " + objectName + " : " + animName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMAnimDataManager.cs", 147);
			return 0f;
		}
		NKMObjectAnimTimeData nKMObjectAnimTimeData = m_dicObjectAnimBundleData[bundleName];
		if (nKMObjectAnimTimeData == null)
		{
			Log.Error("NKMAnimDataManager No Exist ANIM Bundle: " + bundleName + " : " + objectName + " : " + animName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMAnimDataManager.cs", 154);
			return 0f;
		}
		return nKMObjectAnimTimeData.GetAnimTimeMax(objectName, animName);
	}

	public void SetAnimTimeMax(string bundleName, string objectName, string animName, float animTimeMax)
	{
		GetBundleData(bundleName).SetAnimTimeMax(objectName, animName, animTimeMax);
	}
}
