using System.Collections.Generic;
using NKM;

namespace NKC;

public class NKCGenericUnitSort : NKCUnitSortSystem
{
	public NKCGenericUnitSort(NKMUserData userData, UnitListOptions options, IEnumerable<NKMUnitData> lstUnitData)
		: base(userData, options, lstUnitData)
	{
	}

	protected override IEnumerable<NKMUnitData> GetTargetUnitList(NKMUserData userData)
	{
		return null;
	}
}
