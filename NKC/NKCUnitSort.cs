using System.Collections.Generic;
using NKM;

namespace NKC;

public class NKCUnitSort : NKCUnitSortSystem
{
	public NKCUnitSort(NKMUserData userData, UnitListOptions options)
		: base(userData, options)
	{
	}

	public NKCUnitSort(NKMUserData userData, UnitListOptions options, bool useLocal)
		: base(userData, options, useLocal)
	{
	}

	protected override IEnumerable<NKMUnitData> GetTargetUnitList(NKMUserData userData)
	{
		return userData.m_ArmyData.m_dicMyUnit.Values;
	}
}
