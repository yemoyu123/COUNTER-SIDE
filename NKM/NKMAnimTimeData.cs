using System.Collections.Generic;

namespace NKM;

public class NKMAnimTimeData
{
	public Dictionary<string, float> m_dicAnimTime = new Dictionary<string, float>();

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		int num = 1;
		string rValue = "";
		float rValue2 = 0f;
		while (cNKMLua.OpenTable(num))
		{
			cNKMLua.GetData(1, ref rValue);
			cNKMLua.GetData(2, ref rValue2);
			if (!m_dicAnimTime.ContainsKey(rValue))
			{
				m_dicAnimTime.Add(rValue, rValue2);
			}
			num++;
			cNKMLua.CloseTable();
		}
		return true;
	}
}
