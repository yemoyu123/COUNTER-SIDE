using System.Collections.Generic;
using NKM;
using NKM.Templet;

namespace NKC;

public class NKCUnitCollection : NKCUnitSortSystem
{
	public List<NKMUnitData> m_lstCollection;

	public NKCUnitCollection(NKMUserData userData, UnitListOptions options)
		: base(userData, options)
	{
	}

	protected override IEnumerable<NKMUnitData> GetTargetUnitList(NKMUserData userData)
	{
		if (m_lstCollection == null)
		{
			m_lstCollection = BuildUnitCollection(userData);
		}
		return m_lstCollection;
	}

	private List<NKMUnitData> BuildUnitCollection(NKMUserData userData)
	{
		List<NKMUnitData> list = new List<NKMUnitData>();
		foreach (int item in userData.m_ArmyData.m_illustrateUnit)
		{
			if (NKMUnitManager.GetUnitTempletBase(item).m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL)
			{
				list.Add(NKCUnitSortSystem.MakeTempUnitData(item, 1, 0));
			}
		}
		return list;
	}
}
