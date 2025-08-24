using System.Collections.Generic;
using ClientPacket.Pvp;

namespace NKC;

public static class NKCRankPVPMgr
{
	private static List<PvpPickRateData> pickRates;

	public static List<PvpPickRateData> PickRateData => pickRates;

	public static void SetPickRateData(List<PvpPickRateData> pickRatesData)
	{
		pickRates = pickRatesData;
	}

	public static PvpPickRateData GetPickRateData(PvpPickType rankType)
	{
		return pickRates.Find((PvpPickRateData e) => e.type == rankType);
	}
}
