using System.Collections.Generic;
using NKM.Unit;

namespace NKM;

public class NKMEventUnitChange : IEventConditionOwner, INKMUnitStateEvent
{
	public NKMEventCondition m_Condition = new NKMEventCondition();

	public bool m_bAnimTime = true;

	public float m_fEventTime;

	public bool m_bStateEndTime;

	public string m_UnitStrID = "";

	public Dictionary<int, int> m_dicSummonSkin;

	public NKMEventCondition Condition => m_Condition;

	public float EventStartTime => m_fEventTime;

	public EventRollbackType RollbackType => EventRollbackType.Prohibited;

	public bool bAnimTime => m_bAnimTime;

	public bool bStateEnd => m_bStateEndTime;

	public void DeepCopyFromSource(NKMEventUnitChange source)
	{
		m_Condition.DeepCopyFromSource(source.m_Condition);
		m_bAnimTime = source.m_bAnimTime;
		m_fEventTime = source.m_fEventTime;
		m_bStateEndTime = source.m_bStateEndTime;
		m_UnitStrID = source.m_UnitStrID;
		if (source.m_dicSummonSkin != null)
		{
			m_dicSummonSkin = new Dictionary<int, int>(source.m_dicSummonSkin);
		}
		else
		{
			m_dicSummonSkin = null;
		}
	}

	public bool LoadFromLUA(NKMLua cNKMLua)
	{
		m_Condition.LoadFromLUA(cNKMLua);
		cNKMLua.GetData("m_bAnimTime", ref m_bAnimTime);
		cNKMLua.GetData("m_fEventTime", ref m_fEventTime);
		cNKMLua.GetData("m_bStateEndTime", ref m_bStateEndTime);
		cNKMLua.GetData("m_UnitStrID", ref m_UnitStrID);
		if (cNKMLua.OpenTable("m_dicSummonedUnitSkin"))
		{
			m_dicSummonSkin = new Dictionary<int, int>();
			int num = 1;
			while (cNKMLua.OpenTable(num))
			{
				if (cNKMLua.GetData(1, out var rValue, 0) && cNKMLua.GetData(2, out var rValue2, 0))
				{
					m_dicSummonSkin.Add(rValue, rValue2);
				}
				num++;
				cNKMLua.CloseTable();
			}
			cNKMLua.CloseTable();
		}
		return true;
	}
}
