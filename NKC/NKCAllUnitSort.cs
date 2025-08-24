using System.Collections.Generic;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKC;

public class NKCAllUnitSort : NKCUnitSortSystem
{
	public List<NKMUnitData> m_lstCollection;

	public NKCAllUnitSort(NKMUserData userData, UnitListOptions options)
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
		foreach (NKMUnitTempletBase value in NKMTempletContainer<NKMUnitTempletBase>.Values)
		{
			if (value.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_NORMAL)
			{
				list.Add(NKCUnitSortSystem.MakeTempUnitData(value.m_UnitID, 1, 0));
			}
		}
		return list;
	}

	protected override void BuildUnitStateCache(NKMUserData userData, NKM_DECK_TYPE eNKM_DECK_TYPE)
	{
	}
}
