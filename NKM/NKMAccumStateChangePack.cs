using System.Collections.Generic;
using NKM.Unit;

namespace NKM;

public class NKMAccumStateChangePack : IEventConditionOwner
{
	public NKMEventCondition m_Condition = new NKMEventCondition();

	public List<NKMAccumStateChange> m_listAccumStateChange = new List<NKMAccumStateChange>();

	public NKMEventCondition Condition => m_Condition;

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_Condition.LoadFromLUA(cNKMLua);
		if (cNKMLua.OpenTable("m_listAccumStateChange"))
		{
			int num = 1;
			while (cNKMLua.OpenTable(num))
			{
				NKMAccumStateChange nKMAccumStateChange = null;
				if (m_listAccumStateChange.Count < num)
				{
					nKMAccumStateChange = new NKMAccumStateChange();
					m_listAccumStateChange.Add(nKMAccumStateChange);
				}
				else
				{
					nKMAccumStateChange = m_listAccumStateChange[num - 1];
				}
				nKMAccumStateChange.LoadFromLUA(cNKMLua);
				num++;
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		return true;
	}

	public void DeepCopyFromSource(NKMAccumStateChangePack source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_listAccumStateChange.Clear();
		for (int i = 0; i < source.m_listAccumStateChange.Count; i++)
		{
			NKMAccumStateChange nKMAccumStateChange = new NKMAccumStateChange();
			nKMAccumStateChange.DeepCopyFromSource(source.m_listAccumStateChange[i]);
			m_listAccumStateChange.Add(nKMAccumStateChange);
		}
	}
}
