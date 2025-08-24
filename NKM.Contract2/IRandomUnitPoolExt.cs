using System.Linq;
using NKM.Templet.Base;

namespace NKM.Contract2;

public static class IRandomUnitPoolExt
{
	public static void ValidateCommon(this IRandomUnitPool self)
	{
		foreach (RandomUnitTempletV2 unitTemplet in self.UnitTemplets)
		{
			unitTemplet.Validate();
		}
		if ((from e in self.UnitTemplets
			group e by e.UnitTemplet.m_NKM_UNIT_TYPE).Count() > 1)
		{
			NKMTempletError.Add("[" + self.DebugName + "] 유닛 풀에 오퍼레이터와 유닛이 같이 존재.", "/Users/buildman/buildAgent_ca18-1/work/e0bfb30763b53cef/CounterSide/CODE/CSClient/Assets/ASSET_STATIC/AS_SCRIPT/NKM/Contract2/IRandomUnitPool.cs", 31);
		}
	}
}
