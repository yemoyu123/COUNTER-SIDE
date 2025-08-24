using System.Collections.Generic;
using ClientPacket.Common;

namespace NKM;

public static class NKMOperationPower
{
	public static int Calculate(NKMUnitData ship, IEnumerable<NKMUnitData> units, long leaderUnitUID, NKMInventoryData invenData, bool bPVP = false, Dictionary<int, NKMBanData> dicNKMBanData = null, Dictionary<int, NKMUnitUpData> dicNKMUpData = null, int operatorPower = 0)
	{
		int num = 0;
		if (ship != null)
		{
			num += ship.CalculateOperationPower(null);
		}
		if (operatorPower != 0)
		{
			num += operatorPower;
		}
		int num2 = 0;
		if (units != null)
		{
			foreach (NKMUnitData unit in units)
			{
				if (unit != null)
				{
					num2 += unit.CalculateOperationPower(invenData);
				}
			}
		}
		return num + num2;
	}

	public static int Calculate(int totalOperationPower, int totalUnitOperationPower, int totalSummonCost, int unitCount)
	{
		return totalOperationPower;
	}
}
