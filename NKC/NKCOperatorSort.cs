using System.Collections.Generic;
using System.Linq;
using NKM;

namespace NKC;

public class NKCOperatorSort : NKCOperatorSortSystem
{
	public NKCOperatorSort(NKMUserData userData, OperatorListOptions options)
		: base(userData, options)
	{
	}

	public NKCOperatorSort(NKMUserData userData, OperatorListOptions options, bool local)
		: base(userData, options, local)
	{
	}

	protected override IEnumerable<NKMOperator> GetTargetOperatorList(NKMUserData userData)
	{
		return userData.m_ArmyData.m_dicMyOperator.Values.ToList();
	}
}
