using System.Collections.Generic;
using NKM;

namespace NKC;

public class NKCGenericOperatorSort : NKCOperatorSortSystem
{
	public NKCGenericOperatorSort(NKMUserData userData, OperatorListOptions options, IEnumerable<NKMOperator> lstOperatorData)
		: base(userData, options, lstOperatorData)
	{
	}

	protected override IEnumerable<NKMOperator> GetTargetOperatorList(NKMUserData userData)
	{
		return null;
	}
}
