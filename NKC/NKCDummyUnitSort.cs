using System.Collections.Generic;
using NKM;

namespace NKC;

public class NKCDummyUnitSort : NKCUnitSortSystem
{
	public NKCDummyUnitSort(UnitListOptions options, IEnumerable<NKMUnitData> lstUnitData)
		: base(options, lstUnitData)
	{
	}

	protected override IEnumerable<NKMUnitData> GetTargetUnitList(NKMUserData userData)
	{
		return null;
	}
}
