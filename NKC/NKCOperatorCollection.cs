using System.Collections.Generic;
using NKM;
using NKM.Templet;

namespace NKC;

public class NKCOperatorCollection : NKCOperatorSortSystem
{
	public List<NKMOperator> m_lstCollection;

	public NKCOperatorCollection(NKMUserData userData, OperatorListOptions options)
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
		foreach (int item in userData.m_ArmyData.m_illustrateUnit)
		{
			NKMUnitTempletBase unitTempletBase = NKMUnitManager.GetUnitTempletBase(item);
			if (unitTempletBase.m_NKM_UNIT_TYPE == NKM_UNIT_TYPE.NUT_OPERATOR)
			{
				list.Add(NKCOperatorUtil.GetDummyOperator(unitTempletBase));
			}
		}
		return list;
	}
}
