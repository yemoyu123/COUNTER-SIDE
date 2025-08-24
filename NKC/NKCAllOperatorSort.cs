using System.Collections.Generic;
using NKM;
using NKM.Templet;
using NKM.Templet.Base;

namespace NKC;

public class NKCAllOperatorSort : NKCOperatorSortSystem
{
	public List<NKMOperator> m_lstCollection;

	public NKCAllOperatorSort(NKMUserData userData, OperatorListOptions options)
		: base(userData, options)
	{
	}

	protected override IEnumerable<NKMOperator> GetTargetOperatorList(NKMUserData userData)
	{
		if (m_lstCollection == null)
		{
			m_lstCollection = BuildUnitCollection(userData);
		}
		return m_lstCollection;
	}

	private List<NKMOperator> BuildUnitCollection(NKMUserData userData)
	{
		List<NKMOperator> list = new List<NKMOperator>();
		foreach (NKMUnitTempletBase value in NKMTempletContainer<NKMUnitTempletBase>.Values)
		{
			if (value.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_OPERATOR)
			{
				list.Add(NKCOperatorUtil.GetDummyOperator(value));
			}
		}
		return list;
	}

	protected override void BuildUnitStateCache(NKMUserData userData, NKM_DECK_TYPE eNKM_DECK_TYPE)
	{
	}
}
