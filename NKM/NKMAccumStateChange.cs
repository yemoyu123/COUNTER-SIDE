using System.Collections.Generic;

namespace NKM;

public class NKMAccumStateChange
{
	public List<string> m_listAccumStateName = new List<string>();

	public byte m_AccumCount;

	public float m_fAccumCountCoolTime;

	public float m_fAccumMainCoolTime;

	public string m_TargetStateName = "";

	public string m_AirTargetStateName = "";

	public NKMMinMaxFloat m_fRange = new NKMMinMaxFloat(-1f, -1f);

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_listAccumStateName.Clear();
		if (cNKMLua.OpenTable("m_listAccumStateName"))
		{
			int i = 1;
			for (string rValue = ""; cNKMLua.GetData(i, ref rValue); i++)
			{
				m_listAccumStateName.Add(rValue);
			}
			cNKMLua.CloseTable();
		}
		cNKMLua.GetData("m_AccumCount", ref m_AccumCount);
		cNKMLua.GetData("m_fAccumCountCoolTime", ref m_fAccumCountCoolTime);
		cNKMLua.GetData("m_fAccumMainCoolTime", ref m_fAccumMainCoolTime);
		cNKMLua.GetData("m_TargetStateName", ref m_TargetStateName);
		cNKMLua.GetData("m_AirTargetStateName", ref m_AirTargetStateName);
		m_fRange.LoadFromLua(cNKMLua, "m_fRange");
		return true;
	}

	public void DeepCopyFromSource(NKMAccumStateChange source)
	{
		m_listAccumStateName.Clear();
		for (int i = 0; i < source.m_listAccumStateName.Count; i++)
		{
			m_listAccumStateName.Add(source.m_listAccumStateName[i]);
		}
		m_AccumCount = source.m_AccumCount;
		m_fAccumCountCoolTime = source.m_fAccumCountCoolTime;
		m_fAccumMainCoolTime = source.m_fAccumMainCoolTime;
		m_TargetStateName = source.m_TargetStateName;
		m_AirTargetStateName = source.m_AirTargetStateName;
		m_fRange.DeepCopyFromSource(source.m_fRange);
	}
}
