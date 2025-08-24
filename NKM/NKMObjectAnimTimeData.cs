using System.Collections.Generic;
using Cs.Logging;

namespace NKM;

public class NKMObjectAnimTimeData
{
	public Dictionary<string, NKMAnimTimeData> m_dicObjectAnim = new Dictionary<string, NKMAnimTimeData>();

	public float GetAnimTimeMax(string objectName, string animName)
	{
		if (!m_dicObjectAnim.ContainsKey(objectName))
		{
			Log.Error("NKMAnimDataManager No Exist Anim: " + objectName + " : " + animName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMAnimDataManager.cs", 48);
			return 0f;
		}
		NKMAnimTimeData nKMAnimTimeData = m_dicObjectAnim[objectName];
		if (nKMAnimTimeData == null)
		{
			Log.Error("NKMAnimDataManager No Exist Anim: " + objectName + " : " + animName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMAnimDataManager.cs", 55);
			return 0f;
		}
		if (!nKMAnimTimeData.m_dicAnimTime.ContainsKey(animName))
		{
			Log.Error("NKMAnimDataManager No Exist Anim: " + objectName + " : " + animName, "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/NKMAnimDataManager.cs", 61);
			return 0f;
		}
		return nKMAnimTimeData.m_dicAnimTime[animName];
	}

	public void SetAnimTimeMax(string objectName, string animName, float animTimeMax)
	{
		NKMAnimTimeData nKMAnimTimeData = null;
		if (!m_dicObjectAnim.ContainsKey(objectName))
		{
			nKMAnimTimeData = new NKMAnimTimeData();
			m_dicObjectAnim.Add(objectName, nKMAnimTimeData);
		}
		nKMAnimTimeData = m_dicObjectAnim[objectName];
		if (!nKMAnimTimeData.m_dicAnimTime.ContainsKey(animName))
		{
			nKMAnimTimeData.m_dicAnimTime.Add(animName, 0f);
		}
		nKMAnimTimeData.m_dicAnimTime[animName] = animTimeMax;
	}

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		string rValue = "";
		cNKMLua.GetData("animName", ref rValue);
		if (!m_dicObjectAnim.ContainsKey(rValue))
		{
			NKMAnimTimeData nKMAnimTimeData = new NKMAnimTimeData();
			nKMAnimTimeData.LoadFromLUA(cNKMLua);
			m_dicObjectAnim.Add(rValue, nKMAnimTimeData);
		}
		return true;
	}
}
