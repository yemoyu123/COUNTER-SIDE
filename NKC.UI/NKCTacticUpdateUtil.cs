using System.Collections.Generic;
using NKM;

namespace NKC.UI;

public static class NKCTacticUpdateUtil
{
	public static bool IsMaxTacticLevel(int iUnitLevel)
	{
		return iUnitLevel == 6;
	}

	public static bool HasMaxTacticLevelToSameUnits(int iTargetUnitID)
	{
		foreach (KeyValuePair<long, NKMUnitData> item in NKCScenManager.CurrentUserData().m_ArmyData.m_dicMyUnit)
		{
			if (item.Value != null && item.Value.m_UnitID == iTargetUnitID && item.Value.tacticLevel == 6)
			{
				return true;
			}
		}
		return false;
	}
}
