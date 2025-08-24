using System.Collections.Generic;
using NKM;

namespace NKC;

public class NKCShipSort : NKCUnitSortSystem
{
	public NKCShipSort(NKMUserData userData, UnitListOptions options)
		: base(userData, options)
	{
	}

	public NKCShipSort(NKMUserData userData, UnitListOptions options, bool useLocal)
		: base(userData, options, useLocal)
	{
	}

	protected override IEnumerable<NKMUnitData> GetTargetUnitList(NKMUserData userData)
	{
		return userData.m_ArmyData.m_dicMyShip.Values;
	}
}
